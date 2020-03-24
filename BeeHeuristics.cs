using System;
using System.Threading;
using ColorShapeLinks.Common;
using ColorShapeLinks.Common.AI;
using System.Collections.Generic;

namespace BeeAI
{
    public static class BeeHeuristics
    {
        // Heuristic function
        public static float Heuristic(Board board, PColor color)
        {
            // Max points the ai can hold
            float maxPoints = 100;
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

                    }
                }
            }
            // Return the final heuristic score for the given board
            return h;
        }
    }
}