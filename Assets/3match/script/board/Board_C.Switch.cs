using UnityEngine;
using System.Collections;

public partial class Board_C : MonoBehaviour
{

    public enum moveStatus//avoid multiple moves in the same step
    {
        waitingNewMove,
        switchingGems,
        switchingGemsAnimationOngoing
    }
    public moveStatus current_moveStatus;

    public int temp_direction = -1;

    public bool isStartBadSwitchAnimation = false;
    public bool isEndBadSwitchAnimation = false;
    public bool isSwitchGemAnimation = false;
    tile_C tileMinor;
    tile_C tileMain;

    Vector3 displacementMain;
    Vector3 displacementMinor;

    Vector3 directionMain;
    Vector3 directionMinor;

    public bool[,] position_of_gems_that_will_explode;
    /* [0 main_gem ; 1 minor_gem]
         *0 = the one 2 to up
         *1 = the one 1 to up
         *2 = the one 2 to right
         *3 = the one 1 to right
         *4 = the one 2 to down
         *5 = the one 1 to down
         *6 = the one 2 to left
         *7 = the one 1 to left
    */
    public int number_of_padlocks_involved_in_explosion;
    public int number_of_elements_to_damage;
    public int number_of_elements_to_damage_with_bonus;
    public int number_of_elements_to_damage_with_SwitchingGems;
    public int gems_useful_moved;//help to know if the move is a double-move (if = 2: main gem AND minor gem will explode)

    public enum ExplosionCause
    {
        switchingGems,
        bonus,
        secondayExplosion
    }


    public GameObject avatar_main_gem;
    public int main_gem_selected_x = -10;
    public int main_gem_selected_y = -10;
    public int main_gem_color = -10;

    public GameObject avatar_minor_gem;
    public int minor_gem_destination_to_x = -10;
    public int minor_gem_destination_to_y = -10;
    public int minor_gem_color = -10;


    public void SwitchingGems()
    {
        
        //print("++ Board_C.SwitchingGems main_gem_selected_x = " + main_gem_selected_x + " main_gem_selected_y = " + main_gem_selected_y);
        //print("++ Board_C.SwitchingGems minor_gem_destination_to_x = " + minor_gem_destination_to_x + " minor_gem_destination_to_y = " + minor_gem_destination_to_y);
        current_moveStatus = moveStatus.switchingGemsAnimationOngoing;

        //Switching gems ongoing
        board_array_master[main_gem_selected_x, main_gem_selected_y, 11] = 6;
        board_array_master[minor_gem_destination_to_x, minor_gem_destination_to_y, 11] = 6;


        //reset variables
        number_of_elements_to_damage_with_SwitchingGems = 0;
        explode_same_color_again_with = 0;//0 = false; 1 = with main gem; 2 =with minor gem
        score_of_this_turn_move = 0;
        n_gems_exploded_with_main_gem = 0;
        n_gems_exploded_with_minor_gem = 0;


        //if (!player_can_move_when_gem_falling)
        {
            cursor.gameObject.SetActive(false);
            Cancell_hint();
            player_can_move = false;
        }

        Move_gems_to_target_positions();
        Center_camera_to_move();
        //print("-- Board_C.SwitchingGems");
    }

