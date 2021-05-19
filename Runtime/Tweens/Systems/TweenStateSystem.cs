using Unity.Entities;
using UnityEngine;

namespace Timespawn.EntityTween.Tweens
{
    [UpdateInGroup(typeof(TweenSimulationSystemGroup))]
    [UpdateAfter(typeof(TweenApplySystemGroup))]
    internal class TweenStateSystem : SystemBase
    {
        private EndSimulationEntityCommandBufferSystem endSimECBSystem;


        protected override void OnCreate()
        {
            base.OnCreate();

            endSimECBSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            BufferFromEntity<TweenDestroyCommand> destroyBufferFromEntity =
                GetBufferFromEntity<TweenDestroyCommand>(true);

            EntityCommandBuffer.ParallelWriter
                parallelWriter = endSimECBSystem.CreateCommandBuffer().AsParallelWriter();

            Entities
                .WithReadOnly(destroyBufferFromEntity)
                .WithNone<TweenPause>()
                .ForEach((Entity entity, int entityInQueryIndex, ref DynamicBuffer<TweenState> tweenBuffer) =>
                {
                    for (int i = tweenBuffer.Length - 1; i >= 0; i--)
                    {
                        TweenState tween = tweenBuffer[i];

                        if (tween.IsFinished)
                        {
                            //Debug.Log("TweenStateSystem tween.IsFinished");
                            continue;
                        }

                        bool isInfiniteLoop = tween.LoopCount == TweenState.LOOP_COUNT_INFINITE;
                        float normalizedTime = tween.GetNormalizedTime();


                        if (tween.IsReverting && normalizedTime <= 0.0f)
                        {
                            if (!isInfiniteLoop)
                            {
                                tween.LoopCount--;
                            }


                            tween.IsReverting = false;
                            tween.Time = 0.0f;
                        }
                        else if (!tween.IsReverting && normalizedTime >= 1.0f)
                        {
                            if (tween.IsPingPong)
                            {
                                tween.IsReverting = true;
                                tween.Time = tween.Duration / 2.0f;
                            }
                            else
                            {
                                if (!isInfiniteLoop)
                                {
                                    tween.LoopCount--;
                                }


                                tween.Time = 0.0f;
                            }
                        }

                        if (!isInfiniteLoop && tween.LoopCount == 0)
                        {
                            tween.IsFinished = true;

                            if (tween.EndCallBackEntity != Entity.Null)
                            {
                                parallelWriter.AddComponent<TweenEndCallback>(entityInQueryIndex,
                                    tween.EndCallBackEntity);
                            }

                            if (!destroyBufferFromEntity.HasComponent(entity))
                            {
                                parallelWriter.AddBuffer<TweenDestroyCommand>(entityInQueryIndex, entity);
                            }

                            parallelWriter.AppendToBuffer(entityInQueryIndex, entity,
                                new TweenDestroyCommand(tween.Id));
                        }

                        tweenBuffer[i] = tween;
                    }
                }).ScheduleParallel();

            endSimECBSystem.AddJobHandleForProducer(Dependency);
        }
    }
}