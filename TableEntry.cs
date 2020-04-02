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
        public float MaxScore {get; set;}
        public float MinScore {get; set;}
        public FutureMove Move{get; set;}
        public int Depth{get; set;}

    }
}