    public void AfterSwitchAnim()
    { 

        //if this move make explode something
        if (This_switch_will_produce_an_explosion())
        {
            //create array to note the explosions
            position_of_gems_that_will_explode = new bool[2, 8];

            if (Main_gem_explode())
            {
                gems_useful_moved++;
                n_gems_exploded_with_main_gem++;
                Check_if_this_gem_have_the_same_color_of_the_gem_exploded_in_the_previous_move(board_array_master[minor_gem_destination_to_x, minor_gem_destination_to_y, 1]);
                Detect_which_gems_will_explode(minor_gem_destination_to_x, minor_gem_destination_to_y, 0);
            }
            else
            {
                //no color to keep track here
                if (player_turn)
                    player_previous_exploded_color[0] = -1;
                else
                    enemy_previous_exploded_color[0] = -1;
            }

            if (Minor_gem_explode())
            {
                gems_useful_moved++;
                n_gems_exploded_with_minor_gem++;
                Check_if_this_gem_have_the_same_color_of_the_gem_exploded_in_the_previous_move(board_array_master[main_gem_selected_x, main_gem_selected_y, 1]);
                Detect_which_gems_will_explode(main_gem_selected_x, main_gem_selected_y, 1);
            }
            else
            {
                //no color to keep track here
                if (player_turn)
                    player_previous_exploded_color[1] = -1;
                else
                    enemy_previous_exploded_color[1] = -1;
            }

            //keep track of score
            if (player_turn)
            {
                if (explode_same_color_again_with > 0)
                    player_explode_same_color_n_turn++;
                else
                    player_explode_same_color_n_turn = 0;
            }
            else
            {
                if (explode_same_color_again_with > 0)
                    enemy_explode_same_color_n_turn++;
                else
                    enemy_explode_same_color_n_turn = 0;
            }

            Empty_main_and_minor_gem_selections();
        }
        else //this move is useless (no explosions)
        {
            //...but is a good move if move a bonus with free_switch
            if (trigger_by_select == trigger_by.free_switch &&
                ((board_array_master[main_gem_selected_x, main_gem_selected_y, 4] > 0) || (board_array_master[minor_gem_destination_to_x, minor_gem_destination_to_y, 4] > 0)))
            {
                //Debug.Log("free switch without main explosion");

                //trigger bonus
                //elements_to_damage_array = new GameObject[15];
                if (board_array_master[main_gem_selected_x, main_gem_selected_y, 4] > 0)
                {
                    tile_C tile_script = (tile_C)tiles_array[main_gem_selected_x, main_gem_selected_y];
                    tile_script.Trigger_bonus(false);
                }
                if (board_array_master[minor_gem_destination_to_x, minor_gem_destination_to_y, 4] > 0)
                {
                    tile_C tile_script = (tile_C)tiles_array[minor_gem_destination_to_x, minor_gem_destination_to_y];
                    tile_script.Trigger_bonus(false);
                }

                Empty_main_and_minor_gem_selections();
            }
            else //is a bad move
                //SetStartBadSwitchAnimation();
                SetEndBadSwitchAnimation();
        }


        
        if (number_of_elements_to_damage_with_SwitchingGems + number_of_elements_to_damage_with_bonus > 0)//if there is something to explode
        {
            Cancell_hint();

            if (!player_can_move_when_gem_falling)
                Order_to_gems_to_explode();
        }
    }

    bool This_switch_will_produce_an_explosion()
    {

        if (((board_array_master[main_gem_selected_x, main_gem_selected_y, 5] > 0) || (board_array_master[minor_gem_destination_to_x, minor_gem_destination_to_y, 5] > 0)) &&
            (temp_direction == 4 && ((board_array_master[main_gem_selected_x, main_gem_selected_y, 6] > 0)
                          || (board_array_master[minor_gem_destination_to_x, minor_gem_destination_to_y, 7] > 0)))

            || (temp_direction == 5 && ((board_array_master[main_gem_selected_x, main_gem_selected_y, 7] > 0)
                            || (board_array_master[minor_gem_destination_to_x, minor_gem_destination_to_y, 6] > 0)))


            || (temp_direction == 6 && ((board_array_master[main_gem_selected_x, main_gem_selected_y, 8] > 0)
                             || (board_array_master[minor_gem_destination_to_x, minor_gem_destination_to_y, 9] > 0)))


            || (temp_direction == 7 && ((board_array_master[main_gem_selected_x, main_gem_selected_y, 9] > 0)
                             || (board_array_master[minor_gem_destination_to_x, minor_gem_destination_to_y, 8] > 0)))
            )
            return true;
        else
            return false;
    }

    bool Main_gem_explode()
    {
        if ((temp_direction == 4 && (board_array_master[main_gem_selected_x, main_gem_selected_y, 6] > 0))
                || (temp_direction == 5 && (board_array_master[main_gem_selected_x, main_gem_selected_y, 7] > 0))
                || (temp_direction == 6 && (board_array_master[main_gem_selected_x, main_gem_selected_y, 8] > 0))
                || (temp_direction == 7 && (board_array_master[main_gem_selected_x, main_gem_selected_y, 9] > 0))
                )
            return true;
        else
            return false;
    }

