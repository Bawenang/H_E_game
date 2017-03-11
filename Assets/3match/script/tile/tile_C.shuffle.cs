using UnityEngine;
using System.Collections;

public partial class tile_C : MonoBehaviour
{
    private float blink_time_start = 0.0f;
    private float blink_time = 0.0f;
    private float blink_delay = 0.2f;
    private float blink_duration = 1.0f;
    private bool isShuffleUpdate = false;

    public void SetShuffleUpdate()
    {
        blink_time_start = Time.realtimeSinceStartup;
        blink_time = Time.realtimeSinceStartup;
        isShuffleUpdate = true;
        blink_delay = 0.2f;
        blink_duration = 1.0f;
    }

    public void Shuffle_update()//call from Board_C.Shuffle(), Gems_teleport()
    {

        float duration = Time.realtimeSinceStartup - blink_time_start;
        float toggleDelay = Time.realtimeSinceStartup - blink_time;
        //minimize gem
        if (duration < blink_duration)
        {
            if (toggleDelay > blink_delay)
            {
                my_gem_renderer.enabled = !my_gem_renderer.enabled;
                blink_time += blink_delay;
            }
        }
        else
        {
            isShuffleUpdate = false;
            my_gem_renderer.enabled = false;

            //update gem
            my_gem_renderer.sprite = board.gem_colors[board.board_array_master[_x, _y, 1]];

            //return to normal size
            /*
            blink_time = Time.realtimeSinceStartup;

            while (Time.realtimeSinceStartup - blink_time < 1.0f)
            {
                my_gem_renderer.enabled = !my_gem_renderer.enabled;

                yield return new WaitForSeconds(0.2f);
            }
            */

            my_gem_renderer.enabled = true;

            Check_if_shuffle_is_done();
        }
    }

    void Check_if_shuffle_is_done()
    {
        board.number_of_gems_to_mix--;
        if (board.number_of_gems_to_mix == 0)
        {
            print("shuffle end");
            board.shuffle_ongoing = false;

        }
    }
}
