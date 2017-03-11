using UnityEngine;
using UnityEngine.UI;
using System.Collections;


    public class inventory_bonus_button : MonoBehaviour {


	public int my_id;
	public bonus_inventory my_inventory;
	Image my_image;
	Color base_color;


	void Start()
		{
		my_image = GetComponent<Image>();
		base_color = my_image.color;
		}

	public void Click_me()
		{

		}

	public void Activate()
		{

		}

	public void Deselect()
		{

        }
}

