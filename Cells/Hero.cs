using System;
using System.Collections.Generic;
using System.Text;

namespace MazeKz.Cells
{
    public class Hero : CellBase
    {
        public int Money { get; set; }

        public Hero(int x, int y, Maze maze) : base(x, y, maze)
        {
        }

        public override bool IsStepAllowed()
        {
            throw new Exception("The Hero must not step on himself.") ;
        }
    }
}
