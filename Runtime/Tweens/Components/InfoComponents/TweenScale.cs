using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Timespawn.EntityTween.Tweens
{
    /// <summary>
    /// Uniform scale tween.
    /// </summary>
    [WriteGroup(typeof(LocalToWorldTransform ))]
    public struct TweenScale : IComponentData, ITweenId, ITweenInfo<float>
    {
        public int Id;
        public float Start;
        public float End;

        public TweenScale(in int id, in float start, in float end)
        {
            Id = id;
            Start = start;
            End = end;
        }

        public void SetTweenId(in int id)
        {
            Id = id;
        }

        public int GetTweenId()
        {
            return Id;
        }

        public void SetTweenInfo(in float start, in float end)
        {
            Start = start;
            End = end;
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