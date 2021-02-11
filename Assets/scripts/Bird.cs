using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public class Bird : MonoBehaviour
{
    private static readonly int Die = Animator.StringToHash("die");

    public int rotateRate = 10;
    public float upSpeed = 10;
    public ScoreManager scoreManager;

    public AudioClip jumpUp;
    public AudioClip hit;
    public AudioClip score;

    private bool _dead;
    private bool _landed;

    private Sequence _birdIdleStateAnim;

    private Rigidbody2D _rigidbody2D;

    private Action _updateAction = () => { };


    private void Start()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();

        const float birdOffset = 0.05f;
        const float birdTime = 0.3f;
        var birdStartY = transform.position.y;

        _birdIdleStateAnim = DOTween.Sequence()
            .Append(transform.DOMoveY(birdStartY + birdOffset, birdTime).SetEase(Ease.OutQuad))
            .Append(transform.DOMoveY(birdStartY - 2 * birdOffset, 2 * birdTime).SetEase(Ease.InQuad))
            .Append(transform.DOMoveY(birdStartY, birdTime).SetEase(Ease.Linear))
            .SetLoops(-1);
    }

    private void Update()
    {
        _updateAction();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        switch (other.name)
        {
            case "Land":
            case "pipe_up":
            case "pipe_down":
            {
                if (!_dead)
                {
                    var movables = GameObject.FindGameObjectsWithTag("movable");
                    foreach (var movable in movables)
                    {
                        movable.BroadcastMessage("GameOver");
                    }

                    GetComponent<Animator>().SetTrigger(Die);
                    AudioSource.PlayClipAtPoint(hit, Vector3.zero);
                }

                if (other.name == "Land")
                {
                    _rigidbody2D.gravityScale = 0;
                    _rigidbody2D.velocity = Vector2.zero;

                    _landed = true;
                }

                break;
            }
            case "pass_trigger":
                scoreManager.AddScore();
                AudioSource.PlayClipAtPoint(score, Vector3.zero);
                break;
        }
    }

    public void SetPlayableState()
    {
        _updateAction = () =>
        {
            if (!_landed)
            {
                var rotate = Mathf.Min(Mathf.Max(-90, _rigidbody2D.velocity.y * rotateRate + 60), 30);
                transform.rotation = Quaternion.Euler(0f, 0f, rotate);
            }
            else
            {
                _rigidbody2D.rotation = -90;
            }
        };

        _birdIdleStateAnim.Kill();
        JumpUp(new InputAction.CallbackContext());
    }

    public void JumpUp(InputAction.CallbackContext context)
    {
        if (context.phase != InputActionPhase.Started || _dead) return;

        _rigidbody2D.velocity = new Vector2(0, upSpeed);
        AudioSource.PlayClipAtPoint(jumpUp, Vector3.zero);
    }

    public void GameOver()
    {
        _dead = true;
    }
}