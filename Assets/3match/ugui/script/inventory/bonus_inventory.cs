using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;


    public class bonus_inventory : MonoBehaviour {

	public Board_C board;
	public GameObject inventory_button;

	public bool player;


	public GameObject[] bonus_list;
	Text[] bonus_quantity;

	// Use this for initialization
	void Start () {
	
	}
	
    public void Deselect(int bonus_id)
    {

    }

    public void Update_bonus_count(int bonus_id)//call from bonus_inventory.Start(), Board_C.Update_inventory_bonus(int bonus_id, int quantity)
	{

	}
}