    bool Minor_gem_explode()
    {
        if ((temp_direction == 4 && (board_array_master[minor_gem_destination_to_x, minor_gem_destination_to_y, 7] > 0))
             || (temp_direction == 5 && (board_array_master[minor_gem_destination_to_x, minor_gem_destination_to_y, 6] > 0))
             || (temp_direction == 6 && (board_array_master[minor_gem_destination_to_x, minor_gem_destination_to_y, 9] > 0))
             || (temp_direction == 7 && (board_array_master[minor_gem_destination_to_x, minor_gem_destination_to_y, 8] > 0))
             )
            return true;
        else
            return false;
    }

    void Check_if_this_gem_have_the_same_color_of_the_gem_exploded_in_the_previous_move(int current_color)
    {
        if (player_turn)
        {
            if ((current_color == player_previous_exploded_color[0]) //if this gem have the same color of the gem exploded in the previous move
                || (current_color == player_previous_exploded_color[1]))
            {
                //Debug.Log("player explode same color with main gem");
                player_previous_exploded_color[0] = current_color;
                explode_same_color_again_with = 1; //main gem
                if (gain_turn_if_explode_same_color_of_previous_move)
                {
                    if ((!chain_turns_limit) || (current_player_chain_lenght < max_chain_turns))
                    {
                        //Debug.Log("chain_turns_limit: " + chain_turns_limit + " * current_player_chain_lenght: " + current_player_chain_lenght + " * max_chain_turns: " + max_chain_turns);
                        //Debug.Log("player gain a move with main gem - same color");
                        Gain_turns(move_gained_for_explode_same_color_in_two_adjacent_turn);
                    }
                }
            }
            else
                player_previous_exploded_color[0] = current_color;
        }
        else //enemy turn
        {
            if ((current_color == enemy_previous_exploded_color[0]) //if this gem have the same color of the gem exploded in the previous move
                || (current_color == enemy_previous_exploded_color[1]))
            {
                //Debug.Log("enemy explode same color with main gem");
                enemy_previous_exploded_color[0] = current_color;
                explode_same_color_again_with = 1; //main gem
                if (gain_turn_if_explode_same_color_of_previous_move)
                {
                    if ((!chain_turns_limit) || (current_enemy_chain_lenght < max_chain_turns))
                    {
                        //Debug.Log("chain_turns_limit: " + chain_turns_limit + " * current_enemy_chain_lenght: " + current_enemy_chain_lenght + " * max_chain_turns: " + max_chain_turns);
                        //Debug.Log("enemy gain a move with main gem  - same color");
                        Gain_turns(move_gained_for_explode_same_color_in_two_adjacent_turn);
                    }
                }
            }
            else
                enemy_previous_exploded_color[0] = current_color;
        }
    }

