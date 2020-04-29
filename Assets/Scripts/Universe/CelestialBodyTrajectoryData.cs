using Unity.Mathematics;
using UnityEngine;

namespace Universe
{
    public struct CelestialBodyTrajectoryData
    {
        public float3 Position;
        public float3 NewPosition;
        public float3 Velocity;
        public float3 Acceleration;
        public readonly float Mass;

        public CelestialBodyTrajectoryData(CelestialBody cBody)
        {
            Position = cBody.transform.position;
            NewPosition = Position;
            Velocity = cBody.initialVelocity;
            Acceleration = float3.zero;
            Mass = cBody.mass;
        }
    }
}