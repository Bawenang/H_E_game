using UnityEngine;
using System.Collections;

public partial class Board_C : MonoBehaviour
{


    void StartBoardUpdate()
    {
        player_turn = true;
        player_can_move = true;
        start_time = Time.timeSinceLevelLoad;
        time_duration = start_time;
        stage_started = true;
        current_moveStatus = moveStatus.waitingNewMove;
        //BoardUpdate();
    }


    void BoardUpdate()
    {
        if (!shuffle_ongoing)
        {
            //Debug.Log("---------------------------------------------- gem length = " + gem_length);
            for (int i = 0; i < gem_length; ++i)
            {
                //Debug.Log("---------------------------------------------- gem " + i + " collected = " + number_of_gems_collect_by_the_player_in_frame[i]);
                number_of_gems_collect_by_the_player_in_frame[i] = 0;
            }

            if (game_end)
            {
                Game_end();
                return;
            }

 

            //player input
            if (current_moveStatus == moveStatus.switchingGems)
                SwitchingGems();


            //read
            Check_vertical_falling_bottom_to_top();
            Check_diagonal_falling("BoardUpdate()");
            Check_secondary_explosions();
            Check_new_gem_to_generate();
            Check_ALL_possible_moves();

            if (BoardUpdating())
            {
                Cancell_hint();

                //execute
                ExecuteChanges();
                //calculate score
                Calculate_score();
                //create
                Generate_new_gems();


            }
            else
                Check_if_show_hint();
            
        }


    }
    


    bool BoardUpdating()
    {
        //print(number_of_elements_to_damage + " ... " + number_of_elements_to_damage_with_SwitchingGems + " ... " + number_of_moves_possible + " ... " + number_of_gems_to_move + " ... " + number_of_new_gems_to_create);
        int elements_to_update = (number_of_elements_to_damage + number_of_elements_to_damage_with_SwitchingGems + number_of_elements_to_damage_with_bonus + number_of_gems_to_move + number_of_new_gems_to_create);

        if (shuffle_ongoing || elements_to_update > 0)
            return true;
        else
            return false;
    }


    void ExecuteChanges()
    {
        for (int y = _Y_tiles - 1; y >= 0; y--)//from bottom to top
        {
            for (int x = 0; x < _X_tiles; x++)
            {

                if (board_array_master[x, y, 1] < 0 || tiles_array[x, y].my_gem == null) //if no tile or no gem
                    continue; //skip this x,y


                if (board_array_master[x, y, 11] == 1 || board_array_master[x, y, 11] == 666)//destroy
                {
                    board_array_master[x, y, 11] = 111; // explosion ongoing
                    tile_C tile_script = (tile_C)tiles_array[x, y];
                    tile_script.Explosion();

                }
                else if (board_array_master[x, y, 11] >= 3 && board_array_master[x, y, 11] <= 5)//falling
                {
                    tile_C tile_script = (tile_C)tiles_array[x, y];
                    tile_script.Fall_by_one_step(board_array_master[x, y, 11] - 3);
                }

            }

        }
    }



    void Generate_new_gems()//call from BoardUpdate()
    {
        if (gem_emitter_rule == gem_emitter.off)
        {
            number_of_new_gems_to_create = 0;
            return;
        }

        for (int i = 0; i < number_of_tiles_leader; i++)
        {
            if (board_array_master[tiles_leader_array[i]._x, tiles_leader_array[i]._y, 11] == 2)//creation
            {
                tile_C tile_script = (tile_C)tiles_array[tiles_leader_array[i]._x, tiles_leader_array[i]._y];
                tile_script.Create_gem();
            }
        }
    }


}