    void Detect_which_gems_will_explode(int __x, int __y, int _gem)//call from Switch_gem
    {
        //_gem 0 = main gem 
        //_gem 1 = minor gem 

        //vertical check
        //2 up and down
        if (((__y - 1) >= 0) && ((__y + 1) < _Y_tiles))
        {
            if ((board_array_master[__x, __y, 1] == board_array_master[__x, __y - 1, 1]) && (board_array_master[__x, __y, 1] == board_array_master[__x, __y + 1, 1]))
            {

                position_of_gems_that_will_explode[_gem, 1] = true;
                Annotate_explosions(__x, __y + 1, ExplosionCause.switchingGems);


                position_of_gems_that_will_explode[_gem, 5] = true;
                Annotate_explosions(__x, __y - 1, ExplosionCause.switchingGems);

                //if there is one more down
                if (((__y - 2) >= 0) && (board_array_master[__x, __y, 1] == board_array_master[__x, __y - 2, 1]))
                {
                    position_of_gems_that_will_explode[_gem, 4] = true;
                    Annotate_explosions(__x, __y - 2, ExplosionCause.switchingGems);
                }

                //if there is one more up
                if (((__y + 2) < _Y_tiles) && (board_array_master[__x, __y, 1] == board_array_master[__x, __y + 2, 1]))
                {
                    position_of_gems_that_will_explode[_gem, 0] = true;
                    Annotate_explosions(__x, __y + 2, ExplosionCause.switchingGems);
                }

            }
        }
        //2 up
        if ((__y + 2) < _Y_tiles)
        {
            if ((board_array_master[__x, __y, 1] == board_array_master[__x, __y + 2, 1]) && (board_array_master[__x, __y, 1] == board_array_master[__x, __y + 1, 1]))
            {

                position_of_gems_that_will_explode[_gem, 0] = true;
                Annotate_explosions(__x, __y + 2, ExplosionCause.switchingGems);


                position_of_gems_that_will_explode[_gem, 1] = true;
                Annotate_explosions(__x, __y + 1, ExplosionCause.switchingGems);

            }
        }
        //2 down
        if ((__y - 2) >= 0)
        {
            if ((board_array_master[__x, __y, 1] == board_array_master[__x, __y - 2, 1]) && (board_array_master[__x, __y, 1] == board_array_master[__x, __y - 1, 1]))
            {

                position_of_gems_that_will_explode[_gem, 5] = true;
                Annotate_explosions(__x, __y - 1, ExplosionCause.switchingGems);

                position_of_gems_that_will_explode[_gem, 4] = true;
                Annotate_explosions(__x, __y - 2, ExplosionCause.switchingGems);

            }
        }

        //horizontal check
        //2 right and left
        if (((__x - 1) >= 0) && ((__x + 1) < _X_tiles))
        {
            if ((board_array_master[__x, __y, 1] == board_array_master[__x - 1, __y, 1]) && (board_array_master[__x, __y, 1] == board_array_master[__x + 1, __y, 1]))
            {

                position_of_gems_that_will_explode[_gem, 3] = true;
                Annotate_explosions(__x + 1, __y, ExplosionCause.switchingGems);

                position_of_gems_that_will_explode[_gem, 7] = true;
                Annotate_explosions(__x - 1, __y, ExplosionCause.switchingGems);

                //if there is one more at left
                if (((__x - 2) >= 0) && (board_array_master[__x, __y, 1] == board_array_master[__x - 2, __y, 1]))
                {
                    position_of_gems_that_will_explode[_gem, 6] = true;
                    Annotate_explosions(__x - 2, __y, ExplosionCause.switchingGems);
                }
                //if there is one more at right
                if (((__x + 2) < _X_tiles) && (board_array_master[__x, __y, 1] == board_array_master[__x + 2, __y, 1]))
                {
                    position_of_gems_that_will_explode[_gem, 2] = true;
                    Annotate_explosions(__x + 2, __y, ExplosionCause.switchingGems);
                }
            }
        }
        //2 right
        if ((__x + 2) < _X_tiles)
        {
            if ((board_array_master[__x, __y, 1] == board_array_master[__x + 2, __y, 1]) && (board_array_master[__x, __y, 1] == board_array_master[__x + 1, __y, 1]))
            {

                position_of_gems_that_will_explode[_gem, 3] = true;
                Annotate_explosions(__x + 1, __y, ExplosionCause.switchingGems);

                position_of_gems_that_will_explode[_gem, 2] = true;
                Annotate_explosions(__x + 2, __y, ExplosionCause.switchingGems);

            }
        }
        //2 left
        if ((__x - 2) >= 0)
        {
            if ((board_array_master[__x, __y, 1] == board_array_master[__x - 2, __y, 1]) && (board_array_master[__x, __y, 1] == board_array_master[__x - 1, __y, 1]))
            {

                position_of_gems_that_will_explode[_gem, 7] = true;
                Annotate_explosions(__x - 1, __y, ExplosionCause.switchingGems);

                position_of_gems_that_will_explode[_gem, 6] = true;
                Annotate_explosions(__x - 2, __y, ExplosionCause.switchingGems);

            }
        }



        //explode the gem moved
        if (_gem == 0) //main gem
        {
            //calculate score
            for (int i = 0; i < 8; i++)
            {
                if (position_of_gems_that_will_explode[_gem, i])
                    n_gems_exploded_with_main_gem++;
            }
            Annotate_explosions(__x, __y, ExplosionCause.switchingGems);
        }
        if (_gem == 1) //minor gem
        {
            //calculate score
            for (int i = 0; i < 8; i++)
            {
                if (position_of_gems_that_will_explode[_gem, i])
                    n_gems_exploded_with_minor_gem++;
            }
            Annotate_explosions(__x, __y, ExplosionCause.switchingGems);
        }

        //if this is a big explosion
        if ((n_gems_exploded_with_main_gem > 3) || (n_gems_exploded_with_minor_gem > 3))
        {
            if (gain_turn_if_explode_more_than_3_gems)
            {
                if (player_turn)
                {
                    if ((!chain_turns_limit) || (current_player_chain_lenght < max_chain_turns))
                    {
                        //Debug.Log("chain_turns_limit: " + chain_turns_limit + " * current_player_chain_lenght: " + current_player_chain_lenght + " * max_chain_turns: " + max_chain_turns);
                        if ((n_gems_exploded_with_main_gem == 4) || (n_gems_exploded_with_minor_gem == 4))
                            Gain_turns(move_gained_when_explode_more_than_3_gems[0]);
                        else if ((n_gems_exploded_with_main_gem == 5) || (n_gems_exploded_with_minor_gem == 5))
                            Gain_turns(move_gained_when_explode_more_than_3_gems[1]);
                        else if ((n_gems_exploded_with_main_gem == 6) || (n_gems_exploded_with_minor_gem == 6))
                            Gain_turns(move_gained_when_explode_more_than_3_gems[2]);
                        else if ((n_gems_exploded_with_main_gem == 7) || (n_gems_exploded_with_minor_gem == 7))
                            Gain_turns(move_gained_when_explode_more_than_3_gems[3]);

                    }

                }
                else //enemy turn
                {
                    if ((!chain_turns_limit) || (current_enemy_chain_lenght < max_chain_turns))
                    {
                        //Debug.Log("chain_turns_limit: " + chain_turns_limit + " * current_enemy_chain_lenght: " + current_enemy_chain_lenght + " * max_chain_turns: " + max_chain_turns);
                        if ((n_gems_exploded_with_main_gem == 4) || (n_gems_exploded_with_minor_gem == 4))
                            Gain_turns(move_gained_when_explode_more_than_3_gems[0]);
                        else if ((n_gems_exploded_with_main_gem == 5) || (n_gems_exploded_with_minor_gem == 5))
                            Gain_turns(move_gained_when_explode_more_than_3_gems[1]);
                        else if ((n_gems_exploded_with_main_gem == 6) || (n_gems_exploded_with_minor_gem == 6))
                            Gain_turns(move_gained_when_explode_more_than_3_gems[2]);
                        else if ((n_gems_exploded_with_main_gem == 7) || (n_gems_exploded_with_minor_gem == 7))
                            Gain_turns(move_gained_when_explode_more_than_3_gems[3]);

                    }
                }

            }



            if (gem_explosion_fx_rule_selected == gem_explosion_fx_rule.only_for_big_explosion)
            {
                if ((_gem == 0) && (n_gems_exploded_with_main_gem > 3)) //check if main gem will become a bonus
                {
                    tiles_array[minor_gem_destination_to_x, minor_gem_destination_to_y].use_fx_big_explosion_here = n_gems_exploded_with_main_gem;
                }
                if ((_gem == 1) && (n_gems_exploded_with_minor_gem > 3)) //check if minor gem will become a bonus
                {
                    tiles_array[main_gem_selected_x, main_gem_selected_y].use_fx_big_explosion_here = n_gems_exploded_with_minor_gem;
                }
            }

        }

    }

