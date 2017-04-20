using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementController : MonoBehaviour {

    [SerializeField] Element[] _elements;
    private GameController _gameController;

    public GameController gameController
    {
        get
        {
            return _gameController;
        }

        set
        {
            _gameController = value;
            for (int i = 0; i < _elements.Length; ++i)
            {
                _elements[i].gameController = _gameController;
            }
        }
    }

    void Awake()
    {
        for (int i = 0; i < _elements.Length; ++i)
        {
            _elements[i].elementType = (ElementType) i;
        }
    }

	// Use this for initialization
	void Start ()
    {
        
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void AddElement(ElementType eleType)
    {
        _elements[(int)eleType].Add(1);
    }

    public void SetElementCompleteEvents()
    {
        for (int i = 0; i < _elements.Length; ++i)
        {
            _elements[i].SetElementCompleted();
        }
    }
}
