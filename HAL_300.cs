using System.Threading;
using ColorShapeLinks.Common;
using ColorShapeLinks.Common.AI;

namespace AI_Hal300
{
    public class HAL_300 : AbstractThinker
    {
        public override void Setup(string str)
        {
            base.Setup(str);
        }

        public override FutureMove Think(Board board, CancellationToken ct)
        {
            return FutureMove.NoMove;
        }
    }
}