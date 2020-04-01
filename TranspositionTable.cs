using System;
using ColorShapeLinks.Common;
using System.Collections.Generic;

namespace BeeAI
{
    public class TranspositionTable
    {
        // Square / Shape / Color
        private uint[][][] zobristKey;
        private IDictionary<ulong, TableEntry> entries;
        private ulong key;
        public ulong CurrentKey {get; private set;}
        public TranspositionTable(int cols, int rows)
        {
            Random rnd = new Random();

            entries = new Dictionary<ulong, TableEntry>();
            
            // Zobrist key to hold the size of the board, 2 different colors
            // and 2 different shapes.
            int keySize = (cols * rows);
            zobristKey = new uint[cols * cols][][];
            // Init the key
            for (int i = 0; i < keySize; i++)
            {
                zobristKey[i] = new uint[2][];
                for (int j = 0; j < 2; j++)
                {
                    zobristKey[i][j] = new uint[2];
                    for (int z = 0; z < 2; z++)
                    {
                        zobristKey[i][j][z] = (uint) rnd.Next();   
                    }
                }
            }
        }

        public ulong UpdateHash(int column, int row, PColor color)
        {
            int shape = (int) color.Shape();
            // Xor occupied locations in turn
            key ^= zobristKey[column * row][shape][(int) color];
            
            return key;
        }
    }
}