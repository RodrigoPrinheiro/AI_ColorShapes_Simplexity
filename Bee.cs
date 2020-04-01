using System;
using System.Diagnostics;
using System.Threading;
using ColorShapeLinks.Common;
using ColorShapeLinks.Common.AI;

namespace BeeAI
{
    public class Bee : AbstractThinker
    {
        /// <summary>
        /// Infinity constant to facilitate reading the code.
        /// </summary>
        private const float INFINITY = float.PositiveInfinity;
        /// <summary>
        /// Debug variable to see how many iterations the AI did.
        /// </summary>        
        private int iterations;
        private float previous;
        public override void Setup(string str)
        {
            base.Setup(str);
            Random random = new Random();
            previous = 0;

            
        }

        public override FutureMove Think(Board board, CancellationToken ct)
        {
            iterations = 0;
            // Invoke minimax, starting with zero depth
            (FutureMove move, float score) decision = NegaScout(board, ct, board.Turn, 0, 3
            , -INFINITY, INFINITY);
            
            previous = decision.score;
            // Return best move
            return decision.move;
        }

        private  (FutureMove move, float score) Aspiration(Board board, CancellationToken ct,
            int maxDepth, float previous)
        {
            float alpha = previous - 100;
            float beta = previous + 100;

            while (true)
            {
                (FutureMove move, float score) result = NegaScout(board, ct, board.Turn, 0,
                     maxDepth, alpha, beta);

                if (result.score <= alpha)
                    alpha = -INFINITY;
                else if (result.score >= beta)
                    beta = INFINITY;
                else
                    return result;
            }
        }

        private(FutureMove move, float score) NegaScout(
            Board board, CancellationToken ct, PColor turn, int depth, int maxDepth,
            float alpha, float beta)
        {
            // Move to return and its heuristic value
            (FutureMove move, float score) best;
            // Current board state
            Winner winner;

            // If a cancellation request was made...
            if (ct.IsCancellationRequested)
            {
                // ...set a "no move" and skip the remaining part of
                // the algorithm
                return best = (FutureMove.NoMove, 0);
            }
            // Otherwise, if it's a final board, return the appropriate
            // evaluation
            else if ((winner = board.CheckWinner()) != Winner.None)
            {
                if (winner.ToPColor() == turn)
                {
                    // AI player wins, return highest possible score
                    return best = (FutureMove.NoMove, INFINITY);
                }
                else if (winner.ToPColor() == turn.Other())
                {
                    // Opponent wins, return lowest possible score
                    return best = (FutureMove.NoMove, -INFINITY);
                }
                else
                {
                    // A draw, return zero
                    return best = (FutureMove.NoMove, 0f);
                }
            }
            // If we're at maximum depth and don't have a final board, use
            // the heuristic
            else if (depth == maxDepth)
            {
                return best = (FutureMove.NoMove, BeeHeuristics.Honeycomb(board, turn));
            }
            // Initialize the selected move...
            best = (FutureMove.NoMove, -INFINITY);
            float adaptiveBeta = beta;

            // Test each column
            for (int i = 0; i < Cols; i++)
            {
                // Skip full columns
                if (board.IsColumnFull(i)) continue;

                // Test shapes
                for (int j = 0; j < 2; j++)
                {
                    // Order by the shape, play's the current turn shape first
                    // Get current shape
                    PShape shape = (PShape)j;

                    // Use this variable to keep the current board's score
                    float eval;

                    // Skip unavailable shapes
                    if (board.PieceCount(turn, shape) == 0) continue;

                    // Test move, call minimax and undo move
                    board.DoMove(shape, i);
                    // Get score and witch the sign
                    eval = -ABNegamax(board, ct, turn.Other(), depth + 1, maxDepth,
                         -adaptiveBeta, -Math.Max(alpha, best.score)).score;
                    board.UndoMove();

                    if (eval > best.score)
                    {
                        // Do a regular Negamax search if this is true
                        if (adaptiveBeta == beta || depth >= maxDepth - 2)
                        {
                            best.score = eval;
                            best.move = new FutureMove(i, shape);
                        }
                        // Else call the Negascout again
                        else
                        {
                            float bestNegativeScore;
                            board.DoMove(shape, i);
                            (best.move, bestNegativeScore) = NegaScout(board, ct,
                                turn.Other(), depth, maxDepth, -beta, -eval);
                            best.score = -bestNegativeScore;
                            board.UndoMove();
                        }

                        if (best.score >= beta)
                        {
                            return best;
                        }

                        // Update window
                        adaptiveBeta = Math.Max(alpha, best.score) + 1;
                    }
                }
            }

            // Return movement and its heuristic value
            return best;
        }

        // Minimax implementation
        private(FutureMove move, float score) ABNegamax(
            Board board, CancellationToken ct, PColor turn, int depth, int maxDepth,
            float alpha, float beta)
        {
            iterations++;
            // Move to return and its heuristic value
            (FutureMove move, float score) best;
            // Current board state
            Winner winner;

            // If a cancellation request was made...
            if (ct.IsCancellationRequested)
            {
                // ...set a "no move" and skip the remaining part of
                // the algorithm
                return best = (FutureMove.NoMove, 0);
            }
            // Otherwise, if it's a final board, return the appropriate
            // evaluation
            else if ((winner = board.CheckWinner()) != Winner.None)
            {
                if (winner.ToPColor() == turn)
                {
                    // AI player wins, return highest possible score
                    return best = (FutureMove.NoMove, INFINITY);
                }
                else if (winner.ToPColor() == turn.Other())
                {
                    // Opponent wins, return lowest possible score
                    return best = (FutureMove.NoMove, -INFINITY);
                }
                else
                {
                    // A draw, return zero
                    return best = (FutureMove.NoMove, 0f);
                }
            }
            // If we're at maximum depth and don't have a final board, use
            // the heuristic
            else if (depth == maxDepth)
            {
                return best = (FutureMove.NoMove, BeeHeuristics.Honeycomb(board, turn));
            }

            // Initialize the selected move...
            best = (FutureMove.NoMove, -INFINITY);

            // Test each column
            for (int i = 0; i < Cols; i++)
            {
                // Skip full columns
                if (board.IsColumnFull(i)) continue;

                // Test shapes
                for (int j = 0; j < 2; j++)
                {
                    // Order by the shape, play's the current turn shape first
                    // Get current shape
                    PShape shape = (PShape)j;

                    // Use this variable to keep the current board's score
                    float eval;

                    // Skip unavailable shapes
                    if (board.PieceCount(turn, shape) == 0) continue;

                    // Test move, call minimax and undo move
                    board.DoMove(shape, i);
                    // Get score and witch the sign
                    eval = -ABNegamax(board, ct, turn.Other(), depth + 1, maxDepth,
                         -beta, -Math.Max(alpha, best.score)).score;
                    board.UndoMove();

                    if (eval > best.score)
                    {
                        best.score = eval;
                        best.move = new FutureMove(i, shape);

                        // Prune the branch
                        if (best.score >= beta) return best;
                    }

                }
            }
            
            return best;
        }
    }
}