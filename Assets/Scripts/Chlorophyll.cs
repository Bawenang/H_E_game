using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Chlorophyll : MonoBehaviour {

    public delegate void ResourceAdded(ResourceType resType, float adder, float finalValue);
    public static event ResourceAdded OnResourceAdded;

    public static bool firstChloroActive = false;

    public const float maxResourceValue = 12.0f;

    public GameController gameController;

    private bool _isAvailable;
    public GameObject availableGO;
    public GameObject unavailableGO;

    public SugarParticle sugarParticle;

    public Image imageCO2;
    public Image imageH2O;
    public Image imageSun;
    public Image imageChloro;

    public Button buyButton;

    public int price;

    public float co2 = 0.0f;
    public float h2o = 0.0f;
    public float sun = 0.0f;
    public float chloro = 0.0f;

    public bool isAvailable
    {
        get
        {
            return _isAvailable;
        }

        set
        {
            _isAvailable = value;

            availableGO.SetActive(_isAvailable);
            unavailableGO.SetActive(!_isAvailable);
        }
    }

    void Awake()
    {

    }


    // Use this for initialization
    void Start () {

        if (isAvailable)
            chloro = maxResourceValue;

        UpdateSliders();
        UpdateChloro();
    }

    public void OnBuy()
    {
        if (price <= gameController.sugar)
            isAvailable = true;
    }

    /// <summary>
    /// Add a resource. Max each resource value for a chlorophyll is 6. But we make it 12 because the sun is stored in halves.
    /// </summary>
    /// <param name="resType">Resource type.</param>
    /// <param name="value">Increment value. value range = 0 - 6. For the case of the SUN, it will depend on the weather.</param>
    /// <returns>Overflow value if it's more than 12.</returns>
    public float AddResource(ResourceType resType, float value)
    {
        float overflow = 0;
        float finalValue = 0;

        switch (resType)
        {
            case ResourceType.CO2:
                co2 += value;
                if (co2 > maxResourceValue) //Overflow and will have to be thrown away or stored to another chloro.
                {
                    overflow = co2 - maxResourceValue;
                    co2 = maxResourceValue;
                }
                finalValue = co2;
                break;
            case ResourceType.H2O:
                h2o += value;
                if (h2o > 12.0f) //Overflow and will have to be thrown away or stored to another chloro.
                {
                    overflow = h2o - maxResourceValue;
                    h2o = maxResourceValue;
                }
                finalValue = h2o;
                break;
            case ResourceType.SUN:
                sun += value;
                if (sun > 12.0f) //Overflow and will have to be thrown away or stored to another chloro.
                {
                    overflow = sun - maxResourceValue;
                    sun = maxResourceValue;
                }
                finalValue = sun;
                break;
            case ResourceType.CHLORO:
                chloro += value;
                if (chloro > 12.0f) //Overflow and will have to be thrown away or stored to another chloro.
                {
                    overflow = chloro - maxResourceValue;
                    chloro = maxResourceValue;
                }
                finalValue = chloro;
                break;
        }

        CheckForPhoto();
        UpdateSliders();
        UpdateChloro();



        if (OnResourceAdded != null)
            OnResourceAdded(resType, value, finalValue);
        return overflow;
    }

    public void UpdateSliders()
    {
        imageCO2.fillAmount = Mathf.Clamp01(co2 / 12.0f);
        imageH2O.fillAmount = Mathf.Clamp01(h2o / 12.0f);
        imageSun.fillAmount = Mathf.Clamp01(sun / 12.0f);
        imageChloro.fillAmount = Mathf.Clamp01(chloro / 12.0f);
    }

    public void UpdateChloro()
    {
        if (chloro >= 12.0f && gameController.sugar >= price)
        {
            if (gameController.currentStage == 2 && !firstChloroActive && !isAvailable)
            {
                firstChloroActive = true;
                gameController.dialogCtrl.PlaySequence(5, true);

#if UNITY_WEBGL
                gameController.LOLSubmitProgressWithCurrentScore(5);
#endif
            }
            buyButton.interactable = true;
        }
        else
            buyButton.interactable = false;
    }

    public bool CheckForPhoto()
    {
        if (isAvailable)
        {
            if (co2 >= 12.0f && h2o >= 12.0f && sun >= 12.0f)
            {
                co2 -= 12.0f;
                h2o -= 12.0f;
                sun -= 12.0f;

                //gameController.AddSugar(1);
                PlayParticle();

#if UNITY_WEBGL
                gameController.boardController.Play_sfx(gameController.boardController.sugar_out_sfx_str);
#else
                gameController.boardController.Play_sfx(gameController.boardController.sugar_out_sfx);
#endif
            }
        }

        return false;
    }

    public void PlayParticle()
    {
        sugarParticle.MoveToIcon();
    }

    public void AddSugar(int value)
    {
        gameController.AddSugar(value);
    }
}
