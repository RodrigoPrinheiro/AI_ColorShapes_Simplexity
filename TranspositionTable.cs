using System;
using ColorShapeLinks.Common;
using System.Collections.Generic;
using ColorShapeLinks.Common.AI;

namespace BeeAI
{
    /// <summary>
    /// Transposition table used to store board positions and their
    /// corresponding move.
    /// </summary>
    public class TranspositionTable
    {
        // Square / Shape / Color
        /// <summary>
        /// Zobrist key array, ordered by Position, Shape and color.
        /// </summary>
        private readonly uint[][][] zobristKey;
        /// <summary>
        /// Dictionary to store the Transposition Table entries,
        /// Key value is the hashed board position and value is
        /// the move, score of the move and score type.
        /// </summary>
        private IDictionary<ulong, TableEntry> entries;
        /// <summary>
        /// Cols size of the board to hash for
        /// </summary>
        private int cols;
        /// <summary>
        /// Rows size of the board to hash for
        /// </summary>
        private int rows;
        /// <summary>
        /// Number of entries in the dictionary to see from outside
        /// </summary>
        public int Entries => entries.Count;
        /// <summary>
        /// Public constructor.
        /// </summary>
        /// <param name="cols"> Column size of the board</param>
        /// <param name="rows"> Row size of the board</param>
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

        /// <summary>
        /// Public method to hash a board and get its corresponding key.
        /// </summary>
        /// <param name="board"> Board to hash</param>
        /// <returns> 
        /// Hashed board value, used as a key value in the transposition table
        /// </returns>
        public ulong HashBoard(Board board)
        {
            ulong result = 0;

            // Loop through the board
            for (int i = 0; i < cols; i++)
            {
                for (int j = 0; j < rows; j++)
                {
                    // Check if there is anything
                    Piece? piece = board[j, i];
                    if (!piece.HasValue) continue;
                    
                    // Xor the result with the key using the board position,
                    // color and shape.
                    result ^= zobristKey[j * cols + i]
                        [(int) piece.Value.shape][(int) piece.Value.color];
                }
            }

            return result;
        }

        /// <summary>
        /// Updates a hashed key with one extra position;
        /// </summary>
        /// <param name="column"> column position of the move</param>
        /// <param name="row"> row position of the move</param>
        /// <param name="color"> color of the piece</param>
        /// <param name="shape"> shape of the piece</param>
        /// <param name="oldKey"> old key to add the move to</param>
        /// <returns>
        /// New key with the desired move incorporated in.
        /// </returns>
        public ulong UpdateHash(int column, int row, PColor color, PShape shape, ulong oldKey)
        {
            ulong newKey = 0;
            // Xor occupied locations in turn
            newKey = oldKey ^ zobristKey[column * row][(int) shape][(int) color];
            
            return newKey;
        }

        /// <summary>
        /// Stores a given hashed board and value into the dictionary
        /// </summary>
        /// <param name="nodeKey"> board key to add</param>
        /// <param name="entry"> board value corresponding the key</param>
        /// <returns>
        /// The key stored in the dictionary for debug.
        /// </returns>
        public ulong StoreKey(ulong nodeKey, TableEntry entry)
        {
            if(!entries.ContainsKey(nodeKey))
            {
                entries.Add(nodeKey, entry);
                return nodeKey;
            }

            if (entries[nodeKey].Depth < entry.Depth)
            {
                entries[nodeKey] = entry;
            }
            

            return nodeKey;
        }

        /// <summary>
        /// Gets an entry from the Transposition table dictionary
        /// </summary>
        /// <param name="key"> key to search for</param>
        /// <param name="entry"> Value corresponding to the key</param>
        /// <returns>
        /// Returns true if the entry was found, false otherwise
        /// </returns>
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