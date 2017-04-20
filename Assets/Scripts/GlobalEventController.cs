using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalEventController : MonoBehaviour {

    public GameController gameController;

    public int elementCount;
    public GlobalEventDialog eventDialog;
    public Goals goals;
    public ElementEventController[] eleEventControllerList = new ElementEventController[6];
    

    public Queue<GlobalEvent> eventQueue = new Queue<GlobalEvent>(); //Active event queue

    private bool _isDialogActive = false;

    // Use this for initialization
    void Start () {

	}
	
	// Update is called once per frame
	void Update ()
    {
        while (!gameController.boardController.game_end && !gameController.dialogCtrl.isShown && eventQueue.Count > 0 )
        {
            Show();
        }
	}

    public void EnqueueRandom(ElementType eleType)
    {
        GlobalEvent evt = eleEventControllerList[(int)eleType].GetRandom();

        if (evt != null)
            eventQueue.Enqueue(evt);
    }

    public void Show()
    {
        if (eventQueue.Count > 0)
        {
            GlobalEvent evt = eventQueue.Dequeue();

            //Show event!!!!
            Show(evt);
        }
    }

    public void Stop()
    {
        _isDialogActive = false;
    }

    public void Show(GlobalEvent evt)
    {
        eventDialog.Show(evt);
        gameController.dialogCtrl.PlaySequenceWithSfx(5);

        goals.Add(GoalType.SCORE, evt.goalData.score);
        goals.Add(GoalType.EARTH, evt.goalData.earth);
        goals.Add(GoalType.HUMANITY, evt.goalData.humanity);
        goals.Add(GoalType.WEALTH, evt.goalData.wealth);
        goals.Add(GoalType.LAW, evt.goalData.law);

        _isDialogActive = true;
    }
}
