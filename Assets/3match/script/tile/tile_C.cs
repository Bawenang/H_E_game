using UnityEngine;
using System.Collections;

public partial class tile_C : MonoBehaviour
{

    public Board_C board;
    public explosion_score explosion_score_script;
    public int use_fx_big_explosion_here;

    //bonus fx limits
    int fx_end_r;
    int fx_end_l;
    int fx_end_up;
    int fx_end_down;

    public int after_explosion_I_will_become_this_bonus; // 0 = no bonus

    public int _x;
    public int _y;
    int move_direction;

    int[] two_last_colors_created = { 0, 1 };//this avoid to create 3 gems with the same color
    int random_color;

    private GameObject _my_gem;

    public GameObject my_gem
    {
        get
        {
            //if (_my_gem == null)
                //Debug.Log("=================================== GETTING NULL MY_GEM!!!!!!!!!!!!");
            return _my_gem;
        }

        set
        {
            //if (value == null)
            //    Debug.Log("=================================== SETTING NULL MY_GEM!!!!!!!!!!!!");
            //else if (_my_gem == null)
            //    Debug.Log("=================================== SETTING MY_GEM NOT NULL FROM NULL!!!!!!!!!!!!");
            _my_gem = value;
        }
    }

    public GameObject my_padlock;
    public SpriteRenderer my_gem_renderer;

    /* if you have the menu kit, DELETE THIS LINE
	game_master my_game_master;
	// if you have the menu kit, DELETE THIS LINE*/

    void Start()
    {

    }

    public void Debug_show_available_moves(int show_this_move)
    {
        if (show_this_move == 3)
            GetComponent<Renderer>().material.color = Color.gray;
        else if (show_this_move > 3)
            GetComponent<Renderer>().material.color = Color.black;
        else
            GetComponent<Renderer>().material.color = Color.white;

    }

    public void Debug_show_my_color()
    {
        if (board.board_array_master[_x, _y, 1] == -99)
            GetComponent<Renderer>().material.color = Color.gray;
        else if (board.board_array_master[_x, _y, 1] == 0)
            GetComponent<Renderer>().material.color = Color.red;
        else if (board.board_array_master[_x, _y, 1] == 1)
            GetComponent<Renderer>().material.color = Color.cyan;
        else if (board.board_array_master[_x, _y, 1] == 2)
            GetComponent<Renderer>().material.color = Color.magenta;
        else if (board.board_array_master[_x, _y, 1] == 3)
            GetComponent<Renderer>().material.color = Color.yellow;
    }

    #region interaction

