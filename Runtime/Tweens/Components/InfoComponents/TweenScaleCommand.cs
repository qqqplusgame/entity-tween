using Unity.Entities;
using Unity.Mathematics;

namespace Timespawn.EntityTween.Tweens
{
    /// <summary>
    /// Uniform scale tween.
    /// </summary>
    internal struct TweenScaleCommand : IComponentData, ITweenParams, ITweenInfo<float>
    {
        public TweenParams TweenParams;
        public float Start;
        public float End;

        public TweenScaleCommand(in TweenParams tweenParams, in float start, in float end)
        {
            TweenParams = tweenParams;
            Start = start;
            End = end;
        }

        public void SetTweenInfo(in float start, in float end)
        {
            Start = start;
            End = end;
        }

        public void SetTweenParams(in TweenParams tweenParams)
        {
            TweenParams = tweenParams;
        }

        public TweenParams GetTweenParams()
        {
            return TweenParams;
        }

        public float GetTweenStart()
        {
            return Start;
        }

        public float GetTweenEnd()
        {
            return End;
        }
    }
}