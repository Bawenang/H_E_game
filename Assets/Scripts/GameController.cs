using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public enum ResourceType
{
    CO2,
    H2O,
    SUN,
    CHLORO
}

public class GameController : MonoBehaviour {

    public delegate void GameTickCallback(float timer, float timeLeft, float timeIncrement);
    public static event GameTickCallback OnGameTick;

    public delegate void SugarAdded(int value);
    public static event SugarAdded OnSugarAdded;

    static readonly Color sunnyColor = new Color(0.0f, 0.0f, 0.0f, 0.0f);
    static readonly Color cloudyColor = new Color(0.0f, 0.0f, 0.0f, 0.25f);

    public Board_C boardController;
    public DialogController dialogCtrl;
    public Chlorophyll[] chloroList;
    public StageDisplay stageDisplay;
    public int currentStage = 1;
    public int activeChloro;
    public int availableChloro;

    public Text sugarText;

    public int sugar;
    public int targetSugar;

    public WeatherType weather = WeatherType.SUNNY;
    //public Image weatherImg;
    public Image weatherIcon;
    public Sprite[] weatherSprites;
    public Sprite weatherGemFull;
    public Sprite weatherGemHalf;

    public Image pauseIcon;
    private bool _pause = false;

    [SerializeField] private Toggle _musicToggle;
    [SerializeField] private Toggle _soundToggle;

    [SerializeField] private string _bgmStr;
    [SerializeField] private AudioClip _bgmClip;

    public bool pause
    {
        get
        {
            return _pause;
        }

        set
        {
            _pause = value;
            weatherIcon.gameObject.SetActive(!_pause);
            pauseIcon.gameObject.SetActive(_pause);
        }
    }

    public int score
    {
        get
        {
            return boardController.player_score;
        }
        set
        {
            boardController.player_score = value;
        }
    }

    public bool isMusic
    {
        get
        {
            if (AudioController.Exists())
                return AudioController.instance.isMusic;
            else
                return false;
        }

        set
        {
            if (AudioController.Exists())
                AudioController.instance.isMusic = value;
        }
    }

    public bool isSound
    {
        get
        {
            if (AudioController.Exists())
                return AudioController.instance.isSound;
            else
                return false;
        }

        set
        {
            if (AudioController.Exists())
                AudioController.instance.isSound = value;
        }
    }

    // Use this for initialization
    void Awake () {
        boardController.gameController = this;
        dialogCtrl.gameCtrl = this;

        WeatherController.OnWeatherChange += OnWeatherChange;
        AudioController.OnSoundChanged += OnSoundChanged;



        SetWeather(weather);

        SetAudios();


        for (int i = 0; i < chloroList.Length; ++i)
        {
            chloroList[i].gameController = this;
            chloroList[i].gameObject.SetActive(i < activeChloro);
            chloroList[i].isAvailable = i < availableChloro;
        }

        sugar = 0;
        if (targetSugar == 0)
            targetSugar = 1;

        string targetSugarStr = targetSugar > 0 ? targetSugar.ToString() : "999";
        sugarText.text = sugar.ToString() + " / " + targetSugarStr;
	}

    void Start()
    {
        if (LoLController.Exists() && LoLController.instance.isUsingLoL)
            score = LoLController.instance.score;

        if (AudioController.Exists())
#if UNITY_WEBGL
            AudioController.instance.SetBGM_Name(_bgmStr);
#else
            AudioController.instance.SetBGMClip(_bgmClip);
#endif

#if UNITY_WEBGL
        if (LoLController.Exists() && LoLController.instance.isUsingLoL)
        {
            switch (currentStage)
            {
                case 1:
                    LOLSubmitProgressWithCurrentScore(0);
                    break;
                case 2:
                    LOLSubmitProgressWithCurrentScore(4);
                    break;
                case 3:
                    LOLSubmitProgressWithCurrentScore(9);
                    break;
            }
        }
#endif
    }
	
	// Update is called once per frame
	void Update() {
        if (boardController.player_turn && targetSugar > 0 && (sugar >= targetSugar))
        {
            boardController.Player_win();
            boardController.Game_end();
        }

    }

    void OnDestroy()
    {
        WeatherController.OnWeatherChange -= OnWeatherChange;
        AudioController.OnSoundChanged -= OnSoundChanged;


    }

