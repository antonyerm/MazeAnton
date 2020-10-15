using System;

namespace MazeKz
{
    class Program
    {
        static void Main(string[] args)
        {
            var mazeGenerator = new MazeGenerator();
            var maze = mazeGenerator.GenerateKruskal(121, 81);

            var draw = new Drawer();
            draw.DrawMaze(maze);
        }
    }
}
