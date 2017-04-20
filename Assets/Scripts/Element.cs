using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ElementType
{
    SURVIVAL,
    RESOURCE,
    INDUSTRY,
    ECONOMY,
    LAW,
    ENVIRONMENT
    
}



public class Element : MonoBehaviour {

    public ElementType elementType;
    [SerializeField] private Image _icon;
    [SerializeField] private Image _bar;

    [SerializeField] private int _maxValue;
    [SerializeField] private int _curValue;

    [SerializeField]
    private bool _isActiveEventOnStart = true;

    public GameController gameController;

    public void OnEnable()
    {
        if (_isActiveEventOnStart)
            GameController.OnElementCompleted += OnElementCompleted;
    }

    public void OnDisable()
    {
        GameController.OnElementCompleted -= OnElementCompleted;
    }

	// Use this for initialization
	void Start ()
    {
        _bar.fillAmount = 0.0f;
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public void Add(int value)
    {
        _curValue += value;

        if (elementType == ElementType.LAW)
        {
            gameController.goals.Add(GoalType.LAW, 1);
        }

        if (_curValue >= _maxValue)
        {
            _curValue -= _maxValue;

            //OnElementCompleted(elementType);
            gameController.OnElementCompletedImpl(elementType);
        }

        if (_bar != null)
        {
            _bar.fillAmount = (float)_curValue / (float)_maxValue;
        }
    }

    public void Subtract(int value)
    {
        _curValue -= value;

        if (_curValue <= 0)
        {
            _curValue = 0;
        }

        if (_bar != null)
        {
            _bar.fillAmount = (float)_curValue / (float)_maxValue;
        }
    }

    public void OnElementCompleted(ElementType eleType)
    {
        if (eleType == elementType)
            gameController.globalEventController.EnqueueRandom(elementType);
    }

    public void SetElementCompleted()
    {
        GameController.OnElementCompleted += OnElementCompleted;
    }
}
