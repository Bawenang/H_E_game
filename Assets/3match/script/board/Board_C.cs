using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;


using System.IO;


using System.Collections.Generic;

public partial class Board_C : MonoBehaviour {

    public GameController gameController;


    public static Board_C this_board;

	public bool diagonal_falling;
	bool diagonal_falling_preference_direction_R; //it is alternate in each step

	public bool player_can_move_when_gem_falling;

    public enum start_after
	{
		time,
		press_button
	}
	public start_after start_after_selected = start_after.time;
	public float start_after_n_seconds;
	
	public float accuracy;
    public float accuracySquare;
    public float falling_accel;
    public float falling_speed;
	public float switch_speed;


    //interaction
    public int touch_number;
    public bool player_can_move;
    public int n_combo;//number of combo explosion after main explosion

    #region custom editor
    public bool show_sprites;
		public bool show_s_bonus_gui;
		public bool show_s_on_board_bonus;
		public bool show_s_tiles;
		public bool show_s_gems;
		public bool show_s_padlocks;
		public bool show_s_blocks;
		public bool show_s_falling_blocks;
		public bool show_s_misc;
	public bool show_rules;
		public bool show_score;
        public bool show_gem_emiter;
        public bool show_bonus;
			public bool show_player_charge_bonus;
			public bool show_enemy_charge_bonus;
				//these variables help to stop the board analysis when AI find the most high value possible
				int max_value_destroy_one;
				int max_value_switch_gem_teleport_click1;
				int max_value_switch_gem_teleport_click2;
				int max_value_destroy_3x3;
				int max_value_destroy_horizontal;
				int max_value_destroy_vertical;
				int max_value_destroy_horizontal_and_vertical;
			public bool show_player_inventory_bonus;
			public bool show_enemy_inventory_bonus;
		public bool show_move_reward_system;
		public bool show_player;
		public bool show_versus;
			public bool show_enemy;
	public bool show_camera_setup;
	public bool show_audio;
	public bool show_visual_fx;
		public bool show_frame_elements;
	public bool show_advanced;
		public bool show_advanced_timing;
		public bool show_advanced_ugui;
		public bool show_garbages;
	
	#endregion

	#region Menu Kit
	public bool show_menu_kit_setup;
	public GameObject Stage_uGUI_obj;

	public int three_stars_target_score;
	public int three_stars_target_score_advantage_vs_enemy;
	public int three_stars_target_player_hp_spared;
	public float three_stars_target_time_spared;
	public int three_stars_target_moves_spared;
	public int three_stars_target_gems_collect_advantage_vs_enemy;
	public int three_stars_target_additional_gems_collected;

	public int two_stars_target_score;
	public int two_stars_target_score_advantage_vs_enemy;
	public int two_stars_target_player_hp_spared;
	public float two_stars_target_time_spared;
	public int two_stars_target_moves_spared;
	public int two_stars_target_gems_collect_advantage_vs_enemy;
	public int two_stars_target_additional_gems_collected;

	public int additional_gems_collected_by_the_player;
	public int current_star_score;

	public bool use_star_progress_bar;
	#endregion




	#region visual fx

	public enum gem_explosion_fx_rule
	{
		no_fx,
		for_each_gem,
		only_for_big_explosion
	}
	public gem_explosion_fx_rule gem_explosion_fx_rule_selected = gem_explosion_fx_rule.no_fx;

	public bool activate_main_gem_fx_for_big_explosion;
	public bool activate_minor_gem_fx_for_big_explosion;

	public explosion_fx[] gem_explosion_fx;
	public Transform[] gem_big_explosion_fx;

	public bool bonus_have_explosion_fx;
		public Transform destroy_one_fx;
		public Transform destroy_3x3_fx;
		public Transform destroy_horizontal_fx;
		public Transform destroy_vertical_fx;
		public Transform destroy_horizontal_and_vertical_fx;

	public enum gem_explosion_animation_type
	{
		no_animation,
		default_animation,
		custom_animation
	}
	public gem_explosion_animation_type gem_explosion_animation_type_selected = gem_explosion_animation_type.default_animation;

	public bool praise_the_player;
		public bool for_big_explosion;
		public bool for_secondary_explosions;
		public bool for_explode_same_color_again;
		public bool for_gain_a_star;
		public bool for_gain_a_turn;

	#endregion


	
	public int current_level = 1;

	
	public Transform pivot_board;
	public Transform garbage_recycle;


	public Transform cursor;
	public bool player_win;
    public bool isTimeLose;

    #region rules
    public enum win_requirement
	{
		destroy_all_tiles,
		enemy_hp_is_zero,
		collect_gems,
		reach_target_score,
		take_all_tokens, //special token to move until the bottom of the board
		//reach_the_exit 	//move the player avatar to exit-tile
		destroy_all_gems,//require shuffle off and gem creation off
		destroy_all_padlocks,
		destroy_all_blocks,
		play_until_lose

	}
	public win_requirement win_requirement_selected = win_requirement.destroy_all_tiles;
	public bool continue_to_play_after_win_until_lose_happen;
    public int total_gems_on_board_at_start;//need for destroy_all_gems
    public bool all_explosions_are_completed;


    public enum lose_requirement
	{
		timer,
		player_hp_is_zero,
		enemy_collect_gems,
		enemy_reach_target_score,
		player_have_zero_moves,
		relax_mode // player can't lose
		
	}
	public lose_requirement lose_requirement_selected = lose_requirement.timer;

	//manage turns
	public bool versus = false;//true = play versus AI
	public bool player_turn = false;//keep it false here in order to give the first move to player
	public int max_moves;
	public int move_gained_for_explode_same_color_in_two_adjacent_turn;
	public int[] move_gained_when_explode_more_than_3_gems;

	public bool game_end;



