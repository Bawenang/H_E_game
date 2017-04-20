using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public enum DialogType
{
    STARTING,
    PAUSE,
    WIN,
    LOSE_TIME,
    LOSE_EARTH,
    REVIEW,
    EVENT
}

public class DialogController : MonoBehaviour {

    [SerializeField] private DialogSequence[] _sequences;

    [SerializeField] private Canvas _dialogCanvas;

    [SerializeField] private Button[] _lowerLevelButtons;

    public GameController gameCtrl;
    public MainmenuController mainMenuCtrl;
    public TestController testCtrl;

    private int _currentSequence = -1;


    public int CurrentSequence
    {
        get
        {
            return _currentSequence;
        }
        set
        {
            _currentSequence = value;


        }
    }

    public bool isShown
    {
        get
        {
            return _dialogCanvas.gameObject.activeSelf;
        }
    }

    // Use this for initialization
    void Awake () {
        DialogSequence.controller = this;
        _dialogCanvas.gameObject.SetActive(false);
    }

    public void PlayCurrentSequence(bool playSfx)
    {
        _dialogCanvas.gameObject.SetActive(true);
        _sequences[_currentSequence].StartSequence();

        for (int i = 0; i < _lowerLevelButtons.Length; ++i)
        {
            _lowerLevelButtons[i].interactable = false;
        }

        if (playSfx)
        {
#if UNITY_WEBGL
            if (gameCtrl != null) gameCtrl.boardController.Play_sfx(gameCtrl.boardController.dialog_sfx_str);
            else if (mainMenuCtrl != null) mainMenuCtrl.PlaySfx(mainMenuCtrl.dialog_sfx_str);
            else if (testCtrl != null) testCtrl.PlaySfx(testCtrl.dialog_sfx_str);
#else
            if (gameCtrl != null) gameCtrl.boardController.Play_sfx(gameCtrl.boardController.dialog_sfx);
            else mainMenuCtrl.PlaySfx(mainMenuCtrl.dialog_sfx);
#endif
        }
    }

    public void PlaySequence(int idx, bool playSfx)
    {
        _currentSequence = idx;
        PlayCurrentSequence(playSfx);
        if (gameCtrl != null)
            gameCtrl.pause = true;
    }

    public void PlaySequenceWithSfx(int idx)
    {
        PlaySequence(idx, true);
    }

    public void PlaySequenceNotWithSfx(int idx)
    {
        PlaySequence(idx, false);
    }

    public void StopSequence()
    {
        _sequences[_currentSequence].StopSequence();
        if (gameCtrl != null)
            gameCtrl.pause = false;

        _dialogCanvas.gameObject.SetActive(false);

        for (int i = 0; i < _lowerLevelButtons.Length; ++i)
        {
            _lowerLevelButtons[i].interactable = true;
        }
    }

    public void HideDarkBG()
    {
        //_dialogCanvas.gameObject.SetActive(false);
    }

    public void ShowNextDialog(bool playSfx)
    {
        _sequences[_currentSequence].ShowNext();

        if (playSfx)
#if UNITY_WEBGL
            gameCtrl.boardController.Play_sfx(gameCtrl.boardController.dialog_sfx_str);
#else
            gameCtrl.boardController.Play_sfx(gameCtrl.boardController.dialog_sfx);
#endif
    }
}
