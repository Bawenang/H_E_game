using UnityEngine;
using System.Collections;

public partial class Board_C : MonoBehaviour{

    public float timer;
    public float time_left;
    public float time_bonus;
    public bool stage_started;
    public float start_time;
    public float time_duration;

    void Timer()//call from update
    {
        if (stage_started && (game_end != true))
        {
            //if (time_bonus < 0)
            //    Debug.LogError(time_bonus);
            
            if (!gameController.pause)
                time_duration += Time.deltaTime;

            //Debug.Log("Duration = " + time_duration);
            //time_left = (timer + start_time + time_bonus) - Time.timeSinceLevelLoad;
            time_left = (timer + start_time + time_bonus) - time_duration;

            gameController.DoGameTick(Time.deltaTime);


            gui_timer_text.text = Get_String_Time(time_left);

            if (time_left <= 0)
            {
                time_left = 0;
                Player_lose();
            }
            else if (time_left > timer)
                time_left = timer;
        }
    }

    public void Add_time_bonus(float add_this)// call from Check_secondary_explosions(), tile_C.Check_if_shuffle_is_done(), tile_C.Check_if_gem_movements_are_all_done()
    {
        
        if ((time_left + add_this) > timer)
        {
            time_bonus += timer - time_left;
        }
        else
        {
            time_bonus += add_this;
        }

    }

    public string Get_String_Time(float seconds)
    {
        int secInt = (int)seconds;
        int minInt = secInt / 60;
        secInt = secInt % 60;
        string res = minInt.ToString() + " : " + secInt.ToString("D2");
        return res;
    }
}
