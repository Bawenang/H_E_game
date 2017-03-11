using UnityEngine;
using System.Collections;

public partial class Board_C : MonoBehaviour {

    void Check_new_gem_to_generate() //call from: Board_C.freeMovement.BoardUpdate(), Board_C.turnMovement.Start_update_board()
    {
        if (gem_emitter_rule == gem_emitter.off)
        {
            number_of_new_gems_to_create = 0;
            return;
        }

        for (int i = 0; i < number_of_tiles_leader; i++)
        {
            if (board_array_master[tiles_leader_array[i]._x, tiles_leader_array[i]._y, 1] == -99)//empty 
            {
                if (board_array_master[tiles_leader_array[i]._x, tiles_leader_array[i]._y, 11] == 111 || board_array_master[tiles_leader_array[i]._x, tiles_leader_array[i]._y, 11] == 222 || board_array_master[tiles_leader_array[i]._x, tiles_leader_array[i]._y, 11] == 333)//explosion or creation falling animation ongoing
                    continue;

                board_array_master[tiles_leader_array[i]._x, tiles_leader_array[i]._y, 11] = 2;//gem creation
                board_array_master[tiles_leader_array[i]._x, tiles_leader_array[i]._y, 13] = 1;
                number_of_new_gems_to_create++;


                //empty tile are reserved to this generated new gem (this is needed only when the explosion is a column on the top of the board, to prevet diagona falling from adiacent columns)
                for (int yy = tiles_leader_array[i]._y + 1; yy < _Y_tiles; yy++)
                {
                    if ((board_array_master[tiles_leader_array[i]._x, yy, 0] > -1) && (board_array_master[tiles_leader_array[i]._x, yy, 1] == -99))
                    {
                        board_array_master[tiles_leader_array[i]._x, yy, 13] = 1;
                        //tiles_array[tiles_leader_array[i]._x, yy].GetComponent<Renderer>().material.color = Color.yellow;
                    }
                    else
                        break;
                }
            }

        }

    }

}
