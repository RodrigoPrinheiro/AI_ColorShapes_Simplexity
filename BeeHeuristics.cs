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
        const float AMOUNT_PIECE = 4;
        const float AMOUNT_COLOR = 1;
        const float AMOUNT_SHAPE = 2;
        const float WIN = float.PositiveInfinity;

        // Heuristic function
        /// <summary>
        /// Heuristic for the Bee AI agent
        /// </summary>
        /// <param name="board"> Current game Board</param>
        /// <param name="color"> Color to do the heuristic for</param>
        /// <param name="placedPieces"> Number of placed pieces from both players</param>
        /// <returns> Heuristic value of the board</returns>
        public static float Honeycomb(Board board, PColor color)
        {
            // Max points the ai can hold
            float h = 0;

            foreach (IEnumerable<Pos> line in board.winCorridors)
            {
                int piecesInLine = 0;
                foreach (Pos pos in line)
                {
                    bool canUseLine = true;
                    if (board[pos.row, pos.col].HasValue)
                    {
                        Piece p = board[pos.row, pos.col].Value;

                        if (color.FriendOf(p) && canUseLine)
                        {
                            h += AMOUNT_PIECE;
                            piecesInLine++;
                        }
                        else
                        {
                            canUseLine = false;
                            h -= AMOUNT_PIECE;
                            piecesInLine = 0;
                        }

                    }
                }

                if (piecesInLine == board.piecesInSequence - 1)
                {
                    h += AMOUNT_PIECE * board.piecesInSequence;
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

            for (int i = minY; i < maxY; i++)
            {
                for (int j = minX; j < maxX; j++)
                {

                    if (!board[i, j].HasValue || (i == y && j == x)) continue;
                    Piece p = board[i, j].Value;

                    // If the piece curresponds to the player
                    if (p.color == color && p.shape == color.Shape())
                    {
                        pieceHeuristic += AMOUNT_PIECE;
                    }
                    else if (p.color != color && p.shape != color.Shape())
                    {
                        pieceHeuristic -= AMOUNT_PIECE;
                    }
                    // If the piece has only the same color
                    else if (p.color == color && p.shape != color.Shape())
                    {
                        pieceHeuristic += AMOUNT_COLOR;
                        pieceHeuristic -= AMOUNT_SHAPE;
                    }
                    else if (p.color != color && p.shape == color.Shape())
                    {
                        pieceHeuristic -= AMOUNT_COLOR;
                        pieceHeuristic += AMOUNT_SHAPE;
                    }
                }
            }

            return pieceHeuristic;
        }
    }
}