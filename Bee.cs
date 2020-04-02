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
        private TranspositionTable hashTable;
        private ulong currentKey;
        public override void Setup(string str)
        {
            base.Setup(str);
            Random random = new Random();
            hashTable = new TranspositionTable(Cols, Rows);
            currentKey = 0;
        }

        public override FutureMove Think(Board board, CancellationToken ct)
        {
            iterations = 0;

            Console.WriteLine(hashTable.Entries);
            
            currentKey =  hashTable.HashBoard(board);
            Console.WriteLine(currentKey);
            
            // Invoke minimax, starting with zero depth
            (FutureMove move, float score) decision = ABNegamax(board, ct, board.Turn, 0, 2
            , -INFINITY, INFINITY, currentKey);
            
            Console.WriteLine(decision.move.column);
            // Return best move
            return decision.move;
        }

        private int GetLastRow(Board board, FutureMove decision)
        {
            int decisionRow = board.DoMove(decision.shape, decision.column);
            // Undo the move
            board.UndoMove();
            return decisionRow;
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

            // If a cancellation request was made...
            if (ct.IsCancellationRequested)
            {
                // ...set a "no move" and skip the remaining part of
                // the algorithm
                return best = (FutureMove.NoMove, 0);
            }

            // Check if there is an entry in the hash table
            TableEntry nodeEntry;
            if (hashTable.GetEntry(nodeKey, out nodeEntry))
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
                    // Temp variable to store the y coordinate in the board.
                    int row;
                    // Order by the shape, play's the current turn shape first
                    // Get current shape
                    PShape shape = (PShape) j == 0 ? turn.Shape() : turn.Other().Shape();
                    // Use this variable to keep the current board's score
                    float eval;
                    ulong nextNodeKey = 0;
                    // Skip unavailable shapes
                    if (board.PieceCount(turn, shape) == 0) continue;

                    // Test move, call minimax and undo move
                    row = board.DoMove(shape, i);
                    // Update hash key for next Negamax node
                    nextNodeKey = hashTable.UpdateHash(i, row, turn, shape, nodeKey);
                    // Get score and witch the sign
                    eval = -ABNegamax(board, ct, turn.Other(), depth + 1, maxDepth,
                         -beta, -Math.Max(alpha, best.score), nextNodeKey).score;
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
            
            if (best.score != oldAlpha)
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