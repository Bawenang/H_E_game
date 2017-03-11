using UnityEngine;
using System.Collections;

public partial class Board_C : MonoBehaviour {

    int current_turn = 0;
    void UpdateTurn()//call from Check_ALL_possible_moves(), Update_turn_order_after_a_bad_move(), Player_lose()
    {
        //print("UpdateTurn()");

        //Debug.Log("number_of_bonus_on_board: " + number_of_bonus_on_board);
        //reset variables of the previous move
        n_combo = 0;
        number_of_padlocks_involved_in_explosion = 0;
        number_of_elements_to_damage = 0;
        number_of_elements_to_damage_with_bonus = 0;
        number_of_elements_to_damage_with_SwitchingGems = 0;
        number_of_gems_to_move = 0;
        number_of_new_gems_to_create = 0;
        gems_useful_moved = 0;
        all_explosions_are_completed = false;


        if (!stage_started)
        {
            start_time = Time.timeSinceLevelLoad;
            time_duration = start_time;
            stage_started = true;
        }
        if (!game_end)
        {
            //ListOfPotentialMoves();

            current_turn++;

            temp_direction = -1;

            //Debug.Log("********** turn " + current_turn);
            current_moveStatus = Board_C.moveStatus.waitingNewMove;
            if (!versus)//if player play alone
            {
                //Debug.Log("Player's turn - not versus");
                if (turn_gained)
                {
                    turn_gained = false;
                    current_player_chain_lenght++;
                }
                else
                    current_player_chain_lenght = 0;
                //Debug.Log("chain_turns_limit: " + chain_turns_limit + " * current_player_chain_lenght: " + current_player_chain_lenght + " * max_chain_turns: " + max_chain_turns);

                player_turn = true;
                player_can_move = true;

                if (show_hint)
                {
                    use_hint = true;
                    Invoke("Show_hint", show_hint_after_n_seconds);
                }
            }
            else //if player play against AI
            {
                if (turn_gained)
                {
                    turn_gained = false;
                    if (player_turn)
                        current_player_chain_lenght++;
                    else
                        current_enemy_chain_lenght++;

                    //Debug.Log("chain_turns_limit: " + chain_turns_limit + " * current_player_chain_lenght: " + current_player_chain_lenght + " * max_chain_turns: " + max_chain_turns);
                    //Debug.Log("chain_turns_limit: " + chain_turns_limit + " * current_enemy_chain_lenght: " + current_enemy_chain_lenght + " * max_chain_turns: " + max_chain_turns);

                }
                else
                {
                    if (player_turn)//if the previous was of the player
                    {
                        player_turn = false;// now is enemy turn
                        current_player_chain_lenght = 0;
                    }
                    else
                    {
                        player_turn = true;//else is palyer turn
                        current_enemy_chain_lenght = 0;
                    }
                }

                if (player_turn)
                {
                    //Debug.Log("Player's turn");
                    player_can_move = true;

                    if (show_hint)
                    {
                        use_hint = true;
                        Invoke("Show_hint", show_hint_after_n_seconds);
                    }
                }
            }
        }
        else
        {
            Game_end();
        }
    }





    public void Start_update_board()//call from: tile_C.update.If_all_explosion_are_completed(), tile_C.update.Check_if_gem_movements_are_all_done()
    {
            if (needTochangeWeatherIcons)
            {
                for (int i = 0; i < _X_tiles; ++i)
                {
                    for (int j = 0; j < _Y_tiles; ++j)
                    {
                        if (board_array_master[i, j, 1] == 2)
                        {
                            tiles_array[i, j].my_gem_renderer.sprite = gem_colors[2];
                        }
                    }
                }

                needTochangeWeatherIcons = false;
            }

        //Debug.Log ("Start_update_board ");

        //if (!player_can_move_when_gem_falling)
        cursor.gameObject.SetActive(false);

        if (diagonal_falling)
        {
            //print("diagonal falling");
            Check_new_gem_to_generate();

            //check vertical falling
            for (int y = 0; y < _Y_tiles - 1; y++)//don't check last line
            {
                for (int x = 0; x < _X_tiles; x++)
                {
                    if ((board_array_master[x, y, 13] == 0) && (board_array_master[x, y, 10] == 1))//this could be fall
                    {
                        //check if have an empty tile under
                        if ((board_array_master[x, y + 1, 0] > -1) && (board_array_master[x, y + 1, 1] == -99))
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
            //check diagonal falling
            diagonal_falling_preference_direction_R = !diagonal_falling_preference_direction_R;
            for (int y = 0; y < _Y_tiles - 1; y++)//don't check last line
            {
                for (int x = 0; x < _X_tiles; x++)
                {
                    if ((board_array_master[x, y, 13] == 0) && (board_array_master[x, y, 10] == 1))//this could be fall
                    {
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


            //now you know what gems must be fall, so...
            Update_board_by_one_step();

        }
        else //diagonal falling not allowed
        {
            //print("diagonal falling not allowed");
            //read board form down to up (and left to right)
            for (int y = _Y_tiles - 1; y >= 0; y--)
            {
                for (int x = 0; x < _X_tiles; x++)
                {
                    if ((board_array_master[x, y, 0] > -1) && (board_array_master[x, y, 13] == 0))//if there is a tile unchecked...
                    {
                        //...and it is empty
                        if (board_array_master[x, y, 1] == -99)
                        {
                            tile_C tile_script = (tile_C)tiles_array[x, y];
                            tile_script.Count_how_much_gem_there_are_to_move_over_me();
                        }
                    }
                }
            }
            //now you know what gems must be fall, so...
            Start_gem_fall();
        }

    }

    void Start_gem_fall()//call from "board_C.turnMovement.Start_update_board()"
    {
        if ((number_of_gems_to_move > 0) || (number_of_new_gems_to_create > 0))
        {
            gem_falling_ongoing = true;

            for (int y = _Y_tiles - 1; y >= 0; y--)
            {
                for (int x = 0; x < _X_tiles; x++)
                {
                    if (board_array_master[x, y, 0] > -1)//if there is a tile
                    {
                        //search free tiles
                        if ((board_array_master[x, y, 1] == -99)  //if this tile is empty
                             && (board_array_master[x, y, 13] <= 0)) //and not yet checked
                        {
                            tile_C tile_script = (tile_C)tiles_array[x, y];
                            tile_script.Make_fall_all_free_gems_over_this_empty_tile();
                        }
                    }
                }
            }

        }
        else
        {
            if (!gem_falling_ongoing)
                Check_ALL_possible_moves();
        }
    }

}
