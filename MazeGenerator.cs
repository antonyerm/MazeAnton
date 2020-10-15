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

        public Maze GenerateRandom(int width = 10, int height = 5)
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
                    var number = _random.Next(1, 4);

                    var cellType = (CellType)number;

                    var cell = new Cell(x, y, cellType);
                    maze.Cells.Add(cell);
                }
            }

            return maze;
        }

        public Maze GenerateSmart(int width = 10, int height = 5)
        {
            //Создали лабиринты полный стен
            _maze = GenerateMazeFullWall(width, height);

            //Координаты шахтёр
            var minerX = 0;
            var minerY = 0;

            List<Cell> cellsAllowToBreak = new List<Cell>();
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
                var nearCells = GetNearCells(minerX, minerY, CellType.Wall);
                // пополнили ими cellsAllowToBreak, т.е. всеми существующими вокруг стенными ячейками
                cellsAllowToBreak.AddRange(nearCells);
                // исключили из этих стен те, рядом с которыми много земли. Оставили только те, где рядом одна земля,
                // и кроме того убрали повторяющиеся ячейки
                cellsAllowToBreak = cellsAllowToBreak
                    .Where(wall =>
                        GetNearCells(wall.X, wall.Y, CellType.Ground).Count() <= 1)
                    .Distinct()
                    .ToList();

                //Выбрать случайную ячейку, куда шагнёт шахтёр
                var randomCell = GetRandomCell(cellsAllowToBreak);

                minerX = randomCell?.X ?? 0;
                minerY = randomCell?.Y ?? 0;
            } while (cellsAllowToBreak.Any());

            return _maze;
        }

        private void DrawMaze()
        {
            var drawer = new Drawer();
            drawer.DrawMaze(_maze);
            //Console.WriteLine("----------------------------------");
            //Thread.Sleep(200);
        }

        private Cell GetRandomCell(List<Cell> nearCells)
        {
            if (!nearCells.Any())
            {
                return null;
            }
            var randomIndex = _random.Next(0, nearCells.Count);
            return nearCells[randomIndex];
        }

        private List<Cell> GetNearCells(int minerX, int minerY, CellType type)
        {
            var nearCells = new List<Cell>()
            {
                _maze[minerX - 1, minerY],
                _maze[minerX + 1, minerY],
                _maze[minerX, minerY + 1],
                _maze[minerX, minerY - 1],
            };
            nearCells = nearCells
                .Where(x => x != null && x.CellType == type)
                .ToList();
            return nearCells;
        }

        private void BreakWall(int minerX, int minerY)
        {
            var cell = _maze[minerX, minerY];
            cell.CellType = CellType.Ground;
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
                    maze.Cells.Add(new Cell(x, y, CellType.Wall));
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

        public Maze GenerateKruskal(int width = 30, int height = 20)
        {
            _maze = this.GenerateMazeWithEdges(width, height);

            // здесь будут храниться наборы (замкнутые области) в виде координат лабиринта и int-значений внутри.
            // для начала нашли все ячейки типа земля.
            var sets = new List<CellWithSetInfo>();
            var CellsWhichAreGround = _maze.Cells.Where(cell => cell.CellType == CellType.Ground);
            // затем заполнили sets новыми ячейками, для которых взяли из найденных ячеек координаты, а номер набора SetNmber поставив -1
            sets.AddRange(from cell in CellsWhichAreGround
                          select new CellWithSetInfo { X = cell.X, Y = cell.Y, SetNumber = - 1});
            // Нашли грани
            var edges = new List<Cell>();
            edges.AddRange(_maze.Cells.Where(cell => cell.CellType == CellType.Wall).ToList());
            // Удаляем крестовины, т.е. оставляем только грани, рядом с которыми 1..2 земли 
            edges = edges.Where(cell => GetNearCells(cell.X, cell.Y, CellType.Ground).Count() == 2).ToList();

            // К этому моменту edges - все грани без крестовин, sets - все пустые поля с инициализированными номерами наборов.

            // Счетчик наборов при заполнении пустого массива наборов
            int setsCounter = 1;

            do
            {
                // взяли случайную грань, чтобы ее сломать
                var randomEdgeToBreak = edges[_random.Next(0, edges.Count())];
                // нашли в окружении грани ячейки типа земля, будeм их объединять в набор
                var cellsToConnect = GetNearCells(randomEdgeToBreak.X, randomEdgeToBreak.Y, CellType.Ground);
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
                    _maze[randomEdgeToBreak.X, randomEdgeToBreak.Y].CellType = CellType.Ground;
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
                        _maze[randomEdgeToBreak.X, randomEdgeToBreak.Y].CellType = CellType.Ground;
                    }
                    else
                    {
                        // если номера наборов разные, то объединить больший набор с меньшим. 
                        if (minSetNumber != maxSetNumber)
                        {
                            sets.Where(cell => cell.SetNumber == maxSetNumber).ToList().ForEach(c => c.SetNumber = minSetNumber);
                            // сломать эту грань
                            _maze[randomEdgeToBreak.X, randomEdgeToBreak.Y].CellType = CellType.Ground;
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

        private Maze GenerateMazeWithEdges(int width, int height)
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
                        maze.Cells.Add(new Cell(x, y, CellType.Ground));
                    }
                    else
                    {
                        maze.Cells.Add(new Cell(x, y, CellType.Wall));
                    }
                }
            }

            return maze;
        }
    }
}