    public void Update_turn_order_after_a_bad_move()
    {
        //print("++ Board_C.Update_turn_order_after_a_bad_move main_gem_selected_x = " + main_gem_selected_x + " main_gem_selected_y = " + main_gem_selected_y);
        //print("++ Board_C.Update_turn_order_after_a_bad_move minor_gem_destination_to_x = " + minor_gem_destination_to_x + " minor_gem_destination_to_y = " + minor_gem_destination_to_y);

        //print("Update_turn_order_after_a_bad_move()");

                current_player_chain_lenght = 0;
                player_can_move = true;

        current_moveStatus = moveStatus.waitingNewMove;
    }




    void Move_gems_to_target_positions() //call from Switch_gem( or from Update_board_by_one_step(
    {
        //Debug.Log("Move_gems_to_target_positions()");
        int temp_main_bonus = 0;
        int temp_minor_bonus = 0;


        //update color in array
        board_array_master[main_gem_selected_x, main_gem_selected_y, 1] = minor_gem_color;
        board_array_master[minor_gem_destination_to_x, minor_gem_destination_to_y, 1] = main_gem_color;

        //update bonus position
        temp_main_bonus = board_array_master[main_gem_selected_x, main_gem_selected_y, 4];
        temp_minor_bonus = board_array_master[minor_gem_destination_to_x, minor_gem_destination_to_y, 4];
        board_array_master[main_gem_selected_x, main_gem_selected_y, 4] = temp_minor_bonus;
        board_array_master[minor_gem_destination_to_x, minor_gem_destination_to_y, 4] = temp_main_bonus;

        //update gem representation
        tile_C tile_script_main_gem = tiles_array[main_gem_selected_x, main_gem_selected_y];
        tile_C tile_script_minor_gem = tiles_array[minor_gem_destination_to_x, minor_gem_destination_to_y];


        tile_script_main_gem.my_gem = avatar_minor_gem;

        tile_script_minor_gem.my_gem = avatar_main_gem;


        SetSwitchGemAnimation(tile_script_main_gem, tile_script_minor_gem);
    }

