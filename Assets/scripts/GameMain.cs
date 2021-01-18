using DG.Tweening;
using UnityEngine;

public class GameMain : MonoBehaviour
{
    public GameObject readyPic;
    public GameObject tipPic;
    public ScoreManager scoreManager;
    public PipeSpawner pipeSpawner;


    public void StartGame()
    {
        readyPic.GetComponent<SpriteRenderer>().material.DOFade(0f, 0.2f);
        tipPic.GetComponent<SpriteRenderer>().material.DOFade(0f, 0.2f);

        pipeSpawner.StartSpawning();
    }
}