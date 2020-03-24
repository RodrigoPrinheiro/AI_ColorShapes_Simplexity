using System;
using System.Threading;
using ColorShapeLinks.Common;
using ColorShapeLinks.Common.AI;

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
            AbstractThinker bee = new Bee();

            board.DoMove(PShape.Round, 0);
            board.DoMove(PShape.Square, 1);
            board.DoMove(PShape.Round, 2);
            board.DoMove(PShape.Square, 3);

            board.DoMove(PShape.Round, 6);
            board.DoMove(PShape.Round, 6);
            board.DoMove(PShape.Round, 6);
            
            bee.Setup("");
            bee.Think(board, CancellationToken.None);

            //Console.WriteLine(BeeHeuristics.Heuristic(board, PColor.White));
        }
    }
}
