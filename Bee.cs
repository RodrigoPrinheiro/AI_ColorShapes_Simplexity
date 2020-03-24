using System;
using System.Threading;
using ColorShapeLinks.Common;
using ColorShapeLinks.Common.AI;

namespace BeeAI
{
    public class Bee : AbstractThinker
    {
        private int maxDepth;
        private int turns;
        public override void Setup(string str)
        {
            turns = 0;
        }

        public override FutureMove Think(Board board, CancellationToken ct)
        {
            turns += 2;
            // Invoke minimax, starting with zero depth
            (FutureMove move, float score) decision =
                ABNegamax(board, ct, board.Turn, board.Turn, 0, float.NegativeInfinity, float.PositiveInfinity);

            // Return best move
            return decision.move;
        }

        // Minimax implementation
        private(FutureMove move, float score) ABNegamax(
            Board board, CancellationToken ct,
            PColor player, PColor turn, int depth, float alpha, float beta)
        {
            // Move to return and its heuristic value
            (FutureMove move, float score) selectedMove;

            // Current board state
            Winner winner;

            // If a cancellation request was made...
            if (ct.IsCancellationRequested)
            {
                // ...set a "no move" and skip the remaining part of the algorithm
                selectedMove = (FutureMove.NoMove, float.NaN);
            }
            // Otherwise, if it's a final board, return the appropriate evaluation
            else if ((winner = board.CheckWinner()) != Winner.None)
            {
                if (winner.ToPColor() == player)
                {
                    // AI player wins, return highest possible score
                    selectedMove = (FutureMove.NoMove, float.PositiveInfinity);
                }
                else if (winner.ToPColor() == player.Other())
                {
                    // Opponent wins, return lowest possible score
                    selectedMove = (FutureMove.NoMove, float.NegativeInfinity);
                }
                else
                {
                    // A draw, return zero
                    selectedMove = (FutureMove.NoMove, 0f);
                }
            }
            // If we're at maximum depth and don't have a final board, use
            // the heuristic
            else if (depth == maxDepth)
            {
                selectedMove = (FutureMove.NoMove, BeeHeuristics.Honeycomb(board, player, turns, areaOfDiagonal));
            }
            else // Board not final and depth not at max...
            {
                //...so let's test all possible moves and recursively call Minimax()
                // for each one of them

                // Initialize the selected move...
                selectedMove = (FutureMove.NoMove, float.NegativeInfinity);
                (FutureMove move, float score) bestMove = selectedMove;

                // Test each column
                for (int i = 0; i < Cols; i++)
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

                        // Skip unavailable shapes
                        if (board.PieceCount(turn, shape) == 0) continue;

                        // Test move
                        board.DoMove(shape, i);
                        // Call minimax
                        eval = ABNegamax(board, ct, player, turn.Other(), depth + 1, -beta, -alpha).score;
                        // Undo move
                        board.UndoMove();

                        // Is this the best move so far?
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
                                selectedMove = bestMove;
                                return selectedMove;
                            }
                        }

                        //! Original code
                        // // If we're maximizing, is this the best move so far?
                        // if (turn == player &&
                        //     eval >= selectedMove.score)
                        // {
                        //     // If so, keep it
                        //     selectedMove = (new FutureMove(i, shape), eval);
                        // }
                        // // Otherwise, if we're minimizing, is this the worst move so far?
                        // else if (turn == player.Other() &&
                        //     eval <= selectedMove.score)
                        // {
                        //     // If so, keep it
                        //     selectedMove = (new FutureMove(i, shape), eval);
                        // }
                    }
                }
            }

            // Return movement and its heuristic value
            return selectedMove;
        }

    }
}