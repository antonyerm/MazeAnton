using MazeKz.Cells;
using System;

namespace MazeKz
{
    class Program
    {
        public static ConsoleUiHelper uiHelper {get; set; }

        static void Main(string[] args)
        {
            uiHelper = new ConsoleUiHelper();
            uiHelper.Play();
        }
    }
}
