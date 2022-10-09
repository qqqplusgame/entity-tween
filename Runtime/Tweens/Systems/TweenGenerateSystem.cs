using Timespawn.EntityTween.Tweens;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;


#if UNITY_TINY_ALL_0_31_0
using Unity.Tiny;
#elif UNITY_2D_ENTITIES
using Unity.U2D.Entities;
#endif

[assembly: Unity.Jobs.RegisterGenericJobType(typeof(Timespawn.EntityTween.Tweens.TweenTranslationGenerateSystem.GenerateJob))]
[assembly: Unity.Jobs.RegisterGenericJobType(typeof(Timespawn.EntityTween.Tweens.TweenRotationGenerateSystem.GenerateJob))]
[assembly: Unity.Jobs.RegisterGenericJobType(typeof(Timespawn.EntityTween.Tweens.TweenScaleGenerateSystem.GenerateJob))]

#if UNITY_TINY_ALL_0_31_0 || UNITY_2D_ENTITIES
[assembly: RegisterGenericJobType(typeof(Timespawn.EntityTween.Tweens.TweenTintGenerateSystem.GenerateJob))]
#endif

namespace Timespawn.EntityTween.Tweens
{
    [UpdateInGroup(typeof(TweenGenerateSystemGroup))]
    internal abstract partial class TweenGenerateSystem<TTweenCommand, TTweenInfo, TTarget, TTweenInfoValue> : SystemBase
        where TTweenCommand : unmanaged, IComponentData, ITweenParams, ITweenInfo<TTweenInfoValue>
        where TTweenInfo : unmanaged, IComponentData, ITweenId, ITweenInfo<TTweenInfoValue>
        where TTarget : unmanaged, IComponentData
        where TTweenInfoValue : struct
    {
        [BurstCompile]
        internal struct GenerateJob : IJobChunk
        {
            [ReadOnly] public int TweenInfoTypeIndex;
            [ReadOnly] public double ElapsedTime;
            [ReadOnly] public EntityTypeHandle EntityType;
            [ReadOnly] public ComponentTypeHandle<TTweenCommand> TweenCommandType;
            [ReadOnly] public ComponentTypeHandle<TTarget> TargetType;
            [ReadOnly] public BufferTypeHandle<TweenState> TweenBufferType;

            public EntityCommandBuffer.ParallelWriter ParallelWriter;

            public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
            {
                bool hasTweenBuffer = chunk.Has(TweenBufferType);
                bool hasTargetType = chunk.Has(TargetType);

                NativeArray<Entity> entities = chunk.GetNativeArray(EntityType);
                NativeArray<TTweenCommand> commands = chunk.GetNativeArray(TweenCommandType);
                for (int i = 0; i < entities.Length; i++)
                {
                    Entity entity = entities[i];
                    TTweenCommand command = commands[i];

                    if (!hasTweenBuffer)
                    {
                        ParallelWriter.AddBuffer<TweenState>(unfilteredChunkIndex, entity);
                        break;
                    }

                    if (!hasTargetType)
                    {
                        ParallelWriter.AddComponent<TTarget>(unfilteredChunkIndex, entity);
                    }

                    TweenState tween = new TweenState(command.GetTweenParams(), ElapsedTime, unfilteredChunkIndex,
                        TweenInfoTypeIndex);
                    ParallelWriter.AppendToBuffer(unfilteredChunkIndex, entity, tween);

                    TTweenInfo info = default;
                    info.SetTweenId(tween.Id);
                    info.SetTweenInfo(command.GetTweenStart(), command.GetTweenEnd());
                    ParallelWriter.AddComponent(unfilteredChunkIndex, entity, info);

                    ParallelWriter.RemoveComponent<TTweenCommand>(unfilteredChunkIndex, entity);
                }
            }
            
        }

        private EntityQuery TweenCommandQuery;

        protected override void OnCreate()
        {
            TweenCommandQuery = GetEntityQuery(ComponentType.ReadOnly<TTweenCommand>());
        }

        protected override void OnUpdate()
        {
            double elapsedTime = World.Time.ElapsedTime;
            EndSimulationEntityCommandBufferSystem endSimECBSystem =
                World.GetOrCreateSystemManaged<EndSimulationEntityCommandBufferSystem>();

            GenerateJob job = new GenerateJob
            {
                TweenInfoTypeIndex = TypeManager.GetTypeIndex(typeof(TTweenInfo)),
                ElapsedTime = elapsedTime,
                EntityType = GetEntityTypeHandle(),
                TweenCommandType = GetComponentTypeHandle<TTweenCommand>(true),
                TargetType = GetComponentTypeHandle<TTarget>(true),
                TweenBufferType = GetBufferTypeHandle<TweenState>(true),
                ParallelWriter = endSimECBSystem.CreateCommandBuffer().AsParallelWriter(),
            };

            Dependency = job.ScheduleParallel(TweenCommandQuery, Dependency);
            endSimECBSystem.AddJobHandleForProducer(Dependency);
        }
    }

    internal class TweenTranslationGenerateSystem : TweenGenerateSystem<TweenTranslationCommand, TweenTranslation, LocalToWorldTransform, float3> {}
    internal class TweenRotationGenerateSystem : TweenGenerateSystem<TweenRotationCommand, TweenRotation, LocalToWorldTransform, quaternion> {}
    internal class TweenScaleGenerateSystem : TweenGenerateSystem<TweenScaleCommand, TweenScale, LocalToWorldTransform, float> {}

#if UNITY_TINY_ALL_0_31_0 || UNITY_2D_ENTITIES
    internal class TweenTintGenerateSystem : TweenGenerateSystem<TweenTintCommand, TweenTint, SpriteRenderer, float4> {}
#endif
}