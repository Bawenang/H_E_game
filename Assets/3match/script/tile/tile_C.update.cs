using UnityEngine;
using System.Collections;

public partial class tile_C : MonoBehaviour {

    private bool isMoveGameToThisTile = false;
    private bool isShowToken = false;
    private bool isCreateFallingGem =  false;
    private bool isDestroyAnimation = false;
    private bool isEndFallAnimation = false;
    private Vector3 endFallDirection;

    #region destroy gem

    public void Check_the_content_of_this_tile()
    {
        if (board.board_array_master[_x, _y, 4] == -100)
        {
            //Debug.Log("junk in " + _x + "," +_y);
            board.Annotate_explosions(_x, _y, Board_C.ExplosionCause.secondayExplosion);
        }
        else if (board.board_array_master[_x, _y, 4] == -200)
        {
            //Debug.Log("token in " + _x + "," +_y);
            board.Annotate_explosions(_x, _y, Board_C.ExplosionCause.secondayExplosion);
            board.number_of_token_collected++;
            board.Update_token_count();
        }
        else if ((board.board_array_master[_x, _y, 4] > 0) && (board.trigger_by_select == Board_C.trigger_by.inventory))
        {
            //board.board_array_master[_x, _y, 11] = 0;

            //add bonus to inventory
            board.Update_inventory_bonus(board.board_array_master[_x, _y, 4], +1);

            board.board_array_master[_x, _y, 4] = 0;//deactivate bonus to avoid trigger when explode
            Update_on_board_bonus_count();
            after_explosion_I_will_become_this_bonus = 0;

            board.Annotate_explosions(_x, _y, Board_C.ExplosionCause.secondayExplosion);

        }
    }


    public void Explosion()
    {
        //Debug.Log(_x + "," + _y + " Explosion()");
        if (board.player_can_move_when_gem_falling)
            board.elements_to_damage_list.Remove(this);

        //Debug.Log(_x+","+_y + " = " + board.board_array_master[_x,_y,1]);
        if (board.board_array_master[_x, _y, 1] >= 0) //if there is something here
        {
            if (board.board_array_master[_x, _y, 1] < board.gem_length)//if this is a normal gem
            {
                if (board.board_array_master[_x, _y, 3] > 0)//if there is a padlock
                {
                    board.board_array_master[_x, _y, 11] = 0;//reset explosion

                    board.board_array_master[_x, _y, 3]--;

                    if (!board.player_can_move_when_gem_falling)
                        {
                        board.number_of_elements_to_damage--;
                        board.number_of_padlocks_involved_in_explosion--;
                        }

                    //check padlock hp
                    if (board.board_array_master[_x, _y, 3] > 0)
                    {
                        SpriteRenderer sprite_lock = my_padlock.GetComponent<SpriteRenderer>();
                        sprite_lock.sprite = board.lock_gem_hp[board.board_array_master[_x, _y, 3] - 1];
                    }
                    else //the padlock hp is 0, so destroy the padlock
                    {
                        Destroy(my_padlock);
                        board.board_array_master[_x, _y, 10] = 1;//now can fall

                        board.padlock_count--;
                        if ((board.win_requirement_selected == Board_C.win_requirement.destroy_all_padlocks) && (board.padlock_count == 0))
                            board.Player_win();
                    }

                    If_all_explosion_are_completed();
                }
                else//no padlock
                {
                    if (board.player_turn)
                        board.total_number_of_gems_destroyed_by_the_player[board.board_array_master[_x, _y, 1]]++;

                    Destroy_gem_avatar();
                }
            }
            else if ((board.board_array_master[_x, _y, 1] >= 41) && (board.board_array_master[_x, _y, 1] < 60)) //it is a block
            {
                Damage_block();
            }
            else if (board.board_array_master[_x, _y, 1] == 9)//this was a bonus
                Destroy_gem_avatar();
        }
        else//this tile is empty, so...
            {
            if (!board.player_can_move_when_gem_falling)
                board.number_of_elements_to_damage--;//strike off this explosion
            }
    }

