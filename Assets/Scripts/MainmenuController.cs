using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class MainmenuController : MonoBehaviour {

    public string bgm_str;
    //[SerializeField] private AudioClip _bgmClip;

    public string dialog_sfx_str;
    public string button_sfx_str;

    public AudioClip dialog_sfx;
    public AudioClip button_sfx;

    private List<AudioSource> audioSrcList = new List<AudioSource>();

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

    void Awake()
    {
        AudioController.OnSoundChanged += OnSoundChanged;

        AudioSource[] sources = GetComponents<AudioSource>();

        audioSrcList.AddRange(sources);

    }

    // Use this for initialization
    void Start () {

#if UNITY_WEBGL
        if (AudioController.Exists())
            AudioController.instance.SetBGM_Name(bgm_str);
#endif

        if (LoLController.Exists() && LoLController.instance.isUsingLoL)
        {
            LoLController.instance.LOLSubmitProgressWithCurrentScore(0);
            LoLController.instance.score = 0;
        }
    }
	

    void OnDestroy()
    {
        AudioController.OnSoundChanged -= OnSoundChanged;

        if (AudioController.Exists())
            AudioController.instance.StopBGM();
    }
    public void StartGame()
    {
        SceneManager.LoadScene("game");
    }

    public void OnSoundChanged(bool soundValue)
    {
        for (int i = 0; i < audioSrcList.Count; ++i)
        {
            audioSrcList[i].mute = !soundValue;
        }
    }

    public void PlaySfx(AudioClip my_clip)
    {
        if (my_clip)
        {
            AudioSource src = audioSrcList.Find(x => !x.isPlaying);
            if (src == null)
            {
                src = gameObject.AddComponent<AudioSource>();
                src.priority = 0;
                src.spatialBlend = 0.0f;
                src.volume = 1.0f;
                src.pitch = 1.0f;
                src.panStereo = 0.0f;
                src.loop = false;
                audioSrcList.Add(src);

                if (AudioController.Exists())
                    src.mute = !AudioController.instance.isSound;
            }
            src.clip = my_clip;
            src.Play();
        }
    }

    public void PlaySfx(string my_clip_str)
    {
        
        if (LoLController.Exists() && LoLController.instance.isUsingLoL)
            LoLSDK.LOLSDK.Instance.PlaySound(my_clip_str);
    }

    public void PlayButtonSfx()
    {
#if UNITY_WEBGL
        PlaySfx(button_sfx_str);
#else
        PlaySfx(button_sfx);
#endif
    }
}
