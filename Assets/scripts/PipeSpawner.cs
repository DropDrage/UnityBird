using UnityEngine;

public class PipeSpawner : MonoBehaviour
{
    private const float PipeMinY = 1.4f;
    private const float PipeMaxY = 3.9f;

    public float spawnTime = 5f;
    public float firstSpawnDelay = 3f;

    public GameObject pipe;


    public void StartSpawning()
    {
        InvokeRepeating(nameof(Spawn), firstSpawnDelay, spawnTime);
    }

    private void Spawn()
    {
        var transform1 = transform;
        var pos = new Vector2(transform1.position.x, Random.Range(PipeMinY, PipeMaxY));

        Instantiate(pipe, pos, transform1.rotation);
    }

    public void GameOver()
    {
        CancelInvoke(nameof(Spawn));
    }
}