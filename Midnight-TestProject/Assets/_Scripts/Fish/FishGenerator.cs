using Unity.Jobs;
using Unity.Collections;
using Unity.Burst;
using UnityEngine.Jobs;
using math = Unity.Mathematics.math;
using random = Unity.Mathematics.Random;
using UnityEngine;

[BurstCompile]
struct PositionUpdateJob : IJobParallelForTransform
{
    public NativeArray<Vector3> objectVelocities;

    public Vector3 bounds;
    public Vector3 center;

    public float jobDeltaTime;
    public float time;
    public float swimSpeed;
    public float turnSpeed;
    public int swimChangeFrequency;

    public float seed;

    public void Execute(int i, TransformAccess transform)
    {
        Vector3 currentVelocity = objectVelocities[i];
        random randomGen = new random((uint)(i * time + 1 + seed));

        transform.position +=
        transform.localToWorldMatrix.MultiplyVector(new Vector3(0, 0, 1)) *
        swimSpeed *
        jobDeltaTime *
        randomGen.NextFloat(0.3f, 1.0f);

        if (currentVelocity != Vector3.zero)
        {
            transform.rotation =
            Quaternion.Lerp(transform.rotation,
            Quaternion.LookRotation(currentVelocity), turnSpeed * jobDeltaTime);
        }

        Vector3 currentPosition = transform.position;

        bool randomise = true;

        if (currentPosition.x > center.x + bounds.x / 2 ||
            currentPosition.x < center.x - bounds.x / 2 ||
            currentPosition.z > center.z + bounds.z / 2 ||
            currentPosition.z < center.z - bounds.z / 2)
        {
            Vector3 internalPosition = new Vector3(center.x +
            randomGen.NextFloat(-bounds.x / 2, bounds.x / 2) / 1.3f,
            0,
            center.z + randomGen.NextFloat(-bounds.z / 2, bounds.z / 2) / 1.3f);

            currentVelocity = (internalPosition - currentPosition).normalized;

            objectVelocities[i] = currentVelocity;

            transform.rotation = Quaternion.Lerp(transform.rotation,
            Quaternion.LookRotation(currentVelocity),
            turnSpeed * jobDeltaTime * 2);

            randomise = false;
        }

        if (randomise)
        {
            if (randomGen.NextInt(0, swimChangeFrequency) <= 2)
            {
                objectVelocities[i] = new Vector3(randomGen.NextFloat(-1f, 1f),
                0, randomGen.NextFloat(-1f, 1f));
            }
        }
    }
}


public class FishGenerator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform _fishPrefab;
    private PositionUpdateJob _positionUpdateJob;
    private JobHandle _positionUpdateJobHandle;

    [Header("Spawn Settings")]
    [SerializeField] private int _amountOfFish;
    [SerializeField] private Vector3 _spawnBounds;
    [SerializeField] private float _spawnHeight;
    [SerializeField] private int _swimChangeFrequency;

    [Header("Settings")]
    [SerializeField] private float _swimSpeed;
    [SerializeField] private float _turnSpeed;


    private NativeArray<Vector3> _velocities;
    private TransformAccessArray _transformAccessArray;

    private void Start()
    {
        _velocities = new NativeArray<Vector3>(_amountOfFish, Allocator.Persistent);

        _transformAccessArray = new TransformAccessArray(_amountOfFish);

        for (int i = 0; i < _amountOfFish; i++)
        {

            float distanceX =
            Random.Range(-_spawnBounds.x / 2, _spawnBounds.x / 2);

            float distanceZ =
            Random.Range(-_spawnBounds.z / 2, _spawnBounds.z / 2);

            Vector3 spawnPoint =
            (transform.position + Vector3.up * _spawnHeight) + new Vector3(distanceX, 0, distanceZ);

            Transform t =
            (Transform)Instantiate(_fishPrefab, spawnPoint,
            Quaternion.identity);

            _transformAccessArray.Add(t);
        }
    }

    private void Update()
    {
        _positionUpdateJob = new PositionUpdateJob()
        {
            objectVelocities = _velocities,
            jobDeltaTime = Time.deltaTime,
            swimSpeed = this._swimSpeed,
            turnSpeed = this._turnSpeed,
            time = Time.time,
            swimChangeFrequency = this._swimChangeFrequency,
            center = transform.position,
            bounds = _spawnBounds,
            seed = System.DateTimeOffset.Now.Millisecond
        };

        // 2
        _positionUpdateJobHandle = _positionUpdateJob.Schedule(_transformAccessArray);
    }

    private void LateUpdate()
    {
        _positionUpdateJobHandle.Complete();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(transform.position + Vector3.up * _spawnHeight, _spawnBounds);
    }

    private void OnDestroy()
    {
        _transformAccessArray.Dispose();
        _velocities.Dispose();
    }
}
