using UnityEngine;
using System.Collections;

public class SugarIcon : MonoBehaviour {

    private static SugarIcon _instance;

    public GameController gameController;

    void Awake()
    {
        if (_instance == null)
            _instance = this;
    }

	// Use this for initialization
	void Start () {
	
	}

    void OnDestroy()
    {
        if (_instance == this)
            _instance = null;
    }
	
    public static SugarIcon Instance
    {
        get
        {
            if (_instance == null)
                _instance = GameObject.FindObjectOfType<SugarIcon>();

            return _instance;
        }
    }

    public void OnTriggerEnter(Collider col)
    {
        SugarParticle sugar = col.GetComponent<SugarParticle>();
        if (sugar == null)
            return;

        gameController.AddSugar(1);
        sugar.SendBack();
        gameController.boardController.Play_sfx(gameController.boardController.sugar_in_sfx);
    }


}
