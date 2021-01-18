using UnityEngine;

public class Pipe : MonoBehaviour
{
    public float moveSpeed;

    public GameObject passPoint;
    public PipeSpawner pipeSpawner;


    private void Start()
    {
        var body = transform.GetComponent<Rigidbody2D>();
        body.velocity = new Vector2(moveSpeed, 0);
    }

    private void Update()
    {
        if (transform.position.x <= -0.4)
        {
            pipeSpawner.Despawn(gameObject);
        }
    }

    public void GameOver()
    {
        var body = transform.GetComponent<Rigidbody2D>();
        body.velocity = Vector2.zero;
    }
}