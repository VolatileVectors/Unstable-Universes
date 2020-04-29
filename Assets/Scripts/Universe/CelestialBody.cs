using System;
using System.Collections.Generic;
using UnityEngine;

namespace Universe
{
    public class CelestialBody : MonoBehaviour
    {
        public float surfaceGravity;
        public float mass;
        public float radius;
        public Vector3 initialVelocity;
        public Vector3 rotationAxis;
        public float angularVelocity;

        private void Awake()
        {
            if (surfaceGravity > 0f)
            {
                mass = surfaceGravity * radius * radius;
            }
        }

        private void Update()
        {
            transform.localRotation *= Quaternion.AngleAxis(angularVelocity * Time.deltaTime, rotationAxis);
        }

        private void OnValidate()
        {
            if (surfaceGravity > 0f)
            {
                mass = surfaceGravity * radius * radius;
            }
        }
    }
}