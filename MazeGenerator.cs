using MazeKz.Cells;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;

namespace MazeKz
{
    public class MazeGenerator
    {
        private Random _random = new Random();
        private Maze _maze;

        public Maze GenerateSmart(int width = 10, int height = 5)
        {
            //Создали лабиринты полный стен
            _maze = GenerateMazeFullWall(width, height);

            //Координаты шахтёр
            var minerX = 0;
            var minerY = 0;

            this.GenerateWall(minerX, minerY);

            _maze.Hero = new Hero(0, 0, _maze);

            this.GenerateCoin();

            return _maze;
        }

        private void GenerateCoin()
        {
            var groundCells = _maze.Cells.OfType<Ground>().ToList();
            for (int i = 0; i < 10; i++)
            {
                var randomGround = GetRandomCell(groundCells);
                var coin = new Coin(randomGround.X, randomGround.Y, _maze);
                _maze.ReplaceCell(coin);
            }
        }

        private void GenerateWall(int minerX, int minerY)
        {
            List<CellBase> cellsAllowToBreak = new List<CellBase>();
            do
            {
                //DrawMaze();

                //Разломать стeну, по координатам шахтёра
                BreakWall(minerX, minerY);
                // Нашли внутри cellsAllowToBreak координату шахтера
                var brokenWall = cellsAllowToBreak.SingleOrDefault(x => x.X == minerX && x.Y == minerY);
                // если такая действительно нашлась, удаляем эту ячейку из cellsAllowToBreak
                if (brokenWall != null)
                {
                    cellsAllowToBreak.Remove(brokenWall);
                }

                //Выбрать ближайшие к шахтёры ячейки // которые являются стенами и существуют
                var nearCells = GetNearCells<Wall>(minerX, minerY);
                // пополнили ими cellsAllowToBreak, т.е. всеми существующими вокруг стенными ячейками
                cellsAllowToBreak.AddRange(nearCells);
                // исключили из этих стен те, рядом с которыми много земли. Оставили только те, где рядом одна земля,
                // и кроме того убрали повторяющиеся ячейки
                cellsAllowToBreak = cellsAllowToBreak
                    .Where(wall =>
                        GetNearCells<Ground>(wall.X, wall.Y).Count() <= 1)
                    .Distinct()
                    .ToList();

                //Выбрать случайную ячейку, куда шагнёт шахтёр
                var randomCell = GetRandomCell(cellsAllowToBreak);

                minerX = randomCell?.X ?? 0;
                minerY = randomCell?.Y ?? 0;
            } while (cellsAllowToBreak.Any());
        }

        private void DrawMaze()
        {
            var drawer = new Drawer();
            drawer.DrawMaze(_maze);
            //Console.WriteLine("----------------------------------");
            //Thread.Sleep(200);
        }

        private CellBase GetRandomCell(IEnumerable<CellBase> nearCells)
        {
            if (!nearCells.Any())
            {
                return null;
            }

            var list = nearCells.ToList();
            var randomIndex = _random.Next(0, list.Count);
            return list[randomIndex];
        }

        private IEnumerable<CellBase> GetNearCells<T>(int minerX, int minerY) where T : CellBase
        {
            var nearCells = new List<CellBase>()
            {
                _maze[minerX - 1, minerY],
                _maze[minerX + 1, minerY],
                _maze[minerX, minerY + 1],
                _maze[minerX, minerY - 1],
            };
            var answer = nearCells
                .Where(x => x != null)
                .OfType<T>();
            return answer;
        }

        private void BreakWall(int minerX, int minerY)
        {
            _maze.ReplaceCell(new Ground(minerX, minerY, _maze));
        }

        private Maze GenerateMazeFullWall(int width, int height)
        {
            var maze = new Maze
            {
                Width = width,
                Height = height
            };

            for (int y = 0; y < maze.Height; y++)
            {
                for (int x = 0; x < maze.Width; x++)
                {
                    maze.Cells.Add(new Wall(x, y, maze));
                }
            }

            return maze;
        }

        // ANTON'S PART
        private class CellWithSetInfo
        {
            public int X { get; set; }

            public int Y { get; set; }

            public int SetNumber { get; set; }

            public override string ToString()
            {
                return $"{X},{Y} {SetNumber}";
            }

        }

        public Maze GenerateSmartByAnton(int width = 30, int height = 20)
        {
            this._maze = this.GenerateMazeFullOfEdges(width, height);
            this.GenerateKruskal();

            this.GenerateHero();

            this.GenerateCoin();

            return this._maze;
        }

