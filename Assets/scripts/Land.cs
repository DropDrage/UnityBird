using DG.Tweening;
using UnityEngine;

public class Land : MonoBehaviour
{
    private Sequence _landSequence;


    private void Start()
    {
        var position = transform.position;
        _landSequence = DOTween.Sequence()
            .Append(transform.DOMoveX(position.x - 0.48f, 0.5f).SetEase(Ease.Linear))
            .Append(transform.DOMoveX(position.x, 0f).SetEase(Ease.Linear))
            .SetLoops(-1);
    }

    public void GameOver()
    {
        _landSequence.Kill();
    }
}