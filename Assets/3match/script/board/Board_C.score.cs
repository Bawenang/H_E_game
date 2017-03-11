using UnityEngine;
using System.Collections;

using System;

public partial class Board_C : MonoBehaviour
{


    public bool show_score_gui;
    public int target_score;

    public int player_score;
    public int enemy_score;
    public int score_of_this_turn_move;
    bool this_is_the_primary_explosion;
    public int n_gems_exploded_with_main_gem;
    public int n_gems_exploded_with_minor_gem;

    public int score_reward_for_damaging_tiles;
    public int[] score_reward_for_explode_gems; //3 gems; 4; 5; 6; and 7
    public int score_reward_for_each_explode_gems_in_secondary_explosion;
    public float score_reward_for_explode_gems_of_the_same_color_in_two_or_more_turns_subsequently;
    int explode_same_color_again_with = 0;//0 = false; 1 = with main gem; 2 =with minor gem
    int[] player_previous_exploded_color = { -1, -1 };//[0] = with main gem; [1] = with minor gem
    int player_explode_same_color_n_turn;
    int[] enemy_previous_exploded_color = { -1, -1 };//[0] = with main gem; [1] = with minor gem
    int enemy_explode_same_color_n_turn;
    public float score_reward_for_secondary_combo_explosions;
    public int every_second_saved_give;
    public int every_move_saved_give;
    public int every_hp_saved_give;




    void Calculate_primary_explosion_score(int number_of_elements_to_damage_temp)
    {
        this_is_the_primary_explosion = false;

        if (explode_same_color_again_with == 0) //no same color
        {
            //Debug.Log(n_gems_exploded_with_main_gem + " and " + n_gems_exploded_with_minor_gem);
            if ((n_gems_exploded_with_main_gem > 0) && (n_gems_exploded_with_minor_gem > 0))
                score_of_this_turn_move = score_reward_for_explode_gems[n_gems_exploded_with_main_gem - 3] + score_reward_for_explode_gems[n_gems_exploded_with_minor_gem - 3]; // "-3" because the array length (because 0, 1 and 2 explosion are impossible)
            else if (n_gems_exploded_with_main_gem > 0)
                score_of_this_turn_move = score_reward_for_explode_gems[n_gems_exploded_with_main_gem - 3];
            else if (n_gems_exploded_with_minor_gem > 0)
                score_of_this_turn_move = score_reward_for_explode_gems[n_gems_exploded_with_minor_gem - 3];
        }
        else if (explode_same_color_again_with == 1) //same color with main gem
        {
            score_of_this_turn_move = score_reward_for_explode_gems[n_gems_exploded_with_main_gem - 3];

            if (player_turn)
                score_of_this_turn_move += (int)Math.Ceiling(player_explode_same_color_n_turn * score_reward_for_explode_gems_of_the_same_color_in_two_or_more_turns_subsequently);
            else
                score_of_this_turn_move += (int)Math.Ceiling(enemy_explode_same_color_n_turn * score_reward_for_explode_gems_of_the_same_color_in_two_or_more_turns_subsequently);

            if (n_gems_exploded_with_minor_gem > 0)
                score_of_this_turn_move += score_reward_for_explode_gems[n_gems_exploded_with_minor_gem - 3];
        }
        else if (explode_same_color_again_with == 2) //same color with minor gem
        {
            score_of_this_turn_move = score_reward_for_explode_gems[n_gems_exploded_with_minor_gem - 3];

            if (player_turn)
                score_of_this_turn_move += (int)Math.Ceiling(player_explode_same_color_n_turn * score_reward_for_explode_gems_of_the_same_color_in_two_or_more_turns_subsequently);
            else
                score_of_this_turn_move += (int)Math.Ceiling(enemy_explode_same_color_n_turn * score_reward_for_explode_gems_of_the_same_color_in_two_or_more_turns_subsequently);

            if (n_gems_exploded_with_main_gem > 0)
                score_of_this_turn_move += score_reward_for_explode_gems[n_gems_exploded_with_main_gem - 3];
        }

        if (player_turn)
        {
            player_score += score_of_this_turn_move;

        }
        else
            enemy_score += score_of_this_turn_move;
    }

    void Calculate_secondary_explosion_score(int number_of_elements_to_damage_temp)
    {
        score_of_this_turn_move = 0;
        score_of_this_turn_move = (int)Math.Ceiling(score_reward_for_each_explode_gems_in_secondary_explosion * number_of_elements_to_damage * (1 + (n_combo * score_reward_for_secondary_combo_explosions)));
        if (player_turn)
            player_score += score_of_this_turn_move;
        else
            enemy_score += score_of_this_turn_move;

    }

    void Calculate_score()//(int number_of_elements_to_damage_temp, string calledFrom)
    {
        /*
        print("Calculate_score    called from: " + calledFrom);
		if (this_is_the_primary_explosion)//calculate score
		{
			Calculate_primary_explosion_score(number_of_elements_to_damage_temp);
		}
		else //this is a secondary explosion
		{
			Calculate_secondary_explosion_score(number_of_elements_to_damage_temp);
		}
		
		Update_score();
        Check_win_score_condition();*/

        bool update_score = false;
        if (number_of_elements_to_damage_with_SwitchingGems > 0)
        {
            update_score = true;
            Calculate_primary_explosion_score(number_of_elements_to_damage_with_SwitchingGems);

            if (player_can_move_when_gem_falling)
                number_of_elements_to_damage_with_SwitchingGems = 0;
        }
        if (number_of_elements_to_damage > 0)
        {
            update_score = true;
            Calculate_secondary_explosion_score(number_of_elements_to_damage+number_of_elements_to_damage_with_bonus);

            if (player_can_move_when_gem_falling)
                number_of_elements_to_damage = 0;
        }
        if (update_score)
        {
            Update_score();
            Check_win_score_condition();
        }
    }

    void Check_win_score_condition()
    {
        if (win_requirement_selected == win_requirement.reach_target_score)
        {
            if (player_turn && (player_score >= target_score))
                Player_win();
        }

        if (lose_requirement_selected == lose_requirement.enemy_reach_target_score)
        {
            if (!player_turn && (enemy_score >= target_score))
                Player_lose();
        }
    }
}
