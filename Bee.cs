using System;
using System.Diagnostics;
using System.Threading;
using ColorShapeLinks.Common;
using ColorShapeLinks.Common.AI;

namespace BeeAI
{
    public class Bee : AbstractThinker
    {
        private const float INFINITY = float.PositiveInfinity;
        private const float TIMER_WIGGLE_ROOM_FACTOR = 0.10f;
        private Stopwatch stopwatch;
        private int iterations;

        private bool DeepeningTimeIsUp => 
            stopwatch.ElapsedMilliseconds > 
            TimeLimitMillis - TimeLimitMillis * TIMER_WIGGLE_ROOM_FACTOR;

        public override void Setup(string str)
        {
            base.Setup(str);
            // Try to get the maximum depth from the parameters
            stopwatch = new Stopwatch();
        }

        public override FutureMove Think(Board board, CancellationToken ct)
        {
            // Start the time count
            stopwatch.Start();
            iterations = 0;

            // Invoke minimax, starting with zero depth
            (FutureMove move, float score) decision =
                ABNegamax(board, ct, board.Turn, 0, -INFINITY, INFINITY);

            // Stop time count
            stopwatch.Reset();
            Console.WriteLine("Iterations: " + iterations);

            // Return best move
            return decision.move;
        }

        // Minimax implementation
        private(FutureMove move, float score) ABNegamax(
            Board board, CancellationToken ct, PColor turn, int depth,
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
                best = (FutureMove.NoMove, float.NaN);
            }
            // Otherwise, if it's a final board, return the appropriate
            // evaluation
            else if ((winner = board.CheckWinner()) != Winner.None)
            {
                if (winner.ToPColor() == turn)
                {
                    // AI player wins, return highest possible score
                    best = (FutureMove.NoMove, INFINITY);
                }
                else if (winner.ToPColor() == turn.Other())
                {
                    // Opponent wins, return lowest possible score
                    best = (FutureMove.NoMove, -INFINITY);
                }
                else
                {
                    // A draw, return zero
                    best = (FutureMove.NoMove, 0f);
                }
            }
            // If we're at maximum depth and don't have a final board, use
            // the heuristic
            else if (DeepeningTimeIsUp)
            {
                best = (FutureMove.NoMove, BeeHeuristics.Honeycomb(board, turn));
            }
            else // Board not final and depth not at max...
            {
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
                        // Get current shape
                        PShape shape = (PShape) j;

                        // Use this variable to keep the current board's score
                        float eval;

                        // Skip unavailable shapes
                        if (board.PieceCount(turn, shape) == 0) continue;

                        // Test move, call minimax and undo move
                        board.DoMove(shape, i);
                        eval = -ABNegamax(board, ct, turn.Other(), depth + 1, -beta, -Math.Max(alpha, best.score)).score;
                        board.UndoMove();

                        if (eval > best.score)
                        {
                            best.score = eval;
                            best.move = new FutureMove(i, shape);
                        }

                        if (best.score >= beta) break;
                    }
                }
            }

            // Return movement and its heuristic value
            return best;
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