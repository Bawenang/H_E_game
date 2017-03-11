using UnityEngine;
using System.Collections;

public partial class Board_C : MonoBehaviour
{
    public Transform my_hint;
    public bool show_hint;
    public float show_hint_after_n_seconds;
    bool use_hint;

    void Show_hint()//call from Annotate_potential_moves()
    {
        if (use_hint)
        {
            if (number_of_moves_possible > 0)//show a gem move
            {
                //Debug.Log("move hint");
                int random_hint = UnityEngine.Random.Range(0, number_of_gems_moveable - 1);
                my_hint.position = tiles_array[list_of_moves_possible[random_hint, 1], list_of_moves_possible[random_hint, 2]].transform.position;
                my_hint.GetComponent<Animation>().Play("hint_anim");
                my_hint.gameObject.SetActive(true);
                for (int i = 4; i <= 7; i++)
                {
                    if (list_of_moves_possible[random_hint, i] > 0)
                    {
                        my_hint.GetChild(i - 4).gameObject.SetActive(true);
                    }
                    else
                        my_hint.GetChild(i - 4).gameObject.SetActive(false);
                }
            }

        }
    }

    public void Cancell_hint()
    {
        CancelInvoke("Show_hint");
        use_hint = false;
        my_hint.gameObject.SetActive(false);
        for (int i = 0; i < 4; i++)
        {
            my_hint.GetChild(i).gameObject.SetActive(false);
        }
    }


    void Check_if_show_hint()//call from Board_C.freeMovement.BoardUpdate()
    {
        if (show_hint)
        {
            if (use_hint)
            {
                if ((number_of_elements_to_damage_with_SwitchingGems + number_of_elements_to_damage) > 0)
                    Cancell_hint();
            }
            else
            {
                use_hint = true;
                Invoke("Show_hint", show_hint_after_n_seconds);
            }
        }
    }
}