    public void MyOnMouseDown()
    {
        if (board.game_end)
            return;
        //Debug.Log("++++ tile_C.MyOnMouseDown");

        board.touch_number++;
        //if (board.touch_number == 1)
        {
            board.cursor.position = this.transform.position;

            /*
            Debug.Log( "** 5 = " + board.board_array_master[_x, _y, 5]
                + "** 6 down = " + board.board_array_master[_x, _y, 6]
                + "** 7 up = " + board.board_array_master[_x, _y, 7]
                + "** 8 right = " + board.board_array_master[_x, _y, 8]
                + "** 9 left = " + board.board_array_master[_x, _y, 9]
                ); */
            //Debug.Log("state " + board.board_array_master[_x, _y, 11] + " - check? " + board.board_array_master[_x, _y, 13]);
            /*
            Debug.Log("kind " + board.board_array_master[_x, _y, 1]
                     + " useful moves " + board.board_array_master[_x, _y, 5]);

            
			Debug.Log(
				"tile hp " + board.board_array_master[_x,_y,0]
				+ "kind " + board.board_array_master[_x,_y,1]
				+ "block hp " + board.board_array_master[_x,_y,14]
				+ "** 5 = " + board.board_array_master[_x,_y,5]
				+ "** 6 down = " + board.board_array_master[_x,_y,6]
				+ "** 7 up = " + board.board_array_master[_x,_y,7]
				+ "** 8 right = " + board.board_array_master[_x,_y,8]
				+ "** 9 left = " + board.board_array_master[_x,_y,9]
				);*/

            /*
				Board_C debug = (Board_C)boardthis_board.GetComponent("Board_C");
				if (debug.debug)//change gem color when click over it
				{
					if (boardarray_master[_x,_y,1] +1 == board.gem_length)
						boardarray_master[_x,_y,1]  = 0;
					else
						boardarray_master[_x,_y,1] ++;

					SpriteRenderer sprite_gem = my_gem.GetComponent<SpriteRenderer>();
						sprite_gem.sprite = board.gem_colors[boardarray_master[_x,_y,1]];


				}
				else
				{
				Debug.Log(_x + "," + _y 
				          + "** 1 = " + boardarray_master[_x,_y,1] 
				          + "** 2 = " + boardarray_master[_x,_y,2]
				          + "** 3 = " + boardarray_master[_x,_y,3]
				          + "** 4 = " + boardarray_master[_x,_y,4]
				          + "** 5 = " + boardarray_master[_x,_y,5]
				          + "** 6 = " + boardarray_master[_x,_y,6]
				          + "** 7 = " + boardarray_master[_x,_y,7]
				          + "** 8 = " + boardarray_master[_x,_y,8]
				          + "** 9 = " + boardarray_master[_x,_y,9]
				          + "** 10 = " + boardarray_master[_x,_y,10]
				          + "** 11 = " + boardarray_master[_x,_y,11]
				          + "** 12 = " + boardarray_master[_x,_y,12]
				          + "** 13 = " + boardarray_master[_x,_y,13]
				          + "** 14 = " + boardarray_master[_x,_y,14]
				          + "** 15 = " + boardarray_master[_x,_y,15]
				          + "** 16 = " + boardarray_master[_x,_y,16]
				          + "** 17 = " + boardarray_master[_x,_y,17]
				          );
				}*/

            if (board.board_array_master[_x, _y, 11] != 0)
                return;

            if (board.player_turn && (board.player_can_move || board.player_can_move_when_gem_falling))
            {
#if UNITY_WEBGL
                board.Play_sfx(board.click_sfx_str);
#else
                board.Play_sfx(board.click_sfx);
#endif

                board.n_combo = 0;//interact break the combo

                    if ((board.board_array_master[_x, _y, 1] >= 0) && (board.board_array_master[_x, _y, 1] <= 9) //if this is a gem
                            && (board.board_array_master[_x, _y, 3] == 0)) //no padlock
                    {


                        if ((board.trigger_by_select == Board_C.trigger_by.click) && (board.board_array_master[_x, _y, 1] == 9) && (board.board_array_master[_x, _y, 4] > 0))// click on a clickable bonus
                        {
                            //Debug.Log("click bonus");
                            Cancell_old_selection();
                            
                            Trigger_bonus(true);

                            return;
                        }

                        if (board.main_gem_selected_x < 0)//if none gem is selected
                        {
                            I_become_main_gem();
                        }
                        else if ((board.main_gem_selected_x == _x) && (board.main_gem_selected_y == _y))
                        {
                            //you have click on the gem already selected
                        }
                        else if ((board.main_gem_selected_x - 1 == _x) && (board.main_gem_selected_y == _y))
                        {
                            //click on the gem next left of the main gem
                            I_become_minor_gem(7);
                        }
                        else if ((board.main_gem_selected_x + 1 == _x) && (board.main_gem_selected_y == _y))
                        {
                            //click on the gem next right of main gem
                            I_become_minor_gem(6);
                        }
                        else if ((board.main_gem_selected_x == _x) && (board.main_gem_selected_y - 1 == _y))
                        {
                            //click on the gem next up main gem
                            I_become_minor_gem(5);
                        }
                        else if ((board.main_gem_selected_x == _x) && (board.main_gem_selected_y + 1 == _y))
                        {
                            //click on the gem next down main gem
                            I_become_minor_gem(4);
                        }
                        else //click on a gem not adjacent the gem already selected, so this gem will be the new main gem
                        {
                            I_become_main_gem();
                        }
                    }
                    else
                        Cancell_old_selection();
                }
                else if (board.player_can_move && board.current_moveStatus == Board_C.moveStatus.waitingNewMove)
                {
                    Cancell_old_selection();
                    //Try_to_use_bonus_on_this_tile();
                }
            }
        //Debug.Log("---- tile_C.MyOnMouseDown");
    }

    public void MyOnMouseEnter()
    {
        if (board.player_turn && (board.player_can_move || board.player_can_move_when_gem_falling))
        {
            if (board.touch_number == 0)
            {
                board.cursor.position = this.transform.position;
                board.cursor.gameObject.SetActive(true);
            }

        }

        if (board.touch_number == 1)
            Gem_drag();

    }

    public void MyOnMouseExit()
    {
        if (board.player_turn && (board.player_can_move || board.player_can_move_when_gem_falling))
        {
            if (board.touch_number == 0)
                board.cursor.gameObject.SetActive(false);
        }

    }


    void Cancell_old_selection()
    {
        //Debug.Log("Cancell_old_selection");
            board.main_gem_selected_x = -10;
            board.main_gem_selected_y = -10;
            board.main_gem_color = -10;
            board.minor_gem_destination_to_x = -10;
            board.minor_gem_destination_to_y = -10;
            board.minor_gem_color = -10;
    }


