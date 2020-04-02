using ColorShapeLinks.Common.AI;

namespace BeeAI
{
    /// <summary>
    /// Enumeratio for the table entry accuracy
    /// </summary>
    public enum ScoreType
    {
        Accurate,
        Alpha,
        Beta,
    }

    public struct TableEntry
    {
        public ScoreType Type {get; set;}
        public float Score {get; set;}
        public FutureMove Move{get; set;}
        public int Depth{get; set;}

        public (FutureMove move, float score) Value => (Move, Score);

    }
}