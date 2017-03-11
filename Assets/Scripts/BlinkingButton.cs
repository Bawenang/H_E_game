using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BlinkingButton : MonoBehaviour {

    public float blinkTime = 0.5f;

    public Color fromColor;
    public Color toColor;

    private Button _button;
    private Color _colorValue;
    private bool _isToColor = true;
    private float _time = 0.0f;

    public void Awake()
    {
        _button = GetComponentInChildren<Button>();
        _button.interactable = false;
    }

    public void Update()
    {
        if (_time >= 0.0f)
        {
            if (_isToColor)
            {
                _colorValue = EaseInCubic(fromColor, toColor, _time / blinkTime);
            }
            else 
            {
                _colorValue = EaseOutCubic(toColor, fromColor, _time / blinkTime);
            }


            _time += Time.deltaTime;

            if (_time > blinkTime)
            {
                _time -= blinkTime;
                _isToColor = !_isToColor;
            }

            ColorBlock cb = _button.colors;
            cb.disabledColor = _colorValue;
            _button.colors = cb;
        }
    }

    private Color EaseInCubic(Color start, Color end, float value)
    {
        //return Color.Lerp(start, end, value * value * value);
        return Color.Lerp(start, end, value);
    }

    private Color EaseOutCubic(Color start, Color end, float value)
    {
        value--;
        //return Color.Lerp(start, end, value * value * value + 1);
        return Color.Lerp(start, end, value + 1);
    }

}
