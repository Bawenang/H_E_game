using UnityEngine;
using System.Collections;

public partial class tile_C {



    public void Try_to_use_bonus_on_this_tile()
    {
        //Debug.Log("Try_to_use_bonus_on_this_tile: " + _x + "," + _y);
        if (board.board_array_master[_x, _y, 1] >= 0) //click on a tile that have something in itself
        {
  
        }
    }

    public void Enemy_click_on_this_bonus()
    {
        Trigger_bonus(true);
    }

    public void Trigger_bonus(bool start_explosion)
    {
        if (board.board_array_master[_x, _y, 11] == 111)
            return;

        //Debug.Log(_x +","+_y + " Trigger_bonus");
        if (board.board_array_master[_x, _y, 4] == 1) //active hammer (need a click on a target gem to be activated)
        {
        }
        else if (board.board_array_master[_x, _y, 4] == 2) //switch_gem_teleport
        {
        }
        else if (board.board_array_master[_x, _y, 4] == 3) //explode bomb
        {
            Destroy_3x3(start_explosion);
        }
        else if (board.board_array_master[_x, _y, 4] == 4) //destroy_horizontal
        {
            Destroy_horizontal(start_explosion);
        }
        else if (board.board_array_master[_x, _y, 4] == 5) //destroy_vertical
        {
            Destroy_vertical(start_explosion);
        }
        else if (board.board_array_master[_x, _y, 4] == 6) //destroy_horizontal_and_vertical
        {
            Destroy_horizontal_and_vertical(start_explosion);
        }
        else if (board.board_array_master[_x, _y, 4] == 7) //destroy_all_gem_with_this_color (need a click on a target gem to be activated)
        {
        }
        else if (board.board_array_master[_x, _y, 4] == 8) //give_more_time
        {
            Give_more_time(start_explosion);
        }
        else if (board.board_array_master[_x, _y, 4] == 9) //give_more_moves
        {
            Give_more_moves(start_explosion);
        }
    }

    void Update_on_board_bonus_count()
    {
        if (board.number_of_bonus_on_board > 0)
            board.number_of_bonus_on_board--;

        //Debug.Log("Update_on_board_bonus_count: " + board.number_of_bonus_on_board);
    }

    void Reset_charge_fill()
    {

    }

    void Gems_teleport()
    {
        if ((board.board_array_master[_x, _y, 1] >= 0) && (board.board_array_master[_x, _y, 1] < 9)) //is a gem
        {
            if (board.board_array_master[_x, _y, 3] == 0) //no padlock
            {
                if (board.main_gem_selected_x == -10) //select first gem
                {
                    board.main_gem_selected_x = _x;
                    board.main_gem_selected_y = _y;
                    board.main_gem_color = board.board_array_master[_x, _y, 1];
                    //Debug.Log("teleport select first gem: " + _x + "," + _y);
                }
                else //select second gem
                {
                    if ((board.main_gem_selected_x == _x) && (board.main_gem_selected_y == _y))
                    {
                        //you have click on the same gem, so deselect it
                        board.main_gem_selected_x = -10;
                        board.main_gem_selected_y = -10;
                        board.main_gem_color = -10;
                    }
                    else
                    {
                        //board.minor_gem_destination_to_x = _x;
                        //board.minor_gem_destination_to_y = _y;
                        board.minor_gem_color = board.board_array_master[_x, _y, 1];
                        //Debug.Log("teleport select second gem: " + _x + "," + _y);

                        //activate teleport
                        board.number_of_gems_to_mix = 2;
                        board.player_can_move = false;

                        //change gems
                        board.board_array_master[_x, _y, 1] = board.main_gem_color;
                        board.board_array_master[board.main_gem_selected_x, board.main_gem_selected_y, 1] = board.minor_gem_color;

                        board.Play_bonus_sfx(2);

                        //update gem
                        SetShuffleUpdate();

                        tile_C tile_script = (tile_C)board.tiles_array[board.main_gem_selected_x, board.main_gem_selected_y];
                        tile_script.SetShuffleUpdate();


                    }
                }
            }
        }

    }

    void Give_more_moves(bool start_explosion)
    {
    }

    void Give_more_time(bool start_explosion)
    {

    }



    void Destroy_one(bool start_explosion)
    {

        if ((board.board_array_master[_x, _y, 1] >= 0) && (board.board_array_master[_x, _y, 1] <= 9) && (board.board_array_master[_x, _y, 4] >= -100) //it is a gem or junk
            && (board.board_array_master[_x, _y, 4] <= 0))//but not a bonus
        {

            board.player_can_move = false;
            board.cursor.gameObject.SetActive(false);

            Reset_charge_fill();
            board.Update_inventory_bonus(1, -1);


            board.Annotate_explosions(_x, _y, Board_C.ExplosionCause.bonus);

            if (start_explosion)
            {
                board.Play_bonus_sfx(1);
                board.Order_to_gems_to_explode();
            }

        }
        else if ((board.board_array_master[_x, _y, 1] >= 41) && (board.board_array_master[_x, _y, 1] < 60)) //it is a block
        {
            board.player_can_move = false;
            board.cursor.gameObject.SetActive(false);

            Reset_charge_fill();
            board.Update_inventory_bonus(1, -1);


            board.number_of_elements_to_damage++;
            Damage_block();
        }

        Update_on_board_bonus_count();
    }

    void Destroy_3x3(bool start_explosion)
    {
 
    }

    void Destroy_horizontal(bool start_explosion)
    {

    }

    void Destroy_on_the_right_side()
    {

    }

    void Destroy_on_the_left_side()
    {

    }

    void Destroy_above()
    {

    }

    void Destroy_beneath()
    {

    }

    void Destroy_vertical(bool start_explosion)
    {

    }

    void Destroy_horizontal_and_vertical(bool start_explosion)
    {

    }

    void Destroy_all_gems_with_this_color()
    {

    }


    public void Change_gem_in_bonus()
    {

    }

    void Create_special_element(int element_id)
    {

    }
}
