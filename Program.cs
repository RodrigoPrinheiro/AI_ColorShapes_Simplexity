using System;
using ColorShapeLinks.Common;

namespace BeeAI
{
    class Program
    {
        static void Main(string[] args)
        {
            
        }

        static void TestHeuristic()
        {
            Board board = new Board();

            board.DoMove(PShape.Square, 0);
            board.DoMove(PShape.Square, 3);
            board.DoMove(PShape.Round, 3);
            board.DoMove(PShape.Square, 3);

        }
    }
}
