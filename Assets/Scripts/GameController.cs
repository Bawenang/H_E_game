using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;


public class GameController : MonoBehaviour {

    public delegate void GameTickCallback(float timer, float timeLeft, float timeIncrement);
    public static event GameTickCallback OnGameTick;

    public delegate void ElementGained(ElementType eleType);
    public static event ElementGained OnElementGained;

    public delegate void ElementCompleted(ElementType eleType);
    public static event ElementCompleted OnElementCompleted;

    static readonly Color sunnyColor = new Color(0.0f, 0.0f, 0.0f, 0.0f);
    static readonly Color cloudyColor = new Color(0.0f, 0.0f, 0.0f, 0.25f);

    public Board_C boardController;
    public DialogController dialogCtrl;
    public ElementController eleController;
    public GlobalEventController globalEventController;
    public Goals goals;
    public StageDisplay stageDisplay;
    public int currentStage = 1;


    public Image pauseIcon;
    public Image unpauseIcon;
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
            pauseIcon.gameObject.SetActive(_pause);
            unpauseIcon.gameObject.SetActive(!_pause);
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

        AudioController.OnSoundChanged += OnSoundChanged;


        SetAudios();



        eleController.gameController = this;
        globalEventController.gameController = this;

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
                    LOLSubmitProgressWithCurrentScore(1);
                    break;
                case 2:
                    LOLSubmitProgressWithCurrentScore(4);
                    break;
                case 3:
                    LOLSubmitProgressWithCurrentScore(6);
                    break;
            }
        }
#endif
    }

    // Update is called once per frame
    void Update() {
        if (boardController.player_turn && !dialogCtrl.isShown && goals.Get(GoalType.HUMANITY) >= goals.maxHumanity )
        {
            boardController.Player_win();
            boardController.Game_end();
        }
        else if (boardController.player_turn && !dialogCtrl.isShown && goals.Get(GoalType.EARTH) <= 0)
        {
            boardController.Player_lose(false);
        }

    }

    void OnDestroy()
    {
        AudioController.OnSoundChanged -= OnSoundChanged;
#if UNITY_WEBGL
        if (LoLController.Exists() && LoLController.instance.isUsingLoL)
        {
            switch (currentStage)
            {
                case 1:
                    LOLSubmitProgressWithCurrentScore(3);
                    break;
                case 2:
                    LOLSubmitProgressWithCurrentScore(5);
                    break;
                case 3:
                    LOLSubmitProgressWithCurrentScore(7);
                    break;
            }
        }
#endif
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

    public void AddElement(ElementType eleType)
    {
        eleController.AddElement(eleType);
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
            LoLController.instance.LOLSubmitProgressWithCurrentScore(progress);
    }

    public void OnElementCompletedImpl(ElementType eleType)
    {
        if (OnElementCompleted != null)
        {
            OnElementCompleted(eleType);
        }
    }

    public void SetElementCompleteEvents()
    {
        eleController.SetElementCompleteEvents();
    }
}
