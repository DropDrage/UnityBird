using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public Sprite[] scoreSprites;
    public GameObject scorePrefab;
    public float digitOffset;

    private readonly LinkedList<SpriteRenderer> _nowShowScores = new LinkedList<SpriteRenderer>();
    private int _nowScore;


    private void Start()
    {
        _nowScore = 0;
        SetScore(_nowScore);
    }

    public void AddScore()
    {
        _nowScore++;
        SetScore(_nowScore);
    }

    public void SetScore(int score)
    {
        var digits = score.ToString().Select(d => d - '0').ToArray();
        var scoreSize = digits.Length;
        var nowOffset = (scoreSize - 1) * digitOffset;

        if (scoreSize > _nowShowScores.Count)
        {
            var transform1 = transform;
            var position = transform1.position;
            var nowX = position.x - nowOffset;
            var pos = new Vector2(nowX, position.y);

            var newScore = Instantiate(scorePrefab, pos, transform1.rotation).GetComponent<SpriteRenderer>();
            newScore.sprite = scoreSprites[digits.Last()];
            _nowShowScores.AddFirst(newScore);
        }

        var i = 0;
        for (var node = _nowShowScores.First; node != null; node = node.Next, ++i)
        {
            if (digits[i] == node.Value?.sprite.name.LastOrDefault() - '0') continue;

            node.Value.sprite = scoreSprites[digits[i]];
        }
    }
}