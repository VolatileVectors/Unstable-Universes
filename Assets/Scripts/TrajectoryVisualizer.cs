using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class TrajectoryVisualizer : MonoBehaviour
{
    public int simulationSteps = 1;
    public int sampleRate = 100;
    public float timeStep = 1f / 30f;

    public string duration;
    public CelestialBody centerOfMassFrame;

    private CelestialBodyTrajectory[] _trajectories;
    private CelestialBodyTrajectory _centerOfMassFrame;
    private readonly Color[] _gizmoColors = {Color.yellow, Color.red, Color.blue, Color.green, Color.magenta};

    private void Update()
    {
        if (Application.isPlaying || _trajectories == null) return;

        foreach (var trajectory in _trajectories)
        {
            trajectory.Init();
        }

        for (var i = 1; i < simulationSteps; i++)
        {
            foreach (var trajectory in _trajectories)
            {
                trajectory.UpdateVelocity(_trajectories, timeStep);
            }

            foreach (var trajectory in _trajectories)
            {
                trajectory.UpdatePosition(_centerOfMassFrame.Position);
            }
        }
    }

    private void OnEnable()
    {
        var bodies = FindObjectsOfType<CelestialBody>();
        _trajectories = new CelestialBodyTrajectory[bodies.Length];
        for (int i = 0; i < bodies.Length; ++i)
        {
            _trajectories[i] = new CelestialBodyTrajectory(bodies[i], simulationSteps, sampleRate);
            if (bodies[i] == centerOfMassFrame)
            {
                _centerOfMassFrame = _trajectories[i];
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying || _trajectories == null) return;
        var colorIndex = 0;
        foreach (var trajectory in _trajectories)
        {
            Gizmos.color = _gizmoColors[colorIndex++];
            colorIndex %= _gizmoColors.Length;
            for (var i = 1; i < trajectory.LineSegments.Length; ++i)
            {
                Gizmos.DrawLine(trajectory.LineSegments[i - 1], trajectory.LineSegments[i]);
            }
        }
    }

    private void OnValidate()
    {
        duration = TimeSpan.FromSeconds(simulationSteps * timeStep).ToString("hh':'mm':'ss");
    }
}