using UnityEngine;
using UnityEngine.UI;

public class TimeDisplay : MonoBehaviour
{
    Text _text;

    void Awake()
    {
        _text = GetComponent<Text>();
    }

    void Start()
    {
        //text'e yazi atma islemi
        _text.text = "timer";
    }

    //bu islem update icinde olmali
    void Update()
    {
        
    }
}