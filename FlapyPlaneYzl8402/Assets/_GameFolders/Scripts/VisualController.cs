using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualController : MonoBehaviour
{
    //bu yontem en performasli yontemdir ama inspector uzerinden surukle birak yapmak lazim dogru nesneyi surukle birak yapmak lazim ve en cok basimaza gelen hata surukle birak yapmak unutulur ve hata olur
    //[SerializeField] SpriteRenderer _spriteRenderer;

    [SerializeField] float _speed = 1f;
    
    SpriteRenderer _spriteRenderer;
    

    void Awake()
    {
        //GetComponent ayni GameObject uzerinde olan Script'lerin referansina erisir
        //_spriteRenderer = GetComponent<SpriteRenderer>();
        
        //bu ise ilk buldugu SpriteRenderer'in referansi bize doner bu kendi GameObject uzeride de olablir veya child gameobject uzerinde de oalblir
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    void Start()
    {
        //true ise null
        //false ise null degildir
        Debug.Log(_spriteRenderer == null);
    }

    void Update()
    {
        float y = _spriteRenderer.size.y;
        float x = _spriteRenderer.size.x + (Time.deltaTime * _speed);
        _spriteRenderer.size = new Vector2(x, y);
    }
}
