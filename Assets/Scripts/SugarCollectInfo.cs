using UnityEngine;
using System.Collections;

public class SugarCollectInfo : MonoBehaviour {

    [SerializeField]
    GameController gameCtrl;

    private bool hasShown = false;

    void Awake()
    {
        GameController.OnSugarAdded += this.SugarCollected;
        this.gameObject.SetActive(false);
    }

    // Use this for initialization
    void Start()
    {

    }

    void OnDestroy()
    {
        GameController.OnSugarAdded -= this.SugarCollected;
    }


    public void SugarCollected(int value)
    {
        if (!hasShown)
        {
            if (gameCtrl.currentStage == 1)
            {
#if UNITY_WEBGL
                if (LoLController.Exists() && LoLController.instance.isUsingLoL)
                    gameCtrl.LOLSubmitProgressWithCurrentScore(2);
#endif

                gameCtrl.dialogCtrl.PlaySequence(6, true);
            }

            hasShown = true;
        }
    }
}