    void SetSwitchGemAnimation(tile_C main_tile, tile_C minor_tile)
    {
        isSwitchGemAnimation = true;
        tileMain = main_tile;
        tileMinor = minor_tile;

        displacementMain = main_tile.transform.position - main_tile.my_gem.transform.position;
        displacementMinor = minor_tile.transform.position - minor_tile.my_gem.transform.position;

        directionMain = displacementMain.normalized;
        directionMinor = displacementMinor.normalized;
    }

    public void Switch_gem_animation()
    {

        if (Vector3.Angle(directionMain, displacementMain.normalized) < 1.0f && displacementMain.sqrMagnitude > accuracySquare)
        {

            tileMain.my_gem.transform.Translate(directionMain * switch_speed * Time.deltaTime, Space.World);
            tileMinor.my_gem.transform.Translate(directionMinor * switch_speed * Time.deltaTime, Space.World);

            displacementMain = tileMain.transform.position - tileMain.my_gem.transform.position;
            displacementMinor = tileMinor.transform.position - tileMinor.my_gem.transform.position;
        }
        else
        {
            isSwitchGemAnimation = false;
            tileMain.my_gem.transform.position = tileMain.transform.position;
            tileMinor.my_gem.transform.position = tileMinor.transform.position;

            AfterSwitchAnim();
        }
    }

    void Empty_main_and_minor_gem_selections()
    {
        //Debug.LogWarning("Empty_main_and_minor_gem_selections() " + call_from + " ... " + minor_gem_destination_to_x);
        //NEW CODE TO TEST:
        /*
        if (main_gem_selected_x != -10)
        {
            tiles_array[minor_gem_destination_to_x, minor_gem_destination_to_y].GetComponent<tile_C>().use_fx_big_explosion_here = 0;
		tiles_array[main_gem_selected_x, main_gem_selected_y].GetComponent<tile_C>().use_fx_big_explosion_here = 0;
        }*/

        if (board_array_master[main_gem_selected_x, main_gem_selected_y, 11] == 6)//if not exploded
        {
            board_array_master[main_gem_selected_x, main_gem_selected_y, 11] = 0;
            tiles_array[main_gem_selected_x, main_gem_selected_y].my_gem_renderer = tiles_array[main_gem_selected_x, main_gem_selected_y].my_gem.GetComponent<SpriteRenderer>();
        }

        if (board_array_master[minor_gem_destination_to_x, minor_gem_destination_to_y, 11] == 6)//if not exploded
        {
            board_array_master[minor_gem_destination_to_x, minor_gem_destination_to_y, 11] = 0;
            tiles_array[minor_gem_destination_to_x, minor_gem_destination_to_y].my_gem_renderer = tiles_array[main_gem_selected_x, main_gem_selected_y].my_gem.GetComponent<SpriteRenderer>();
        }

        //Debug.LogWarning("Empty_main_and_minor_gem_selections() " + call_from);// + player_can_move);
        main_gem_selected_x = -10;
        main_gem_selected_y = -10;
        avatar_main_gem = null;
        main_gem_color = -10;
        minor_gem_destination_to_x = -10;
        minor_gem_destination_to_y = -10;
        avatar_minor_gem = null;
        minor_gem_color = -10;

        current_moveStatus = moveStatus.waitingNewMove;
    }


