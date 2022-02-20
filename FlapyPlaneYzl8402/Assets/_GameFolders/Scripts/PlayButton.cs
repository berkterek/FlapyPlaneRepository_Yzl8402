using UnityEngine;
using UnityEngine.UI;

public class PlayButton : MonoBehaviour
{
    Button _button;

    void Awake()
    {
        _button = GetComponent<Button>();
    }

    void OnEnable()
    {
        //onClick bir unity event'tir ve bu event'lere AddListener ile method code ile atanir
        _button.onClick.AddListener(HandleOnButtonClicked);
    }

    void OnDisable()
    {
        //onClick bir unity event'tir ve unity event'ten method cikarmak icin RemoveListener ile cikaririz
        _button.onClick.RemoveListener(HandleOnButtonClicked);
    }
    
    void HandleOnButtonClicked()
    {
        GameManager.Instance.LoadGameScene();
    }
}
