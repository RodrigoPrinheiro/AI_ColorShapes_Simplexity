using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using ColorShapeLinks.Common;
using ColorShapeLinks.Common.AI;
using ColorShapeLinks.Common.Session;

namespace BeeAI
{
    public class Bee : AbstractThinker
    {
        private const float TIMER_WIGGLE_ROOM_FACTOR = 0.10f;
        private int maxDepth;
        private int initialMaxDepth;
        private int turns;
        // Stopwatch to keep track the ai think time
        Stopwatch stopwatch;

        public override void Setup(string str)
        {
            base.Setup(str);

            // Try to get the maximum depth from the parameters
            if (!int.TryParse(str, out maxDepth))
            {
                // If not possible, set it to 3 by default
                maxDepth = 3;
            }
            initialMaxDepth = maxDepth;
            stopwatch = new Stopwatch();
            turns = 0;
        }

        public override FutureMove Think(Board board, CancellationToken ct)
        {
            stopwatch.Start();
            turns += 2;

            // Invoke minimax, starting with zero depth
            (FutureMove move, float score) decision =
                ABNegamax(board, ct, board.Turn, board.Turn, 0, float.NegativeInfinity, float.PositiveInfinity);
            stopwatch.Reset();
            // Return best move
            return decision.move;
        }

        // Minimax implementation
        private(FutureMove move, float score) ABNegamax(
            Board board, CancellationToken ct,
            PColor player, PColor turn, int depth, float alpha, float beta)
        {

            // Move to return and its heuristic value
            (FutureMove move, float score) bestMove;

            // Current board state
            Winner winner;
            // Otherwise, if it's a final board, return the appropriate evaluation
            if ((winner = board.CheckWinner()) != Winner.None)
            {
                if (winner.ToPColor() == player)
                {
                    // AI player wins, return highest possible score
                    bestMove = (FutureMove.NoMove, float.PositiveInfinity);
                }
                else if (winner.ToPColor() == player.Other())
                {
                    // Opponent wins, return lowest possible score
                    bestMove = (FutureMove.NoMove, float.NegativeInfinity);
                }
                else
                {
                    // A draw, return zero
                    bestMove = (FutureMove.NoMove, 0f);
                }
            }
            // If a cancellation request was made...
            else if (ct.IsCancellationRequested || stopwatch.ElapsedMilliseconds > TimeLimitMillis - TimeLimitMillis * TIMER_WIGGLE_ROOM_FACTOR)
            {
                // ...set a "no move" and skip the remaining part of the algorithm
                maxDepth = initialMaxDepth;
                bestMove = (FutureMove.NoMove, BeeHeuristics.Honeycomb(board, player, turns));
            }
            else // Board not final and depth not at max...
            {
                // infinite depth
                //...so let's test all possible moves and recursively call Negamax()
                // for each one of them
                maxDepth = depth + 1;
                bestMove = (FutureMove.NoMove, float.NegativeInfinity);

                // Initialize the selected..

                // Go through all columns
                for (int i = 0; i < board.cols; i++)
                {
                    // Skip full columns
                    if (board.IsColumnFull(i)) continue;

                    // Test shapes
                    for (int j = 0; j < 2; j++)
                    {
                        // Get current shape
                        PShape shape = (PShape) j;

                        // Use this variable to keep the current board's score
                        float eval;
                        // Store the move heuristic

                        // // Check if we should stop here, if yes, return the last completed best
                        // if (depth == maxDepth)
                        // {
                        //     bestMove = (FutureMove.NoMove, BeeHeuristics.Honeycomb(board, player, turns));
                        // }

                        // Skip unavailable shapes
                        if (board.PieceCount(turn, shape) == 0) continue;

                        // Test move
                        board.DoMove(shape, i);
                        // Call minimax on a new depth
                        eval = -ABNegamax(board, ct, player, turn.Other(), depth + 1, -beta, -alpha).score;
                        // Undo move
                        board.UndoMove();

                        // Is this the best move so far?
                        //if (eval > bestMove.score)
                        if (eval > bestMove.score)
                        {
                            // If so, update alpha
                            alpha = eval;

                            // Keep the best move
                            bestMove = (new FutureMove(i, shape), eval);

                            // Is alpha higher than beta?
                            if (alpha >= beta)
                            {
                                // If so, make alpha-beta cut and return the
                                // best move so far
                                return bestMove;
                            }
                        }
                    }
                }
                //lastCompletedDepthBestMove = bestMove;
            }
            // Return movement and its heuristic value
            return bestMove;
        }

        private float DebugHoneycomb(Board board, PColor color)
        {
            // Distance between two points
            float Dist(float x1, float y1, float x2, float y2)
            {
                return (float) Math.Sqrt(
                    Math.Pow(x1 - x2, 2) + Math.Pow(y1 - y2, 2));
            }

            // Determine the center row
            float centerRow = board.rows / 2;
            float centerCol = board.cols / 2;

            // Maximum points a piece can be awarded when it's at the center
            float maxPoints = Dist(centerRow, centerCol, 0, 0);

            // Current heuristic value
            float h = 0;

            // Loop through the board looking for pieces
            for (int i = 0; i < board.rows; i++)
            {
                for (int j = 0; j < board.cols; j++)
                {
                    // Get piece in current board position
                    Piece? piece = board[i, j];

                    // Is there any piece there?
                    if (piece.HasValue)
                    {
                        // If the piece is of our color, increment the
                        // heuristic inversely to the distance from the center
                        if (piece.Value.color == color)
                            h += maxPoints - Dist(centerRow, centerCol, i, j);
                        // Otherwise decrement the heuristic value using the
                        // same criteria
                        else
                            h -= maxPoints - Dist(centerRow, centerCol, i, j);
                        // If the piece is of our shape, increment the
                        // heuristic inversely to the distance from the center
                        if (piece.Value.shape == color.Shape())
                            h += maxPoints - Dist(centerRow, centerCol, i, j);
                        // Otherwise decrement the heuristic value using the
                        // same criteria
                        else
                            h -= maxPoints - Dist(centerRow, centerCol, i, j);
                    }
                }
            }
            // Return the final heuristic score for the given board
            return h;
        }
    }
}