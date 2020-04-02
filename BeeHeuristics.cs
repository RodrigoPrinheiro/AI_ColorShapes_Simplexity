using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ColorShapeLinks.Common;
using ColorShapeLinks.Common.AI;

namespace BeeAI
{
    public static class BeeHeuristics
    {
        /// <summary>
        /// The base max value that will decrement
        /// </summary>
        public const float WIN_VALUE = 1000;
        /// <summary>
        /// The base max value that will decrement
        /// </summary>
        private const float START_VALUE = 100;

        /// <summary>
        /// The amount of points to remove if the color ain't the same
        /// </summary>
        const float AMOUNT_COLOR = 1;
        /// <summary>
        /// The amount of points to remove if the shape ain't the same
        /// </summary>
        const float AMOUNT_SHAPE = 2;
        /// <summary>
        /// If a piece is the complete opposite
        /// </summary>
        const float AMOUNT_PIECE = AMOUNT_COLOR + AMOUNT_SHAPE;

        // Heuristic function
        /// <summary>
        /// Heuristic for the Bee AI agent
        /// </summary>
        /// <param name="board"> Current game Board</param>
        /// <param name="color"> Color to do the heuristic for</param>
        /// <returns> Heuristic value of the board</returns>
        public static float Honeycomb(Board board, PColor color)
        {
            // Max points the ai can hold
            float h = START_VALUE;
            bool enemyCheckFound = false;
            Board boardCop = board.Copy();
            int lasCol = boardCop.UndoMove().col;

            // Run through every win corridor
            foreach (IEnumerable<Pos> line in board.winCorridors)
            {
                float Dist(float x1, float y1, float x2, float y2)
                {
                    return (float) Math.Sqrt(
                        Math.Pow(x1 - x2, 2) + Math.Pow(y1 - y2, 2));
                }

                // Set defaults
                int allyPiecesInLine = 0;
                int enemyPiecesInLine = 0;
                bool canUseLine = true;
                bool enemyCanUseLine = true;
                // Distance between two points

                // Determine the center row
                float centerRow = board.rows / 2;
                float centerCol = board.cols / 2;

                // Maximum points a piece can be awarded when it's at the center
                float maxPoints = Dist(centerRow, centerCol, 0, 0);

                // Check every position on the win corridor
                foreach (Pos pos in line)
                {
                    // If there is no piece there
                    if (!board[pos.row, pos.col].HasValue) continue;

                    Piece p = board[pos.row, pos.col].Value;

                    // Check if the piece is friendly towards the given piece
                    if (color.FriendOf(p))
                    {
                        // Is the color different
                        if (color != p.color)
                            h -= AMOUNT_COLOR;
                        // Is the shape different
                        if (color.Shape() != p.shape)
                            h -= AMOUNT_SHAPE;

                        // The line is not corrupted
                        if (canUseLine)
                            allyPiecesInLine++;

                        h += Dist(centerRow, centerCol, pos.row, pos.col);

                        enemyCanUseLine = false;
                    }
                    // The line is unusable
                    else
                    {
                        canUseLine = false;
                        h -= AMOUNT_PIECE;

                        if (enemyCanUseLine)
                        {
                            enemyPiecesInLine++;
                        }

                        h -= Dist(centerRow, centerCol, pos.row, pos.col)/2;
                    }
                }

                // Do we have a winning sequence?
                if (allyPiecesInLine == board.piecesInSequence - 1)
                {
                    return WIN_VALUE;
                }
                // Does the enemy have a winning sequence?
                if (enemyPiecesInLine == board.piecesInSequence - 1)
                {
                    enemyCheckFound = true;
                }

                // Ome final uneven point
                h -= enemyPiecesInLine - allyPiecesInLine;
            }

            // Can the enemy win next turn
            return enemyCheckFound ? float.NegativeInfinity : h;
        }

    }
}