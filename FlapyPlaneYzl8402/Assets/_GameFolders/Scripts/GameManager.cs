using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] float _currentTime = 0f;
    [SerializeField] int _score = 0;
    [SerializeField] int _bestScore = 0;

    const string BEST_SCORE = "best_score";

    public float CurrentTime
    {
        get
        {
            _currentTime += Time.deltaTime;
            return _currentTime;
        }
    }

    public static GameManager Instance { get; private set; }

    //bu event iki int method'u alan methodlar icine atanablir
    public event Action<int, int> OnGameOvered;

    void Awake()
    {
        SingletonThisObject();
    }

    void Start()
    {
        // //uzun yanimi eger bu key varsa bestscore verisini cek
        //  if (PlayerPrefs.HasKey(BEST_SCORE))
        //  {
        //      _bestScore = PlayerPrefs.GetInt(BEST_SCORE);    
        //  }
        //  else
        //  {
        //       //yoksa best score'a 0 degerini ata 
        //      _bestScore = 0;
        //  }

        //kisa yazimi
        //BEST_SCORE varsa o datayi getir yoksa varsayilian olarak 0 degerini ata demis oluyoruz
        _bestScore = PlayerPrefs.GetInt(BEST_SCORE, 0);
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

        _score = (int)_currentTime;

        //eger score buyukse best score'dan 
        if (_score > _bestScore)
        {
            //demekki yeni best score simdiki score olur
            _bestScore = _score;
            PlayerPrefs.SetInt(BEST_SCORE, _bestScore);
        }

        _currentTime = 0f;

        //kisa yazimi
        //eger null degilse bu OnGameOvered event'i tetikle
        OnGameOvered?.Invoke(_score, _bestScore);
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
        SceneManager.LoadScene("Menu");
    }
}