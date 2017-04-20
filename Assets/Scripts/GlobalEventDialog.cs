using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GlobalEventDialog : MonoBehaviour {

    [SerializeField] private Text _titleText;
    [SerializeField] private Text _descriptionText;

    [SerializeField] private EventEffectUI[] _effects;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Show(GlobalEvent evt)
    {
        _titleText.text = evt.eventTitle;
        _descriptionText.text = evt.eventDescription;

        for (int i = 0; i < _effects.Length; ++i)
        {
            int value = 0;
            switch (_effects[i].goalType)
            {
                case GoalType.EARTH:
                    value = evt.goalData.earth;
                    break;
                case GoalType.HUMANITY:
                    value = evt.goalData.humanity;
                    break;
                case GoalType.WEALTH:
                    value = evt.goalData.wealth;
                    break;
                case GoalType.LAW:
                    value = evt.goalData.law;
                    break;
            }

            if (value > 0)
            {
                _effects[i].Show(EffectArrowType.UP);
            }
            else if (value < 0)
            {
                _effects[i].Show(EffectArrowType.DOWN);
            }
            else
            {
                _effects[i].Show(EffectArrowType.EQUAL);
            }
        }
    }
}
