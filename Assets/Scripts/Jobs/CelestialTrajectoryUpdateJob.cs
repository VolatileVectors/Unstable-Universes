using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using static Unity.Mathematics.math;
using Universe;

namespace Jobs
{
    using Unity.Mathematics;

    [BurstCompile(CompileSynchronously = true)]
    public struct CelestialTrajectoryUpdateJob : IJob
    {
        public int SimulationSteps;
        public int SampleRate;
        public int CenterOfMassFrameIndex;
        public NativeArray<CelestialBodyTrajectoryData> BodyData;
        [WriteOnly] public NativeArray<float3> LineSegments;

        public void Execute()
        {
            var sampleSize = SimulationSteps / SampleRate + 1;
            for (var i = 0; i < SimulationSteps; i++)
            {
                for (var current = 0; current < BodyData.Length; current++)
                {
                    var currentBody = BodyData[current];
                    currentBody.NewPosition = currentBody.Position + currentBody.Velocity * UniverseSimulator.RefreshRate +
                                              currentBody.Acceleration * UniverseSimulator.RefreshRateSqrHalf;
                    var newAcceleration = float3.zero;
                    for (var other = 0; other < BodyData.Length; other++)
                    {
                        if (current == other) continue;
                        var posDelta = BodyData[other].Position - currentBody.Position;
                        newAcceleration += normalize(posDelta) * BodyData[other].Mass / lengthsq(posDelta);
                    }

                    currentBody.Velocity += (currentBody.Acceleration + newAcceleration) * UniverseSimulator.RefreshRateHalf;
                    currentBody.Acceleration = newAcceleration;
                    BodyData[current] = currentBody;
                }

                for (var current = 0; current < BodyData.Length; current++)
                {
                    var currentBody = BodyData[current];
                    currentBody.Position = currentBody.NewPosition - BodyData[CenterOfMassFrameIndex].NewPosition;
                    BodyData[current] = currentBody;
                    if ((i + 1) % SampleRate == 0)
                    {
                        LineSegments[current * sampleSize + (i + 1) / SampleRate] = currentBody.Position;
                    }
                }
            }
        }
    }
}