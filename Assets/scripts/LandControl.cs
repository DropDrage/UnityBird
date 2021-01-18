using DG.Tweening;
using UnityEngine;

public class LandControl : MonoBehaviour
{
    private Sequence _landSequence;

    private void Start()
    {
        // land continue moving
        _landSequence = DOTween.Sequence();

        var position = transform.position;
        _landSequence.Append(transform.DOMoveX(position.x - 0.48f, 0.5f).SetEase(Ease.Linear))
            .Append(transform.DOMoveX(position.x, 0f).SetEase(Ease.Linear))
            .SetLoops(-1);
    }

    public void GameOver()
    {
        _landSequence.Kill();
    }
}