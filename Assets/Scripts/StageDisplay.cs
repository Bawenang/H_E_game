using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StageDisplay : MonoBehaviour {

    private const float c_showFadeTime = 0.5f;
    private const float c_idleTime = 1.0f;

    [SerializeField] private Text _stageText;
    [SerializeField] private string _stageName;

    public Canvas displayCanvas;

    private float _alphaValue;
    private float _time;
    private string _onCompleteMethodName;
    private GameObject _onCompleteGO;
    private object _onCompleteParams;

    private float _showTimeMark;
    private float _idleTimeMark;
    private float _fadeTimeMark;

    public void Start()
    {
        _showTimeMark = c_showFadeTime;
        _idleTimeMark = c_showFadeTime + c_idleTime;
        _fadeTimeMark = c_showFadeTime + c_idleTime + c_showFadeTime;
    }

    public void Update()
    {
        if (_time >= 0.0f)
        {
            if (_time < _showTimeMark)
            {
                //_alphaValue = EaseInCubic(0.0f, 1.0f, _time / _showTimeMark);
                _stageText.enabled = false;
            }
            else if (_time < _idleTimeMark)
            {
                //_alphaValue = 1.0f;
                _stageText.enabled = true;
            }
            else if (_time < _fadeTimeMark)
            {
                //_alphaValue = EaseOutCubic(1.0f, 0.0f, (_time - _idleTimeMark) / (_fadeTimeMark - _idleTimeMark));
                _stageText.enabled = false;
            }
            else
            {
                OnCompleteAnim();
            }

            _time += Time.deltaTime;

            //_stageText.color = new Color(_stageText.color.r, _stageText.color.g, _stageText.color.b, _alphaValue);
        }
    }

    public void Show(int stage, string onCompleteMethodName, GameObject onCompleteGO, object onCompleteParams)
    {
        gameObject.SetActive(true);
        displayCanvas.gameObject.SetActive(true);
        if (stage < 4)
            _stageText.text = "Stage " + stage.ToString() + "\n" + _stageName;
        else
            _stageText.text = "Final Test Stage";
        //_stageText.color = new Color(_stageText.color.r, _stageText.color.g, _stageText.color.b, 0.0f);

        _onCompleteMethodName = onCompleteMethodName;
        _onCompleteGO = onCompleteGO;
        _onCompleteParams = onCompleteParams;

        _time = 0.0f;
        _alphaValue = 0.0f;

    }



    private float EaseInCubic(float start, float end, float value)
    {
        end -= start;
        return end * value * value * value + start;
    }

    private float EaseOutCubic(float start, float end, float value)
    {
        value--;
        end -= start;
        return end * (value * value * value + 1) + start;
    }

    public void OnCompleteAnim()
    {
        gameObject.SetActive(false);
        displayCanvas.gameObject.SetActive(false);

        _time = -1.0f;
        //_stageText.color = new Color(_stageText.color.r, _stageText.color.g, _stageText.color.b, 0.0f);
        _stageText.enabled = true;

        _onCompleteGO.SendMessage(_onCompleteMethodName, _onCompleteParams, SendMessageOptions.DontRequireReceiver);
    }
}
