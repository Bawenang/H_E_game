using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementEventController : MonoBehaviour {

    public ElementType eleType;
    public GlobalEvent[] eventList;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public GlobalEvent GetRandom()
    {
        GlobalEvent randEvent = null;

        if (eventList.Length > 0)
            randEvent = eventList[Random.Range(0, eventList.Length)];

        return randEvent;
    }
}
