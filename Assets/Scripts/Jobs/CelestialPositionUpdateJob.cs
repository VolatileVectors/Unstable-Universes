using Unity.Collections;
using Unity.Mathematics;
using UnityEngine.Jobs;

namespace Jobs
{
    [Unity.Burst.BurstCompile(CompileSynchronously = true)]
    public struct CelestialPositionUpdateJob : IJobParallelForTransform
    {
        [ReadOnly] public NativeArray<float3> NewPosition;
        [WriteOnly] public NativeArray<float3> CurrentPosition;
        public int CenterOfMassFrameIndex;

        public void Execute(int index, TransformAccess transform)
        {
            var position = NewPosition[index] - NewPosition[CenterOfMassFrameIndex];
            CurrentPosition[index] = position;
            transform.localPosition = position;
        }
    }
}