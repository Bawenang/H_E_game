using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public partial class Board_C : MonoBehaviour
{

    void Awake()
    {

        /* if you have the menu kit, DELETE THIS LINE
		if (game_master.game_master_obj)
			my_game_master = (game_master)game_master.game_master_obj.GetComponent("game_master");
		
		my_game_uGUI = (game_uGUI)Stage_uGUI_obj.GetComponent("game_uGUI");
		my_game_uGUI.show_progress_bar = use_star_progress_bar;
		// if you have the menu kit, DELETE THIS LINE*/

        Initiate_variables();


        /* if you have the menu kit, DELETE THIS LINE
		if (my_game_uGUI.my_progress_bar && my_game_uGUI.show_progress_bar)
			{
			if (win_requirement_selected == win_requirement.reach_target_score)
				{
				//setup menu kit progress bar
				my_game_uGUI.progress_bar_use_score = true;
				my_game_uGUI.my_progress_bar.star_target_values[0] = target_score;
				my_game_uGUI.my_progress_bar.star_target_values[1] = two_stars_target_score;
				my_game_uGUI.my_progress_bar.star_target_values[2] = three_stars_target_score;
				//turn off board progress bar

				gui_player_target_score_slider.gameObject.SetActive(false);

				}
			else if (win_requirement_selected == win_requirement.collect_gems)
				{
				my_game_uGUI.progress_bar_use_score = false;
				my_game_uGUI.my_progress_bar.star_target_values[0] = total_number_of_gems_remaining_for_the_player;
				my_game_uGUI.my_progress_bar.star_target_values[1] = total_number_of_gems_remaining_for_the_player + two_stars_target_additional_gems_collected;
				my_game_uGUI.my_progress_bar.star_target_values[2] = total_number_of_gems_remaining_for_the_player + three_stars_target_additional_gems_collected;
				}

			}
		// if you have the menu kit, DELETE THIS LINE*/

        Load_board();
        Setup_camera();
        Create_new_board();

    }
    
    void Reset_variables()//call from Initiate_variables()
    {
        player_win = false;
        game_end = false;
        game_is_ended = false;//this avoid to call end screen multiple times when player_can_move_when_gem_falling = true;
        versus = false;
        player_turn = false;
        current_board_orientation = 0;
        number_of_token_collected = 0;
        token_showed = false;
        elements_to_damage_list = new List<tile_C>();
    }

    void Initiate_variables()//call from Awake
    {
        Reset_variables();
        this_board = this;

        AudioSource[] sources = GetComponents<AudioSource>();
        audioSrcList.AddRange(sources);

        cursor.gameObject.SetActive(false);

        number_of_bottom_tiles = new int[4];

        //emission rules
        Initiate_emitter_variables();

        //gem count
        total_number_of_gems_destroyed_by_the_player = new int[gem_length];
        number_of_gems_collect_by_the_player = new int[gem_length];
        number_of_gems_collect_by_the_player_in_frame = new int[gem_length];
        additional_gems_collected_by_the_player = 0;

        //score
        current_star_score = 0;
        if (LoLController.Exists() && LoLController.instance.isUsingLoL)
            player_score = LoLController.instance.score;
        else
            player_score = 0;
        enemy_score = 0;

        if (number_of_gems_to_destroy_to_win.Length == 0)
            number_of_gems_to_destroy_to_win = new int[gem_length];

        player_this_gem_color_is_collected = new bool[gem_length];

        for (int n = 0; n < gem_length; n++)
        {
            total_number_of_gems_remaining_for_the_player += number_of_gems_to_destroy_to_win[n];
        }
        total_number_of_gems_required_colletted = 0;
            

        time_left = timer;


        Initiate_ugui();
    }

}
