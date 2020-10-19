using System;
using System.Collections.Generic;
using System.Text;

namespace MazeKz.Cells
{
    class GoldMine : CellBase
    {
        public GoldMine(int x, int y, Maze maze) : base(x, y, maze)
        {
        }

        public override bool IsStepAllowed()
        {
            Maze.Hero.Money += 5;

            var ground = new Ground(X, Y, Maze);
            Maze.ReplaceCell(ground);

            return true;
        }
    }
}
