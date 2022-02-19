using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] float _moveSpeed = 1f;
    [SerializeField] float _deadCounterSpeed = 1f;
    [SerializeField] float _maxDeadTime = 10f;
    
    Rigidbody2D _rigidbody2D;
    float _counter;

    void Awake()
    {
        _counter = 0f;
        _rigidbody2D = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        _rigidbody2D.velocity = Vector2.left * _moveSpeed;
    }

    void Update()
    { 
        //Time.deltaTime ile burda bir sayac olusturduk ve bu sayaca speed atadik istersek bu speed'i arttirabliir veya azaltabilriz
        _counter += Time.deltaTime * _deadCounterSpeed;

        if (_counter > _maxDeadTime)
        {
            Destroy(this.gameObject);
        }

        Debug.Log(_counter);
    }
}