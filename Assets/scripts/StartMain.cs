using System;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class StartMain : MonoBehaviour
{
    public GameObject back_ground;
    public Sprite[] back_list;

    private GameObject nowPressBtn;

    // Use this for initialization
    private void Start()
    {
        // random background
        var index = Random.Range(0, back_list.Length);
        back_ground.GetComponent<SpriteRenderer>().sprite = back_list[index];
    }

    // Update is called once per frame
    private void Update()
    {
        // Handle native touch events
        foreach (var touch in Input.touches)
        {
            HandleTouch(touch.position, touch.phase);
        }

        // Simulate touch events from mouse events
        if (Input.touchCount != 0) return;
        if (Input.GetMouseButtonDown(0))
        {
            HandleTouch(Input.mousePosition, TouchPhase.Began);
        }

        if (Input.GetMouseButton(0))
        {
            HandleTouch(Input.mousePosition, TouchPhase.Moved);
        }

        if (Input.GetMouseButtonUp(0))
        {
            HandleTouch(Input.mousePosition, TouchPhase.Ended);
        }
    }

    private void HandleTouch(Vector2 touchPosition, TouchPhase touchPhase)
    {
        var wp = Camera.main.ScreenToWorldPoint(touchPosition);
        var worldPos = new Vector2(wp.x, wp.y);
        switch (touchPhase)
        {
            case TouchPhase.Began:
                print(touchPosition);
                print(worldPos);

                foreach (var c in Physics2D.OverlapPointAll(worldPos))
                {
                    name = c.gameObject.name;
                    print(name);
                    if (name == "start_btn" || name == "rank_btn" || name == "rate_btn")
                    {
                        c.transform.DOMoveY(c.transform.position.y - 0.03f, 0f);
                        nowPressBtn = c.gameObject;
                    }
                }

                break;
            case TouchPhase.Moved:
                // TODO
                break;
            case TouchPhase.Ended:
                if (nowPressBtn)
                {
                    nowPressBtn.transform.DOMoveY(nowPressBtn.transform.position.y + 0.03f, 0f);

                    foreach (var c in Physics2D.OverlapPointAll(worldPos))
                    {
                        name = c.gameObject.name;
                        print(name);

                        if (name == nowPressBtn.name && name == "start_btn")
                        {
                            OnPressStart();
                        }
                    }

                    nowPressBtn = null;
                }

                break;
            case TouchPhase.Stationary:
                break;
            case TouchPhase.Canceled:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(touchPhase), touchPhase, null);
        }
    }

    private static void OnPressStart()
    {
        Application.LoadLevel(1);
    }
}