    public void DoUpdate()
    //void Update()
    {
        //Debug.Log("++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++");
        
        if (isMoveGameToThisTile)
        {
            //Debug.Log("isMoveGameToThisTile");
            Move_gem_to_this_tile();
        }

        else if (isShuffleUpdate)
        {
            //Debug.Log("isShuffleUpdate");
            Shuffle_update();
        }

        else if (isShowToken)
        {
            //Debug.Log("isShowToken");
            Show_token();
        }

        else if (isCreateFallingGem)
        {
            //Debug.Log("isCreateFallingGem");
            Create_falling_gem();
        }

        else if (isEndFallAnimation)
        {
            //Debug.Log("isEndFallAnimation");
            End_fall_animation();
        }


        else if (isDestroyAnimation)
        {
            //Debug.Log("isDestroyAnimation");
            Destroy_animation();
        }

        //Debug.Log("---------------------------------------------------------------");
    }


    void Update_tile_hp()
    {

    }

    void Destroy_gem_avatar()
    {
        //Debug.Log("--------------- Destroy_gem_avatar() START! x = " + _x + ", y = " + _y);
        board.Add_time_bonus(board.time_bonus_for_gem_explosion);

        if (board.explosions_damages_tiles)
            Update_tile_hp();

        //damage adjacent blocks
        if ((_y + 1 < board._Y_tiles) && (board.board_array_master[_x, _y + 1, 14] > 0))
        {
            board.number_of_elements_to_damage++;

            tile_C script_target_block = (tile_C)board.tiles_array[_x, _y + 1];
            script_target_block.Damage_block();

        }
        if ((_y - 1 >= 0) && (board.board_array_master[_x, _y - 1, 14] > 0))
        {
            board.number_of_elements_to_damage++;

            tile_C script_target_block = (tile_C)board.tiles_array[_x, _y - 1];
            script_target_block.Damage_block();

        }
        if ((_x + 1 < board._X_tiles) && (board.board_array_master[_x + 1, _y, 14] > 0))
        {
            board.number_of_elements_to_damage++;

            tile_C script_target_block = (tile_C)board.tiles_array[_x + 1, _y];
            script_target_block.Damage_block();

        }
        if ((_x - 1 >= 0) && (board.board_array_master[_x - 1, _y, 14] > 0))
        {
            board.number_of_elements_to_damage++;

            tile_C script_target_block = (tile_C)board.tiles_array[_x - 1, _y];
            script_target_block.Damage_block();

        }


        blink_time = Time.realtimeSinceStartup;
        my_gem_renderer = my_gem.GetComponent<SpriteRenderer>();
        SetDestroyAnimation();


        if (board.play_this_bonus_sfx == -1)//you don't play sound explosion fx if you must play a bonus sfx
#if UNITY_WEBGL
            board.Play_sfx(board.explosion_sfx_str);
#else
            board.Play_sfx(board.explosion_sfx);
#endif

        //Debug.Log("--------------- Destroy_gem_avatar() END! x = " + _x + ", y = " + _y);
    }

    public void SetDestroyAnimation()
    {
        isDestroyAnimation = true;

        blink_time_start = Time.realtimeSinceStartup;
        blink_time = Time.realtimeSinceStartup;
        blink_delay = 0.2f;
        blink_duration = 1.0f;
    }

