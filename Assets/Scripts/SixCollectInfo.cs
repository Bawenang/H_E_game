using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SixCollectInfo : MonoBehaviour {

    private bool hasShown = false;
    [SerializeField] GameController gameCtrl;

    private const string c_firstPart = "You have collected 6 pieces of ";
    private const string c_secondPart = ". Now, collect 6 pieces of the other elements to begin the photosynthetic process! Remember! Six is the magic number...";

    [SerializeField] private Text _infoText;

    void Awake()
    {
        Chlorophyll.OnResourceAdded += this.CheckResource;
        this.gameObject.SetActive(false);
    }

	// Use this for initialization
	void Start () {
	
	}

    void OnDestroy()
    {
        Chlorophyll.OnResourceAdded -= this.CheckResource;
    }

    public void SetText (ResourceType resType)
    {
        string resTxt = "";

        switch (resType)
        {
            case ResourceType.CO2:
                resTxt = "CO<size=24>2</size> ";
                break;
            case ResourceType.H2O:
                resTxt = "H<size=24>2</size>O ";
                break;
            case ResourceType.SUN:
                resTxt = "Sunlight ";
                break;
        }

        _infoText.text = c_firstPart + resTxt + c_secondPart;
    }

    public void CheckResource(ResourceType resType, float adder, float finalValue)
    {
        if (!hasShown && resType != ResourceType.CHLORO && finalValue >= Chlorophyll.maxResourceValue)
        {
            SetText(resType);
            if (gameCtrl.currentStage == 1)
            {
#if UNITY_WEBGL
                if (LoLController.Exists() && LoLController.instance.isUsingLoL)
                    gameCtrl.LOLSubmitProgressWithCurrentScore(1);
#endif
                gameCtrl.dialogCtrl.PlaySequence(5, true);
            }
            hasShown = true;
        }
    }
}
