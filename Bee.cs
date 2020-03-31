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
        /// Constant with how much less time from the desqualification time the AI
        /// will have.
        /// </summary>
        private const float TIMER_WIGGLE_ROOM_FACTOR = 0.15f;

        private const int INITIAL_MAX_DEPTH = 2;
        /// <summary>
        /// Stopwatch to count how much time the AI has left to think
        /// </summary>
        private Stopwatch stopwatch;
        /// <summary>
        /// Debug variable to see how many iterations the AI did.
        /// </summary>        
        private int iterations;
        /// <summary>
        /// Order array containing the order in witch the AI should search for a move.
        /// </summary>
        private int[] columnOrdering;

        private int maxDepth;

        private bool DeepeningTimeIsUp =>
            stopwatch.ElapsedMilliseconds >
            TimeLimitMillis - TimeLimitMillis * TIMER_WIGGLE_ROOM_FACTOR;

        public override void Setup(string str)
        {
            base.Setup(str);
            // Try to get the maximum depth from the parameters
            stopwatch = new Stopwatch();

            columnOrdering = new int[Cols];
            // Setup the move order, go through the board width (x)
            for (int i = 0; i < Cols; i++)
            {
                // With i = 0 it will be the middle column of the board.
                // With i = 1 then it will be Cols/2 + (-1)
                // with i = 1 then it will be Cols/2 + 1
                // Etc until we have a distribution of middle of the board thowards the
                // edges.
                // Ends up being 3, 2, 4, 1, 5, 0, 6 in a regular size board.
                columnOrdering[i] = Cols / 2 + (1 - 2 * (i % 2)) * (i + 1) / 2;
                Console.WriteLine(columnOrdering[i]);
            }
            maxDepth = INITIAL_MAX_DEPTH;
        }

        public override FutureMove Think(Board board, CancellationToken ct)
        {
            // Start the time count
            stopwatch.Start();
            iterations = 0;
            (FutureMove move, float score, Board board) decision =
                (FutureMove.NoMove, -INFINITY, board);
            (FutureMove move, float score, Board board) lastDepthDecision = decision;

            while (!DeepeningTimeIsUp)
            {
                lastDepthDecision = decision;

                // Invoke minimax, starting with zero depth
                decision =
                    ABNegamax(decision.board, ct, decision.board.Turn, 0, -INFINITY, decision.score);

                if (decision.score == float.NaN) decision = lastDepthDecision;
            }

            // Stop time count
            stopwatch.Reset();
            Console.WriteLine("Iterations: " + iterations);
            Console.WriteLine("Decision: " + decision.move.column);

            // Return best move
            return decision.move;
        }

        // Minimax implementation
        private(FutureMove move, float score, Board board) ABNegamax(
            Board board, CancellationToken ct, PColor turn, int depth,
            float alpha, float beta)
        {
            iterations++;
            // Move to return and its heuristic value
            (FutureMove move, float score, Board board) best;

            // Current board state
            Winner winner;

            // If a cancellation request was made...
            if (ct.IsCancellationRequested)
            {
                // ...set a "no move" and skip the remaining part of
                // the algorithm
                best = (FutureMove.NoMove, float.NaN, board);
            }
            // Otherwise, if it's a final board, return the appropriate
            // evaluation
            else if ((winner = board.CheckWinner()) != Winner.None)
            {
                if (winner.ToPColor() == turn)
                {
                    // AI player wins, return highest possible score
                    best = (FutureMove.NoMove, INFINITY, board);
                }
                else if (winner.ToPColor() == turn.Other())
                {
                    // Opponent wins, return lowest possible score
                    best = (FutureMove.NoMove, -INFINITY, board);
                }
                else
                {
                    // A draw, return zero
                    best = (FutureMove.NoMove, 0f, board);
                }
            }
            else if (DeepeningTimeIsUp)
            {
                best = (FutureMove.NoMove, float.NaN, null);
            }
            // If we're at maximum depth and don't have a final board, use
            // the heuristic
            else if (depth == maxDepth)
            {
                best = (FutureMove.NoMove, BeeHeuristics.Honeycomb(board, turn), board);
            }
            else // Board not final and depth not at max...
            {
                // Initialize the selected move...
                best = (FutureMove.NoMove, -INFINITY, board);

                // Test each column
                for (int i = 0; i < Cols; i++)
                {
                    // Skip full columns
                    if (board.IsColumnFull(columnOrdering[i])) continue;

                    // Test shapes
                    for (int j = 0; j < 2; j++)
                    {
                        // Get current shape
                        PShape shape = (PShape) j;

                        // Use this variable to keep the current board's score
                        float eval = -INFINITY;

                        // Skip unavailable shapes
                        if (board.PieceCount(turn, shape) == 0) continue;

                        // Test move, call minimax and undo move
                        board.DoMove(shape, columnOrdering[i]);
                        eval = -ABNegamax(board, ct, turn.Other(), depth + 1, -beta, -Math.Max(alpha, best.score)).score;
                        board.UndoMove();

                        if (eval == float.NaN)
                        {
                            best.score = eval;
                            best.move = FutureMove.NoMove;
                            return best;
                        }

                        if (eval > best.score)
                        {
                            best.score = eval;
                            best.move = new FutureMove(columnOrdering[i], shape);
                        }

                        if (best.score >= beta) break;
                    }
                }

                // if (!DeepeningTimeIsUp && depth + 1 == maxDepth)
                // {
                //     (FutureMove move, float score, Board board) depthBest = best;
                //     best = ABNegamax(board, ct, turn, depth, alpha, beta);

                //     if (best.score == float.NaN) 

                //     maxDepth = depth + 1;
                // }
            }

            // Return movement and its heuristic value
            return best;
        }
    }
}