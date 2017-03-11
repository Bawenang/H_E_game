using UnityEngine;
using System.Collections;

public partial class Board_C : MonoBehaviour {

    bool game_is_ended;

    public void Game_end()
    {
        if (game_is_ended)
            return;

        game_is_ended = true;

        cursor.gameObject.SetActive(false);

        if (player_win)
            Show_win_screen();
		else
            Show_lose_screen();
    }

    public void Player_win()
    {
        if (!game_end && !player_win)
        {
            player_win = true;

            if (!continue_to_play_after_win_until_lose_happen)
            {
                game_end = true;
                player_can_move = false;
            }
            else
            {
                if (current_star_score < 1)
                    current_star_score = 1;
            }
            //Debug.LogWarning("You win!");
        }
    }

    public void Player_lose()
    {
        if (!game_end)
        {
            game_end = true;
            if (player_can_move)
            {
                player_can_move = false;
                UpdateTurn();
            }
            //Debug.LogWarning("You lose!");
        }
    }

    void Show_win_screen()
    {
        if (lose_requirement_selected == lose_requirement.timer)
            player_score += (int)(every_second_saved_give * time_left);

        if (Stage_uGUI_obj)//use menu kit win screen
        {
            /* if you have the menu kit, DELETE THIS LINE

			Debug.Log("time_left: "  + time_left + " ** three_stars_target_time_spared: "+ three_stars_target_time_spared + " ** two_stars_target_time_spared: " + two_stars_target_time_spared);
			if (player_score >= three_stars_target_score) //3 stars
				{
				if (continue_to_play_after_win_until_lose_happen)
					{
					//
					//if (win_requirement_selected == win_requirement.collect_gems)
					//	{
					//	if (three_stars_target_additional_gems_collected <= additiona_gems_collected_by_the_player)
					//		current_star_score= 3;
					//	}
					//else
					//	current_star_score = 3;
					}
				else
					{
					if (((lose_requirement_selected == lose_requirement.enemy_reach_target_score) && ( (player_score-enemy_score) >= three_stars_target_score_advantage_vs_enemy))
					|| ((lose_requirement_selected == lose_requirement.player_hp_is_zero) && (current_player_hp >= three_stars_target_player_hp_spared))
					|| ((lose_requirement_selected == lose_requirement.timer) && (time_left >= three_stars_target_time_spared))
					|| ((lose_requirement_selected == lose_requirement.player_have_zero_moves) && (current_player_moves_left >= three_stars_target_moves_spared))
					|| ((lose_requirement_selected == lose_requirement.enemy_collect_gems) && ( total_number_of_gems_remaining_for_the_enemy >= three_stars_target_gems_collect_advantage_vs_enemy))
					)
						{
						current_star_score = 3;
						}
					}
				}
			else if (player_score >= two_stars_target_score)//2 stars
				{
				if (continue_to_play_after_win_until_lose_happen)
					{

					//if (win_requirement_selected == win_requirement.collect_gems)
					//{
					//	if (two_stars_target_additional_gems_collected <= additiona_gems_collected_by_the_player)
					//		current_star_score = 2;
					//}
					//else
					//	current_star_score = 2;
					//}
				else
					{
					if (((lose_requirement_selected == lose_requirement.enemy_reach_target_score) && ( (player_score-enemy_score) >= two_stars_target_score_advantage_vs_enemy))
					|| ((lose_requirement_selected == lose_requirement.player_hp_is_zero) && (current_player_hp >= two_stars_target_player_hp_spared))
					|| ((lose_requirement_selected == lose_requirement.timer) && (time_left >= two_stars_target_time_spared))
					|| ((lose_requirement_selected == lose_requirement.player_have_zero_moves) && (current_player_moves_left >= two_stars_target_moves_spared))
					|| ((lose_requirement_selected == lose_requirement.enemy_collect_gems) && ( total_number_of_gems_remaining_for_the_enemy >= two_stars_target_gems_collect_advantage_vs_enemy))
					)
						{
						current_star_score = 2;
						}
					}
				}
			else //1 star
				{
				current_star_score = 1;
				}

			my_game_uGUI.star_number = current_star_score;
			Debug.Log(my_game_uGUI.star_number + " stars");
			my_game_uGUI.Victory();
			//if you have the menu kit, DELETE THIS LINE */
        }
        else //use default win screen
        {
            //gui_win_screen.SetActive(true);
            //if (gameController.currentStage == 1)
            //{
            //    gameController.currentStage = 2;
                gameController.dialogCtrl.PlaySequence((int)DialogType.WIN, false);
            //}
            //Debug.LogWarning("show win screen!");
            if (AudioController.Exists())
                AudioController.instance.StopBGM();

#if UNITY_WEBGL
            Play_sfx(win_sfx_str);
#else
            Play_sfx(win_sfx);
#endif

        }

    }

    void Show_lose_screen()
    {
        if (Stage_uGUI_obj)//use menu kit win screen
        {
            /* if you have the menu kit, DELETE THIS LINE
				my_game_uGUI.Defeat();
			//if you have the menu kit, DELETE THIS LINE */
        }
        else //use default win screen
        {
            //gui_lose_screen.SetActive(true);
            gameController.dialogCtrl.PlaySequence((int)DialogType.LOSE, false);
            //Debug.LogWarning("show lose screen!");
            if (AudioController.Exists())
                AudioController.instance.StopBGM();

#if UNITY_WEBGL
            Play_sfx(lose_sfx_str);
#else
            Play_sfx(lose_sfx);
#endif
        }
    }



}
