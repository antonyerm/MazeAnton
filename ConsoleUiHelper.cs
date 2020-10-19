using System;
using System.Collections.Generic;
using System.Text;

namespace MazeKz
{
    class ConsoleUiHelper
    {
        public Maze Maze { get; set; }

        public void Play()
        {
            var mazeGenerator = new MazeGenerator();
            this.Maze = mazeGenerator.GenerateSmartByAnton(31, 17);
            // Варинат Павла: var maze = mazeGenerator.GenerateSmart(15, 12);

            var draw = new Drawer();

            var continuePlay = true;
            while (continuePlay)
            {
                draw.DrawMaze(this.Maze);

                var key = Console.ReadKey();
                switch (key.Key)
                {
                    case ConsoleKey.UpArrow:
                    case ConsoleKey.W:
                        this.Maze.TryToStep(Direction.Up);
                        break;
                    case ConsoleKey.DownArrow:
                    case ConsoleKey.S:
                        this.Maze.TryToStep(Direction.Down);
                        break;
                    case ConsoleKey.LeftArrow:
                    case ConsoleKey.A:
                        this.Maze.TryToStep(Direction.Left);
                        break;
                    case ConsoleKey.RightArrow:
                    case ConsoleKey.D:
                        this.Maze.TryToStep(Direction.Right);
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