    public bool explosions_damages_tiles;
	public enum tile_destroyed_give
	{
		nothing,
		more_time,
		more_hp,
		more_moves
	}
	public tile_destroyed_give tile_give = tile_destroyed_give.nothing;
	public int tile_gift_int; //how much hp or moves give
	public float tile_gift_float; //how much time give
	public float time_bonus_for_secondary_explosion;
	public float time_bonus_for_gem_explosion;




	//keep note of the gems destryed
	public int[] number_of_gems_to_destroy_to_win;

			public int[] total_number_of_gems_destroyed_by_the_player;
				public int[] number_of_gems_collect_by_the_player;
                public int[] number_of_gems_collect_by_the_player_in_frame;
                    bool[] player_this_gem_color_is_collected;
				public int total_number_of_gems_remaining_for_the_player;
					public int total_number_of_gems_required_colletted;


    public bool linear_explosion_stop_against_empty_space;
	public bool linear_explosion_stop_against_block;
	public bool linear_explosion_stop_against_bonus;
	public bool linear_explosion_stop_against_token;
	//after_big_explosion
	public enum choose_bonus_by
	{
		gem_color,
		explosion_magnitude
	}
	public choose_bonus_by choose_bonus_by_select;

	public enum trigger_by
	{
		OFF,
		//color,
		click,
		switch_adjacent_gem,
		inventory,
		free_switch
	}
	public trigger_by trigger_by_select;
	bool clickable_bonus_on_boad;
	bool switchable_bonus_on_boad;
	bool free_switchable_bonus_on_boad;
	public int number_of_bonus_on_board;
	public int number_of_junk_on_board;

	public int number_of_token_on_board;
	public int number_of_token_to_collect;
	public int number_of_token_collected;
	public bool show_token_after_all_tiles_are_destroyed;
		bool[,]token_place_card;//if "show_token_after_all_tiles_are_destroyed" is true, use this to keep track of tokens positions
		bool token_showed;

	public int[] player_bonus_inventory;
		public bonus_inventory player_bonus_inventory_script;
	public int[] enemy_bonus_inventory;
		public bonus_inventory enemy_bonus_inventory_script;

	//armors
	public enum armor
	{
		weak,	// = damage * 2
		normal, // = damage * 1
		strong, // = damage * 0.5
		immune,	// = damage = 0
		absorb,
		repel
		
	}
	public bool use_armor;
	public armor[] player_armor;
	public armor[] enemy_armor;


	public int gem_damage_value; //how much damage deliver a gem

	public bool lose_turn_if_bad_move;
	public bool gain_turn_if_explode_same_color_of_previous_move;
	public bool gain_turn_if_explode_more_than_3_gems;
	public bool gain_turn_if_secondary_explosion;
		public int seconday_explosion_maginiture_needed_to_gain_a_turn;
		public int combo_lenght_needed_to_gain_a_turn;
	bool turn_gained;
	public bool chain_turns_limit;
	public int max_chain_turns;
	int current_player_chain_lenght;
	int current_enemy_chain_lenght;
	#endregion



	//manage update board after the move
		public int number_of_gems_to_move;
		public int number_of_new_gems_to_create;
		public int number_of_gems_to_mix;
		public bool gem_falling_ongoing;

	//read board
	public int number_of_moves_possible;
	int[,] list_of_moves_possible;
	public int number_of_gems_moveable;

	public int HP_board;//if 0 mean that all tiles are destroyed (victory requirement)
	public GameObject tile_obj;
	
		public int total_tiles;
		public List<tile_C> elements_to_damage_list;
		public tile_C[,] bottom_tiles_array;//[board orientation, tile]this help to check when token and junk bust exit from the board 
		int[] number_of_bottom_tiles;//[board orientation]
		public int current_board_orientation;
	

	/* if you have the menu kit, DELETE THIS LINE
	game_master my_game_master;
	public game_uGUI my_game_uGUI;
	//if you have the menu kit, DELETE THIS LINE*/


	void Start () {

        accuracySquare = accuracy * accuracy;

        if (start_after_selected == start_after.time)
			{
            if (player_can_move_when_gem_falling)
                StartBoardUpdate();
            else
                {
                if (start_after_n_seconds <= 0)
                    Check_ALL_possible_moves();
                else
                    Invoke("Check_ALL_possible_moves", start_after_n_seconds);
                }
            }
		else//show info_screen
        {
            //gui_info_screen.SetActive(true);
            //gameController.dialogCtrl.PlaySequence(0);

            gameController.stageDisplay.Show(gameController.currentStage, "PlaySequenceWithSfx", gameController.dialogCtrl.gameObject, 0);

        }



    }

    public void OnDestroy()
    {
        if (AudioController.Exists())
            AudioController.instance.StopBGM();

    }

    public void My_start()//call from Canvas > info_screen > button
	{
        if (player_can_move_when_gem_falling)
            StartBoardUpdate();
        else
            {

		    Check_ALL_possible_moves();
            }

        //gui_info_screen.SetActive(false);
    }
	
	void Update()
	{

        if (lose_requirement_selected == lose_requirement.timer)
			Timer();

        //if(player_can_move_when_gem_falling)
        //    BoardUpdate();

        if (isMoveCamera)
            Move_Camera();

        else if (isStartBadSwitchAnimation)
            Start_bad_switch_animation();

        else if (isEndBadSwitchAnimation)
            End_bad_switch_animation();

        else if (isSwitchGemAnimation)
            Switch_gem_animation();

        for (int x = 0; x < _X_tiles; ++x)
        {
            for (int y = 0; y < _Y_tiles; ++y)
            {
                tiles_array[x, y].DoUpdate();
            }
        }
    }






