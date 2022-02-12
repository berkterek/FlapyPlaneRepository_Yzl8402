using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region OnCollision ve OnTrigger kullanimi ve arasindaki farklar

    //OnCollisionEnter2D iki nesne birbirine temas ettigi andir
    // void OnCollisionEnter2D(Collision2D other)
    // {
    //     Debug.Log(nameof(OnCollisionEnter2D));
    // }

    //OnTriggerEnter2D iki nesne birbirine temas ettigi andir
    //OnTrigger ve OnCollision arasindaki fark sudur OnCollision iki nesne carpisir ve birbirinin icinden gecmez OnTrigger'^da ise iki nesne carpisir ve iki nensne birbirinin icinden hayalet gibi gecer
    // void OnTriggerEnter2D(Collider2D other)
    // {
    //     Debug.Log(nameof(OnTriggerEnter2D));
    // }

    #endregion

    [Range(1f,500f)]
    [Tooltip("Force power is used with rigidbody add force method")]
    [SerializeField] float _forcePower = 250f;

    Rigidbody2D _rigidbody2D;
    bool _isJump;

    //bu yontem ile biz kendi uzerimizdeki veya baska gameobject uzerindeki rigidbody component'ine ulasablirzi
    //[SerializeField] Rigidbody2D _rigidbody2D;

    //GetButtonDown() => Input/ her seferinde bir kere ziplama yapmaisni istiyoruz
    //Rigidbody2D => bizim haraket islemi icin kullanicaz

    //Awake method calisma zamani bir kere calir ayni constructor gibi
    void Awake()
    {
        //calisma zmaaini rigidbody2d'i bir kere cache'lemis olduk
        _rigidbody2D = GetComponent<Rigidbody2D>();
    }

    //Update method her bir framede bir calisir ve biz input alicaksak update icinde alir
    void Update()
    {
        //Input class unity developerlarin bizim icin hazirladigi eksi input sistemidir
        //input Jump yani space bastikca bize true doner basmazsak bize false doner
        //Debug.Log(Input.GetButtonDown("Jump"));
        //GetButton basili oldugu kadar true doner
        //Debug.Log(Input.GetButton("Jump"));

        //biz yeni bir rigidbody istemiyoruz biz GameObject uzerindeki rigidbody2D component'ini istiyoruz
        //Rigidbody2D rigidbody2D = new Rigidbody2D();
        //GetComponent ile ayni yerde olan ayni GameObject uzerinde olan bir script'in referansina baska bir script ulasmis olur

        //hatayi yakalamak icin Debug.Log yontemini kullandik arka planda mantik hatasini update methodu icinde cozmek icin
        // bool result = Input.GetButtonDown("Jump");
        // bool result = _isJump;
        
        if (Input.GetButtonDown("Jump"))
        {
            //bu bir fizik islemidir 
            //fizik islemleri fixedupdate icinde yapilir cunku fixed update 0.02 saniyede bir veya fizik motoruna gore calisr
            //Rigidbody2D rigidbody2D = GetComponent<Rigidbody2D>(); //cache
            //rigidbody2D.velocity = new Vector2(0f, 0f); //fizik 
            //rigidbody2D.AddForce(new Vector2(0f, _forcePower)); //fizik
            _isJump = true;
        }

        // Debug.Log(result);
    }

    //fizik islemleri FixedUpdate icerisinde yapilir
    void FixedUpdate()
    {
        if (_isJump)
        {
            _rigidbody2D.velocity = new Vector2(0f,0f);
            _rigidbody2D.AddForce(new Vector2(0f,_forcePower));
            _isJump = false;
        }
    }
}