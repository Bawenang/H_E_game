using UnityEngine;
using System.Collections;

public class OxygenParticle : MonoBehaviour {

    [SerializeField] private float _startingSpeed = 300.0f;
    [SerializeField] private float _minSpeed = 100.0f;
    private float _speedRange = 200.0f;
    private Vector2 _direction = Vector2.one;

    private bool _isMoving;

    [SerializeField] private float _maxTime = 2.0f;
    [SerializeField] private float _minSpeedTime = 1.0f;
    private float _curTime;

    private Vector3 _oriPos = Vector3.zero; 

    // Use this for initialization
    void Awake () {
        _oriPos = this.transform.position;

        _speedRange = _startingSpeed - _minSpeed;
    }

	
	// Update is called once per frame
	void Update ()
    {
        if (_isMoving)
        {
            if (_curTime < _maxTime)
            {
                float speed = (_curTime < _minSpeedTime) ? EaseOutQuad(_startingSpeed, _minSpeed, _curTime / _minSpeedTime) : 0.0f;
                _curTime += Time.deltaTime;

                Vector3 displacement = _direction * speed;
                
                this.transform.position += displacement;
            }
            else
            {
                StopMove();
            }
        }

	
	}

    public void StartMove(Vector3 dir)
    {
        if (_oriPos != Vector3.zero)
            this.transform.position = _oriPos;
        _direction = dir;
        _isMoving = true;
        this.gameObject.SetActive(true);
        _curTime = 0.0f;
    }

    public void StopMove()
    {
        _direction = Vector2.zero;
        _isMoving = false;
        this.gameObject.SetActive(false);
        _curTime = 0.0f;
        this.transform.position = _oriPos;
    }

    private float EaseOutQuad(float start, float end, float value)
    {
        value--;
        end -= start;
        return end * (value * value * value + 1) + start;
    }
}
