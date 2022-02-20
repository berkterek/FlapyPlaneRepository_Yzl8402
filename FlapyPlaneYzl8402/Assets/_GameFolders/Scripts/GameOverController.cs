using UnityEngine;

public class GameOverController : MonoBehaviour
{
    CanvasGroup _canvasGroup;

    void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    void OnEnable()
    {
        GameManager.Instance.OnGameOvered += HandleOnGameOvered;
    }

    void OnDisable()
    {
        GameManager.Instance.OnGameOvered -= HandleOnGameOvered;
    }

    void Start()
    {
        _canvasGroup.alpha = 0f;
        _canvasGroup.interactable = false;
        _canvasGroup.blocksRaycasts = false;
    }
    
    void HandleOnGameOvered()
    {
        _canvasGroup.alpha = 1f;
        _canvasGroup.interactable = true;
        _canvasGroup.blocksRaycasts = true;
    }
}