    void Destroy_animation()
    {
        //Debug.Log("--------------- Destroy_animation() START! x = " + _x + ", y = " + _y);
        //print(_x + "," + _y + " = " + board.board_array_master[_x, _y, 11]);

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
            isDestroyAnimation = false;
            my_gem_renderer.enabled = false;
            if (after_explosion_I_will_become_this_bonus == 0)
                Destroy_gem();
            else
                Change_gem_in_bonus();

            
        }
        //Debug.Log("--------------- Destroy_animation() END! x = " + _x + ", y = " + _y);
    }

    IEnumerator Return_to_normal_size()
    {
        /*
        blink_time = Time.realtimeSinceStartup;
        while (Time.realtimeSinceStartup - blink_time < 1.0f)
        {
            my_gem_renderer.enabled = !my_gem_renderer.enabled;

            yield return new WaitForSeconds(0.2f);
        }
        */
        //if (Time.realtimeSinceStartup - blink_time >= 1.0f)
        yield return 0;
        {
            my_gem_renderer.enabled = true;

            board.number_of_elements_to_damage--;
            board.board_array_master[_x, _y, 11] = 0;//destruction is over

            If_all_explosion_are_completed();
        }
    }

    void Damage_block()
    {
        //Debug.Log("Damage_block");
        board.board_array_master[_x, _y, 14]--;

        if (board.board_array_master[_x, _y, 14] <= 0)//if the hit had destroy the block
        {
            Destroy(my_gem);

            board.block_count--;
            if ((board.win_requirement_selected == Board_C.win_requirement.destroy_all_blocks) && (board.block_count == 0))
                board.Player_win();

            board.board_array_master[_x, _y, 14] = 0;
            //now this tile is empty
            //my_gem = null;
            board.board_array_master[_x, _y, 1] = -99;

        }
        else //update block sprite
        {
            SpriteRenderer sprite_block = my_gem.GetComponent<SpriteRenderer>();
            if ((board.board_array_master[_x, _y, 1] > 40) && (board.board_array_master[_x, _y, 1] < 50)) //normal block
                sprite_block.sprite = board.block_hp[board.board_array_master[_x, _y, 14] - 1];
            else if ((board.board_array_master[_x, _y, 1] > 50) && (board.board_array_master[_x, _y, 1] < 60)) //falling block
                sprite_block.sprite = board.falling_block_hp[board.board_array_master[_x, _y, 14] - 1];
        }

        if (!board.player_can_move_when_gem_falling)
            board.number_of_elements_to_damage--;

        board.board_array_master[_x, _y, 11] = 0;//reset explosion
        If_all_explosion_are_completed();
    }

    void Put_in_garbage(GameObject target)
    {
        target.transform.parent = board.garbage_recycle;
    }

    public void Destroy_gem()
    {
        //Debug.Log("--------------- Destroy_gem() START! x = "+ _x + ", y = " + _y);
        if (board.board_array_master[_x, _y, 1] < 9)//if this is a normal gem
            Update_gems_score();

        if (board.board_array_master[_x, _y, 4] > 0)
        {
            //Debug.Log("DESTROY BONUS");
        }

        board.board_array_master[_x, _y, 11] = 0;//destruction is over
        board.board_array_master[_x, _y, 1] = -99;//now this tile is empty (so it don't have color)
        board.board_array_master[_x, _y, 4] = 0; //no special
        board.board_array_master[_x, _y, 10] = 0; //can't fall
        Put_in_garbage(my_gem);
        my_gem = null;

        if (!board.player_can_move_when_gem_falling)
            board.number_of_elements_to_damage--;

        If_all_explosion_are_completed();
        //Debug.Log("--------------- Destroy_gem() END!  x = " + _x + ", y = " + _y);
    }

    void If_all_explosion_are_completed()
    {
        //print("If_all_explosion_are_completed() " + board.number_of_elements_to_damage + "..." + board.number_of_elements_to_damage_with_SwitchingGems);

        if (board.player_can_move_when_gem_falling || board.all_explosions_are_completed)
            return;

        //Debug.Log(_x + "," + _y + " switch: " + board.number_of_elements_to_damage_with_SwitchingGems  + " ...secondary: " + board.number_of_elements_to_damage + " ...bonus: " + board.number_of_elements_to_damage_with_bonus + " ... padlocks: " + board.number_of_padlocks_involved_in_explosion);
        if ((board.number_of_elements_to_damage + board.number_of_elements_to_damage_with_SwitchingGems + board.number_of_elements_to_damage_with_bonus) == 0 && board.number_of_padlocks_involved_in_explosion == 0)
        {
            //Debug.Log("all explosions are completed");
            board.all_explosions_are_completed = true;
            board.Start_update_board();
        }
    }

    void Update_gems_score()
    {
        //Debug.Log("--------------- Update_gems_score() START!  x = " + _x + ", y = " + _y);
        board.total_gems_on_board_at_start--;

        //print("Update_gems_score()");
        //fill bonus

        if (board.player_turn)
        {
            int gem_idx = board.board_array_master[_x, _y, 1];
            //Debug.Log("Update_gems_score x = " + _x + ", y = " + _y + " --- gem_idx = " + gem_idx);
            board.total_number_of_gems_destroyed_by_the_player[gem_idx]++;
            board.number_of_gems_collect_by_the_player[gem_idx]++;

            ElementType eleType = (ElementType)gem_idx;

            board.gameController.AddElement(eleType);
            //board.number_of_gems_collect_by_the_player_in_frame[board.board_array_master[_x, _y, 1]]++;



            if (board.number_of_gems_collect_by_the_player[board.board_array_master[_x, _y, 1]]
                < board.number_of_gems_to_destroy_to_win[board.board_array_master[_x, _y, 1]])
            {
                if (!board.player_win)
                {
                    board.total_number_of_gems_remaining_for_the_player--;
                    board.total_number_of_gems_required_colletted++;
                }
                if (board.total_number_of_gems_remaining_for_the_player <= 0
                    && board.win_requirement_selected == Board_C.win_requirement.collect_gems)
                {
                    board.This_gem_color_is_collected(board.board_array_master[_x, _y, 1]);
                    board.Player_win();
                }
            }
            else
            {
                board.This_gem_color_is_collected(board.board_array_master[_x, _y, 1]);
            }

            if (board.continue_to_play_after_win_until_lose_happen)
            {
                if (board.win_requirement_selected == Board_C.win_requirement.collect_gems)
                {
                    if (board.player_win)
                    {
                        board.additional_gems_collected_by_the_player++;

                        if ((board.current_star_score < 3) && (board.player_score >= board.three_stars_target_score) && (board.three_stars_target_additional_gems_collected <= board.additional_gems_collected_by_the_player))

                        {
                            //if ((board.player_score >= board.three_stars_target_score) && (board.three_stars_target_additional_gems_collected <= board.additional_gems_collected_by_the_player))
                            //{
                            board.current_star_score = 3;
                            //}
                        }
                        else if ((board.current_star_score < 2) && (board.player_score >= board.two_stars_target_score) && (board.two_stars_target_additional_gems_collected <= board.additional_gems_collected_by_the_player))

                        {
                            //	if ((board.player_score >= board.two_stars_target_score) && (board.two_stars_target_additional_gems_collected <= board.additional_gems_collected_by_the_player))
                            //{
                            board.current_star_score = 2;
                            //}
                        }
                    }

                    /* if you have the menu kit, DELETE THIS LINE
					if (board.use_star_progress_bar)
						board.my_game_uGUI.my_progress_bar.Update_fill(board.total_number_of_gems_required_colletted+board.additional_gems_collected_by_the_player);

					// if you have the menu kit, DELETE THIS LINE*/
                }
            }

            if (board.use_armor)//damage the enemy according to his armor vulnerability 
            {

            }


        }

        //Debug.Log("--------------- Update_gems_score() END!  x = " + _x + ", y = " + _y);

    }
