using Unity.Entities;

namespace Timespawn.EntityTween.Tweens
{
    [UpdateInGroup(typeof(TweenSimulationSystemGroup))]
    [UpdateAfter(typeof(TweenStateSystem))]
    internal partial class TweenResumeSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            EndSimulationEntityCommandBufferSystem endSimECBSystem = World.GetOrCreateSystemManaged<EndSimulationEntityCommandBufferSystem>();
            EntityCommandBuffer.ParallelWriter parallelWriter = endSimECBSystem.CreateCommandBuffer().AsParallelWriter();
            ComponentLookup<TweenPause> pauseFromEntity = GetComponentLookup<TweenPause>();

            Entities
                .WithReadOnly(pauseFromEntity)
                .WithAll<TweenResumeCommand>()
                .ForEach((int entityInQueryIndex, Entity entity) =>
                {
                    if (pauseFromEntity.HasComponent(entity))
                    {
                        parallelWriter.RemoveComponent<TweenPause>(entityInQueryIndex, entity);
                    }

                    parallelWriter.RemoveComponent<TweenResumeCommand>(entityInQueryIndex, entity);
                }).ScheduleParallel();

            endSimECBSystem.AddJobHandleForProducer(Dependency);
        }
    }
}