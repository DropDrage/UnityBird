using DG.Tweening;
using UnityEngine;

public class GameMain : MonoBehaviour
{
    public Bird bird;
    public SpriteRenderer readyPic;
    public SpriteRenderer tipPic;
    public ScoreManager scoreManager;
    public PipeSpawner pipeSpawner;

    private bool _isGameStarted;


    public void StartGame()
    {
        if (_isGameStarted) return;

        bird.SetPlayableState();

        readyPic.material.DOFade(0f, 0.2f);
        tipPic.material.DOFade(0f, 0.2f);

        scoreManager.SetScore(0);
        pipeSpawner.StartSpawning();
        _isGameStarted = true;
    }
}