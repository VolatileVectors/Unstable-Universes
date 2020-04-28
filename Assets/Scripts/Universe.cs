using System;
using Jobs;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;

public class Universe : MonoBehaviour
{
    public const float RefreshRate = 1f / 90f;
    public const float RefreshRateHalf = RefreshRate / 2f;
    public const float RefreshRateSqrHalf = RefreshRate * RefreshRate / 2f;

    public CelestialBody centerOfMassFrame;
    public CelestialBody centerOfUniverse;

    public int iterations = 1;
    private CelestialBody[] _celestialBodies;

    private TransformAccessArray _celestialTransformsArray;

    private NativeArray<Vector3> _newPosition;
    private NativeArray<Vector3> _currentPosition;
    private NativeArray<Vector3> _currentVelocity;
    private NativeArray<Vector3> _currentAcceleration;
    private NativeArray<float> _mass;
    private JobHandle _jobHandleUniverseUpdate;

    private CelestialVelocityUpdateJob _velocityUpdate;
    private CelestialPositionUpdateJob _positionUpdate;

    private float _refreshDelta;

    private void Awake()
    {
        _celestialBodies = FindObjectsOfType<CelestialBody>();
    }

    private void Start()
    {
        var transforms = new Transform[_celestialBodies.Length];
        for (int i = 0; i < _celestialBodies.Length; i++)
        {
            transforms[i] = _celestialBodies[i].transform;
        }

        _celestialTransformsArray = new TransformAccessArray(transforms, 4);
        _newPosition = new NativeArray<Vector3>(_celestialBodies.Length, Allocator.Persistent);
        _currentPosition = new NativeArray<Vector3>(_celestialBodies.Length, Allocator.Persistent);
        _currentVelocity = new NativeArray<Vector3>(_celestialBodies.Length, Allocator.Persistent);
        _currentAcceleration = new NativeArray<Vector3>(_celestialBodies.Length, Allocator.Persistent);
        _mass = new NativeArray<float>(_celestialBodies.Length, Allocator.Persistent);

        _velocityUpdate = new CelestialVelocityUpdateJob()
        {
            CurrentPosition = _currentPosition,
            CurrentVelocity = _currentVelocity,
            CurrentAcceleration = _currentAcceleration,
            NewPosition = _newPosition,
            Mass = _mass
        };

        _positionUpdate = new CelestialPositionUpdateJob()
        {
            CurrentPosition = _currentPosition,
            NewPosition = _newPosition
        };

        for (int i = 0; i < _celestialBodies.Length; i++)
        {
            _newPosition[i] = Vector3.zero;
            _currentPosition[i] = _celestialBodies[i].transform.localPosition;
            _currentVelocity[i] = _celestialBodies[i].initialVelocity;
            _currentAcceleration[i] = Vector3.zero;
            _mass[i] = _celestialBodies[i].mass;

            if (_celestialBodies[i] == centerOfMassFrame)
            {
                _positionUpdate.CenterOfMassFrameIndex = i;
            }
        }
    }

    private void Update()
    {
        _refreshDelta += Time.deltaTime;
        if (_refreshDelta < RefreshRate) return;
        _refreshDelta -= RefreshRate;

        _jobHandleUniverseUpdate = _velocityUpdate.Schedule(_celestialBodies.Length, 4);
        _jobHandleUniverseUpdate = _positionUpdate.Schedule(_celestialTransformsArray, _jobHandleUniverseUpdate);
        for (int i = 1; i < iterations; i++)
        {
            _jobHandleUniverseUpdate = _velocityUpdate.Schedule(_celestialBodies.Length, 4, _jobHandleUniverseUpdate);
            _jobHandleUniverseUpdate = _positionUpdate.Schedule(_celestialTransformsArray, _jobHandleUniverseUpdate);
        }

        JobHandle.ScheduleBatchedJobs();
    }

    private void LateUpdate()
    {
        _jobHandleUniverseUpdate.Complete();
        transform.position -= centerOfUniverse.transform.position;
    }

    private void OnDestroy()
    {
        _celestialTransformsArray.Dispose();
        _newPosition.Dispose();
        _currentPosition.Dispose();
        _currentVelocity.Dispose();
        _currentAcceleration.Dispose();
        _mass.Dispose();
    }
}

/*void RunJobs(int numRuns)
{
TestJob job = new TestJob();
JobHandle jobHandle = job.Schedule();
    for (int i = 1; i < numRuns; ++i)
{
    jobHandle = job.Schedule(jobHandle);
}
JobHandle.ScheduleBatchedJobs();
jobHandle.Complete();
}*/