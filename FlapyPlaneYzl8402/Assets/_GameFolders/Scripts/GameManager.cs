using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public event Action OnGameOvered;

    void Awake()
    {
        SingletonThisObject();
    }

    void SingletonThisObject()
    {
        //Instance bos ise
        if (Instance == null)
        {
            //Instance'a bu refeeransi at
            Instance = this;
            transform.parent = null;
            //DontDestroyOnLoad method'u bizim gameobject'lerimizi yok etme dedigmiz bir method'dur
            DontDestroyOnLoad(this.gameObject);
            //ve bu gameobject nesnenisi yok etme hicbir zaman(biz kendimiz kod ile yok etmedigmiz surece)
        }
        else
        {
            //eger Instance bos deiglse bu nesne daha onceden olsuturudulmus ikinci bir ayni nesneye gerek yok bunu yok et demis oluyoruz
            Destroy(this.gameObject);
        }
    }

    public void GameOver()
    {
        //uzun yazimi
        // if (OnGameOvered != null)
        // {
        //     OnGameOvered.Invoke();
        // }
        
        //kisa yazimi
        //eger null degilse bu OnGameOvered event'i tetikle
        OnGameOvered?.Invoke();
        Time.timeScale = 0f;
    }

    //ContextMenu attribute'u bizim bu method'a inspector uzerinde ulasmamizi ve calistirmmaizi saglar
    [ContextMenu(nameof(LoadGameScene))]
    public void LoadGameScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Game");
    }

    public void LoadMenu()
    {
        //TODO this method for load menu scene
        Debug.Log("Return Menu On Triggered");
    }
}