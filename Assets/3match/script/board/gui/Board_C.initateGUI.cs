using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public partial class Board_C : MonoBehaviour
{

    #region ugui
    public Text gui_player_score;

    public GameObject gui_info_screen;
    public GameObject gui_win_screen;
    public GameObject gui_lose_screen;

    public GameObject gui_timer;
    Text gui_timer_text;

    #endregion



    void Initiate_ugui()//call from Initiate_variables()
    {



        gui_timer_text = gui_timer.GetComponentInChildren<Text>();
        gui_timer_text.text = Get_String_Time(timer);


        Update_score();
        Auto_setup_gui();
    }

    void Auto_setup_gui()//call from Initiate_ugui()
    {




        #region lose requirements
        if (lose_requirement_selected == Board_C.lose_requirement.timer)
            gui_timer.SetActive(true);
        else
            gui_timer.SetActive(false);

        #endregion

        #region win requirements


        #endregion
    }

}
