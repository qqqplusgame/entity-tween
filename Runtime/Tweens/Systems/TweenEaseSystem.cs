using Timespawn.EntityTween.Math;
using Timespawn.EntityTween.Tweens;
using Unity.Entities;
using UnityEngine;

namespace Timespawn.EntityTween
{
    [UpdateInGroup(typeof(TweenSimulationSystemGroup))]
    internal class TweenEaseSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            float deltaTime = Time.DeltaTime;

            Entities
                .WithNone<TweenPause>()
                .ForEach((ref DynamicBuffer<TweenState> tweenBuffer) =>
                {
                    for (int i = 0; i < tweenBuffer.Length; i++)
                    {
                        TweenState tween = tweenBuffer[i];
                        if (tween.IsFinished)
                        {
                            //Debug.Log("TweenEaseSystem tween.IsFinished");
                            continue;
                        }
                        tween.Time += tween.IsReverting ? -deltaTime : deltaTime;

                        float normalizedTime = tween.GetNormalizedTime();
                        tween.EasePercentage =
                            Ease.CalculatePercentage(normalizedTime, tween.EaseType, tween.EaseExponent);
                        //Debug.Log("normalizedTime = " + normalizedTime);
                        tweenBuffer[i] = tween;
                    }
                }).ScheduleParallel();
        }
    }
}