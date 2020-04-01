using ColorShapeLinks.Common.AI;

namespace BeeAI
{
    /// <summary>
    /// Enumeratio for the table entry accuracy
    /// </summary>
    public enum ScoreType
    {
        Accurate,
        AlphaPruned,
        BetaPruned,
    }

    public struct TableEntry
    {
        private ScoreType scoreType;
        private float score;
        private FutureMove move;
        private int depth;
    }
}