        public Maze GenerateKruskal()
        {
            

            // здесь будут храниться наборы (замкнутые области) в виде координат лабиринта и int-значений внутри.
            // для начала нашли все ячейки типа земля.
            var sets = new List<CellWithSetInfo>();
            var CellsWhichAreGround = _maze.Cells.OfType<Ground>(); 
            // затем заполнили sets новыми ячейками, для которых взяли из найденных ячеек координаты, а номер набора SetNmber поставив -1
            sets.AddRange(from cell in CellsWhichAreGround
                          select new CellWithSetInfo { X = cell.X, Y = cell.Y, SetNumber = - 1});
            // Нашли грани
            var edges = new List<CellBase>();
            edges.AddRange(_maze.Cells.OfType<Wall>());
            // Удаляем крестовины, т.е. оставляем только грани, рядом с которыми 1..2 земли 
            edges = edges.Where(cell => GetNearCells<Ground>(cell.X, cell.Y).Count() == 2).ToList();

            // К этому моменту edges - все грани без крестовин, sets - все пустые поля с инициализированными номерами наборов.

            // Счетчик наборов при заполнении пустого массива наборов
            int setsCounter = 1;

            do
            {
                // взяли случайную грань, чтобы ее сломать
                var randomEdgeToBreak = edges[_random.Next(0, edges.Count())];
                // нашли в окружении грани ячейки типа земля, будeм их объединять в набор
                var cellsToConnect = GetNearCells<Ground>(randomEdgeToBreak.X, randomEdgeToBreak.Y);
                // составили список из ячеек sets, где те же ячейки из окружения Edges
                var cellsToConnectInSets = (from cellFromSets in sets
                                            join cellFromEdges in cellsToConnect
                                            on new { cellFromSets.X, cellFromSets.Y } equals new { cellFromEdges.X, cellFromEdges.Y }
                                            select new CellWithSetInfo { X = cellFromSets.X, Y = cellFromSets.Y, SetNumber = cellFromSets.SetNumber }).ToList();

                // нашли минимальное значение SetNumber из этих ячеек
                int minSetNumber = cellsToConnectInSets.Min(cell => cell.SetNumber);
                // нашли максимальное значение SetNumber из этих ячеек
                int maxSetNumber = cellsToConnectInSets.Max(cell => cell.SetNumber);

                // если только начали работать с лабиринтом, то надо назначать номера наборов через счетчик setsCounter
                if (maxSetNumber == -1)
                {
                    foreach (var cell in cellsToConnectInSets)
                    {
                        sets.Single(s => s.X == cell.X && s.Y == cell.Y).SetNumber = setsCounter;
                    }
                    // сломать эту грань
                    _maze.ReplaceCell(new Ground(randomEdgeToBreak.X, randomEdgeToBreak.Y, _maze));
                    setsCounter++;
                }
                else
                {
                    // объединяем с одной начальной ячейкой 
                    if (minSetNumber == -1)
                    {
                        foreach (var cell in cellsToConnectInSets)
                        {
                            sets.Single(s => s.X == cell.X && s.Y == cell.Y).SetNumber = maxSetNumber;
                        }
                        _maze.ReplaceCell(new Ground(randomEdgeToBreak.X, randomEdgeToBreak.Y, _maze));
                    }
                    else
                    {
                        // если номера наборов разные, то объединить больший набор с меньшим. 
                        if (minSetNumber != maxSetNumber)
                        {
                            sets.Where(cell => cell.SetNumber == maxSetNumber).ToList().ForEach(c => c.SetNumber = minSetNumber);
                            // сломать эту грань
                            _maze.ReplaceCell(new Ground(randomEdgeToBreak.X, randomEdgeToBreak.Y, _maze));
                        }
                    }
                }
                // когда номера одинаковые грань не ломаем

                edges.Remove(randomEdgeToBreak);

                // Diagnostics
                // Console.Clear();
                // DrawMaze();
                //for (int y = 0; y <= sets.Max(Cell => Cell.Y); y++)
                //{
                //    for (int x = 0; x <= sets.Max(Cell => Cell.X); x++)
                //    {
                //        Console.Write("{0,3}", sets.SingleOrDefault(c => c.X == x && c.Y == y)?.SetNumber ?? 0);
                //    }
                //    Console.WriteLine();
                //}
                //Console.ReadLine();
                //Console.Clear();
            }
            while (edges.Any());

            return _maze;
        }

        private Maze GenerateMazeFullOfEdges(int width, int height)
        {
            var maze = new Maze
            {
                Width = width,
                Height = height
            };

            for (int y = 0; y < maze.Height; y++)
            {
                for (int x = 0; x < maze.Width; x++)
                {
                    //if y uneven and x uneven then put ground
                    if (y % 2 == 1 && x % 2 == 1)
                    {
                        maze.Cells.Add(new Ground(x, y, maze));
                    }
                    else
                    {
                        maze.Cells.Add(new Wall(x, y, maze));
                    }
                }
            }

            return maze;
        }

        private void GenerateHero()
        {
            var groundCells = _maze.Cells.OfType<Ground>().ToList();
            // var heroPosition = GetRandomCell(groundCells);
            // находим ячейку с минимальными X и Y
            var minGroundX = groundCells.Select(cell => cell.X).Min();
            var groundCellsWithMinX = groundCells.Where(cell => cell.X == minGroundX);
            var minGroundY = groundCellsWithMinX.Select(cell => cell.Y).Min();
            var heroCell = groundCellsWithMinX.Single(cell => cell.Y == minGroundY);
            _maze.Hero = new Hero(heroCell.X, heroCell.Y, _maze);
        }
    }
}