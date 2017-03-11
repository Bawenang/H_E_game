﻿using UnityEngine;
using System.Collections;

public partial class tile_C : MonoBehaviour{



    public void Count_how_much_gem_there_are_to_move_over_me()
    {


        int empty_tiles = 0;
        for (int n_tiles = 0; (_y - n_tiles >= 0); n_tiles++)
        {
            //interrupt the count if
            if ((board.board_array_master[_x, _y - n_tiles, 0] == -1)// no tile 
                || ((board.board_array_master[_x, _y - n_tiles, 1] >= 0) && (board.board_array_master[_x, _y - n_tiles, 10] == 0)))//or this thing can't fall
                break;


            //annotate this tile as checked
            board.board_array_master[_x, _y - n_tiles, 13]--;


            if (board.board_array_master[_x, _y - n_tiles, 10] == 1) //if there is something to move
            {
                //this gem must be fall
                board.number_of_gems_to_move++;
            }
            else if (board.board_array_master[_x, _y - n_tiles, 1] == -99) //this tile is empty
            {
                empty_tiles++;
            }

            if (board.gem_emitter_rule != Board_C.gem_emitter.off)
                {
                if (board.board_array_master[_x, _y - n_tiles, 12] > 0) //I reach the leader-tile
                    board.number_of_new_gems_to_create += empty_tiles;
                }
        }

    }


    public void Make_fall_all_free_gems_over_this_empty_tile()
    {


        bool leader_tile_become_empty = false;
        bool leader_tile_become_active = false;
        int number_of_gems_that_fall_in_this_column = 0;

        for (int yy = _y; yy >= 0; yy--)//look all tiles over me
        {

            //interrupt the count if
            if ((board.board_array_master[_x, yy, 0] == -1)// no tile 
                || (board.board_array_master[_x, yy, 1] >= 0) && (board.board_array_master[_x, yy, 10] == 0)) //or this thing can't fall
                break;
            else //vertical falling
            {
                board.board_array_master[_x, yy, 13] = 1; //annotate this tile as checked

                if (board.board_array_master[_x, yy, 10] == 1) //if there is something that can fall
                {

                    //update gem position
                    //gem color go in this tile
                    board.board_array_master[_x, _y - number_of_gems_that_fall_in_this_column, 1] = board.board_array_master[_x, yy, 1];
                    //cancell gem color from old starting tile
                    board.board_array_master[_x, yy, 1] = -99;
                    //pass special 
                    board.board_array_master[_x, _y - number_of_gems_that_fall_in_this_column, 4] = board.board_array_master[_x, yy, 4];
                    //cancel special 
                    board.board_array_master[_x, yy, 4] = 0;
                    //new gem position
                    board.board_array_master[_x, _y - number_of_gems_that_fall_in_this_column, 10] = 1;
                    //cancell gem from old starting tile
                    board.board_array_master[_x, yy, 10] = 0;
                    //update falling block hp
                    board.board_array_master[_x, _y - number_of_gems_that_fall_in_this_column, 14] = board.board_array_master[_x, yy, 14];
                    board.board_array_master[_x, yy, 14] = 0;


                    //now show the gem avatar fall
                    tile_C script_destination_tile = (tile_C)board.tiles_array[_x, (_y - number_of_gems_that_fall_in_this_column)];
                    tile_C script_origin_tile = (tile_C)board.tiles_array[_x, yy];
                    //link the avatar to new tile 
                    script_destination_tile.my_gem = script_origin_tile.my_gem;
                    script_destination_tile.my_gem_renderer = script_destination_tile.my_gem.GetComponent<SpriteRenderer>();
                    //unlik the avater from the old tile
                    script_origin_tile.my_gem = null;
                    //Debug.Log("Make_fall_all_free_gems_over_this_empty_tile"); 
                    //Debug.Log("Origin tile x = " + _x + ", y = " + yy);
                    //start the animation
                    script_destination_tile.Update_gem_position();


                    //update gem moved count
                    number_of_gems_that_fall_in_this_column++;

                    //if a leader tile become empty, create a new gem
                    if (board.board_array_master[_x, yy, 12] == 1)//if this i a leader tile
                    {
                        leader_tile_become_empty = true;
                        if (board.gem_emitter_rule != Board_C.gem_emitter.off)
                            script_origin_tile.SetCreateFallingGem();
                        else
                            {
                            board.number_of_new_gems_to_create = 0;
                            }
                    }


                }

                else if (!leader_tile_become_empty && !leader_tile_become_active)
                {
                    
                        if ((board.board_array_master[_x, yy, 1] == -99)  //this tile is empty
                        && (board.board_array_master[_x, yy, 12] == 1))//and is a leader tile
                            {
                            if (board.gem_emitter_rule != Board_C.gem_emitter.off)
                                {
                                tile_C script_native_tile = (tile_C)board.tiles_array[_x, yy];
                            script_native_tile.SetCreateFallingGem();
                            leader_tile_become_active = true;
                                }
                             else
                                {
                                board.number_of_new_gems_to_create = 0;
                                If_all_explosion_are_completed();
                                }
                            }
                       

                }
            }
        }
    }

}
