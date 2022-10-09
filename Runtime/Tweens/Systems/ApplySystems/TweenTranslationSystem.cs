using Timespawn.EntityTween.Tweens;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Timespawn.EntityTween
{
    [UpdateInGroup(typeof(TweenApplySystemGroup))]
    internal partial class TweenTranslationSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities
                .WithNone<TweenPause>()
                .ForEach((ref LocalToWorldTransform translation, in DynamicBuffer<TweenState> tweenBuffer,
                    in TweenTranslation tweenInfo) =>
                {
                    for (int i = 0; i < tweenBuffer.Length; i++)
                    {
                        TweenState tween = tweenBuffer[i];
                        
                        // if (tween.IsFinished)
                        // {
                        //     //Debug.Log("TweenTranslationSystem tween.IsFinished");
                        //     continue;
                        // }
                        
                        if (tween.Id == tweenInfo.Id)
                        {
                            translation.Value.Position = math.lerp(tweenInfo.Start, tweenInfo.End, tween.EasePercentage);
                            //Debug.Log("translation = " + translation.Value);
                            break;
                        }
                    }
                }).ScheduleParallel();
        }
    }
}