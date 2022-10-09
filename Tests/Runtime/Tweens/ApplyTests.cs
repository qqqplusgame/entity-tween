﻿using NUnit.Framework;
using Timespawn.EntityTween.Math;
using Timespawn.EntityTween.Tweens;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

#if UNITY_TINY_ALL_0_31_0
using Unity.Tiny;
#endif

#if UNITY_2D_ENTITIES
using Unity.U2D.Entities;
#endif

namespace Timespawn.EntityTween.Tests.Tweens
{
    public class ApplyTests : TweenTestFixture
    {
        [Test]
        public void Move()
        {
            Entity entity = EntityManager.CreateEntity();
            Tween.Move(EntityManager, entity, TestStartFloat3, TestEndFloat3, TestDuration);

            World.Update();
            World.Update();
            OverrideNextDeltaTime(TestDeltaTime);
            World.Update();

            float percentage = Ease.CalculatePercentage(TestDeltaTime, default, default);
            float3 expected = math.lerp(TestStartFloat3, TestEndFloat3, percentage);
            LocalToWorldTransform translation = EntityManager.GetComponentData<LocalToWorldTransform>(entity);
            Assert.AreEqual(expected, translation.Value.Position);
        }

        [Test]
        public void Rotate()
        {
            Entity entity = EntityManager.CreateEntity();
            Tween.Rotate(EntityManager, entity, TestStartQuat, TestEndQuat, TestDuration);

            World.Update();
            World.Update();
            OverrideNextDeltaTime(TestDeltaTime);
            World.Update();

            float percentage = Ease.CalculatePercentage(TestDeltaTime, default, default);
            quaternion expected = math.slerp(TestStartQuat, TestEndQuat, percentage);
            LocalToWorldTransform rotation = EntityManager.GetComponentData<LocalToWorldTransform>(entity);
            Assert.AreEqual(expected, rotation.Value.Rotation);
        }

        [Test]
        public void Scale()
        {
            Entity entity = EntityManager.CreateEntity();
            Tween.Scale(EntityManager, entity, TestStartFloat, TestEndFloat, TestDuration);

            World.Update();
            World.Update();
            OverrideNextDeltaTime(TestDeltaTime);
            World.Update();

            float percentage = Ease.CalculatePercentage(TestDeltaTime, default, default);
            float3 expected = math.lerp(TestStartFloat3, TestEndFloat3, percentage);
            LocalToWorldTransform scale = EntityManager.GetComponentData<LocalToWorldTransform>(entity);
            Assert.AreEqual(expected, scale.Value.Scale);
        }

#if UNITY_TINY_ALL_0_31_0 || UNITY_2D_ENTITIES

        public void Tint()
        {
            Entity entity = EntityManager.CreateEntity();
            Tween.Tint(EntityManager, entity, TestStartFloat4, TestEndFloat4, TestDuration);

            World.Update();
            World.Update();
            OverrideNextDeltaTime(TestDeltaTime);
            World.Update();

            float percentage = Ease.CalculatePercentage(TestDeltaTime, default, default);
            float4 expected = math.lerp(TestStartFloat4, TestEndFloat4, percentage);
            SpriteRenderer renderer = EntityManager.GetComponentData<SpriteRenderer>(entity);
            Assert.AreEqual(expected, renderer.Color);
        }

#endif
    }
}