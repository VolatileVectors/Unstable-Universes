using Unity.Collections;
using Unity.Jobs;
using static Unity.Mathematics.math;
using Universe;

namespace Jobs
{
    using Unity.Mathematics;

    [Unity.Burst.BurstCompile(CompileSynchronously = true)]
    public struct CelestialVelocityUpdateJob : IJobParallelFor
    {
        [WriteOnly] public NativeArray<float3> NewPosition;
        [ReadOnly] public NativeArray<float3> CurrentPosition;
        public NativeArray<float3> CurrentVelocity;
        public NativeArray<float3> CurrentAcceleration;
        [ReadOnly] public NativeArray<float> Mass;

        public void Execute(int index)
        {
            NewPosition[index] = CurrentPosition[index] + CurrentVelocity[index] * UniverseSimulator.RefreshRate +
                                 CurrentAcceleration[index] * UniverseSimulator.RefreshRateSqrHalf;
            var newAcceleration = float3.zero;
            for (int j = 0; j < CurrentPosition.Length; j++)
            {
                if (j == index) continue;
                var posDelta = CurrentPosition[j] - CurrentPosition[index];

                newAcceleration += normalize(posDelta) * Mass[j] / lengthsq(posDelta);
            }

            CurrentVelocity[index] += (CurrentAcceleration[index] + newAcceleration) * UniverseSimulator.RefreshRateHalf;
            CurrentAcceleration[index] = newAcceleration;
        }
    }
}