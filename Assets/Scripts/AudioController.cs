using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class AudioController : MonoBehaviour {

    public delegate void MusicChanged(bool musicValue);
    public static event MusicChanged OnMusicChanged;

    public delegate void SoundChanged(bool soundValue);
    public static event SoundChanged OnSoundChanged;

    private string bgmNameString;
    [SerializeField] private AudioSource bgmSource;

    private static bool _isMusic = true;
    private static bool _isSound = true;

    public bool isMusic
    {
        get
        {
            return _isMusic;
        }

        set
        {
           
            _isMusic = value;

#if UNITY_WEBGL
            if (LoLController.Exists() && LoLController.instance.isUsingLoL)
                LoLSDK.LOLSDK.Instance.ConfigureSound(isSound ? 0.8f : 0.0f, value ? 0.6f : 0.0f, value ? 0.4f : 0.0f);
#else
            bgmSource.mute = !value;
#endif

            if (OnMusicChanged != null)
                OnMusicChanged(value);
        }
    }

    public bool isSound
    {
        get
        {
            return _isSound;
        }

        set
        {
            _isSound = value;

#if UNITY_WEBGL
            if (LoLController.Exists() && LoLController.instance.isUsingLoL)
                LoLSDK.LOLSDK.Instance.ConfigureSound(value ? 0.8f : 0.0f, isMusic ? 0.6f : 0.0f, isMusic ? 0.4f : 0.0f);
#endif

            if (OnSoundChanged != null)
                OnSoundChanged(value);
        }
    }

    //----------------------------------------------------------------
    // Singleton code
    //----------------------------------------------------------------
    // s_Instance is used to cache the instance found in the scene so we don't have to look it up every time.
    private static AudioController s_Instance = null;

    protected bool m_started = false;           // Lets us detect whether Start() has yet been called.
    protected bool m_awake = false;             // Lets us detect whether Awake() has yet been called.

    // This defines a static instance property that attempts to find the manager object in the scene and
    // returns it to the caller.
    public static AudioController instance
    {
        get
        {
            if (s_Instance == null)
            {
                AudioController tmpInst = FindObjectOfType(typeof(AudioController)) as AudioController;
                if (tmpInst != null)
                    tmpInst.Awake();
                s_Instance = tmpInst;

                if (s_Instance == null && Application.isEditor)
                    Debug.LogError("Could not locate a AudioController object. You have to have exactly one AudioController in the scene.");
            }

            return s_Instance;
        }
    }

    public static bool Exists()
    {
        if (s_Instance == null)
        {
            AudioController tmpInst = FindObjectOfType(typeof(AudioController)) as AudioController;
            if (tmpInst != null)
                tmpInst.Awake();
            s_Instance = tmpInst;
        }

        return s_Instance != null;
    }

    public void OnDestroy()
    {
        s_Instance = null;

        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    //----------------------------------------------------------------
    // End Singleton code
    //----------------------------------------------------------------

    void Awake()
    {
        if (m_awake)
            return;
        m_awake = true;

        SceneManager.sceneLoaded += OnSceneLoaded;

        // See if we are a superfluous instance:
        if (s_Instance != null)
        {
            Debug.LogError("You can only have one instance of this singleton object in existence.");
        }
        else
            s_Instance = this;

        DontDestroyOnLoad(this.gameObject);
    }

    // Use this for initialization
    void Start()
    {
        if (m_started)
            return;
        m_started = true;

    }

    public void OnSceneLoaded(Scene loadedScene, LoadSceneMode mode)
    {
        //PlayBGM();
    }

    public void SetBGM_Name(string bgmName)
    {
#if UNITY_WEBGL
        if (LoLController.Exists() && LoLController.instance.isUsingLoL)
            LoLSDK.LOLSDK.Instance.StopSound(bgmNameString);

        bgmNameString = bgmName;

        PlayBGM();
#endif
    }

    public void SetBGMClip(AudioClip clip)
    {
        bgmSource.clip = clip;
        bgmSource.mute = !isMusic;

        PlayBGM();
    }

    public void PlayBGM()
    {
#if UNITY_WEBGL
        if (LoLController.Exists() && LoLController.instance.isUsingLoL)
            LoLSDK.LOLSDK.Instance.PlaySound(bgmNameString, true, true);
#else
        bgmSource.Play();
#endif
    }

    public void StopBGM()
    {
#if UNITY_WEBGL
        if (LoLController.Exists() && LoLController.instance.isUsingLoL)
            LoLSDK.LOLSDK.Instance.StopSound(bgmNameString);
#else
        bgmSource.Stop();
#endif
    }
}
