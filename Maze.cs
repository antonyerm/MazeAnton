using MazeKz.Cells;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MazeKz
{
    public class Maze
    {
        public List<CellBase> Cells { get; set; }

        public List<CellBase> CellsWithHero
        {
            get
            {
                var cellsCopy = Cells.ToList();
                this.ReplaceCell(cellsCopy, Hero);
                return cellsCopy;
            }
        }

        //public int WidthGet => Cells.Max(cell => cell.X);
        public int Width { get; set; }

        public int Height { get; set; }

        public ConsoleColor Color { get; set; }
        public Hero Hero { get; set; }

        public CellBase this[int x, int y]
        {
            get
            {
                return Cells.SingleOrDefault(c => c.X == x && c.Y == y);
            }

            set
            {
                //var oldCell = Cells.SingleOrDefault(c => c.X == x && c.Y == y);
                //Cells.Remove(oldCell);
                //Cells.Add(value);
                this.ReplaceCell(value);
            }

        }
                
        public Maze()
        {
            Cells = new List<CellBase>();
            this.Color = ConsoleColor.Gray;
        }

        public void TryToStep(Direction Direction)
        {
            var heroX = Hero.X;
            var heroY = Hero.Y;

            switch (Direction)
            {
                case Direction.Left:
                    heroX--;
                    break;
                case Direction.Up:
                    heroY--;
                    break;
                case Direction.Right:
                    heroX++;
                    break;
                case Direction.Down:
                    heroY++;
                    break;
            }

            var destination = this[heroX, heroY];
            if (destination == null)
            {
                return;
            }

            if (destination.IsStepAllowed())
            {
                Hero.X = heroX;
                Hero.Y = heroY;
            }
        }

        public void ReplaceCell(CellBase newCell)
        {
            ReplaceCell(this.Cells, newCell);
        }

        public void ReplaceCell(List<CellBase> cells, CellBase newCell)
        {
            var cellToDelete = cells.SingleOrDefault(currentCell => currentCell.X == newCell.X && currentCell.Y == newCell.Y);
            cells.Remove(cellToDelete);
            cells.Add(newCell);
        }
    }
}
