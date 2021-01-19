using System.Collections.Generic;
using System.Linq;
using NeuralNetwork;
using UnityEngine;

public class PipeSpawner : MonoBehaviour
{
    private const float BirdSpawnXOfssetted = NeuralNetworkManager.BirdSpawnX - 0.425f;
    private const float PipeMinY = 1.4f;
    private const float PipeMaxY = 3.9f;

    public float spawnTime = 5f; // The amount of time between each spawn. //1.72f
    public float firstSpawnDelay = 3f;
    public GameObject pipePrefab;

    private Transform _nextPipeCachedPosition;
    private readonly LinkedList<Pipe> _pipes = new LinkedList<Pipe>();


    public void StartSpawning()
    {
        _pipes.Select(p => p.gameObject).ToList().ForEach(Destroy);
        _pipes.Clear();
        CancelInvoke(nameof(Spawn));

        InvokeRepeating(nameof(Spawn), firstSpawnDelay, spawnTime);
    }

    private void Spawn()
    {
        var transform1 = transform;
        var pos = new Vector2(transform1.position.x, Random.Range(PipeMinY, PipeMaxY));

        var pipe = Instantiate(pipePrefab, pos, transform1.rotation).GetComponent<Pipe>();
        pipe.pipeSpawner = this;
        _pipes.AddLast(pipe);
    }

    public void Despawn(GameObject pipe)
    {
        Destroy(pipe);
        _pipes.RemoveFirst();
    }

    public Vector3 GetNextPipePosition()
    {
        if (_nextPipeCachedPosition == null || _nextPipeCachedPosition.position.x < BirdSpawnXOfssetted)
        {
            _nextPipeCachedPosition = _pipes.FirstOrDefault(p => p.transform.position.x > BirdSpawnXOfssetted)
                ?.passPoint.transform;
        }

        return _nextPipeCachedPosition?.position ?? Vector3.down;
    }
}