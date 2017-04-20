using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum GoalType
{
    SCORE,
    EARTH,
    HUMANITY,
    WEALTH,
    LAW
}

[System.Serializable]
public class GoalsData
{
    public int score;
    public int earth;
    public int humanity;
    public int wealth;
    public int law;
}

public class Goals : MonoBehaviour {

    [SerializeField] private GoalsData _data;

    public GameController gameController;

    public int maxEarth;
    public int maxHumanity;

    //UI
    [SerializeField] Text _scoreValueUI;
    [SerializeField] Image _earthValueUI;
    [SerializeField] Image _humanityValueUI;
    [SerializeField] Text _wealthValueUI;
    [SerializeField] Text _lawValueUI;

	// Use this for initialization
	void Start () {
        _earthValueUI.fillAmount = 1.0f;
        _humanityValueUI.fillAmount = 0.0f;

        _data.earth = maxEarth;
        _data.humanity = 0;

        _scoreValueUI.text = "0";
        if (_wealthValueUI != null)
            _wealthValueUI.text = "0";
        if (_lawValueUI != null)
            _lawValueUI.text = "0";
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Add(GoalType goalType, int value)
    {
        switch(goalType)
        {
            case GoalType.SCORE:
                _data.score += value;
                //_scoreValueUI.text = _data.score.ToString();
                gameController.boardController.player_score += value;
                break;
            case GoalType.EARTH:
                _data.earth += value;
                _earthValueUI.fillAmount = (float)_data.earth / (float)maxEarth;
                break;
            case GoalType.HUMANITY:
                _data.humanity += value;
                _humanityValueUI.fillAmount = (float)_data.humanity / (float)maxHumanity;
                break;
            case GoalType.WEALTH:
                _data.wealth += value;
                if (_wealthValueUI != null)
                    _wealthValueUI.text = _data.wealth.ToString();
                break;
            case GoalType.LAW:
                _data.law += value;
                if (_lawValueUI != null)
                    _lawValueUI.text = _data.law.ToString();
                break;
        }
        
    }

    public void Subtract(GoalType goalType, int value)
    {
        switch (goalType)
        {
            case GoalType.SCORE:
                _data.score -= value;
                //_scoreValueUI.text = _data.score.ToString();
                gameController.boardController.player_score -= value;
                break;
            case GoalType.EARTH:
                _data.earth -= value;
                _earthValueUI.fillAmount = (float)_data.earth / (float)maxEarth;
                break;
            case GoalType.HUMANITY:
                _data.humanity -= value;
                _humanityValueUI.fillAmount = (float)_data.humanity / (float)maxHumanity;
                break;
            case GoalType.WEALTH:
                _data.wealth -= value;
                if (_wealthValueUI != null)
                    _wealthValueUI.text = _data.wealth.ToString();
                break;
            case GoalType.LAW:
                _data.law -= value;
                if (_lawValueUI != null)
                    _lawValueUI.text = _data.law.ToString();
                break;
        }

    }

    public int Get(GoalType goalType)
    {
        switch (goalType)
        {
            case GoalType.SCORE:
                return _data.score;
            case GoalType.EARTH:
                return _data.earth;
            case GoalType.HUMANITY:
                return _data.humanity;
            case GoalType.WEALTH:
                return _data.wealth;
            case GoalType.LAW:
                return _data.law;
        }

        return 0;
    }

}
