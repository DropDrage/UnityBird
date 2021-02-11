using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class StartMain : MonoBehaviour
{
    public SpriteRenderer backgroundSprite;
    public Sprite[] backList;

    private GameObject _nowPressBtn;


    private void Start()
    {
        var index = Random.Range(0, backList.Length);
        backgroundSprite.sprite = backList[index];
    }

    public void HandleTouch(InputAction.CallbackContext context)
    {
        var worldPoint = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        var worldPos = new Vector2(worldPoint.x, worldPoint.y);
        switch (context.phase)
        {
            case InputActionPhase.Started:

                foreach (var c in Physics2D.OverlapPointAll(worldPos))
                {
                    name = c.gameObject.name;
                    print(name);
                    if (name == "start_btn" || name == "rank_btn" || name == "rate_btn")
                    {
                        c.transform.DOMoveY(c.transform.position.y - 0.03f, 0f);
                        _nowPressBtn = c.gameObject;
                    }
                }

                break;
            case InputActionPhase.Canceled:
                if (_nowPressBtn)
                {
                    _nowPressBtn.transform.DOMoveY(_nowPressBtn.transform.position.y + 0.03f, 0f);

                    foreach (var c in Physics2D.OverlapPointAll(worldPos))
                    {
                        name = c.gameObject.name;
                        print(name);

                        if (name == _nowPressBtn.name && name == "start_btn")
                        {
                            OnPressStart();
                        }
                    }

                    _nowPressBtn = null;
                }

                break;
            case InputActionPhase.Disabled:
                break;
            case InputActionPhase.Waiting:
                break;
            case InputActionPhase.Performed:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(context.phase), context.phase, null);
        }
    }

    private static void OnPressStart()
    {
        SceneManager.LoadScene(1);
    }
}