    public void SetStartBadSwitchAnimation()
    {
        isStartBadSwitchAnimation = true;
        tileMinor = tiles_array[minor_gem_destination_to_x, minor_gem_destination_to_y];
        tileMain = tiles_array[main_gem_selected_x, main_gem_selected_y];

        displacementMain = tileMinor.my_gem.transform.position - avatar_main_gem.transform.position;
        displacementMinor = tileMain.my_gem.transform.position - avatar_minor_gem.transform.position;

        directionMain = displacementMain.normalized;
        directionMinor = displacementMinor.normalized;
    }

    public void SetEndBadSwitchAnimation()
    {
        isEndBadSwitchAnimation = true;
        tileMain = tiles_array[main_gem_selected_x, main_gem_selected_y];
        tileMinor = tiles_array[minor_gem_destination_to_x, minor_gem_destination_to_y];

        displacementMain = tileMain.my_gem.transform.position - avatar_main_gem.transform.position;
        displacementMinor = tileMinor.my_gem.transform.position - avatar_minor_gem.transform.position;

        directionMain = displacementMain.normalized;
        directionMinor = displacementMinor.normalized;
    }


    void Start_bad_switch_animation()
    {

        //Debug.Log("Start_bad_switch_animation() " + player_can_move);
        displacementMain = tileMinor.my_gem.transform.position - avatar_main_gem.transform.position;

        if (Vector3.Angle(directionMain, displacementMain.normalized) < 1.0f && displacementMain.sqrMagnitude > accuracySquare)
        {
            avatar_main_gem.transform.Translate(directionMain * falling_speed * Time.deltaTime, Space.World);
            avatar_minor_gem.transform.Translate(directionMinor * falling_speed * Time.deltaTime, Space.World);
        }
        else
        {
            isStartBadSwitchAnimation = false;
            avatar_main_gem.transform.position = tileMinor.transform.position;

            avatar_minor_gem.transform.position = tileMain.transform.position;

#if UNITY_WEBGL
            Play_sfx(bad_move_sfx_str);
#else
            Play_sfx(bad_move_sfx);
#endif
            SetEndBadSwitchAnimation();

        }
    }



    void End_bad_switch_animation()
    {
        //print("++ Board_C.End_bad_switch_animation main_gem_selected_x = " + main_gem_selected_x + " main_gem_selected_y = " + main_gem_selected_y);
        //print("++ Board_C.End_bad_switch_animation minor_gem_destination_to_x = " + minor_gem_destination_to_x + " minor_gem_destination_to_y = " + minor_gem_destination_to_y);
        displacementMain = tileMain.my_gem.transform.position - avatar_main_gem.transform.position;

        if (Vector3.Angle(directionMain, displacementMain.normalized) < 1.0f && displacementMain.sqrMagnitude > accuracySquare)
        {
            avatar_main_gem.transform.Translate(directionMain * switch_speed * Time.deltaTime, Space.World);
            avatar_minor_gem.transform.Translate(directionMinor * switch_speed * Time.deltaTime, Space.World);

            displacementMain = tileMain.my_gem.transform.position - avatar_main_gem.transform.position;
            displacementMinor = tileMinor.my_gem.transform.position - avatar_minor_gem.transform.position;
        }
        else
        {
            isEndBadSwitchAnimation = false;
            avatar_main_gem.transform.position = tileMain.transform.position;
            tileMain.my_gem = avatar_main_gem;
            tileMain.my_gem_renderer = tileMain.my_gem.GetComponent<SpriteRenderer>();
            int tempGem = board_array_master[main_gem_selected_x, main_gem_selected_y, 1];
            board_array_master[main_gem_selected_x,main_gem_selected_y, 1] = board_array_master[minor_gem_destination_to_x, minor_gem_destination_to_y, 1];

            avatar_minor_gem.transform.position = tileMinor.transform.position;
            tileMinor.my_gem = avatar_minor_gem;
            tileMinor.my_gem_renderer = tileMain.my_gem.GetComponent<SpriteRenderer>();
            board_array_master[minor_gem_destination_to_x, minor_gem_destination_to_y, 1] = tempGem;

            Empty_main_and_minor_gem_selections();
            Update_turn_order_after_a_bad_move();
            //Debug.Log("End_bad_switch_animation() " + player_can_move);
            temp_direction = -1;
        }

    }

}
