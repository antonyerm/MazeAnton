using System;
using System.Collections.Generic;
using System.Text;

namespace MazeKz
{
    class ConsoleUiHelper
    {
        public void Play()
        {
            var mazeGenerator = new MazeGenerator();
            var maze = mazeGenerator.GenerateSmartByAnton(61, 31);
            // Варинат Павла: var maze = mazeGenerator.GenerateSmart(15, 12);

            var draw = new Drawer();

            var continuePlay = true;
            while (continuePlay)
            {
                draw.DrawMaze(maze);

                var key = Console.ReadKey();
                switch (key.Key)
                {
                    case ConsoleKey.UpArrow:
                    case ConsoleKey.W:
                        maze.TryToStep(Direction.Up);
                        break;
                    case ConsoleKey.DownArrow:
                    case ConsoleKey.S:
                        maze.TryToStep(Direction.Down);
                        break;
                    case ConsoleKey.LeftArrow:
                    case ConsoleKey.A:
                        maze.TryToStep(Direction.Left);
                        break;
                    case ConsoleKey.RightArrow:
                    case ConsoleKey.D:
                        maze.TryToStep(Direction.Right);
                        break;
                    case ConsoleKey.Escape:
                        continuePlay = false;
                    break;
                }
            }

            Console.WriteLine("Goodbye!");
        }
    }
}