#endregion



    void Generate_a_new_gem_in_this_tile()
    {
        //create a new gem in this tile
        random_color = (UnityEngine.Random.Range(0, board.gem_length));

        //Don't create 3 gem with the same color subsequently
        if (two_last_colors_created[0] == two_last_colors_created[1])
        {
            if (random_color == two_last_colors_created[0])
            {
                //update the color choiced				
                if (random_color + 1 < board.gem_length)
                    random_color++;
                else
                    random_color = 0;
            }
        }

        //avoid 3 gem with the same color on x axis too
        if ((_x >= 2) && (board.board_array_master[_x - 1, _y, 12] == 1) && (board.board_array_master[_x - 2, _y, 12] == 1))
        {   //if this gem not will fall	
            if (!((_y < board._Y_tiles - 1) && (board.board_array_master[_x, _y + 1, 1] >= 0))) //there are no empty tile under me
            {
                if (random_color == board.board_array_master[_x - 1, _y, 1])
                {
                    //update the color choiced
                    if (random_color + 1 < board.gem_length)
                        random_color++;
                    else
                        random_color = 0;

                }

            }
        }

        board.board_array_master[_x, _y, 1] = random_color;
        //update list of color choiced by this leader-tile
        two_last_colors_created[0] = two_last_colors_created[1];
        two_last_colors_created[1] = random_color;

        board.board_array_master[_x, _y, 10] = 1;//now this tile have a gem 

        //create gem avatar
        Take_from_garbage();

    }

    public void SetCreateFallingGem()
    {
        isCreateFallingGem = true;
        blink_time = Time.realtimeSinceStartup;
        blink_duration = 0.2f;

        if (board.Emit_special_element())
        {
            int element_id = board.Random_choose_special_element_to_create();

            if (element_id == 0) //create a gem
                Generate_a_new_gem_in_this_tile();
            else
            {
                //Take_from_garbage();
                my_gem = board.garbage_recycle.transform.GetChild(0).gameObject;
                my_gem.transform.position = new Vector3(_x, -_y, 0) + board.pivot_board.position;
                my_gem.transform.parent = board.pivot_board.transform;
                my_gem_renderer = my_gem.GetComponent<SpriteRenderer>();


                Create_special_element(element_id);
            }
        }
        else
            Generate_a_new_gem_in_this_tile();

        my_gem_renderer.enabled = false;
    }

    public void Create_falling_gem()
    {
        if (Time.realtimeSinceStartup - blink_time > blink_duration)
        {
            isCreateFallingGem = false;
            //show animation


            //while (Time.realtimeSinceStartup - blink_time < 0.2f)
            //{
            //my_gem_renderer.enabled = !my_gem_renderer.enabled;


            //}


            my_gem_renderer.enabled = true;

            board.number_of_new_gems_to_create--;

            //look at the tile under me to see if this gem must fall
            if ((_y < board._Y_tiles - 1) && (board.board_array_master[_x, _y + 1, 0] > -1) //there is a tile under me
                && (board.board_array_master[_x, _y + 1, 1] == -99))  //and it is empty
            {

                board.number_of_gems_to_move++;
                board.gem_falling_ongoing = true;
                Search_last_empty_tile_under_me();
                if (board.board_array_master[_x, _y, 1] == -99)//if this leader-tile is empty again
                {
                    //repeat procedure
                    SetCreateFallingGem();
                }


            }
            else //this gem don't fall
            {
                if (board.number_of_new_gems_to_create == 0)
                {
                    Check_if_gem_movements_are_all_done();
                }
            }
        }

    }

    public void Fall_by_one_step(int fall_direction)//0 down, 1 down R, 2 down L
    {
        //Debug.Log("Fall_by_one_step");
        if (fall_direction == 0)
        {
            //move the gem on the tile next up the last tile scanned
            board.board_array_master[_x, _y + 1, 1] = board.board_array_master[_x, _y, 1];
            //color from old tile position
            board.board_array_master[_x, _y, 1] = -99;

            //special
            board.board_array_master[_x, _y + 1, 4] = board.board_array_master[_x, _y, 4];
            board.board_array_master[_x, _y, 4] = 0;

            //the gem go in the new tile position
            board.board_array_master[_x, _y + 1, 10] = 1;
            //empty the old tile position
            board.board_array_master[_x, _y, 10] = 0;

            //show the falling animation
            tile_C tile_script = (tile_C)board.tiles_array[_x, _y + 1];
            tile_script.my_gem = my_gem;
            tile_script.my_gem_renderer = tile_script.my_gem.GetComponent<SpriteRenderer>();
            my_gem = null;

            board.board_array_master[_x, _y + 1, 11] = board.board_array_master[_x, _y, 11];
            board.board_array_master[_x, _y, 11] = 0;

            tile_script.Update_gem_position();

        }
        else if (fall_direction == 1)
        {
            //move the gem on the tile next up the last tile scanned
            board.board_array_master[_x + 1, _y + 1, 1] = board.board_array_master[_x, _y, 1];
            //color from old tile position
            board.board_array_master[_x, _y, 1] = -99;

            //special
            board.board_array_master[_x + 1, _y + 1, 4] = board.board_array_master[_x, _y, 4];
            board.board_array_master[_x, _y, 4] = 0;

            //the gem go in the new tile position
            board.board_array_master[_x + 1, _y + 1, 10] = 1;
            //empty the old tile position
            board.board_array_master[_x, _y, 10] = 0;

            //show the falling animation
            tile_C tile_script = (tile_C)board.tiles_array[_x + 1, _y + 1];
            tile_script.my_gem = my_gem;
            tile_script.my_gem_renderer = tile_script.my_gem.GetComponent<SpriteRenderer>();
            my_gem = null;

            board.board_array_master[_x + 1, _y + 1, 11] = board.board_array_master[_x, _y, 11];
            board.board_array_master[_x, _y, 11] = 0;

            tile_script.Update_gem_position();
        }
        else if (fall_direction == 2)
        {
            //move the gem on the tile next up the last tile scanned
            board.board_array_master[_x - 1, _y + 1, 1] = board.board_array_master[_x, _y, 1];
            //color from old tile position
            board.board_array_master[_x, _y, 1] = -99;

            //special
            board.board_array_master[_x - 1, _y + 1, 4] = board.board_array_master[_x, _y, 4];
            board.board_array_master[_x, _y, 4] = 0;

            //the gem go in the new tile position
            board.board_array_master[_x - 1, _y + 1, 10] = 1;
            //empty the old tile position
            board.board_array_master[_x, _y, 10] = 0;

            //show the falling animation
            tile_C tile_script = (tile_C)board.tiles_array[_x - 1, _y + 1];
            tile_script.my_gem = my_gem;
            tile_script.my_gem_renderer = tile_script.my_gem.GetComponent<SpriteRenderer>();
            my_gem = null;

            board.board_array_master[_x - 1, _y + 1, 11] = board.board_array_master[_x, _y, 11];
            board.board_array_master[_x, _y, 11] = 0;

            tile_script.Update_gem_position();

        }

        //board.number_of_gems_to_move--;
    }

    //public IEnumerator Create_gem()
    public void Create_gem()
    {
        board.board_array_master[_x, _y, 11] = 222;
        //Debug.Log(_x + "," + _y + "Create_gem() " + board.board_array_master[_x, _y, 11] + callFrom);

        Generate_a_new_gem_in_this_tile();

        /*
        blink_time = Time.realtimeSinceStartup;
        while (Time.realtimeSinceStartup - blink_time < 1.0f)
        {
            my_gem_renderer.enabled = !my_gem_renderer.enabled;

            yield return new WaitForSeconds(0.2f);
        }
        */

        my_gem_renderer.enabled = true;

        board.number_of_new_gems_to_create--;
        board.board_array_master[_x, _y, 11] = 0;//xxxx 

        Check_if_gem_movements_are_all_done();
    }


    void Search_last_empty_tile_under_me()
    {

        for (int yy = 1; (_y + yy) <= board._Y_tiles - 1; yy++)//scan tiles under me
        {
            if ((board.board_array_master[_x, (_y + yy), 0] == -1) //if I find no tile
                || (board.board_array_master[_x, _y + yy, 1] >= 0)) //or a tile with something
            {

                //move the gem on the tile next up the last tile scanned
                board.board_array_master[_x, _y + yy - 1, 1] = board.board_array_master[_x, _y, 1];
                //color from old tile position
                board.board_array_master[_x, _y, 1] = -99;

                //special
                board.board_array_master[_x, _y + yy - 1, 4] = board.board_array_master[_x, _y, 4];
                board.board_array_master[_x, _y, 4] = 0;

                //the gem go in the new tile position
                board.board_array_master[_x, _y + yy - 1, 10] = 1;
                //empty the old tile position
                board.board_array_master[_x, _y, 10] = 0;

                //show the falling animation
                tile_C tile_script = (tile_C)board.tiles_array[_x, (_y + yy - 1)];
                tile_script.my_gem = my_gem;
                tile_script.my_gem_renderer = tile_script.my_gem.GetComponent<SpriteRenderer>();
                tile_script.Update_gem_position();
                my_gem = null;

                break;
            }
            else if ((_y + yy) == board._Y_tiles - 1) //if I'm on the last row
            {

                //move gem color on last tile
                board.board_array_master[_x, _y + yy, 1] = board.board_array_master[_x, _y, 1];
                //remove color from old tile
                board.board_array_master[_x, _y, 1] = -99;

                //special 
                board.board_array_master[_x, _y + yy - 1, 4] = board.board_array_master[_x, _y, 4];
                board.board_array_master[_x, _y, 4] = 0;

                //gem go in the new position
                board.board_array_master[_x, _y + yy, 10] = 1;
                //empty the old tile
                board.board_array_master[_x, _y, 10] = 0;


                //show falling animation
                tile_C tile_script = (tile_C)board.tiles_array[_x, (_y + yy)];
                tile_script.my_gem = my_gem;
                tile_script.my_gem_renderer = tile_script.my_gem.GetComponent<SpriteRenderer>();
                my_gem = null;
                tile_script.my_gem.transform.parent = tile_script.transform;
                tile_script.Update_gem_position();


                break;
            }
        }

    }

    public void SetShowToken()
    {
        isShowToken = true;
        blink_time = 0.0f;
        blink_delay = 0.2f;
        blink_duration = 1.0f;
    }

    public void Show_token()
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
            isShowToken = false;


            my_gem_renderer.enabled = false;

            //update gem
            my_gem_renderer.sprite = board.token;

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
        }
    }



