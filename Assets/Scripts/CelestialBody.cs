using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public class CelestialBody : MonoBehaviour
{
    public float surfaceGravity;
    public float mass;
    public float radius;
    public Vector3 initialVelocity;
    public Vector3 rotationAxis;
    public float angularVelocity;

    
    //private Vector3 _newPosition;
    //private Vector3 _currentVelocity;
    //private Vector3 _currentAcceleration;

    private void Awake()
    {
        if (surfaceGravity > 0f)
        {
            mass = surfaceGravity * radius * radius;
        }
    }

/*    public void UpdateVelocity(IEnumerable<CelestialBody> allBodies)
    {
        _newPosition = transform.localPosition + _currentVelocity * Universe.RefreshRate + _currentAcceleration * Universe.RefreshRateSqrHalf;
        var newAccleration = Vector3.zero;
        foreach (var otherBody in allBodies)
        {
            if (otherBody == this) continue;
            var posDelta = otherBody.transform.localPosition - transform.localPosition;
            newAccleration += posDelta.normalized * otherBody.mass / posDelta.sqrMagnitude;
        }

        _currentVelocity += (_currentAcceleration + newAccleration) * Universe.RefreshRateHalf;
        _currentAcceleration = newAccleration;
    }

    public void UpdatePositionAndRotation(Vector3 centerOfMass)
    {
        transform.localPosition = _newPosition - centerOfMass;
    }*/

    private void OnValidate()
    {
        if (surfaceGravity > 0f)
        {
            mass = surfaceGravity * radius * radius;
        }
    }
}