using Timespawn.EntityTween.Math;
using Unity.Entities;
using Unity.Mathematics;

namespace Timespawn.EntityTween.Tweens
{
    public struct TweenEndCallback : ISystemStateComponentData
    {
    }

    public struct TweenState : IBufferElementData, ITweenId
    {
        internal const byte LOOP_COUNT_INFINITE = 0;

        public int Id;
        public EaseType EaseType;
        public byte EaseExponent;
        public float Duration;
        public float Time;
        public float EasePercentage;
        public bool IsPingPong;
        public byte LoopCount;
        public bool IsReverting;
        public bool IsFinished;
        public Entity EndCallBackEntity;

        internal TweenState(
            in EaseType easeType,
            in byte easeExponent,
            in float duration,
            in bool isPingPong,
            in byte loopCount,
            in float delayedStartTime,
            in double elapsedTime,
            in int chunkIndex,
            in int tweenInfoTypeIndex,
            in Entity endCallBackEntity) : this()
        {
            EaseType = easeType;
            EaseExponent = easeExponent;
            Duration = duration;
            IsPingPong = isPingPong;
            LoopCount = loopCount;

            Time = -math.max(delayedStartTime, 0.0f);
            Id = GenerateId(elapsedTime, chunkIndex, tweenInfoTypeIndex);
            IsFinished = false;
            EndCallBackEntity = endCallBackEntity;
        }

        internal TweenState(in TweenParams tweenParams, in double elapsedTime, in int chunkIndex,
            in int tweenInfoTypeIndex)
            : this(tweenParams.EaseType, tweenParams.EaseExponent, tweenParams.Duration, tweenParams.IsPingPong,
                tweenParams.LoopCount, tweenParams.StartDelay, elapsedTime, chunkIndex, tweenInfoTypeIndex
                , tweenParams.EndCallBackEntity)
        {
        }

        public float GetNormalizedTime()
        {
            float oneWayDuration = Duration;
            if (IsPingPong)
            {
                oneWayDuration /= 2.0f;
            }

            return math.clamp(Time / oneWayDuration, 0.0f, 1.0f);
        }

        public void SetTweenId(in int id)
        {
            Id = id;
        }

        public int GetTweenId()
        {
            return Id;
        }

        private int GenerateId(in double elapsedTime, in int entityInQueryIndex, in int tweenInfoTypeIndex)
        {
            unchecked
            {
                int hashCode = (int) EaseType;
                hashCode = (hashCode * 397) ^ EaseExponent.GetHashCode();
                hashCode = (hashCode * 397) ^ Duration.GetHashCode();
                hashCode = (hashCode * 397) ^ IsPingPong.GetHashCode();
                hashCode = (hashCode * 397) ^ LoopCount.GetHashCode();
                hashCode = (hashCode * 397) ^ elapsedTime.GetHashCode();
                hashCode = (hashCode * 397) ^ entityInQueryIndex;
                hashCode = (hashCode * 397) ^ tweenInfoTypeIndex;

                return hashCode;
            }
        }
    }
}