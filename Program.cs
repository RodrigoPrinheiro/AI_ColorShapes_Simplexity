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

            int i = 0;
            foreach( var v in board.winCorridors)
            {
                i++;
            }
            
            Console.WriteLine(i);
            Console.WriteLine(BeeHeuristics.Heuristic(board, PColor.White, 2));
        }
    }
}
