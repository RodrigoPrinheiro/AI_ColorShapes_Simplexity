﻿using System;
using System.Linq;
using System.Threading;
using ColorShapeLinks.Common;
using ColorShapeLinks.Common.AI;
using ColorShapeLinks.Common.Session;

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
            board.DoMove(PShape.Round, 5);
            board.DoMove(PShape.Square, 3);
            board.DoMove(PShape.Round, 5);
            board.DoMove(PShape.Square, 3);
            board.DoMove(PShape.Round, 5);
            board.DoMove(PShape.Square, 3);

            TranspositionTable table = new TranspositionTable(board.cols, board.rows);
            // board.DoMove(PShape.Round, 6);
            // board.DoMove(PShape.Square, 5);
            // board.DoMove(PShape.Round, 4);
            // board.DoMove(PShape.Square, 4);
            // board.DoMove(PShape.Square, 2);

            BoardUpdate(board);
            Console.WriteLine(table.HashBoard(board));
            Console.WriteLine(table.HashBoard(board));
            Console.WriteLine(board.Turn);
            Console.WriteLine(BeeHeuristics.Honeycomb(board, board.Turn));

            // Bee b = new Bee();
            // b.Setup("");

            //int col = b.Think(board, CancellationToken.None).column;
            //Console.WriteLine("Chosen col: " + col);
            //Console.WriteLine("Score: " + col);

        }

        private static void BoardUpdate(Board board)
        {
            for (int r = board.rows - 1; r >= 0; r--)
            {
                for (int c = 0; c < board.cols; c++)
                {
                    char pc = '.';
                    Piece? p = board[r, c];
                    if (p.HasValue)
                    {
                        if (p.Value.Is(PColor.White, PShape.Round))
                            pc = 'w';
                        else if (p.Value.Is(PColor.White, PShape.Square))
                            pc = 'W';
                        else if (p.Value.Is(PColor.Red, PShape.Round))
                            pc = 'r';
                        else if (p.Value.Is(PColor.Red, PShape.Square))
                            pc = 'R';
                        else
                            throw new ArgumentException(
                                $"Invalid piece '{p.Value}'");
                    }
                    Console.Write(pc);
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }
    }
}