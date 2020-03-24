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
        const int AMOUNT_PER_PIECE = 2;
        const int WIN_SET = 10;
        const int WIN_SETUP_HONEY = 100;

        // Heuristic function
        /// <summary>
        /// Heuristic for the Bee AI agent
        /// </summary>
        /// <param name="board"> Current game Board</param>
        /// <param name="color"> Color to do the heuristic for</param>
        /// <param name="placedPieces"> Number of placed pieces from both players</param>
        /// <returns> Heuristic value of the board</returns>
        public static float Honeycomb(Board board, PColor color, int placedPieces)
        {
            // Max points the ai can hold
            float h = 0;
            float s = 0;
            float side = 0;

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
            
            side = board.piecesInSequence;
            s = (side + side + side) / 2;

            // Calculate de area of the diagonal, casting it to an integer already
            // corrects to the number of pieces needed to complete a diagonal.
            areaOfDiagonal = (int)MathF.Sqrt
                (s * (s - side) * (s - side) * (s - side));
            
            // Give it 2 turns of advance to search for possible diagonals
            areaOfDiagonal -= 2;
            winCorridorDepth = placedPieces >= areaOfDiagonal ? wCor.Count() : winHVCount;
            
            int iteration = 0;
            PShape shape = color == PColor.White ? PShape.Round : PShape.Square;

            foreach(IEnumerable<Pos> corridor in wCor)
            {
                // Number of pieces found in this corridor
                int corridorCount = 0;
                // Treat each win corridor
                foreach(Pos p in corridor)
                {
                    Piece? piece = board[p.row, p.col];
                    if (piece.HasValue)
                    {
                        if (piece.Value.Is(color, color.Shape()))
                        {
                            h += AMOUNT_PER_PIECE;
                            corridorCount++;
                        }
                        else if (piece.Value.Is(color.Other(), color.Shape()))
                        {
                            h += AMOUNT_PER_PIECE / 2;
                            corridorCount++;
                        }
                        else
                        {
                            h -= AMOUNT_PER_PIECE;
                            corridorCount = 0;
                        }
                    }
                    else if (corridorCount > board.piecesInSequence - 1)
                    {
                        h += WIN_SET;
                    }
                    else
                    {
                        corridorCount = 0;
                    }
                }
                
                // Increment iteration, if it's higher than the heuristic search
                // depth then brake and return the current board value
                iteration++;
                if (iteration > winCorridorDepth) break;
            }

            return h;
        }
    }
}