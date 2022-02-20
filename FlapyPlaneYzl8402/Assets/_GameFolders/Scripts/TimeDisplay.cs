using UnityEngine;
using UnityEngine.UI;

public class TimeDisplay : MonoBehaviour
{
    Text _text;

    void Awake()
    {
        _text = GetComponent<Text>();
    }

    void Update()
    {
        _text.text = GameManager.Instance.CurrentTime.ToString("00");
    }
}