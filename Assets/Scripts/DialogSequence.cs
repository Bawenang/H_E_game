using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DialogSequence : MonoBehaviour {

    public static DialogController controller;

    [SerializeField] private Dialog[] dialogList;

    private static Dialog _currentShownDialog = null;

    private static bool _animating = false; //When we're animating, we can't show / hide another

    private int _currentDialogIdx = 0;



    //private Dialog _dialogToHide;
    private Dialog _dialogToShow;

    public void StartSequence()
    {
        _currentDialogIdx = 0;
        ShowCurrentDialog();

        if (controller.gameCtrl != null)
            controller.gameCtrl.pause = true; 
    }

    public void StopSequence()
    {
        Hide(_currentShownDialog, true);

        if (controller.gameCtrl != null)
            controller.gameCtrl.pause = false; 
    }

    public void ShowCurrentDialog()
    {
        if (_currentShownDialog != null)
        {
            HideShow(_currentShownDialog, GetCurrentDialog());
        }
        else
        {
            Show(GetCurrentDialog());
        }
    }

    public void ShowNext()
    {
        Dialog toShow = NextDialog();

        ShowCurrentDialog();
    }

    public void ShowPrev()
    {
        Dialog toShow = PrevDialog();

        ShowCurrentDialog();
    }

    public Dialog GetCurrentDialog()
    {
        return dialogList[_currentDialogIdx];
    }

    public Dialog GetDialog(int value)
    {
        if (value < 0 || value > dialogList.Length - 1)
            return null;
        return dialogList[value];
    }

    public Dialog NextDialog()
    {
        if (_currentDialogIdx < dialogList.Length - 1)
            ++_currentDialogIdx;
        else
            return null;

        return GetCurrentDialog();
    }

    public Dialog PrevDialog()
    {
        if (_currentDialogIdx > 0)
            --_currentDialogIdx;
        else
            return null;

        return GetCurrentDialog();
    }


    public void Show(Dialog dialogToShow)
    {
        //if (!_animating)
        {
            //_animating = true;
            //_dialogToShow = dialogToShow;
            //_dialogToShow.gameObject.SetActive(true);
            dialogToShow.gameObject.SetActive(true);
            _currentShownDialog = dialogToShow;
            //iTween.ScaleFrom(_dialogToShow.gameObject, iTween.Hash("scale", Vector3.zero, "time", 0.5f, "easeType", "easeInOutExpo", "loopType", "none", "oncomplete", "AfterShow", "oncompletetarget", this.gameObject));
        }
        //else
            //Debug.Log("Cannot animate. Currently still animating. Please try again!");
    }

    public void HideShow(Dialog dialogToHide, Dialog dialogToShow)
    {
        //if (!_animating)
        {
            //_animating = true;
            //iTween.ScaleTo(dialogToHide.gameObject, iTween.Hash("scale", Vector3.zero, "time", 0.5f, "easeType", "easeInOutExpo", "loopType", "none", "oncomplete", "AfterHideStillAnimate", "oncompletetarget", this.gameObject));
            dialogToHide.gameObject.SetActive(false);
            //_dialogToShow = dialogToShow;
            //_dialogToShow.gameObject.SetActive(true);
            dialogToShow.gameObject.SetActive(true);
            _currentShownDialog = dialogToShow;
            //iTween.ScaleFrom(_dialogToShow.gameObject, iTween.Hash("scale", Vector3.zero, "time", 0.5f, "easeType", "easeInOutExpo", "loopType", "none", "oncomplete", "AfterShow", "oncompletetarget", this.gameObject, "delay", 0.6f));
        }
        //else
            //Debug.Log("Cannot animate. Currently still animating. Please try again!");

    }

    public void AfterShow()
    {
        _animating = false;
        _currentShownDialog = _dialogToShow;
        _dialogToShow = null;
    }
    public void AfterHide(bool endSequence)
    {
        _animating = false;
        AfterHideStillAnimate();

        if (endSequence)
            controller.HideDarkBG();
    }
    public void AfterHideStillAnimate()
    {
        _currentShownDialog.gameObject.SetActive(false);
        _currentShownDialog.gameObject.transform.localScale = Vector3.one;
        _currentShownDialog = null;
    }

    public void Hide(Dialog dialogToHide, bool endSequence)
    {
        //if (!_animating)
        {
            //_animating = true;
            _currentShownDialog = dialogToHide;
            //iTween.ScaleTo(_currentShownDialog.gameObject, iTween.Hash("scale", Vector3.zero, "time", 0.5f, "easeType", "easeInOutExpo", "loopType", "none", "oncomplete", "AfterHide", "oncompletetarget", this.gameObject, "oncompleteparams", endSequence));
            _currentShownDialog.gameObject.SetActive(false);
            _currentShownDialog = null;
        }
        //else
            //Debug.Log("Cannot animate. Currently still animating. Please try again!");
    }
}
