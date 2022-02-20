using UnityEngine;
using UnityEngine.UI;

public class ScoreText : MonoBehaviour
{
    Text _text;

    void Awake()
    {
        _text = GetComponent<Text>();
    }

    void OnEnable()
    {
        GameManager.Instance.OnGameOvered += HandleOnGameOvered;
    }

    void OnDisable()
    {
        GameManager.Instance.OnGameOvered -= HandleOnGameOvered;
    }

    void HandleOnGameOvered(int score, int bestScore)
    {
        _text.text = score.ToString();
    }
}
