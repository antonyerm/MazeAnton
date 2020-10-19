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
            this.Money = 0;
        }

        public override bool IsStepAllowed()
        {
            throw new Exception("The Hero must not step on himself.") ;
        }
    }
}
