using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;

namespace Jobs
{
    public struct CelestialVelocityUpdateJob : IJobParallelFor
    {
        public NativeArray<Vector3> NewPosition;
        [ReadOnly] public NativeArray<Vector3> CurrentPosition;
        public NativeArray<Vector3> CurrentVelocity;
        public NativeArray<Vector3> CurrentAcceleration;
        [ReadOnly] public NativeArray<float> Mass;

        public void Execute(int index)
        {
            NewPosition[index] = CurrentPosition[index] + CurrentVelocity[index] * Universe.RefreshRate + CurrentAcceleration[index] * Universe.RefreshRateSqrHalf;
            var newAcceleration = Vector3.zero;
            for (int j = 0; j < CurrentPosition.Length; j++)
            {
                if (j == index) continue;
                var posDelta = CurrentPosition[j] - CurrentPosition[index];
                newAcceleration += posDelta.normalized * Mass[j] / posDelta.sqrMagnitude;
            }

            CurrentVelocity[index] += (CurrentAcceleration[index] + newAcceleration) * Universe.RefreshRateHalf;
            CurrentAcceleration[index] = newAcceleration;
        }
    }

    public struct CelestialPositionUpdateJob : IJobParallelForTransform
    {
        [ReadOnly] public NativeArray<Vector3> NewPosition;
        public NativeArray<Vector3> CurrentPosition;
        public int CenterOfMassFrameIndex;

        public void Execute(int index, TransformAccess transform)
        {
            CurrentPosition[index] = NewPosition[index] - NewPosition[CenterOfMassFrameIndex];
            transform.localPosition = CurrentPosition[index];
        }
    }
}