    void OnMouseUp()
    {
        board.touch_number--;

    }

    void OnMouseEnter()
    {
        if (!board.gameController.pause && board.player_turn && board.player_can_move)
        //|| board.player_can_move_when_gem_falling)
        {
            if (board.touch_number == 0)
            {
                board.cursor.position = this.transform.position;
                board.cursor.gameObject.SetActive(true);
            }

        }

        if (board.touch_number == 1)
            Gem_drag();

    }

    void OnMouseExit()
    {
        if (board.player_turn && board.player_can_move)
        //|| board.player_can_move_when_gem_falling)
        {
            if (board.touch_number == 0)
                board.cursor.gameObject.SetActive(false);
        }

    }

    void Gem_drag()
    {
        if (board.touch_number == 1 )
        {
            if (board.avatar_main_gem && board.player_can_move)
            {
                //Debug.Log("Gem_drag() " + board.player_can_move);
                if ((board.board_array_master[_x, _y, 1] >= 0) && (board.board_array_master[_x, _y, 1] <= 9) //it is a gem
                    && (board.board_array_master[_x, _y, 3] == 0)) //and it is free
                {
                    if ((board.main_gem_selected_x == _x + 1) && (board.main_gem_selected_y == _y)) //move left
                        I_become_minor_gem(7);
                    else if ((board.main_gem_selected_x == _x - 1) && (board.main_gem_selected_y == _y)) //more right
                        I_become_minor_gem(6);
                    else if ((board.main_gem_selected_y == _y + 1) && (board.main_gem_selected_x == _x)) //move up
                        I_become_minor_gem(5);
                    else if ((board.main_gem_selected_y == _y - 1) && (board.main_gem_selected_x == _x)) //move down
                        I_become_minor_gem(4);
                }
                else
                {
                    //Debug.Log("invalid drag");
                }
            }
        }
    }


    public void I_become_main_gem()
    {
        if (!board.player_can_move || board.current_moveStatus != Board_C.moveStatus.waitingNewMove)
        {
            //Debug.LogWarning("can't store - I_become_main_gem - now");
            return;
        }

        board.my_hint.position = board.tiles_array[_x, _y].transform.position;
        board.my_hint.gameObject.SetActive(true);

        for (int i = 0; i < 4; i++)
        {
            board.my_hint.GetChild(i).gameObject.SetActive(false);
        }

        //Debug.Log("I_become_main_gem() " + _x+","+_y + " board.player_can_move = " + board.player_can_move + " board.current_moveStatus = " + board.current_moveStatus);
        board.avatar_main_gem = my_gem;


        board.main_gem_selected_x = _x;
        board.main_gem_selected_y = _y;
        board.main_gem_color = board.board_array_master[_x, _y, 1];

        if (!board.player_turn)
            board.cursor.gameObject.SetActive(true);

        //empty old selection
        board.minor_gem_destination_to_x = -10;
        board.minor_gem_destination_to_y = -10;
        board.minor_gem_color = -10;
        board.avatar_minor_gem = null;
    }

    public void I_become_minor_gem(int direction)
    {
        if (!board.player_can_move || board.current_moveStatus != Board_C.moveStatus.waitingNewMove)
        {
            //Debug.LogWarning("can't store - I_become_minor_gem -  move now");
            return;
        }

        //Debug.Log("I_become_minor_gem() " + _x+","+_y + " board.player_can_move = " + board.player_can_move + " board.current_moveStatus = " + board.current_moveStatus);
        board.avatar_minor_gem = my_gem;
        board.minor_gem_destination_to_x = _x;
        board.minor_gem_destination_to_y = _y;
        board.minor_gem_color = board.board_array_master[_x, _y, 1];

       
        if (board.player_turn)
        {
            board.cursor.position = this.transform.position;

            //if (board.player_can_move_when_gem_falling)
                //{
                board.temp_direction = direction;
                board.current_moveStatus = Board_C.moveStatus.switchingGems;

                if (board.player_can_move && !board.player_can_move_when_gem_falling)
                    board.SwitchingGems();

                //}
                //else
                //board.Switch_gem(direction, "tile.I_become_minor_gem");
        }
        else
        {
            move_direction = direction;
        }
    }

    void Delay_switch()
    {
        board.cursor.position = this.transform.position;
        //board.Switch_gem(move_direction, "tile.I_become_minor_gem");
        board.temp_direction = move_direction;
        board.current_moveStatus = Board_C.moveStatus.switchingGems;
        if (!board.player_can_move_when_gem_falling)
            board.SwitchingGems();
    }

#endregion



}
