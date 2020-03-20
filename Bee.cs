using ColorShapeLinks.Common.AI;

namespace BeeAI
{
    public class Bee : AbstractThinker
    {
        public override FutureMove Think(ColorShapeLinks.Common.Board board, System.Threading.CancellationToken ct)
        {
            return FutureMove.NoMove;
        }
    }
}