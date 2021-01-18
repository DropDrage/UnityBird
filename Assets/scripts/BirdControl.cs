using NeuralNetwork;
using UnityEngine;

public class BirdControl : MonoBehaviour
{
    public int rotateRate = 10;
    public float upSpeed = 10;
    public PipeSpawner pipeSpawner;

    public NeuralNetworkManager networkManager;
    public NeuralNetwork.NeuralNetwork network;

    public bool dead;

    private bool _landed;
    private float _spawnTime;
    private int _scores;

    private Rigidbody2D _rigidbody;
    private static readonly int Die = Animator.StringToHash("die");


    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();

        _spawnTime = Time.time;
    }

    private void Update()
    {
        if (!_landed && !dead)
        {
            network.fitness = Time.time - _spawnTime + _scores;
            UseNeuralNetwork();
        }

        if (!_landed)
        {
            var v = _rigidbody.velocity.y;
            var rotate = Mathf.Min(Mathf.Max(-90, v * rotateRate + 60), 30);
            transform.rotation = Quaternion.Euler(0f, 0f, rotate);
        }
        else
        {
            _rigidbody.rotation = -90;
        }
    }

    private void UseNeuralNetwork()
    {
        var nearestPipePassPoint = pipeSpawner.FindNextPipePosition();
        var inputs = new float[4];
        inputs[0] = transform.position.y;
        inputs[1] = nearestPipePassPoint.x;
        inputs[2] = nearestPipePassPoint.y - 0.5f;
        inputs[3] = nearestPipePassPoint.y + 0.5f;

        var output = network.FeedForward(inputs);
        if (output[0] > 0)
        {
            JumpUp();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (dead && _landed) return;

        switch (other.tag)
        {
            case "movable":
            {
                if (!dead)
                {
                    GetComponent<Animator>().SetTrigger(Die);
                    var spriteRenderer = GetComponent<SpriteRenderer>();
                    spriteRenderer.color = new Color(1, 0, 0, 0.1f);
                    spriteRenderer.sortingLayerName = "back";

                    GameOver();
                }

                if (other.name == "land")
                {
                    Destroy(_rigidbody);
                    Destroy(GetComponent<SpriteRenderer>());
                    enabled = false;

                    _landed = true;
                }

                break;
            }
            case "pass_trigger":
                ++_scores;
                break;
        }
    }

    public void JumpUp()
    {
        _rigidbody.velocity = new Vector2(0, upSpeed);
    }

    public void GameOver()
    {
        dead = true;
        networkManager.IncreaseDead();
    }
}