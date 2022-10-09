using Timespawn.EntityTween.Tweens;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Timespawn.EntityTween
{
    [UpdateInGroup(typeof(TweenApplySystemGroup))]
    internal partial class TweenRotationSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities
                .WithNone<TweenPause>()
                .ForEach(
                    (ref LocalToWorldTransform  rotation, in DynamicBuffer<TweenState> tweenBuffer, in TweenRotation tweenInfo) =>
                    {
                        for (int i = 0; i < tweenBuffer.Length; i++)
                        {
                            TweenState tween = tweenBuffer[i];
                            // if (tween.IsFinished)
                            //     continue;
                            if (tween.Id == tweenInfo.Id)
                            {
                                rotation.Value.Rotation = math.slerp(tweenInfo.Start, tweenInfo.End, tween.EasePercentage);
                                break;
                            }
                        }
                    }).ScheduleParallel();
        }
    }
}