using System;
using UnityEngine;

namespace SpaceShip
{
    public class ShipController : MonoBehaviour
    {
        public Rigidbody spaceShipRb;
        public float pitchSpeed = 5f;
        public float yawSpeed = 5f;
        public float rollSpeed = 5f;
        public float thrustSpeed = 20f;

        private float _currentPitch;
        private float _currentYaw;
        private float _currentRoll;
        private float _currentThrust;

        public void SetPitch(float value)
        {
            _currentPitch = (value * 2f - 1f) * pitchSpeed;
        }

        public void SetYaw(float value)
        {
            _currentYaw = (value * 2f - 1f) * yawSpeed;
        }

        public void SetRoll(float value)
        {
            _currentRoll = (value * 2f - 1f) * rollSpeed;
        }

        public void SetThrust(float value)
        {
            _currentThrust = (value * 2f - 1f) * thrustSpeed;
        }

        public void FixedUpdate()
        {
            var rotation = spaceShipRb.angularVelocity;
            rotation.x = _currentPitch;
            rotation.y = _currentYaw;
            rotation.z = _currentRoll;
            spaceShipRb.angularVelocity = rotation;

            spaceShipRb.velocity = transform.forward * _currentThrust;
        }
    }
}