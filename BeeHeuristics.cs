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
        public static int Heuristic(Board board, PColor color, int placedPieces)
        {
            // Max points the ai can hold
            float maxPoints = 100;
            float h = 0;
            int areaOfDiagonal;
            int winCorridorDepth;
            // List to hold the win corridors of the game
            // PASS THIS TO THE AI INSTANCE AFTER AS THIS REMAINS THE SAME FOR THE
            // WHOLE RUNTIME.
            IEnumerable<IEnumerable<Pos>> wCor = board.winCorridors;
            // Loop through the win Corridors looking for pieces, if there isn't
            // enough pieces for diagonals then only loop through the first 2
            // collections.

            // Calculate the area of a diagonal triangle
            float p = board.piecesInSequence;
            float s = (p + p + p) / 2;
            areaOfDiagonal = (int)MathF.Sqrt(s * (s - p) * (s - p) * (s - p));

            // Give it 2 turns of advance to search for possible diagonals
            areaOfDiagonal -= 2;
            
            winCorridorDepth = placedPieces >= areaOfDiagonal ? wCor.Count : 4;
            // for (int i = 0; i <)
            // {

            // }

            return wCor.Count;
        }
    }
}