using System;
using ColorShapeLinks.Common;

namespace BeeAI
{
    class Program
    {
        static void Main(string[] args)
        {
            TestHeuristic();
        }

        static void TestHeuristic()
        {
            Board board = new Board();

            board.DoMove(PShape.Round, 0);
            board.DoMove(PShape.Square, 1);
            board.DoMove(PShape.Round, 2);
            board.DoMove(PShape.Square, 3);

            board.DoMove(PShape.Round, 6);
            board.DoMove(PShape.Round, 6);
            board.DoMove(PShape.Round, 6);

            foreach( var v in board.winCorridors)
            {
                Console.WriteLine();
                foreach(var t in v)
                {
                    Console.Write($"{t}, ");
                    Console.WriteLine();
                }
            }

            Console.WriteLine(BeeHeuristics.Heuristic(board, PColor.White));
        }
    }
}
