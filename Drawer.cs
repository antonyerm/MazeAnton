using MazeKz.Cells;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MazeKz
{
    public class Drawer
    {
        public void DrawMaze(Maze maze)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Coins: {0}", maze.Hero.Money);
            Console.ForegroundColor = maze.Color;

            for (int y = 0; y < maze.Height; y++)
            {
                for (int x = 0; x < maze.Width; x++)
                {
                    var cell = maze.CellsWithHero.Single(cell => cell.X == x && cell.Y == y);
                    if (cell is Wall)
                    {
                        Console.Write("█");
                    }
                    else
                    if (cell is Ground)
                    {
                        Console.Write(".");
                    }
                    else
                    if (cell is Hero)
                    {
                        Console.Write("@"); 
                    }
                    else
                    if (cell is Coin)
                    {
                        Console.Write("c");
                    }
                    else
                    if (cell is Portal)
                    {
                        Console.Write("P");
                    }
                    else
                    if (cell is GoldMine)
                    {
                        Console.Write("G");
                    }
                    else
                    if (cell is Well)
                    {
                        Console.Write("W");
                    }

                }

                Console.WriteLine();
            }
        }
    }
}
