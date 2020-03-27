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

            float Dist(float x1, float y1, float x2, float y2)
            {
                return (float)Math.Sqrt(
                    Math.Pow(x1 - x2, 2) + Math.Pow(y1 - y2, 2));
            }

            // Determine the center row
            float centerRow = board.rows / 2;
            float centerCol = board.cols / 2;

            // Maximum points a piece can be awarded when it's at the center
            float maxPoints = Dist(centerRow, centerCol, 0, 0);

            // Loop through the board looking for pieces
            for (int i = 0; i < board.rows; i++)
            {
                for (int j = 0; j < board.cols; j++)
                {
                    if (!board[i, j].HasValue) continue;
                    // Get piece in current board position
                    Piece piece = board[i, j].Value;

                    // Is there any piece there?
                    // If the piece is of our color, increment the
                    // heuristic inversely to the distance from the center
                    if (piece.color == color)
                        h += maxPoints - Dist(centerRow, centerCol, i, j);
                    // Otherwise decrement the heuristic value using the
                    // same criteria
                    else
                        h -= maxPoints - Dist(centerRow, centerCol, i, j);
                    // If the piece is of our shape, increment the
                    // heuristic inversely to the distance from the center
                    if (piece.shape == color.Shape())
                        h += maxPoints - Dist(centerRow, centerCol, i, j);
                    // Otherwise decrement the heuristic value using the
                    // same criteria
                    else
                        h -= maxPoints - Dist(centerRow, centerCol, i, j);

                    // Search for the neighbours of the piece if it is from the player
                    // For each color or shape nearby increase the score.
                    if (piece.color == color || piece.shape == color.Shape())
                    {
                        h += NeighborsValue(j, i, color, board);
                    }
                }
            }
            foreach (IEnumerable<Pos> line in board.winCorridors)
            {
                int piecesInLine = 0;
                foreach (Pos pos in line)
                {
                    if (board[pos.row, pos.col].HasValue)
                    {
                        Piece p = board[pos.row, pos.col].Value;

                        if (!color.FriendOf(p))
                        {
                            piecesInLine++;
                        }
                        else
                        {
                            piecesInLine = 0;
                        }
                    }
                }

                if (piecesInLine >= board.piecesInSequence)
                {
                    h -= WIN;
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