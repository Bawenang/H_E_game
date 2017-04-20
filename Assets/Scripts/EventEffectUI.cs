using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum EffectArrowType
{
    EQUAL,
    UP,
    DOWN
}

public class EventEffectUI : MonoBehaviour {

    public GoalType goalType;

    [SerializeField] private Image _icon;
    [SerializeField] private Image _arrowUp;
    [SerializeField] private Image _arrowDown;
    [SerializeField] private Image _arrowEqual;
	
    // Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Show(EffectArrowType arrowType)
    {
        switch(arrowType)
        {
            case EffectArrowType.EQUAL:
                _arrowEqual.gameObject.SetActive(true);
                _arrowUp.gameObject.SetActive(false);
                _arrowDown.gameObject.SetActive(false);
                break;
            case EffectArrowType.UP:
                _arrowEqual.gameObject.SetActive(false);
                _arrowUp.gameObject.SetActive(true);
                _arrowDown.gameObject.SetActive(false);
                break;
            case EffectArrowType.DOWN:
                _arrowEqual.gameObject.SetActive(false);
                _arrowUp.gameObject.SetActive(false);
                _arrowDown.gameObject.SetActive(true);
                break;
        }
    }
}