    public void DoGameTick(float deltaTime)
    {
        if (GameController.OnGameTick != null)
            OnGameTick(boardController.timer, boardController.time_left, deltaTime);
    }

    public void SetAudios()
    {
        if (AudioController.Exists())
        {
            _musicToggle.isOn = AudioController.instance.isMusic;
            _soundToggle.isOn = AudioController.instance.isSound;
        }
    }

    public void AddSugar(int value)
    {
        sugar += value;
        string sugarStr = targetSugar > 0 ? Mathf.Min(sugar, targetSugar).ToString() : sugar.ToString();
        string targetSugarStr = targetSugar > 0 ? targetSugar.ToString() : "999";
        sugarText.text = sugarStr + " / " + targetSugarStr;

        for (int i = 0; i < chloroList.Length; ++i)
        {
            if (!chloroList[i].isAvailable)
            {
                chloroList[i].UpdateChloro();
            }
        }

        if (OnSugarAdded != null)
            OnSugarAdded(value);
    }

    public void AddResource(ResourceType resType, float value)
    {
        float increment = value;
        for (int i = 0; i < chloroList.Length; ++i)
        {
            if (resType == ResourceType.CHLORO || chloroList[i].isAvailable )
            {
                increment = chloroList[i].AddResource(resType, increment);

                if (increment == 0)
                    break;
            }
        }
    }

    public void SetWeather(WeatherType weaType)
    {

        weather = weaType;


        if (weaType == WeatherType.SUNNY)
        {
            //weatherImg.color = sunnyColor;

            boardController.gem_colors[2] = weatherGemFull;
        }
        else
        {
            //weatherImg.color = cloudyColor;

            boardController.gem_colors[2] = weatherGemHalf;
        }

        for (int i = 0; i < boardController._X_tiles; ++i)
        {
            for (int j = 0; j < boardController._Y_tiles; ++j)
            {
                if (weather == WeatherType.SUNNY && boardController.tile_sprites_array[i, j].sprite == weatherGemHalf)
                {
                    boardController.tile_sprites_array[i, j].sprite = weatherGemFull;
                }
                else if(boardController.tile_sprites_array[i, j].sprite == weatherGemFull)
                {
                    boardController.tile_sprites_array[i, j].sprite = weatherGemHalf;
                }
            }
        }

        weatherIcon.sprite = weatherSprites[(int)weaType];
    }

    public void OnWeatherChange(WeatherType weaType)
    {
        SetWeather(weaType);
    }

    public void StartGame()
    {
        StartCoroutine(StartCoRtn());
        dialogCtrl.StopSequence();
    }

    public void LoadMenu()
    {
        SceneManager.LoadScene("mainmenu");
    }

    public void Pause (bool isPause)
    {
        pause = isPause;
        if (isPause)
            dialogCtrl.PlaySequence((int)DialogType.PAUSE, true); //1 = pause sequence index
        else
            dialogCtrl.StopSequence();
    }

    IEnumerator StartCoRtn()
    {
        yield return new WaitForSeconds(0.6f);
        boardController.My_start();
    }

    public void LoadScene(int stage)
    {
        if (stage == 1)
        {
            SceneManager.LoadScene("game");
        }
        else if (stage == 2)
        {
            SceneManager.LoadScene("game2");
        }
        else if (stage == 3)
        {
            SceneManager.LoadScene("game3");
        }
        else if (stage == 4)
        {
            SceneManager.LoadScene("test");
            
        }
    }

    public void PlayButtonSfx()
    {
#if UNITY_WEBGL
        boardController.Play_sfx(boardController.button_sfx_str);
#else
        boardController.Play_sfx(boardController.button_sfx);
#endif
    }

    public void OnSoundChanged(bool soundValue)
    {
        for (int i = 0; i < boardController.audioSrcList.Count; ++i)
        {
            boardController.audioSrcList[i].mute = !soundValue;
        }
    }

    public void LOLSubmitProgressWithCurrentScore(int progress)
    {
        if (LoLController.Exists() && LoLController.instance.isUsingLoL)
            LoLSDK.LOLSDK.Instance.SubmitProgress(LoLController.instance.score, progress, 12);
    }
}
