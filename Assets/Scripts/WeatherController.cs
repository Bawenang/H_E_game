using UnityEngine;
using System.Collections;

public enum WeatherType
{
    SUNNY,
    CLOUDY,
    RAINY
}

public class WeatherController : MonoBehaviour {

    public delegate void WeatherChangeCallback(WeatherType weatherType);
    public static event WeatherChangeCallback OnWeatherChange;

    [SerializeField] private GameController _gameCtrl;

    float _lastTime = -1.0f;

    [SerializeField] private float _weatherCheckStep = 30.0f; //check every 30 secs.

    [SerializeField][Range(0.0f, 1.0f)] private float _sunnyProbability = 0.5f;

    WeatherType _weatherType = WeatherType.SUNNY;

    bool _hasShownDialog = false;
    bool _isShowingDialog = false;

    public WeatherType weatherType
    {
        get
        {
            return _weatherType;
        }

        set
        {
            _weatherType = value;

            if (OnWeatherChange != null)
                OnWeatherChange(value);
        }
    }

    void Awake()
    {
        GameController.OnGameTick += OnGameTick;
    }

	// Use this for initialization
	void Start () {
	
	}

    void OnDestroy()
    {
        GameController.OnGameTick -= OnGameTick;
    }

    public void OnGameTick(float timer, float timeLeft, float deltaTime)
    {
        if (_lastTime < 0.0f)
            _lastTime = timer;

        if (( _lastTime - timeLeft ) >= _weatherCheckStep)
        {
            _lastTime = timeLeft;

            if (weatherType != WeatherType.SUNNY)
            {
                weatherType = WeatherType.SUNNY;

                if (_gameCtrl.currentStage == 2 && _isShowingDialog)
                {
                    _isShowingDialog = false;
                    _gameCtrl.dialogCtrl.PlaySequence(7, true);

#if UNITY_WEBGL
                    if (LoLController.Exists() && LoLController.instance.isUsingLoL)
                        _gameCtrl.LOLSubmitProgressWithCurrentScore(7);
#endif
                }
            }
            else
            {
                if ( _gameCtrl.currentStage == 2 && !_hasShownDialog)
                {
                    weatherType = WeatherType.CLOUDY;

                    _hasShownDialog = true;
                    _isShowingDialog = true;
                    _gameCtrl.dialogCtrl.PlaySequence(6, true);

#if UNITY_WEBGL
                    if (LoLController.Exists() && LoLController.instance.isUsingLoL)
                        _gameCtrl.LOLSubmitProgressWithCurrentScore(6);
#endif
                }
                else if (_gameCtrl.currentStage != 2)
                {
                    float randomChance = Random.Range(0.0f, 1.0f);

                    if (randomChance > _sunnyProbability)
                    {
                        weatherType = WeatherType.CLOUDY;
                    }
                }
            }
        }
    }
}
