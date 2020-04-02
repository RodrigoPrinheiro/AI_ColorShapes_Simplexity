using System;
using ColorShapeLinks.Common;
using System.Collections.Generic;
using ColorShapeLinks.Common.AI;

namespace BeeAI
{
    public class TranspositionTable
    {
        // Square / Shape / Color
        private readonly uint[][][] zobristKey;
        private IDictionary<ulong, TableEntry> entries;
        private ulong key;
        private int cols;
        private int rows;
        public int Entries => entries.Count;
        public TranspositionTable(int cols, int rows)
        {
            Random rnd = new Random();

            entries = new Dictionary<ulong, TableEntry>(1000000);
            
            // Zobrist key to hold the size of the board, 2 different colors
            // and 2 different shapes.
            this.cols = cols;
            this.rows = rows;
            zobristKey = new uint[cols * cols][][];
            // Init the key
            for (int i = 0; i < cols * rows; i++)
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

        public ulong HashBoard(Board board)
        {
            ulong result = 0;

            for (int i = 0; i < cols; i++)
            {
                for (int j = 0; j < rows; j++)
                {
                    Piece? piece = board[j, i];
                    if (!piece.HasValue) continue;

                    result ^= zobristKey[i * cols + j]
                        [(int) piece.Value.shape][(int) piece.Value.color];
                }
            }

            return result;
        }

        public ulong UpdateHash(int column, int row, PColor color, PShape shape, ulong oldKey)
        {
            // Xor occupied locations in turn
            oldKey ^= zobristKey[column * row][(int) shape][(int) color];
            
            return oldKey;
        }

        public ulong StoreKey(ulong nodeKey, TableEntry entry)
        {
            if(!entries.TryAdd(nodeKey, entry))
            {
                if (entries[nodeKey].Depth < entry.Depth)
                {
                    entries[nodeKey] = entry;
                }
            }
            

            return nodeKey;
        }

        public bool GetEntry(ulong key, out TableEntry entry)
        {
            if (entries.ContainsKey(key))
            {
                entry = entries[key];
                return true;
            }
            entry = default;
            return false;
        }
    }
}