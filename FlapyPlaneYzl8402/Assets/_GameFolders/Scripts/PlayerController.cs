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

    //Update method her bir framede bir calisir ve biz input alicaksak update icinde alir
    void Update()
    {
        //Input class unity developerlarin bizim icin hazirladigi eksi input sistemidir
        Debug.Log(Input.GetButtonDown("Jump"));
    }
}
