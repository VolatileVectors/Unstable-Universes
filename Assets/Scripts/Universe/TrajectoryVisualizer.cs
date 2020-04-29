using System;
using Jobs;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace Universe
{
    [ExecuteInEditMode]
    public class TrajectoryVisualizer : MonoBehaviour
    {
        public UniverseSimulator simulator;
        [Min(1)] public int simulationSteps = 1;
        [Min(1)] public int sampleRate = 100;

        private int _sampleSize = 1;
        [SerializeField] private string duration;

        private readonly Color[] _gizmoColors = {Color.yellow, Color.red, Color.blue, Color.green, Color.magenta};

        private NativeArray<CelestialBodyTrajectoryData> _celestialBodyData;

        private NativeArray<float3> _lineSegments;

        private JobHandle _jobHandleTrajectoryUpdate;
        private CelestialTrajectoryUpdateJob _trajectoryUpdate;

        private void Awake()
        {
            if (Application.isPlaying)
            {
                enabled = false;
                return;
            }
            _trajectoryUpdate = new CelestialTrajectoryUpdateJob();
        }

        private void Update()
        {
            if (Application.isPlaying) return;

            if (_celestialBodyData.IsCreated) _celestialBodyData.Dispose();
            if (_lineSegments.IsCreated) _lineSegments.Dispose();

            var celestialBody = FindObjectsOfType<CelestialBody>();

            _celestialBodyData = new NativeArray<CelestialBodyTrajectoryData>(celestialBody.Length, Allocator.Persistent);
            _lineSegments = new NativeArray<float3>(celestialBody.Length * _sampleSize, Allocator.Persistent);

            for (int i = 0; i < celestialBody.Length; ++i)
            {
                _celestialBodyData[i] = new CelestialBodyTrajectoryData(celestialBody[i]);
                _lineSegments[i * _sampleSize] = _celestialBodyData[i].Position;
                if (celestialBody[i] == simulator.centerOfMassFrame)
                {
                    _trajectoryUpdate.CenterOfMassFrameIndex = i;
                }
            }

            _trajectoryUpdate.SimulationSteps = simulationSteps;
            _trajectoryUpdate.SampleRate = sampleRate;
            _trajectoryUpdate.BodyData = _celestialBodyData;
            _trajectoryUpdate.LineSegments = _lineSegments;

            _jobHandleTrajectoryUpdate = _trajectoryUpdate.Schedule();
            JobHandle.ScheduleBatchedJobs();
        }

        private void OnDrawGizmos()
        {
            _jobHandleTrajectoryUpdate.Complete();
            var colorIndex = 0;
            for (var line = 0; line < _celestialBodyData.Length; line++)
            {
                Gizmos.color = _gizmoColors[colorIndex++];
                colorIndex %= _gizmoColors.Length;
                for (var segment = 1; segment < _sampleSize; segment++)
                {
                    Gizmos.DrawLine(_lineSegments[line * _sampleSize + segment - 1], _lineSegments[line * _sampleSize + segment]);
                }
            }
        }

        private void OnDisable()
        {
            _jobHandleTrajectoryUpdate.Complete();
            if (_celestialBodyData.IsCreated) _celestialBodyData.Dispose();
            if (_lineSegments.IsCreated) _lineSegments.Dispose();
        }

        private void OnValidate()
        {
            if (sampleRate < 1) sampleRate = 1;
            _sampleSize = simulationSteps / sampleRate + 1;
            duration = TimeSpan.FromSeconds(simulationSteps * UniverseSimulator.RefreshRate).ToString("hh':'mm':'ss");
        }
    }
}