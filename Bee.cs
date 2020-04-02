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
        private const float TIMER_WIGGLE_ROOM_FACTOR = 0.15f;
        private int iterations;
        private TranspositionTable hashTable;
        private ulong currentKey;
        private DateTime startTime, endTime;
        private (FutureMove move, float score) previousDecision; 
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
            previousDecision = default;
        }

        public override FutureMove Think(Board board, CancellationToken ct)
        {
            startTime = DateTime.Now;

            iterations = 0;

            currentKey = hashTable.HashBoard(board);
            Console.WriteLine(hashTable.Entries);

            (FutureMove move, float score) finalDecision = default;
            (FutureMove move, float score) tempdecision = default;

            for (int maxDepth = 2; 0 < 1; maxDepth++)
            {
                // Invoke minimax, starting with zero depth
                tempdecision = ABNegamax(board, ct, board.Turn, 0, maxDepth, 0, currentKey);

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
                float gamma, ulong nodeKey)
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
                return best = (FutureMove.NoMove, -INFINITY);
            }

            if (DeepeningTimeIsUp)
            {
                return previousDecision;
            }

            TableEntry nodeEntry;
            if (hashTable.GetEntry(nodeKey, out nodeEntry))
            {
                if (nodeEntry.Depth > maxDepth - depth)
                {
                    if (nodeEntry.MinScore > gamma)
                    {
                        return (nodeEntry.Move, nodeEntry.MinScore);
                    }
                    else if (nodeEntry.MaxScore < gamma)
                    {
                        return (nodeEntry.Move, nodeEntry.MaxScore);
                    }
                    else
                    {
                        nodeEntry.Depth = maxDepth - depth;
                        nodeEntry.MinScore = -INFINITY;
                        nodeEntry.MaxScore = INFINITY;
                    }

                }
            }

            // Otherwise, if it's a final board, return the appropriate
            // evaluation
            if ((winner = board.CheckWinner()) != Winner.None)
            {
                if (winner.ToPColor() == turn)
                {
                    nodeEntry.MinScore = nodeEntry.MaxScore = INFINITY;
                    hashTable.StoreKey(nodeKey, nodeEntry);
                    // AI player wins, return highest possible score
                    return best = (FutureMove.NoMove, INFINITY);
                }
                else if (winner.ToPColor() == turn.Other())
                {
                    nodeEntry.MinScore = nodeEntry.MaxScore = -INFINITY;
                    hashTable.StoreKey(nodeKey, nodeEntry);
                    // Opponent wins, return lowest possible score
                    return best = (FutureMove.NoMove, -INFINITY);
                }
                else
                {
                    nodeEntry.MinScore = nodeEntry.MaxScore = 0;
                    hashTable.StoreKey(nodeKey, nodeEntry);
                    // A draw, return zero
                    return best = (FutureMove.NoMove, 0f);
                }
            }
            // If we're at maximum depth and don't have a final board, use
            // the heuristic
            if (depth == maxDepth)
            {
                nodeEntry.MinScore = nodeEntry.MaxScore = BeeHeuristics.Honeycomb(board, turn);
                    hashTable.StoreKey(nodeKey, nodeEntry);
                return best = (FutureMove.NoMove, nodeEntry.MinScore);
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
                    eval = -ABNegamax(board, ct, turn.Other(), depth + 1, maxDepth,
                        -gamma, nextNodeKey).score;
                    board.UndoMove();

                    if (eval > best.score)
                    {
                        best.score = eval;
                        best.move = new FutureMove(i, shape);
                        nodeEntry.Move = best.move;
                    }
                }
            }
            
            if (best.score < gamma)
                nodeEntry.MaxScore = best.score;
            else
                nodeEntry.MinScore = best.score;

            hashTable.StoreKey(nodeKey, nodeEntry);
            return best;
        }
    }
}