using UnityEngine;
using UnityEngine.UI;

public class TimeDisplay : MonoBehaviour
{
    Text _text;
    float _currentTime;

    void Awake()
    {
        _text = GetComponent<Text>();
        _currentTime = 0f;
    }

    //bu islem update icinde olmali
    void Update()
    {
        _currentTime += Time.deltaTime;
        _text.text = _currentTime.ToString("00");
    }
}