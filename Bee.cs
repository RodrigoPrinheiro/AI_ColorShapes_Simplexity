using System;
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
        private TranspositionTable hashTable;
        private ulong currentKey;
        DateTime startTime, endTime;

        private const float TIMER_WIGGLE_ROOM_FACTOR = 0.15f;
        private bool DeepeningTimeIsUp
        {
            get
            {
                double elapsedTime =
                    ((TimeSpan) (DateTime.Now - startTime)).TotalMilliseconds;

                return elapsedTime >
                    TimeLimitMillis - TimeLimitMillis * TIMER_WIGGLE_ROOM_FACTOR;
            }
        }

        public override void Setup(string str)
        {
            base.Setup(str);
            hashTable = new TranspositionTable(Cols, Rows);
            currentKey = 0;
        }

        public override FutureMove Think(Board board, CancellationToken ct)
        {
            startTime = DateTime.Now;

            iterations = 0;

            currentKey = hashTable.HashBoard(board);

            (FutureMove move, float score) finalDecision = default;
            (FutureMove move, float score) previousDecision = default;
            (FutureMove move, float score) tempdecision = default;

            for (int maxDepth = 2; 0 < 1; maxDepth += 2)
            {
                // Invoke minimax, starting with zero depth
                tempdecision = ABNegamax(board, ct, board.Turn, 0, maxDepth, -INFINITY, INFINITY, currentKey);

                if (DeepeningTimeIsUp)
                {
                    finalDecision = previousDecision;
                    break;
                }

                previousDecision = tempdecision;
            }
            // Return best move
            return finalDecision.move;
        }

        // Minimax implementation
        private(FutureMove move, float score) ABNegamax(
            Board board, CancellationToken ct, PColor turn, int depth, int maxDepth,
            float alpha, float beta, ulong nodeKey)
        {
            iterations++;
            // Move to return and its heuristic value
            (FutureMove move, float score) best;
            // Current board state
            Winner winner;
            float oldAlpha = alpha;

            if (DeepeningTimeIsUp)
            {
                return best = (FutureMove.NoMove, float.NaN);
            }

            // If a cancellation request was made...
            if (ct.IsCancellationRequested)
            {
                // ...set a "no move" and skip the remaining part of
                // the algorithm
                return best = (FutureMove.NoMove, -INFINITY);
            }

            // Check if there is an entry in the hash table
            TableEntry nodeEntry;
            if (hashTable.GetEntry(nodeKey, out nodeEntry))
            {
                if (!board.IsColumnFull(nodeEntry.Move.column))
                {
                    if (nodeEntry.Depth >= depth)
                    {
                        if (nodeEntry.Type == ScoreType.Accurate)
                        {
                            return nodeEntry.Value;
                        }
                        else if (nodeEntry.Type == ScoreType.Alpha)
                        {
                            alpha = Math.Max(alpha, nodeEntry.Score);
                        }
                        else if (nodeEntry.Type == ScoreType.Beta)
                        {
                            beta = Math.Min(beta, nodeEntry.Score);
                        }

                        if (alpha >= beta)
                        {
                            return nodeEntry.Value;
                        }
                    }
                }

            }


            // Otherwise, if it's a final board, return the appropriate
            // evaluation
            if ((winner = board.CheckWinner()) != Winner.None)
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
            if (depth == maxDepth || (winner = board.CheckWinner()) != Winner.None)
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
                    // Temp variable to store the y coordinate in the board.
                    int row;
                    // Order by the shape, play's the current turn shape first
                    // Get current shape
                    PShape shape = (PShape) j;
                    // Use this variable to keep the current board's score
                    float eval;
                    ulong nextNodeKey = 0;
                    // Skip unavailable shapes
                    if (board.PieceCount(turn, shape) == 0) continue;

                    row = board.DoMove(shape, i);

                    // Test move, call minimax and undo move
                    // Update hash key for next Negamax node
                    nextNodeKey = hashTable.UpdateHash(i, row, turn, shape, nodeKey);
                    // Get score and witch the sign
                    eval = -ABNegamax(board, ct, turn.Other(), depth + 1, maxDepth, -beta, -Math.Max(alpha, best.score), nextNodeKey).score;
                    board.UndoMove();

                    if (eval > best.score)
                    {
                        best.score = eval;
                        best.move = new FutureMove(i, shape);

                        // Beta pruning the branch
                        if (best.score >= beta)
                        {
                            // Assign the done entry values
                            nodeEntry.Score = best.score;
                            nodeEntry.Type = ScoreType.Beta;
                            nodeEntry.Depth = depth;
                            nodeEntry.Move = best.move;
                            // Store move in hastable
                            hashTable.StoreKey(nodeKey, nodeEntry);
                            // Return the best value of this pruning
                            return best;
                        }
                    }
                }
            }
            
            if (best.score <= oldAlpha)
                nodeEntry.Type = ScoreType.Alpha;
            else
                nodeEntry.Type = ScoreType.Accurate;

            nodeEntry.Score = best.score;
            nodeEntry.Depth = depth;
            hashTable.StoreKey(nodeKey, nodeEntry);
            return best;
        }
    }
}