#region manage gem falling and creation

    public void Update_gem_position()
    {
        //Debug.Log("Update_gem_position. Origin tile x = " + _x + ", y = " + _y);
        board.gem_falling_ongoing = true;
        SetMoveGemToThisTile();
    }

    public void SetMoveGemToThisTile()
    {
        isMoveGameToThisTile = true;
    }

    void Move_gem_to_this_tile()
    {
        //if (my_gem == null)
            //Debug.Log("Move_gem_to_this_tile failed (" + _x + "," + _y + ")");
        if (board.board_array_master[_x, _y, 11] >= 3 && board.board_array_master[_x, _y, 11] <= 5)//falling
            board.board_array_master[_x, _y, 11] = 333;//falling ongoing

        //if (my_gem == null)
        //    return;

        Vector3 displacement = (transform.position - my_gem.transform.position);
        float angle = Vector3.Angle(displacement.normalized, Vector3.down);
        //float speed = 0.0f;

        if (angle < 1.0f && displacement.sqrMagnitude > board.accuracy)
        {

            //speed = board.falling_accel * Time.deltaTime;
            //speed = Mathf.Min(speed, board.falling_speed);

            if (my_gem)
                //my_gem.transform.Translate(((transform.position - my_gem.transform.position).normalized) * board.falling_speed * Time.deltaTime, Space.World);
                //my_gem.transform.Translate(Vector3.down * speed * Time.deltaTime, Space.World);
                my_gem.transform.Translate(Vector3.down * board.falling_speed * Time.deltaTime, Space.World);
                //my_gem.transform.Translate(Vector3.down * board.falling_speed * 0.015f, Space.World);
            else
                Debug.LogWarning("no gem in " + _x + "," + _y);
        }

        else
        {
            isMoveGameToThisTile = false;

            if (Time.timeSinceLevelLoad > board.latest_sfx_time + 0.01f)
            {
#if UNITY_WEBGL
                board.Play_sfx(board.end_fall_sfx_str);
#else
                board.Play_sfx(board.end_fall_sfx);
#endif
                board.latest_sfx_time = Time.timeSinceLevelLoad;
            }

            if (Vector3.Distance(transform.position, my_gem.transform.position) != 0)
                my_gem.transform.position = transform.position;


            if (board.player_can_move_when_gem_falling || board.diagonal_falling)
                End_falling_gems();
            else
                SetEndFallAnimation();

        }


    }

    public void SetEndFallAnimation()
    {
        isEndFallAnimation = true;
        endFallDirection = Vector3.down;
    }

    public void End_fall_animation()
    {        
        Vector3 temp_position = new Vector3(transform.position.x, transform.position.y - 0.25f, transform.position.z);
        Vector3 displacement = temp_position - my_gem.transform.position;
        float angle = Vector3.Angle(endFallDirection, displacement.normalized);

        //go down
        if (angle < 1.0f && displacement.sqrMagnitude > board.accuracySquare)
        {

            my_gem.transform.Translate(endFallDirection * (board.falling_speed * 0.25f) * Time.deltaTime, Space.World);

        }

        else if (endFallDirection == Vector3.down )
        {
            endFallDirection = Vector3.up;
            my_gem.transform.position = temp_position;
        }
        else
        {
            isEndFallAnimation = false;

            my_gem.transform.position = transform.position;

            End_falling_gems();
        }

    }


    public IEnumerator Switch_gem_animation()
    {

        while (Vector3.Distance(transform.position, my_gem.transform.position) > board.accuracy)
        {
            yield return new WaitForSeconds(0.015f);

            if (my_gem == null)
            {
                break;
            }

            my_gem.transform.Translate(((transform.position - my_gem.transform.position).normalized) * board.switch_speed * Time.deltaTime, Space.World);

            if (Vector3.Distance(transform.position, my_gem.transform.position) <= board.accuracy)
            {
                if (Vector3.Distance(transform.position, my_gem.transform.position) != 0)
                    my_gem.transform.position = transform.position;
            }
        }



    }

    void End_falling_gems()
    {

        //Debug.Log (_x + "," + _y + " = " + board.board_array_master [_x, _y, 1]);
        board.board_array_master[_x, _y, 11] = 0;//xxxx 

        board.number_of_gems_to_move--;

        
        Check_if_gem_movements_are_all_done();
    }


    void Play_sfx(AudioClip clip)
    {
        if (board.Stage_uGUI_obj)
        {
            /* if you have the menu kit, DELETE THIS LINE 
			if (my_game_master && clip)
				my_game_master.Gui_sfx(clip);
			//if you have the menu kit, DELETE THIS LINE */
        }
        else
        {
            GetComponent<AudioSource>().clip = clip;
            GetComponent<AudioSource>().Play();
        }

    }

    void Check_if_gem_movements_are_all_done()
    {
        if (board.player_can_move_when_gem_falling)
            return;

        if (board.number_of_gems_to_move + board.number_of_new_gems_to_create == 0)
        {
            board.gem_falling_ongoing = false;

            if (board.diagonal_falling)
            {
                //board.Debug_Board();
                board.Start_update_board();
            }
            else
                board.Check_secondary_explosions();

        }

    }

    void Take_from_garbage()
    {
        my_gem = board.garbage_recycle.transform.GetChild(0).gameObject;
        my_gem.transform.position = new Vector3(_x, -_y, 0) + board.pivot_board.position;
        my_gem.transform.parent = board.pivot_board.transform;
        my_gem_renderer = my_gem.GetComponent<SpriteRenderer>();

        SpriteRenderer sprite_gem = my_gem.GetComponent<SpriteRenderer>();
        sprite_gem.sprite = board.gem_colors[board.board_array_master[_x, _y, 1]];

    }





#endregion

}
