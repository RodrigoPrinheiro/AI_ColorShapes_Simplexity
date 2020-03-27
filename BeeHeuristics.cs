using System;
using System.Threading;
using ColorShapeLinks.Common;
using ColorShapeLinks.Common.AI;
using System.Collections.Generic;
using System.Linq;

namespace BeeAI
{
    public static class BeeHeuristics
    {
        // Amount per
        const float AMOUNT_PIECE = 3;
        const float AMOUNT_COLOR = 1;
        const float AMOUNT_SHAPE = 4;
        const float LOST = 1000;

        // Heuristic function
        /// <summary>
        /// Heuristic for the Bee AI agent
        /// </summary>
        /// <param name="board"> Current game Board</param>
        /// <param name="color"> Color to do the heuristic for</param>
        /// <param name="placedPieces"> Number of placed pieces from both players</param>
        /// <returns> Heuristic value of the board</returns>
        public static float Honeycomb(Board board, PColor color, int turns)
        {
            // Max points the ai can hold
            float h = 0;

            foreach(IEnumerable<Pos> line in board.winCorridors)
            {
                foreach(Pos pos in line)
                {
                    if (board[pos.row, pos.col].HasValue)
                    {
                        int count = 0;
                        int enemy = 0;
                        Piece p = board[pos.row, pos.col].Value;
                        if (color.FriendOf(p))
                        {
                            h += ++count;
                            enemy = 0;
                        }
                        else
                        {
                            count = 0;
                            enemy++;
                        }
                        
                        if (count == board.piecesInSequence)
                        {
                            h += LOST;
                        }

                        if (enemy == board.piecesInSequence - 1)
                        {
                            h -= LOST;
                        }
                        
                    }
                }
            }

            for(int x = 0; x < board.cols; x++)
            {
                for(int y = 0; y < board.rows; y++)
                {
                    if (!board[y, x].HasValue) continue;

                    Piece p = board[y, x].Value;
                    if (p.color == color || p.shape == color.Shape())
                    {
                        h += NeighborsValue(x, y, color, board);
                    }
                }
            }
            return h;
        }

        private static float NeighborsValue(int x, int y, PColor color, Board board)
        {
            int minX = Math.Max(x - 1, 0);
            int maxX = Math.Max(x + 1, board.cols);
            int minY = Math.Max(y - 1, 0);
            int maxY = Math.Max(y - 1, board.rows);

            float pieceHeuristic = 0;

            for(int i = minY; i < maxY; i++)
            {
                for(int j = minX; j < maxX; j++)
                {
                    
                    if (!board[i, j].HasValue || (i == y && j == x)) continue;
                    Piece p = board[i, j].Value;

                    // If the piece curresponds to the player
                    if (color.FriendOf(p))
                    {
                        pieceHeuristic += AMOUNT_PIECE;
                    }
                }
            }

            return pieceHeuristic;
        }
    }
}