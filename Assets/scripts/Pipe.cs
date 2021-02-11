using UnityEngine;

public class Pipe : MonoBehaviour
{
    public float moveSpeed;


    private void Start()
    {
        GetComponent<Rigidbody2D>().velocity = new Vector2(moveSpeed, 0);
    }

    private void Update()
    {
        if (transform.position.x <= -0.4)
        {
            Destroy(gameObject);
        }
    }

    public void GameOver()
    {
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
    }
}