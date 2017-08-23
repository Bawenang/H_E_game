using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementCompleteUI : MonoBehaviour {

    [SerializeField]
    GameController gameCtrl;

    private bool needToshow = true;

    private List<ElementType> _eleList = new List<ElementType>();

    void Awake()
    {
        GameController.OnElementCompleted += this.OnElementCompleted;
    }

    // Use this for initialization
    void Start()
    {

    }

    void OnDestroy()
    {
        GameController.OnElementCompleted -= this.OnElementCompleted;
    }


    public void OnElementCompleted(ElementType eleType)
    {
        if (needToshow)
        {

#if UNITY_WEBGL
            if (LoLController.Exists() && LoLController.instance.isUsingLoL)
                    gameCtrl.LOLSubmitProgressWithCurrentScore(2);
#endif
            if (!gameCtrl.dialogCtrl.isShown)
                gameCtrl.dialogCtrl.PlaySequence(6, true);


            _eleList.Add(eleType);
        }
    }

    public void StopAndShowEvent()
    {
        for (int i = 0; i < _eleList.Count; ++i)
        {
            gameCtrl.globalEventController.EnqueueRandom(_eleList[i]);
        }
        gameCtrl.SetElementCompleteEvents();

        needToshow = false;

        GameController.OnElementCompleted -= OnElementCompleted;
    }
}
