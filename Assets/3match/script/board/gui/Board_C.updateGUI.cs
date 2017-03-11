using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public partial class Board_C : MonoBehaviour {

    public void Gain_turns(int quantity)
    {
        if (lose_requirement_selected != lose_requirement.player_have_zero_moves)
            return;


        turn_gained = true;

    }


    public void Update_token_count()//call from Create_new_board(), tile_C.Check_the_content_of_this_tile()
    {
    }

    public void This_gem_color_is_collected(int this_color)//call from tile_C.Update_gems_score()
    {
        if (player_turn)
        {
            if (!player_this_gem_color_is_collected[this_color])
            {
                player_this_gem_color_is_collected[this_color] = true;
            }
        }

    }



    public void Update_bonus_fill(int _x, int _y, int n)//call from tile_C.Update_gems_score()
    {
    }

    public void Update_inventory_bonus(int bonus_id, int quantity)//call from tile_C: Give_more_time(), Destroy_one(), Destroy_horizontal(), Destroy_all_gems_with_this_color(), Update_tile_hp(), Destroy_3x3(), Destroy_vertical(), Destroy_horizonal_and_vertical(), Check_the_content_of_this_tile(), Check_if_shuffle_is_done()
    {
        //print("Update_inventory_bonus " + player_turn);
        if (trigger_by_select == trigger_by.inventory)
        {
            if (player_turn)
            {
                player_bonus_inventory[bonus_id] += quantity;
                player_bonus_inventory_script.Update_bonus_count(bonus_id);
                if (quantity < 1)
                    player_bonus_inventory_script.Deselect(bonus_id);
            }
            else
            {
                enemy_bonus_inventory[bonus_id] += quantity;
                enemy_bonus_inventory_script.Update_bonus_count(bonus_id);
                if (quantity < 1)
                    enemy_bonus_inventory_script.Deselect(bonus_id);
            }
        }
    }

    public void Update_score()//call from initiate_ugui(), Order_to_gems_to_explode(), tile_C.Update_tile_hp()
    {

        gui_player_score.text =  player_score.ToString("N0");

        if (LoLController.Exists() && LoLController.instance.isUsingLoL)
            LoLController.instance.score = player_score;


        if (continue_to_play_after_win_until_lose_happen && player_win)
        {
            if (win_requirement_selected != win_requirement.collect_gems)
            {
                if ((current_star_score < 3) && (player_score >= three_stars_target_score))
                {
                    current_star_score = 3;
                }
                else if ((current_star_score < 2) && (player_score >= two_stars_target_score))
                {
                    current_star_score = 2;
                }
            }
        }

        if (Stage_uGUI_obj)//use menu kit win screen
        {
            /* if you have the menu kit, DELETE THIS LINE
				my_game_uGUI.int_score = player_score;
				my_game_uGUI.Update_int_score();
				
			// if you have the menu kit, DELETE THIS LINE */
        }
    }

}
