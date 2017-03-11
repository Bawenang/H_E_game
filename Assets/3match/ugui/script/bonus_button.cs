using UnityEngine.UI;
using UnityEngine;
using System.Collections;


    public class bonus_button : MonoBehaviour {

	public bool player;

	public int slot_number;
	public Image full_image;
		Color base_color;

	public Button my_button;
	//public Toggle my_toggle;

	public int cost;
	Board_C board;


	// Use this for initialization
	void Start () {
		my_button.enabled = false;

		full_image.sprite =  GetComponent<Image>().sprite;
		base_color = full_image.color;
        board = Board_C.this_board;


		Update_fill();

	}

	public void Update_fill()
	{

	}

	public void Click_me()
	{

	}

    public void Deselect()
    {

    }

    public void Activate()
	{

	}

	public void Reset_fill()
	{

	}
	

}
