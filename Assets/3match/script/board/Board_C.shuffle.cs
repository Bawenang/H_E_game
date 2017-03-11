using UnityEngine;
using System.Collections;

public partial class Board_C : MonoBehaviour
{
    //bool can_shuffle_now;
    public bool shuffle_ongoing;

    public enum no_more_moves_rule
    {
        shuffle,
        lose
    }
    public no_more_moves_rule no_more_moves_rule_selected;

    void Check_if_shuffle()
    {
        if (no_more_moves_rule_selected == no_more_moves_rule.shuffle)
            {
            if (player_can_move_when_gem_falling)
                {
                if (!BoardUpdating())
                    Shuffle();
                }
            else
                Shuffle();
            }
        else
            {
            if (player_can_move_when_gem_falling && BoardUpdating())
                return;

            if (win_requirement_selected == Board_C.win_requirement.destroy_all_gems && total_gems_on_board_at_start <= 0)
                player_win = true;
            else
                player_win = false;

            player_can_move = false;
            game_end = true;
            Game_end();
            }    
    }



    //shuffle gems when no move available
    void Shuffle()//call from Check_ALL_possible_moves()
    {
        if (!shuffle_ongoing)
        {

            if (!ShuffleSafetuCheck())//not safe
            return;

            shuffle_ongoing = true;

            //Debug.Log("shuffle");
            for (int y = 0; y < _Y_tiles; y++)
            {
                for (int x = 0; x < _X_tiles; x++)
                {

                    if ((board_array_master[x, y, 1] >= 0) && (board_array_master[x, y, 1] < 9) //there is a gem
                        && (board_array_master[x, y, 3] == 0))//and without padlock
                    {
                        number_of_gems_to_mix++;
                        board_array_master[x, y, 1] = UnityEngine.Random.Range(0, gem_length);
                        board_array_master[x, y, 4] = 0; //reset bonus
                        Avoid_triple_color_gem(x, y);
                        //update gem
                        tile_C tile_script = (tile_C)tiles_array[x, y];
                        tile_script.SetShuffleUpdate();
                    }
                }
            }
        }

    }

    bool ShuffleSafetuCheck()
    {

        for (int y = 0; y < _Y_tiles; y++)
        {
            for (int x = 0; x < _X_tiles; x++)
            {

                if (board_array_master[x, y, 11] != 0)//the board is updating, so don't need shuffle
                {
                    //print("shuffle aborted");
                    return false;
                }
            }
        }

        return true;
    }
}
