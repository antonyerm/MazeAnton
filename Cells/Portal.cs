using System;
using System.Collections.Generic;
using System.Text;

namespace MazeKz.Cells
{
    class Portal : CellBase
    {
        public Portal(int x, int y, Maze maze) : base(x, y, maze)
        {
        }

        public override bool IsStepAllowed()
        {
            var currentWidth = this.Maze.Width;
            var currentHeight = this.Maze.Height;

            var mazeGenerator = new MazeGenerator();
            Program.uiHelper.Maze = mazeGenerator.GenerateSmartByAntonWithHero(Maze.Hero, currentWidth, currentHeight);


            return true;
        }
    }
}
