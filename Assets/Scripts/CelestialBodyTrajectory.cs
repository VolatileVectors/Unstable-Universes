using System;
using System.Collections.Generic;
using UnityEngine;

public class CelestialBodyTrajectory
{
    public Vector3 Position;
    private readonly float _mass;
    public readonly Vector3[] LineSegments;

    private int _step;
    private readonly int _sampleRate;
    private Vector3 _velocity;
    private Vector3 _acceleration;
    private Vector3 _newPosition;

    private readonly Vector3 _initialPosition;
    private readonly Vector3 _initialVelocity;

    public CelestialBodyTrajectory(CelestialBody cBody, int steps, int sampleRate)
    {
        _sampleRate = sampleRate;
        _initialPosition = cBody.transform.localPosition;
        _initialVelocity = cBody.initialVelocity;

        _mass = cBody.mass;
        LineSegments = new Vector3[Math.Max(1, steps / sampleRate)];

        Init();
    }

    public void UpdateVelocity(IEnumerable<CelestialBodyTrajectory> allBodies, float timeStep)
    {
        _newPosition = Position + _velocity * timeStep + _acceleration * (timeStep * timeStep) / 2f;
        var newAcceleration = Vector3.zero;
        foreach (var otherBody in allBodies)
        {
            if (otherBody == this) continue;
            var posDelta = otherBody.Position - Position;
            newAcceleration += posDelta.normalized * otherBody._mass / posDelta.sqrMagnitude;
        }

        _velocity += (_acceleration + newAcceleration) * timeStep / 2f;
        _acceleration = newAcceleration;
    }

    public void UpdatePosition(Vector3 centerOfMass)
    {
        Position = _newPosition - centerOfMass;
        if (++_step % _sampleRate == 0)
        {
            LineSegments[_step / _sampleRate] = Position;
        }
    }

    public void Init()
    {
        _step = 0;
        Position = _initialPosition;
        _velocity = _initialVelocity;
        _acceleration = Vector3.zero;
        LineSegments[0] = Position;
    }
}