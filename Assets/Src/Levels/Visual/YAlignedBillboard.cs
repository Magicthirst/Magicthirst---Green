using Levels.Extensions;

namespace Levels.Visual
{
    public sealed class YAlignedBillboard : Billboard
    {
        protected override void Update()
        {
            transform.LookAt(Camera.position.With(y: transform.position.y));
        }
    }
}
