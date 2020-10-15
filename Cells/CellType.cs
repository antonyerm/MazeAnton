using System;
using System.Collections.Generic;
using System.Text;

namespace MazeKz.Cells
{
    public enum CellType
    {
        Wall = 1,
        Ground = 2,
        Coin = 3,
        WallVertical = 4,   // "|" █
        WallHorizontal = 5, // "-"
        WallJoint = 6, // "+"
    }
}
