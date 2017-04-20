using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

using LoLSDK;

public class LoLController : MonoBehaviour
{

    [SerializeField] private bool _isUsingLoL;
    [SerializeField] private int _maxProgress;
    [SerializeField] private bool _isDisplayingPerformanceTool;
    [SerializeField] private string _sceneAfterInit = "mainmenu";

    private int _score = 0;
    private int _level = 0;
    private int _maxLevel = 10;
    private MultipleChoiceQuestionList _questionList;
    private int _currentQuestionIndex = 0;

    public bool isUsingLoL
    {
        get
        {
            return _isUsingLoL;
        }
    }

    public int score
    {
        get
        {
            return _score;
        }
        set
        {
            _score = value;
        }
    }

    public int level
    {
        get
        {
            return _level;
        }
        set
        {
            _level = value;
        }
    }

    public int maxLevel
    {
        get
        {
            return _maxLevel;
        }
        set
        {
            _maxLevel = value;
        }
    }

    public MultipleChoiceQuestionList questionList
    {
        get
        {
            return _questionList;
        }
        set
        {
            _questionList = value;

        }
    }

    public int currentQuestionIndex
    {
        get
        {
            return _currentQuestionIndex;
        }
    }

    //----------------------------------------------------------------
    // Singleton code
    //----------------------------------------------------------------
    // s_Instance is used to cache the instance found in the scene so we don't have to look it up every time.
    private static LoLController s_Instance = null;

    protected bool m_started = false;           // Lets us detect whether Start() has yet been called.
    protected bool m_awake = false;             // Lets us detect whether Awake() has yet been called.

    // This defines a static instance property that attempts to find the manager object in the scene and
    // returns it to the caller.
    public static LoLController instance
    {
        get
        {
            if (s_Instance == null)
            {
                LoLController tmpInst = FindObjectOfType(typeof(LoLController)) as LoLController;
                if (tmpInst != null)
                    tmpInst.Awake();
                s_Instance = tmpInst;

                if (s_Instance == null && Application.isEditor)
                    Debug.LogError("Could not locate a LoLController object. You have to have exactly one LoLController in the scene.");
            }

            return s_Instance;
        }
    }

    public static bool Exists()
    {
        if (s_Instance == null)
        {
            LoLController tmpInst = FindObjectOfType(typeof(LoLController)) as LoLController;
            if (tmpInst != null)
                tmpInst.Awake();
            s_Instance = tmpInst;
        }

        return s_Instance != null;
    }

    public void OnDestroy()
    {
        if (s_Instance == this)
            s_Instance = null;
    }
    //----------------------------------------------------------------
    // End Singleton code
    //----------------------------------------------------------------

    void Awake()
    {
        if (m_awake)
            return;
        m_awake = true;

        // See if we are a superfluous instance:
        if (s_Instance != null)
        {
            Debug.LogError("You can only have one instance of this singleton object in existence.");
        }
        else
            s_Instance = this;

        DontDestroyOnLoad(this.gameObject);

        Application.targetFrameRate = -1;

        if (_isUsingLoL)
        {
            Startup();
        }

        if (_isDisplayingPerformanceTool)
            LOLSDK.Instance.DisplayPerformanceTestTool();

        if (!string.IsNullOrEmpty(_sceneAfterInit))
            SceneManager.LoadScene(_sceneAfterInit);
    }

    // Use this for initialization
    void Start()
    {
        if (m_started)
            return;
        m_started = true;

    }


    void Startup()
    {
        LOLSDK.Init("com.zeroexperiencestudio.heultimatestruggle");
        LOLSDK.Instance.QuestionsReceived += new QuestionListReceivedHandler(this.QuestionsReceived);

        //LOLSDK.Instance.SubmitProgress(Score, Level, MaxLevel);

    }

    public void QuestionsReceived(MultipleChoiceQuestionList questionList)
    {
        this.questionList = questionList;
    }

    public MultipleChoiceQuestion GetQuestion()
    {
        if (questionList != null && questionList.questions != null && _currentQuestionIndex < questionList.questions.Length)
        {
            MultipleChoiceQuestion question = questionList.questions[_currentQuestionIndex];
            _currentQuestionIndex++;
            return question;
        }

        return null;
    }

    public void LOLSubmitProgressWithCurrentScore(int progress)
    {
        if (LoLController.Exists() && LoLController.instance.isUsingLoL)
            LoLSDK.LOLSDK.Instance.SubmitProgress(LoLController.instance.score, progress, _maxProgress);
    }
}
