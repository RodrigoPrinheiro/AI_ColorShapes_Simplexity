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
        // Heuristic function
        /// <summary>
        /// 
        /// </summary>
        /// <param name="board"></param>
        /// <param name="color"></param>
        /// <param name="placedPieces"></param>
        /// <param name="diagonalA"></param>
        /// <returns></returns>
        public static float Honeycomb(Board board, PColor color, int placedPieces, int diagonalA)
        {
            // Max points the ai can hold
            float maxPoints = 100;
            float h = 0;
            float s = 0;
            float p = 0;

            int winCorridorDepth;
            int areaOfDiagonal;
            // Win horizontal and vertical count
            int winHVCount = 0;

            // List to hold the win corridors of the game
            // PASS THIS TO THE AI INSTANCE AFTER AS THIS REMAINS THE SAME FOR THE
            // WHOLE RUNTIME.
            IEnumerable<IEnumerable<Pos>> wCor = board.winCorridors;

            // Loop through the win Corridors looking for pieces, if there isn't
            // enough pieces for diagonals then only loop through the first 2
            // collections.
            if (board.rows >= board.piecesInSequence)
                winHVCount += board.cols;
            if (board.cols >= board.piecesInSequence)
                winHVCount += board.rows;
            
            p = board.piecesInSequence;
            s = (p + p + p) / 2;
            areaOfDiagonal = (int)MathF.Sqrt(s * (s - p) * (s - p) * (s - p));
            // Give it 2 turns of advance to search for possible diagonals
            areaOfDiagonal -= 2;
            winCorridorDepth = placedPieces >= diagonalA ? wCor.Count() : winHVCount;
            
            int iteration = 0;
            foreach(IEnumerable<Pos> corridor in wCor)
            {
                iteration++;
                if (iteration > winCorridorDepth) break;

                // treat each win corridor
                foreach(Pos position in corridor)
                {
                    if (board.)
                }
            }

            return h;
        }
    }
}