	public void Reset_board()//call from Canvas > Lose_screen > button and Canvas > Win_screen > button
		{
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); 
		}





    #region 2- moves analysis
    public void Check_ALL_possible_moves() 
		{
		//reset count
		number_of_moves_possible = 0;
		clickable_bonus_on_boad = false;
		switchable_bonus_on_boad = false;
		free_switchable_bonus_on_boad = false;

        for (int y = 0; y < _Y_tiles; y++)
        	{
	            for (int x = 0; x < _X_tiles; x++)
	            {
				//reset checks
				board_array_master[x,y,13] = 0;

				Check_moves_of_this_gem(x,y);
				}
			}
		
		if ((number_of_moves_possible <= 0) && (!clickable_bonus_on_boad))
			{
            Check_if_shuffle();
            }
		else
			{
            elements_to_damage_list.Clear();
            ListOfPotentialMoves();
            }

		}

    void ListOfPotentialMoves()
    {
    list_of_moves_possible = new int[number_of_moves_possible, 8];
    /*
    0 = max gems that will explode with this move
    1 = x
    2 = y
    3 = color
        4= up [n. how many gem explode if this gem go up]
        5= down [n. how many gem explode if this gem go down]
        6= right [n. how many gem explode if this gem go right]
        7= left [n. how many gem explode if this gem go left]
        --8 = most big explosion
    */
    number_of_gems_moveable = 0;
				for (int y = 0; y<_Y_tiles; y++)
				{
					for (int x = 0; x<_X_tiles; x++)
					{
                    if (board_array_master[x, y, 11] == 111 || board_array_master[x, y, 11] == 222 || board_array_master[x, y, 11] == 333)//explosion or creation falling animation ongoing
                        continue;

                    if (board_array_master[x, y, 5] >0) //if this gem have at least one move
						{
							//avaible moves of this gem
							//list_of_moves_possible[number_of_gems_moveable,0] = board_array_master[x,y,5];
							//gem color
							list_of_moves_possible[number_of_gems_moveable, 3] = board_array_master[x, y, 1];
							
							//gem position
							list_of_moves_possible[number_of_gems_moveable, 1] = x;
							list_of_moves_possible[number_of_gems_moveable, 2] = y;

							
							
							//moves
							if (board_array_master[x, y, 6]>0)
								{
								list_of_moves_possible[number_of_gems_moveable, 4] = board_array_master[x, y, 6];
								if ( board_array_master[x, y, 6] > list_of_moves_possible[number_of_gems_moveable, 0] )
									list_of_moves_possible[number_of_gems_moveable, 0] = board_array_master[x, y, 6];
								}
							if (board_array_master[x, y, 7]>0)
								{
								list_of_moves_possible[number_of_gems_moveable, 5] = board_array_master[x, y, 7];
								if ( board_array_master[x, y, 7] > list_of_moves_possible[number_of_gems_moveable, 0] )
									list_of_moves_possible[number_of_gems_moveable, 0] = board_array_master[x, y, 7];
								}
							if (board_array_master[x, y, 8]>0)
								{
								list_of_moves_possible[number_of_gems_moveable, 6] = board_array_master[x, y, 8];
								if ( board_array_master[x, y, 8] > list_of_moves_possible[number_of_gems_moveable, 0] )
									list_of_moves_possible[number_of_gems_moveable, 0] = board_array_master[x, y, 8];
								}
							if (board_array_master[x, y, 9]>0)
								{
								list_of_moves_possible[number_of_gems_moveable, 7] = board_array_master[x, y, 9];
								if ( board_array_master[x, y, 9] > list_of_moves_possible[number_of_gems_moveable, 0] )
									list_of_moves_possible[number_of_gems_moveable, 0] = board_array_master[x, y, 9];
								}
							

						//DEBUG show all moves
						/*
							tile_C tile_script = (tile_C)tiles_array[x,y].GetComponent("tile_C");
							if  (list_of_moves_possible[number_of_gems_moveable,0] >= 3) //)&& (_show_all_moves) )
								tile_script.Debug_show_available_moves(list_of_moves_possible[number_of_gems_moveable,0]);
							else
								tile_script.Debug_show_available_moves(0);
						*/

                        if ((board_array_master[x, y, 1] >= 0) && (board_array_master[x, y, 1] <= 8))
                            number_of_gems_moveable++;
						}
					/*
					else //DEBUG
						{
						if (board_array_master[x,y,0] != -1)
							{
							tile_C tile_script = (tile_C)tiles_array[x,y].GetComponent("tile_C");
								tile_script.Debug_show_available_moves(0);
							}
						}
						*/
					}
				}

        if (!player_can_move_when_gem_falling)
            UpdateTurn();

    }


	void Check_moves_of_this_gem(int x, int y) //call from  Check_ALL_possible_moves()
		{
        if (board_array_master[x, y, 11] == 111 || board_array_master[x, y, 11] == 222 || board_array_master[x, y, 11] == 333)//explosion or creation falling animation ongoing
            {
            return;
            }

        //reset array
        board_array_master[x,y,5] = 0; //number of useful moves of this gem [from 0 = none, to 4 = all directions]
			board_array_master[x,y,6] = 0; //up
			board_array_master[x,y,7] = 0; //down
			board_array_master[x,y,8] = 0; //right
			board_array_master[x,y,9] = 0; //left
			
		if ( (board_array_master[x,y,1] >= 0) && (board_array_master[x,y,1] <= 8) && (board_array_master[x,y,3] == 0) )//this gem exist and can move
			{
			# region move to right (x+1,y) if... 
			//it is feasible move to right
			if( ( (x+1)<_X_tiles ) && (board_array_master[x+1,y,0] > -1) //there is a tile
				&& (board_array_master[x+1,y,3] == 0) //no padlock
				&& (board_array_master[x+1,y,1] >= 0) && (board_array_master[x+1,y,1] <= 9) )// there is a gem
				{
				//2 up
					if ((y+2)<_Y_tiles)
					{
						if ((board_array_master[x,y,1]==board_array_master[x+1,y+2,1]) && (board_array_master[x,y,1]==board_array_master[x+1,y+1,1]))
						{
							if (board_array_master[x,y,8]==0)//annotate this move
							{
								number_of_moves_possible++;
								board_array_master[x,y,5]+=1;
							}
							board_array_master[x,y,8]+=2;//this move will make explode +2 gem
						}
					}
				//2 down
					if ((y-2)>=0)
					{
						if ((board_array_master[x,y,1]==board_array_master[x+1,y-2,1]) && (board_array_master[x,y,1]==board_array_master[x+1,y-1,1]))
						{
							if (board_array_master[x,y,8]==0)//annotate this move
							{
								number_of_moves_possible++;
								board_array_master[x,y,5]+=1;
							}
							board_array_master[x,y,8]+=2;//this move will make explode +2 gem
						}
					}
				//1 up and 1 down
					if (((y-1)>=0)&&((y+1)<_Y_tiles))
					{
						if ((board_array_master[x,y,1]==board_array_master[x+1,y-1,1]) && (board_array_master[x,y,1]==board_array_master[x+1,y+1,1]))
						{
							if (board_array_master[x,y,8]==0)//annotate this move
							{
								number_of_moves_possible++;
								board_array_master[x,y,5]+=1;
							}
							//count explosions of this move
							if ((y-2)<0)
								board_array_master[x,y,8]+=1;
							else if ((board_array_master[x,y,1]!=board_array_master[x+1,y-2,1]))
								board_array_master[x,y,8]+=1;
							
							if ((y+2)>=_Y_tiles)
								board_array_master[x,y,8]+=1;
							else if (board_array_master[x,y,1]!=board_array_master[x+1,y+2,1])
								board_array_master[x,y,8]+=1;	
						}
					}
				//2 to right
					if ((x+3)<_X_tiles)
					{
						if ((board_array_master[x,y,1]==board_array_master[x+2,y,1]) && (board_array_master[x,y,1]==board_array_master[x+3,y,1]))
						{
							if (board_array_master[x,y,8]==0)//annotate this move
							{
								number_of_moves_possible++;
								board_array_master[x,y,5]+=1;
							}
							board_array_master[x,y,8]+=2;//this move will make explode +2 gem
						}
					}
				}
			#endregion
			#region move to left (x-1,y) if...
			if( ((x-1) >= 0) && ((board_array_master[x-1,y,0] > -1))  //there is a tile
			  		&& (board_array_master[x-1,y,3] == 0) //no padlock
			 		&& (board_array_master[x-1,y,1] >= 0) && (board_array_master[x-1,y,1] <= 9) )// there is a gem
			   {
				//2 up
				if ((y+2)<_Y_tiles)
				{
					if ((board_array_master[x,y,1]==board_array_master[x-1,y+2,1]) && (board_array_master[x,y,1]==board_array_master[x-1,y+1,1]))
					{
						if (board_array_master[x,y,9]==0)
						{
							number_of_moves_possible++;
							board_array_master[x,y,5]+=1;
						}
						board_array_master[x,y,9]+=2;
					}
				}
				//2 down
				if ((y-2)>=0)
				{
					if ((board_array_master[x,y,1]==board_array_master[x-1,y-2,1]) && (board_array_master[x,y,1]==board_array_master[x-1,y-1,1]))
					{
						if (board_array_master[x,y,9]==0)
						{
							number_of_moves_possible++;
							board_array_master[x,y,5]+=1;
						}							
						board_array_master[x,y,9]+=2;
					}
				}
				//1 up and 1 down
				if (((y-1)>=0)&&((y+1)<_Y_tiles))
				{
					if ((board_array_master[x,y,1]==board_array_master[x-1,y-1,1]) && (board_array_master[x,y,1]==board_array_master[x-1,y+1,1]))
					{
						if (board_array_master[x,y,9]==0)
						{
							number_of_moves_possible++;
							board_array_master[x,y,5]+=1;
						}				
						//count explosions of this move
						if ((y-2)<0)
							board_array_master[x,y,9]+=1;
						else if ((board_array_master[x,y,1]!=board_array_master[x-1,y-2,1]))
							board_array_master[x,y,9]+=1;
						
						if ((y+2)>=_Y_tiles)
							board_array_master[x,y,9]+=1;
						else if (board_array_master[x,y,1]!=board_array_master[x-1,y+2,1])
							board_array_master[x,y,9]+=1;
						
						
					}
				}
				//2 right
				if (x-3>=0)
				{
					if ((board_array_master[x,y,1]==board_array_master[x-2,y,1]) && (board_array_master[x,y,1]==board_array_master[x-3,y,1]))
					{
						if (board_array_master[x,y,9]==0)
						{
							number_of_moves_possible++;
							board_array_master[x,y,5]+=1;
						}
						board_array_master[x,y,9]+=2;
					}
				}
			}
			#endregion
			#region move up (x,y+1) if...
			if( ((y+1)<_Y_tiles) &&  (board_array_master[x,y+1,0]  > -1)  
			   && (board_array_master[x,y+1,3] == 0) //no padlock
			   && (board_array_master[x,y+1,1] >= 0) && (board_array_master[x,y+1,1] <= 9) )// there is a gem
			   {
				//2 up left
				if ((x+2)<_X_tiles)
				{
					if ((board_array_master[x,y,1]==board_array_master[x+2,y+1,1]) && (board_array_master[x,y,1]==board_array_master[x+1,y+1,1]))
					{
						if (board_array_master[x,y,6]==0)
						{
							number_of_moves_possible++;
							board_array_master[x,y,5]+=1;
						}
						board_array_master[x,y,6]+=2;
					}
				}
				//2 up right
				if ((x-2)>=0)
				{
					if ((board_array_master[x,y,1]==board_array_master[x-2,y+1,1]) && (board_array_master[x,y,1]==board_array_master[x-1,y+1,1]))
					{
						if (board_array_master[x,y,6]==0)
						{
							number_of_moves_possible++;
							board_array_master[x,y,5]+=1;
						}
						board_array_master[x,y,6]+=2;
					}
				}
				//due up (1 right and 1 left)
				if (((x-1)>=0)&&((x+1)<_X_tiles))
				{
					if ((board_array_master[x,y,1]==board_array_master[x-1,y+1,1]) && (board_array_master[x,y,1]==board_array_master[x+1,y+1,1]))
					{
						if (board_array_master[x,y,6]==0)
						{
							number_of_moves_possible++;
							board_array_master[x,y,5]+=1;
						}
						
						if ((x-2)<0)
							board_array_master[x,y,6]+=1;
						else if (board_array_master[x,y,1]!=board_array_master[x-2,y+1,1])
							board_array_master[x,y,6]+=1;
						
						if ((x+2)>=_X_tiles)
							board_array_master[x,y,6]+=1;
						else if (board_array_master[x,y,1]!=board_array_master[x+2,y+1,1])
							board_array_master[x,y,6]+=1;
						
						
					}
				}
				//2 up
				if (y+3<_Y_tiles)
				{
					if ((board_array_master[x,y,1]==board_array_master[x,y+2,1]) && (board_array_master[x,y,1]==board_array_master[x,y+3,1]))
					{
						if (board_array_master[x,y,6]==0)
						{
							number_of_moves_possible++;
							board_array_master[x,y,5]+=1;
						}
						board_array_master[x,y,6]+=2;
					}
				}
			}
			#endregion
			#region move down (x,y-1) if...
			if( ((y-1)>=0) &&  (board_array_master[x,y-1,0] > -1)
			   && (board_array_master[x,y-1,3] == 0) //no padlock
			   && (board_array_master[x,y-1,1] >= 0) && (board_array_master[x,y-1,1] <= 9) )// there is a gem
			   
			   {
				if ((x+2)<_X_tiles)
				{
					if ((board_array_master[x,y,1]==board_array_master[x+2,y-1,1]) && (board_array_master[x,y,1]==board_array_master[x+1,y-1,1]))
					{
						if (board_array_master[x,y,7]==0)
						{
							number_of_moves_possible++;
							board_array_master[x,y,5]+=1;
						}
						board_array_master[x,y,7]+=2;
					}
				}
				//2 down right
				if ((x-2)>=0)
				{
					if ((board_array_master[x,y,1]==board_array_master[x-2,y-1,1]) && (board_array_master[x,y,1]==board_array_master[x-1,y-1,1]))
					{
						if (board_array_master[x,y,7]==0)
						{
							number_of_moves_possible++;
							board_array_master[x,y,5]+=1;
						}
						board_array_master[x,y,7]+=2;
					}
				}
				//2 down (1 right and 1 left)
				if (((x-1)>=0)&&((x+1)<_X_tiles))
				{
					if ((board_array_master[x,y,1]==board_array_master[x-1,y-1,1]) && (board_array_master[x,y,1]==board_array_master[x+1,y-1,1]))
					{
						if (board_array_master[x,y,7]==0)
						{
							number_of_moves_possible++;
							board_array_master[x,y,5]+=1;
						}
						
						
						if ((x-2)<0)
							board_array_master[x,y,7]+=1;
						else if (board_array_master[x,y,1]!=board_array_master[x-2,y-1,1])
							board_array_master[x,y,7]+=1;
						
						if ((x+2)>=_X_tiles)
							board_array_master[x,y,7]+=1;
						else if (board_array_master[x,y,1]!=board_array_master[x+2,y-1,1])
							board_array_master[x,y,7]+=1;
						
						
					}
				}
				//2 down
				if (y-3>=0)
				{
					if ((board_array_master[x,y,1]==board_array_master[x,y-2,1]) && (board_array_master[x,y,1]==board_array_master[x,y-3,1]))
					{
						if (board_array_master[x,y,7]==0)
						{
							number_of_moves_possible++;
							board_array_master[x,y,5]+=1;
						}
						board_array_master[x,y,7]+=2;
					}
				}
			}
			#endregion

			//count myself in the explosion
			if (board_array_master[x,y,6] > 0)
				board_array_master[x,y,6]++;
			if (board_array_master[x,y,7] > 0)
				board_array_master[x,y,7]++;
			if (board_array_master[x,y,8] > 0)
				board_array_master[x,y,8]++;
			if (board_array_master[x,y,9] > 0)
				board_array_master[x,y,9]++;
		    }
		else if (board_array_master[x,y,4] > 0) //if there is a bonus on the board
			{
			if (trigger_by_select == trigger_by.click)
				clickable_bonus_on_boad = true;
			else if (trigger_by_select == trigger_by.switch_adjacent_gem)
				{
				//if this bonus can be trigger by a gem
				switchable_bonus_on_boad = true;
				}
			else if (trigger_by_select == trigger_by.free_switch)
				{
				//if this bonus can be move
				free_switchable_bonus_on_boad = true;
               /*
                //this bonus can be moved in these directions:
                //if it can move to right
                if (((x + 1) < _X_tiles) && (board_array_master[x + 1, y, 0] > -1) //there is a tile
                    && (board_array_master[x + 1, y, 3] == 0) //no padlock
                    && (board_array_master[x + 1, y, 1] >= 0) && (board_array_master[x + 1, y, 1] <= 9))// there is a gem
                        {
                        //number_of_moves_possible++;
                        board_array_master[x, y, 5] += 1;
                        board_array_master[x, y, 8]++;
                        }
                //if it can move to left
                if (((x - 1) >= 0) && ((board_array_master[x - 1, y, 0] > -1))  //there is a tile
                    && (board_array_master[x - 1, y, 3] == 0) //no padlock
                    && (board_array_master[x - 1, y, 1] >= 0) && (board_array_master[x - 1, y, 1] <= 9))// there is a gem
                        {
                        //number_of_moves_possible++;
                        board_array_master[x, y, 5] += 1;
                        board_array_master[x, y, 9]++;
                        }
                //if it can move up
                if (((y + 1) < _Y_tiles) && (board_array_master[x, y + 1, 0] > -1)
               && (board_array_master[x, y + 1, 3] == 0) //no padlock
               && (board_array_master[x, y + 1, 1] >= 0) && (board_array_master[x, y + 1, 1] <= 9))// there is a gem
                    {
                    //number_of_moves_possible++;
                    board_array_master[x, y, 6]++;
                    board_array_master[x, y, 5] += 1;
                    }
                //if it can move down
                if (((y - 1) >= 0) && (board_array_master[x, y - 1, 0] > -1)
               && (board_array_master[x, y - 1, 3] == 0) //no padlock
               && (board_array_master[x, y - 1, 1] >= 0) && (board_array_master[x, y - 1, 1] <= 9))// there is a gem
                    {
                    //number_of_moves_possible++;
                    board_array_master[x, y, 7]++;
                    board_array_master[x, y, 5] += 1;
                    }*/
            }
        }   

		
	}

   
	


	
	#endregion

	#region 3- play the move
	


	void Assign_in_board_bonus(int xx, int yy, int explosion_magnitude)
	{
        //Debug.Log("Assign_in_board_bonus");
        tile_C tile_script = (tile_C)tiles_array[xx, yy];

	}

    public void Annotate_explosions(int xx, int yy, ExplosionCause explosion_caused_by)// = ExplosionCause.secondayExplosion)
		{
        //print("Annotate_explosions " + xx + "," + yy + " ... " + board_array_master[xx, yy, 11] + " cause: " + explosion_caused_by);

        if (board_array_master[xx, yy, 11] == 111 || board_array_master[xx, yy, 11] == 222 || board_array_master[xx, yy, 11] == 333)//explosion or creation falling animation ongoing
            return;

        if  (board_array_master[xx,yy,11] == 0 || (board_array_master[xx, yy, 11] == 6 && (explosion_caused_by == ExplosionCause.switchingGems || explosion_caused_by == ExplosionCause.bonus))) // if this explosion not is already marked
			{
            //Debug.Log("Annotate_explosions " + xx +"," + yy + " *** number_of_elements_to_damage: " + number_of_elements_to_damage + " *** elements_to_damage_array: " + elements_to_damage_array.Length);
            if (explosion_caused_by == ExplosionCause.switchingGems)
                {
                board_array_master[xx, yy, 11] = 666;// this is a primary explosion
                number_of_elements_to_damage_with_SwitchingGems++;
                }
            else
                {
                board_array_master[xx,yy,11] = 1;// this gem explode
                if (explosion_caused_by == ExplosionCause.secondayExplosion)
                    number_of_elements_to_damage++;
                else if (explosion_caused_by == ExplosionCause.bonus)
                    number_of_elements_to_damage_with_bonus++;
                }

            //elements_to_damage_array[number_of_elements_to_damage] = tiles_array[xx,yy];
            elements_to_damage_list.Add(tiles_array[xx, yy]);
			//Debug.Log("elements_to_damage_array["+number_of_elements_to_damage+"] = " + elements_to_damage_array[number_of_elements_to_damage]);

			
            //print("number_of_elements_to_damage: " + number_of_elements_to_damage);

			if (board_array_master[xx,yy,3]>0)
				number_of_padlocks_involved_in_explosion++;

			if (board_array_master[xx,yy,4] > 0)//if this is a bonus, trigger it!
				{
                //Debug.Log("annotate explosion trigger bonus");
                tile_C tile_script = (tile_C)tiles_array[xx, yy];
					tile_script.Trigger_bonus(false);
				}
			}
		}




    public void Order_to_gems_to_explode() //not use in step_by_step_update
		{
        //print("Order_to_gems_to_explode(): switch = " + number_of_elements_to_damage_with_SwitchingGems + " ... secondary = " + number_of_elements_to_damage + " ... bonus = " + number_of_elements_to_damage_with_bonus);
        if (player_can_move_when_gem_falling)
            return;


		Cancell_hint();
        Calculate_score();// (number_of_elements_to_damage_temp, "Order_to_gems_to_explode");
        
		for (int n = 0; n < elements_to_damage_list.Count; n++)
			{
            //tile_C script_gem = (tile_C)elements_to_damage_array[n].GetComponent("tile_C");
            tile_C script_gem = (tile_C)elements_to_damage_list[n];
                script_gem.Explosion();
            }

        elements_to_damage_list.Clear();

    }
	#endregion
	
	#region 4- aftermath of the explosion(s)
	public void Debug_Board()
	{
		/*
		for (int y = 0; y < _Y_tiles; y++)
		{
			for (int x = 0; x < _X_tiles; x++)
			{
				tiles_array[x,y].GetComponent<Renderer>().material.color = Color.white;
				
			}
		}
*/
/*
		for (int y = 0; y < _Y_tiles; y++)
		{
			for (int x = 0; x < _X_tiles; x++)
			{
				tiles_array[x,y].GetComponent<Renderer>().material.color = Color.white;
				
			}
		}*/
		/*
		for (int y = 0; y < _Y_tiles; y++)
		{
			for (int x = 0; x < _X_tiles; x++)
			{

				if (board_array_master[x,y,11] != 0)
					tiles_array[x,y].GetComponent<Renderer>().material.color = Color.red;
			}
		}*/
        /*
		for (int y = 0; y < _Y_tiles; y++)
		{
			for (int x = 0; x < _X_tiles; x++)
			{
				
				tile_C tile_script = (tile_C)tiles_array[x,y].GetComponent("tile_C");
				tile_script.Debug_show_my_color();
				
			}
		}*/

        /*
		for (int y = 0; y < _Y_tiles; y++)
		{
			for (int x = 0; x < _X_tiles; x++)
			{
				tiles_array[x,y].GetComponent<Renderer>().material.color = Color.white;
				
			}
		}
		 */

       // Debug.Break ();
        //Start_update_board ("Debug_Board()");
    }



    void Check_vertical_falling()
    {
        for (int y = 0; y < _Y_tiles - 1; y++)//don't check last line
        {
            for (int x = 0; x < _X_tiles; x++)
            {
                if ((board_array_master[x, y, 13] == 0) && (board_array_master[x, y, 10] == 1))//this could be fall
                {
                    if (board_array_master[x, y, 11] == 111 || board_array_master[x, y, 11] == 222 || board_array_master[x, y, 11] == 333)//explosion or creation falling animation ongoing
                        continue;

                    //check if have an empty tile under
                    if ((board_array_master[x, y + 1, 0] > -1) && (board_array_master[x, y + 1, 1] == -99)
                       || board_array_master[x, y + 1, 11] == 3) //or something falling

                    {
                        //vertical fall
                        //tiles_array[x,y].GetComponent<Renderer>().material.color = Color.gray;
                        board_array_master[x, y, 11] = 3;//vertical fall
                        board_array_master[x, y, 13] = 1;//tile checked
                        number_of_gems_to_move++;

                        //all empty tiles under this gem are reserved to it
                        for (int yy = y + 1; yy < _Y_tiles; yy++)
                        {
                            if ((board_array_master[x, yy, 0] > -1) && (board_array_master[x, yy, 1] == -99))
                            {
                                board_array_master[x, yy, 13] = 1;
                                //tiles_array[x,yy].GetComponent<Renderer>().material.color = Color.grey;
                            }
                            else
                                break;
                        }
                    }
                }
            }
        }
    }

    void Check_vertical_falling_bottom_to_top()
    {
        for (int y = _Y_tiles - 2; y >= 0; y--)//from bottom to top
            {
                for (int x = 0; x<_X_tiles; x++)
                {
                if (board_array_master[x, y, 11] == 111 || board_array_master[x, y, 11] == 222 || board_array_master[x, y, 11] == 333)//explosion or creation falling animation ongoing
                    continue;

                if ((board_array_master[x, y, 13] == 0) && (board_array_master[x, y, 10] == 1))//this could be fall
                    {
                        //check if have an empty tile under
                        if ((board_array_master[x, y + 1, 0] > -1) && (board_array_master[x, y + 1, 1] == -99)
                           || board_array_master[x, y + 1, 11] == 3 || board_array_master[x, y + 1, 11] == 4 || board_array_master[x, y + 1, 11] == 5) //or something falling

                        {
                            //vertical fall
                            //tiles_array[x,y].GetComponent<Renderer>().material.color = Color.gray;
                            board_array_master[x, y, 11] = 3;//vertical fall
                            board_array_master[x, y, 13] = 1;//tile checked
                            number_of_gems_to_move++;

                            //all empty tiles under this gem are reserved to it
                            for (int yy = y + 1; yy<_Y_tiles; yy++)
                            {
                                if ((board_array_master[x, yy, 0] > -1) && (board_array_master[x, yy, 1] == -99))
                                {
                                    board_array_master[x, yy, 13] = 1;
                                    //tiles_array[x,yy].GetComponent<Renderer>().material.color = Color.grey;
                                }
                                else
                                    break;
                            }
                        }
                    }
                }
            }
    }
    
    void Check_diagonal_falling(string calledFrom)
    {
        if (!diagonal_falling)
            return;

      //print("Check_diagonal_falling() - " + calledFrom);
      diagonal_falling_preference_direction_R = !diagonal_falling_preference_direction_R;
        for (int y = 0; y < _Y_tiles - 1; y++)//don't check last line
        {
            for (int x = 0; x < _X_tiles; x++)
            {
                if (board_array_master[x, y, 11] == 111 || board_array_master[x, y, 11] == 222 || board_array_master[x, y, 11] == 333)//explosion or creation falling animation ongoing
                    continue;

                if ((board_array_master[x, y, 13] == 0) && (board_array_master[x, y, 10] == 1))//this could be fall
                {

                    if (diagonal_falling_preference_direction_R)
                    {
                        Diagonal_fall_R(x, y);
                        Diagonal_fall_L(x, y);
                    }
                    else
                    {
                        Diagonal_fall_L(x, y);
                        Diagonal_fall_R(x, y);
                    }
                }
            }
        }
    }






    void Diagonal_fall_R(int x, int y)
	{
		if (x+1 <_X_tiles)
		{
		if ((board_array_master[x + 1, y + 1, 0] > -1) &&
                (board_array_master[x+1,y+1,13] ==  0 && board_array_master[x+1,y+1,1] == -99) //empty
             )// || (board_array_master[x + 1, y + 1, 13] == 1 && board_array_master[x+1, y+1, 11] == 4)) //or something falling R
            {
			//tiles_array[x,y].GetComponent<Renderer>().material.color = Color.red;
			board_array_master[x,y,11] = 4;//R falling
			board_array_master[x,y,13] = 1;
			number_of_gems_to_move++;
			
			board_array_master[x+1,y+1,13] = 1;//reserved target position
			}
		}
	}

	void Diagonal_fall_L(int x, int y)
	{
		if (x-1 >= 0)
		{
			if ((board_array_master[x-1,y+1,13] ==  0) && (board_array_master[x-1,y+1,0] > -1) && (board_array_master[x-1,y+1,1] == -99))
			{
				//if (tiles_array[x,y].GetComponent<Renderer>().material.color != Color.red)
				//	tiles_array[x,y].GetComponent<Renderer>().material.color = Color.blue;
				//else
				//	tiles_array[x,y].GetComponent<Renderer>().material.color = Color.magenta;
				board_array_master[x,y,11] = 5;//L falling
				board_array_master[x,y,13] = 1;
				number_of_gems_to_move++;
				
				board_array_master[x-1,y+1,13] = 1;//reserved target position
			}
		}
	}

    

    void Update_board_by_one_step()
    {

        if ((number_of_gems_to_move > 0) || (number_of_new_gems_to_create > 0) || (number_of_elements_to_damage + number_of_elements_to_damage_with_SwitchingGems + number_of_elements_to_damage_with_bonus > 0))
        {
            if (number_of_elements_to_damage > 0)
            {
                Cancell_hint();
                Calculate_score();
            }

            //Debug.Log ("Update_board_by_one_step()");
            //for (int y = 0; y < _Y_tiles; y++)
            for (int y = _Y_tiles - 1; y >= 0; y--)//from bottom to top
            {
                for (int x = 0; x < _X_tiles; x++)
                {
                    //Debug.Log(x + "," + y + " = " + board_array_master[x,y,11]);
                    board_array_master[x, y, 13] = 0;//no checked

                    if (board_array_master[x, y, 11] == 1)//destroy
                    {
                        tile_C tile_script = (tile_C)tiles_array[x, y];
                        tile_script.Explosion();
                    }
                    else if (board_array_master[x, y, 11] == 2)//creation
                    {
                        //Debug.Log(x + "," + y + " = " + "create gem");
                        tile_C tile_script = (tile_C)tiles_array[x, y];
                        tile_script.Create_gem();
                    }
                    else if (board_array_master[x, y, 11] >= 3)//falling
                    {
                        //Debug.Log(x + "," + y + " = " + "falling gem");
                        tile_C tile_script = (tile_C)tiles_array[x, y];
                        tile_script.Fall_by_one_step(board_array_master[x, y, 11] - 3);
                    }
                }
            }

        }
        else
        {
            if (!gem_falling_ongoing)
                Check_secondary_explosions();
        }
    }




    void Search_secondary_exposions()
        {
        for (int y = 0; y<_Y_tiles; y++)
        	{
	            for (int x = 0; x<_X_tiles; x++)
	            {
                //take advantage of this cycle to rest these variables:
                board_array_master[x, y, 13] = 0;//tile not checked

                if ((board_array_master[x, y, 11] != 0))
                    continue;

				if ( (board_array_master[x, y, 1] >= 0)  && (board_array_master[x, y, 1] < 9) )//if there is a gem here
					{
					if ((x+1<_X_tiles)&&(x-1>=0))//horizontal
						if ( (board_array_master[x, y, 1]==board_array_master[x + 1, y, 1])&&(board_array_master[x, y, 1]==board_array_master[x - 1, y, 1]) ) //if the adjacent tiles have gems with the same color
							{
                            if (board_array_master[x + 1, y, 11] != 0 || board_array_master[x - 1, y, 11] != 0)//...but ignore if they are busy
                                continue;

                                Annotate_explosions(x, y, ExplosionCause.secondayExplosion);

                                Annotate_explosions(x+1, y, ExplosionCause.secondayExplosion);
                                if ( (x+2<_X_tiles) && (board_array_master[x + 2, y, 1] == board_array_master[x, y, 1]) )
                                        Annotate_explosions(x+2, y, ExplosionCause.secondayExplosion);

                                Annotate_explosions(x-1, y, ExplosionCause.secondayExplosion);
                                if ( (x-2>=0) && (board_array_master[x - 2, y, 1] == board_array_master[x, y, 1]) )
                                        Annotate_explosions(x-2, y, ExplosionCause.secondayExplosion);
                            }
					if ( ((y+1<_Y_tiles)&&(y-1>=0)) )//vertical
						if ( (board_array_master[x, y, 1]==board_array_master[x, y + 1, 1])&&(board_array_master[x, y, 1]==board_array_master[x, y - 1, 1]) )//if the adjacent tiles have gems with the same color
							{
                            if (board_array_master[x,y+1, 11] != 0 || board_array_master[x, y-1, 11] != 0)//...but ignore if they are busy
                                continue;

                            Annotate_explosions(x, y, ExplosionCause.secondayExplosion);

                            Annotate_explosions(x, y+1, ExplosionCause.secondayExplosion);
                            if ( (y+2<_Y_tiles) && (board_array_master[x, y + 2, 1] == board_array_master[x, y, 1]) )
                                        Annotate_explosions(x, y+2, ExplosionCause.secondayExplosion);

                            Annotate_explosions(x, y-1, ExplosionCause.secondayExplosion);
                            if ( (y-2>=0) && (board_array_master[x, y - 2, 1] == board_array_master[x, y, 1]) )
                                        Annotate_explosions(x, y-2, ExplosionCause.secondayExplosion);
                        }
					}
				}
			}
}



    


	void Check_bottom_tiles()
		{
		for (int i = 0; i < number_of_bottom_tiles[current_board_orientation]; i++)
			{
			bottom_tiles_array[current_board_orientation,i].Check_the_content_of_this_tile();
			}
		}

	public void Check_secondary_explosions()//call from: Update_board_by_one_step(),  Board_C.freeMovement.BoardUpdate(), tile_C.shuffle.Check_if_shuffle_is_done(), tile_C.update.Check_if_gem_movements_are_all_done(), inventory_bonus_button.Activate(), 
    {
        if (!player_can_move_when_gem_falling)
            {
            all_explosions_are_completed = false;
            number_of_elements_to_damage = 0;
            number_of_elements_to_damage_with_SwitchingGems = 0;
            number_of_elements_to_damage_with_bonus = 0;
            number_of_padlocks_involved_in_explosion = 0;
		    number_of_gems_to_move = 0;
		    number_of_new_gems_to_create = 0;
		    score_of_this_turn_move = 0;
		    play_this_bonus_sfx = -1;
            }

        if ( ( (trigger_by_select == trigger_by.inventory) && (number_of_bonus_on_board > 0) )
		    ||(number_of_token_on_board > 0)  
		    || (number_of_junk_on_board > 0))
			Check_bottom_tiles();

        Search_secondary_exposions();


		
		if (number_of_elements_to_damage > 0)//if there is at least an explosion
			{
            n_combo++;
            Add_time_bonus(time_bonus_for_secondary_explosion);

            if (!player_can_move_when_gem_falling)
                Order_to_gems_to_explode();

            }
		    else
                {
                if (!player_can_move_when_gem_falling)
                    Check_ALL_possible_moves();
                }

    }  
	#endregion	
 


}
