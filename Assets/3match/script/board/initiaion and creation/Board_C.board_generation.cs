using UnityEngine;
using System.Collections;
using System;

public partial class Board_C : MonoBehaviour {

    public TextAsset file_txt_asset;
    string fileContents = string.Empty;

    public tile_C[,] tiles_array;
    public SpriteRenderer[,] tile_sprites_array;
    public int _X_tiles;
    public int _Y_tiles;

    int number_of_tiles_leader;
    tile_C[] tiles_leader_array;

    public GameObject tile_content;
    public GameObject over_gem;
    public Sprite[] gem_colors;//the avatars of the gems
    public int gem_length;

    public Sprite[] lock_gem_hp;
    public int padlock_count;

    public Sprite[] block_hp;
    public int block_count;

    public Sprite[] falling_block_hp;

    public Sprite immune_block;
    public Sprite junk;
    public Sprite token;

    public Sprite[] ice_hp;
    public Sprite[] need_color;
    public Sprite[] item_color;
    public Sprite[] key_color;
    public Sprite[] door_color;
    public Sprite[] start_goal_path;
    public Sprite player_avatar;

    public Vector3 topLeftBoardPoint;
    public Vector3 bottomRightBoardPoint;

    //bonus
    public Sprite[] on_board_bonus_sprites;

    public int[,,] board_array_master;//this keep track of the status of every tiles, gems, lock and so on... in the board
                                      /* 0 = tile [-1 = "no tile"; 0 = "hp = 0"; 1 = "hp 1"...]
                                       * 1 = gem [-99 = no gem; 
                                       * 			from 0 to 6 color gem; 
                                       * 				7 = special explosion good 
                                       * 				8 = special explosion bad 
                                       * 				9 = neutre = it don't explode when 3 in a row
                                       * 			10 = random gem; 
                                       * 			20 = ?; 
                                       * 			30 = ?; 
                                       * 			40 = immune block
                                       * 			41 = block hp 1
                                       * 			42 = block hp 2
                                       * 			43 = block hp 3
                                       * 			51 = falling block hp 1
                                       * 			52 = falling block hp 2
                                       * 			53 = falling block hp 3
                                       * 			60 = need a
                                       * 			61 = need b
                                       * 			62 = need c
                                       * 			63 = need d
                                       * 			64 = need e
                                       * 			70 = key a
                                       * 			71 = key b
                                       * 			72 = key c
                                       * 			73 = key d
                                       * 			74 = key e
                                       * 2 = special tile:
                                       * 		0 = no special
                                       * 		1 = start
                                       * 		2 = goal
                                       * 		3 = path
                                       * 		10 = door a
                                       * 		11 = door b
                                       * 		12 = door c
                                       * 		13 = door d
                                       * 		14 = door e
                                       * 		20 = item a
                                       * 		21 = item b
                                       * 		22 = item c
                                       * 		23 = item d
                                       * 		24 = item e
                                       * 3 = restraint [0 = no padlock; 1 = padlock hp1...
                                       * 				11 = ice hp1...
                                       * 4 = special [-200 = token
                                       * 				-100 = junk
                                       * 				0 = no
                                       * 				1= destroy_one
                                       * 				2= Switch_gem_teleport
                                       * 				3= bomb
                                       * 				4= horiz
                                       * 				5= vertic
                                       * 				6= horiz_and_vertic
                                       * 				7= destroy_all_same_color
                                       * 
                                       * 				8= more_time
                                       * 				9= more_moves
                                       * 				10= more_hp
                                       * 				11= rotate_board_L
                                       * 				12= rotate_board_R
                                       * 
                                       * 				13 = destroy single random gems (meteor shower)
                                       * 
                                       * 				100 = time bomb
                                       * 
                                       * 
                                       * 5 = number of useful moves of this gem [from 0 = none, to 4 = all directions]
                                              6 = up [n. how many gem explode if this gem go up]
                                              7 = down [n. how many gem explode if this gem go down]
                                              8 = right [n. how many gem explode if this gem go right]
                                              9 = left [n. how many gem explode if this gem go left]
                                          10 = this thing can fall (0=false;1=true) (2 = explode if reach board bottom border)
                                          11 = current tile action in progress (0=none; 1=explosion;                    2=creation;                         3=falling down;4=falling down R;5=falling down L;   6 = Switching this gem)
                                                                                        111 = explosion ongoing         222= creation ongoing               333 = falling animation ongoing                     666 = primary explosion
                                                                                        
                                          12 = this tile generate gems (0= no; 1= yes; 2= yes and it is activated)
                                          13 = tile already checked (0=false;1=true)
                                          14 = block_hp
                                       */
    private int board_array_master_length = 15;

    void Load_board()//call from awake
    {
        if (file_txt_asset)
        {
            fileContents = file_txt_asset.text;


            string[] parts = fileContents.Split(new string[] { "\r\n" }, StringSplitOptions.None);

            _X_tiles = Int16.Parse(parts[0]);
            _Y_tiles = Int16.Parse(parts[1]);


            board_array_master = new int[_X_tiles, _Y_tiles, board_array_master_length];
            tiles_array = new tile_C[_X_tiles, _Y_tiles];
            tile_sprites_array = new SpriteRenderer[_X_tiles, _Y_tiles];

            for (int y = 0; y < _Y_tiles + 2; y++)
            {
                if (y > 1)
                {
                    for (int x = 0; x < _X_tiles; x++)
                    {
                        string[] tile = parts[y].Split(new string[] { "|" }, StringSplitOptions.None);

                        for (int z = 0; z < 5; z++)
                        {
                            string[] tile_characteristic = tile[x].Split(new string[] { "," }, StringSplitOptions.None);
                            board_array_master[x, y - 2, z] = Int16.Parse(tile_characteristic[z]);
                        }
                    }

                }
            }
        }
        else
        {
            Board_camera.backgroundColor = Color.red;
            //Debug.LogError("Stage file is empty");
        }
    }

    void Create_new_board()//call from Awake()
    {
        //int leader_tiles_count = 0;

        if (show_token_after_all_tiles_are_destroyed && win_requirement_selected == win_requirement.take_all_tokens)
            token_place_card = new bool[_X_tiles, _Y_tiles];

        for (int y = 0; y < _Y_tiles; y++)
        {
            for (int x = 0; x < _X_tiles; x++)
            {

                if (board_array_master[x, y, 0] > -1)//if there is a tile
                {
                    Vector2 this_position = new Vector2(x + pivot_board.position.x, -y + pivot_board.position.y);

                    //generate tile
                    GameObject tileGO = (GameObject)Instantiate(tile_obj, this_position, Quaternion.identity);
                    tiles_array[x, y] = tileGO.GetComponent<tile_C>();
                    tiles_array[x, y].name = "x:" + x + ",y:" + y;
                    tiles_array[x, y].board = this;
                    //tiles_array[x,y].GetComponent<tile_C>().explosion_score_script = the_gui_score_of_this_move.gameObject.GetComponent<explosion_score>();

                    tiles_array[x, y].transform.parent = pivot_board;
                    total_tiles++;
                    //search leader tiles

                    if ((y == 0) || ((y > 0) && (board_array_master[x, y - 1, 0] == -1))) //if the tile is on the first row or don't have another tile over
                    {
                        board_array_master[x, y, 12] = 1;//this is a leader tile
                        number_of_tiles_leader++;
                    }
                    //search bottom tiles for everi orientation:
                    //orientation 0 = 0°
                    if ((y == _Y_tiles - 1) || ((y < _Y_tiles - 1) && (board_array_master[x, y + 1, 0] == -1)))
                    {
                        number_of_bottom_tiles[0]++;
                    }
                    //orientation 1 = 90°
                    if ((x == _X_tiles - 1) || ((x < _X_tiles - 1) && (board_array_master[x + 1, 1, 0] == -1)))
                    {
                        number_of_bottom_tiles[1]++;
                    }
                    //orientation 2 = 180°
                    if ((y == 0) || ((y > 0) && (board_array_master[x, y - 1, 0] == -1))) //if the tile is on the first row or don't have another tile over
                    {
                        number_of_bottom_tiles[2]++;
                    }
                    //orientation 3 = 270°
                    if ((x == 0) || ((x > 0) && (board_array_master[x - 1, y, 0] == -1))) //if the tile is on the first row or don't have another tile over
                    {
                        number_of_bottom_tiles[3]++;
                    }

                }


                if ((board_array_master[x, y, 4] == -200) && (show_token_after_all_tiles_are_destroyed))//token
                {
                    //take note of the token position and create a normal gem here
                    number_of_token_to_collect++;
                    token_place_card[x, y] = true;
                    board_array_master[x, y, 4] = 0;
                    board_array_master[x, y, 1] = 10;
                }

                if (board_array_master[x, y, 1] == 10)//if there is a random gem here
                {
                    board_array_master[x, y, 1] = UnityEngine.Random.Range(0, gem_length);
                }


                if (board_array_master[x, y, 1] >= 0)//if there is something in this tile
                {
                    //this thing can fall?
                    if (board_array_master[x, y, 3] == 0) // no restraint
                    {
                        if ((board_array_master[x, y, 1] < 40) //is a gem, junk or token
                            || ((board_array_master[x, y, 1] >= 51) && (board_array_master[x, y, 1] <= 59)) // is a falling block
                            || ((board_array_master[x, y, 1] >= 70) && (board_array_master[x, y, 1] <= 79))) // is a key

                        {
                            board_array_master[x, y, 10] = 1;
                            //if is junk or token: explode when reach the bottom of the board
                            if ((board_array_master[x, y, 1] >= 20) && (board_array_master[x, y, 1] <= 39))
                                board_array_master[x, y, 10] = 2;
                        }
                    }
                }
            }
        }

        tiles_leader_array = new tile_C[number_of_tiles_leader];//the tiles in this array will create the falling gems
        int leader_tiles_count = 0;
        total_gems_on_board_at_start = 0;
        //feed bottom tiles array:
        //search most long bottom tile list:
        int temp_bottom_tiles_array_lenght = 0;
        for (int i = 0; i < 4; i++)
        {
            if (number_of_bottom_tiles[i] > temp_bottom_tiles_array_lenght)
                temp_bottom_tiles_array_lenght = number_of_bottom_tiles[i];
        }
        bottom_tiles_array = new tile_C[4, temp_bottom_tiles_array_lenght];
        int[] temp_bottom_tiles_array_count = new int[4];

        for (int y = 0; y < _Y_tiles; y++)
        {
            for (int x = 0; x < _X_tiles; x++)
            {
                if (board_array_master[x, y, 0] > -1)//if there is a tile
                {
                    Vector2 this_position = new Vector2(x + pivot_board.position.x, -y + pivot_board.position.y);

                    Avoid_triple_color_gem(x, y);

                    //create visual representation 
                    //the tile
                    tile_C tile_script = (tile_C)tiles_array[x, y];
                    tile_script._x = x;
                    tile_script._y = y;


                    //and is leader...
                    if (board_array_master[x, y, 12] == 1)
                    {
                        tiles_leader_array[leader_tiles_count] = tile_script;
                        leader_tiles_count++;
                    }

                    //orientation 0 = 0°
                    if ((y == _Y_tiles - 1) || ((y < _Y_tiles - 1) && (board_array_master[x, y + 1, 0] == -1)))
                    {
                        bottom_tiles_array[0, temp_bottom_tiles_array_count[0]] = tile_script;
                        temp_bottom_tiles_array_count[0]++;
                    }
                    //orientation 1 = 90°
                    if ((x == _X_tiles - 1) || ((x < _X_tiles - 1) && (board_array_master[x + 1, 1, 0] == -1)))
                    {
                        bottom_tiles_array[1, temp_bottom_tiles_array_count[1]] = tile_script;
                        temp_bottom_tiles_array_count[1]++;
                    }
                    //orientation 2 = 180°
                    if ((y == 0) || ((y > 0) && (board_array_master[x, y - 1, 0] == -1)))
                    {
                        bottom_tiles_array[2, temp_bottom_tiles_array_count[2]] = tile_script;
                        temp_bottom_tiles_array_count[2]++;
                    }
                    //orientation 3 = 270°
                    if ((x == 0) || ((x > 0) && (board_array_master[x - 1, y, 0] == -1)))
                    {
                        bottom_tiles_array[3, temp_bottom_tiles_array_count[3]] = tile_script;
                        temp_bottom_tiles_array_count[3]++;
                    }

                    SpriteRenderer sprite_hp = tile_script.GetComponent<SpriteRenderer>();

                    if ((board_array_master[x, y, 0] == 0) && (board_array_master[x, y, 2] > 0)) //if this is a special tile and is visible
                    {
                        if ((board_array_master[x, y, 2] >= 1) && (board_array_master[x, y, 2] <= 9))
                            sprite_hp.sprite = start_goal_path[board_array_master[x, y, 2] - 1];
                        else if ((board_array_master[x, y, 2] >= 10) && (board_array_master[x, y, 2] <= 19))
                            sprite_hp.sprite = door_color[board_array_master[x, y, 2] - 10];

                    }
                    //update hp board
                    HP_board += board_array_master[x, y, 0];


                    //create gem
                    if ((board_array_master[x, y, 1] >= 0) && (board_array_master[x, y, 1] < 9))//if here go a gem
                    {
                        //I can put the gem on the board
                        total_gems_on_board_at_start++;
                        tile_script.my_gem = (GameObject)Instantiate(tile_content, this_position, Quaternion.identity);//"-y" in order to have 0,0 in the up-left corner
                        tile_script.my_gem.transform.parent = pivot_board.transform;
                        tile_script.my_gem_renderer = tile_script.my_gem.GetComponent<SpriteRenderer>();
                        tile_sprites_array[x, y] = tile_script.my_gem_renderer;

                        if (x == 0 && y == 0)
                        {
                            topLeftBoardPoint = tile_script.transform.position - new Vector3(0.5f, -0.5f, 0.0f);
                            bottomRightBoardPoint = topLeftBoardPoint + new Vector3(8.0f, -8.0f, 0.0f);
                        }

                        tile_script.my_gem.name = "gem" + board_array_master[x, y, 1].ToString();
                        //gem color:
                        SpriteRenderer sprite_gem = tile_script.my_gem_renderer;
                        sprite_gem.sprite = gem_colors[board_array_master[x, y, 1]];


                        //create padlock or ice
                        if (board_array_master[x, y, 3] > 0)
                        {
                            tile_script.my_padlock = (GameObject)Instantiate(over_gem, this_position, Quaternion.identity);
                            tile_script.my_padlock.transform.parent = pivot_board;

                            if (board_array_master[x, y, 3] < 11)
                            {
                                padlock_count++;
                                tile_script.my_padlock.name = "padlock";
                                SpriteRenderer sprite_lock = tile_script.my_padlock.GetComponent<SpriteRenderer>();
                                sprite_lock.sprite = lock_gem_hp[board_array_master[x, y, 3] - 1];
                            }
                            else
                            {
                                tile_script.my_padlock.name = "ice";
                                SpriteRenderer sprite_lock = tile_script.my_padlock.GetComponent<SpriteRenderer>();
                                sprite_lock.sprite = ice_hp[board_array_master[x, y, 3] - 11];
                            }
                        }


                    }
                    else //there is somethin that not is a gem
                    {
                        //auxiliary gem in garbage that will be use when this tile will be free
                        ((GameObject)Instantiate(tile_content, new Vector2(x, -y), Quaternion.identity)).transform.parent = garbage_recycle;

                        if (board_array_master[x, y, 1] == 9) //this is a special content
                        {
                            tile_script.my_gem = (GameObject)Instantiate(tile_content, this_position, Quaternion.identity);//"-y" in order to have 0,0 in the up-left corner
                            tile_script.my_gem.transform.parent = pivot_board;
                            tile_script.my_gem_renderer = tile_script.my_gem.GetComponent<SpriteRenderer>();
                            if (board_array_master[x, y, 4] == -100)//junk
                            {
                                tile_script.my_gem.name = "junk";
                                number_of_junk_on_board++;
                                tile_script.my_gem_renderer.sprite = junk;
                            }
                            else if (board_array_master[x, y, 4] == -200)//token
                            {
                                number_of_token_on_board++;

                                tile_script.my_gem.name = "token";
                                SpriteRenderer sprite_gem = tile_script.my_gem_renderer;
                                sprite_gem.sprite = token;

                            }
                            else if (board_array_master[x, y, 4] > 0)//bonus
                            {
                                number_of_bonus_on_board++;
                                tile_script.my_gem.name = "bonus";
                                SpriteRenderer sprite_gem = tile_script.my_gem_renderer;
                                sprite_gem.sprite = on_board_bonus_sprites[board_array_master[x, y, 4]];
                            }
                        }

                        else if (board_array_master[x, y, 1] == 40)//immune block
                        {
                            tile_script.my_gem = (GameObject)Instantiate(tile_content, this_position, Quaternion.identity);//"-y" in order to have 0,0 in the up-left corner
                            tile_script.my_gem.transform.parent = pivot_board;
                            tile_script.my_gem.name = "immune_block";
                            tile_script.my_gem_renderer = tile_script.my_gem.GetComponent<SpriteRenderer>();

                            tile_script.my_gem_renderer.sprite = immune_block;
                        }
                        else if ((board_array_master[x, y, 1] > 40) && (board_array_master[x, y, 1] < 50))//block
                        {
                            board_array_master[x, y, 14] = (board_array_master[x, y, 1] - 40);

                            tile_script.my_gem = (GameObject)Instantiate(tile_content, this_position, Quaternion.identity);//"-y" in order to have 0,0 in the up-left corner
                            tile_script.my_gem.transform.parent = pivot_board;
                            tile_script.my_gem.name = "block";
                            tile_script.my_gem_renderer = tile_script.my_gem.GetComponent<SpriteRenderer>();
                            block_count++;

                            tile_script.my_gem_renderer.sprite = block_hp[board_array_master[x, y, 1] - 41];
                        }
                        else if ((board_array_master[x, y, 1] > 50) && (board_array_master[x, y, 1] < 60))// falling block
                        {
                            board_array_master[x, y, 14] = (board_array_master[x, y, 1] - 50);

                            tile_script.my_gem = (GameObject)Instantiate(tile_content, this_position, Quaternion.identity);//"-y" in order to have 0,0 in the up-left corner
                            tile_script.my_gem.transform.parent = pivot_board;
                            tile_script.my_gem.name = "falling_block";
                            tile_script.my_gem_renderer = tile_script.my_gem.GetComponent<SpriteRenderer>();
                            block_count++;

                            tile_script.my_gem_renderer.sprite = falling_block_hp[board_array_master[x, y, 1] - 51];
                        }
                        else if ((board_array_master[x, y, 1] >= 60) && (board_array_master[x, y, 1] < 70))// need
                        {
                            tile_script.my_gem = (GameObject)Instantiate(tile_content, this_position, Quaternion.identity);//"-y" in order to have 0,0 in the up-left corner
                            tile_script.my_gem.transform.parent = pivot_board;
                            tile_script.my_gem_renderer = tile_script.my_gem.GetComponent<SpriteRenderer>();
                            tile_script.my_gem.name = "need";

                            tile_script.my_gem_renderer.sprite = need_color[board_array_master[x, y, 1] - 60];
                        }
                        else if ((board_array_master[x, y, 1] >= 70) && (board_array_master[x, y, 1] < 80))// key
                        {
                            tile_script.my_gem = (GameObject)Instantiate(tile_content, this_position, Quaternion.identity);//"-y" in order to have 0,0 in the up-left corner
                            tile_script.my_gem.transform.parent = pivot_board;
                            tile_script.my_gem_renderer = tile_script.my_gem.GetComponent<SpriteRenderer>();
                            tile_script.my_gem.name = "key";

                            tile_script.my_gem_renderer.sprite = key_color[board_array_master[x, y, 1] - 70];
                        }


                        else if ((board_array_master[x, y, 2] >= 20) && (board_array_master[x, y, 2] <= 29))//item
                        {
                            tile_script.my_gem = (GameObject)Instantiate(tile_content, this_position, Quaternion.identity);//"-y" in order to have 0,0 in the up-left corner
                            tile_script.my_gem.transform.parent = pivot_board;
                            tile_script.my_gem.name = "item" + board_array_master[x, y, 2].ToString();
                            tile_script.my_gem_renderer = tile_script.my_gem.GetComponent<SpriteRenderer>();

                            tile_script.my_gem_renderer.sprite = item_color[board_array_master[x, y, 2] - 20];

                        }
                    }
                }
                else //there are no tile here
                    board_array_master[x, y, 1] = -99;//no color here
            }

        }

        if (win_requirement_selected == win_requirement.take_all_tokens)
        {
            if (number_of_token_on_board > 0)
                number_of_token_to_collect = number_of_token_on_board;
            else
            {
                if (show_token_after_all_tiles_are_destroyed && (HP_board == 0))
                {
                    Show_all_token_on_board();
                }
            }
            if (number_of_token_to_collect > 0)
                Update_token_count();
            //else
                //Debug.LogWarning("win condition is 'Take_all_tolens' but this stage file don't have token!");
        }

        if ((number_of_bonus_on_board > 0) && (trigger_by_select == trigger_by.OFF))
        {
            //Debug.LogWarning("This stage file have on board bonus, but you don't have setup any rule to trigger it. So, by default, these bonus will be trigger on click");
            trigger_by_select = trigger_by.click;
        }

        //Debug.Log("Board created. HP board = " + HP_board);
        if (emit_token_only_after_all_tiles_are_destroyed && HP_board == 0)
            allow_token_emission = true;

        Search_max_bonus_values_for_charge_bonus();
    }

    public void Show_all_token_on_board()//call from Create_new_board(), tile_C.Update_tile_hp()
    {
        if (!token_showed)
        {
            token_showed = true;

            for (int y = 0; y < _Y_tiles; y++)
            {
                for (int x = 0; x < _X_tiles; x++)
                {
                    if (token_place_card[x, y])
                    {
                        board_array_master[x, y, 1] = 9;
                        board_array_master[x, y, 4] = -200;
                        tile_C tile_script = (tile_C)tiles_array[x, y];
                        tile_script.SetShowToken();
                    }
                }
            }
        }
    }

    void Avoid_triple_color_gem(int x, int y)//call from Shuffle(), Create_new_board()
    {
        int attempt_count = 0;
        if ((board_array_master[x, y, 1] >= 0) && (board_array_master[x, y, 1] < 9))//if this is a gem
        {
            if (((x + 1 < _X_tiles) && (x - 1 >= 0)) && ((y + 1 < _Y_tiles) && (y - 1 >= 0)))
            {
                attempt_count = 0;
                while ((attempt_count <= gem_length)
                       && ((board_array_master[x, y, 1] == board_array_master[x + 1, y, 1]) && (board_array_master[x, y, 1] == board_array_master[x - 1, y, 1])
                    || (board_array_master[x, y, 1] == board_array_master[x, y + 1, 1]) && (board_array_master[x, y, 1] == board_array_master[x, y - 1, 1])))
                {
                    //vertical check
                    if (board_array_master[x, y, 1] + 1 < gem_length)
                        board_array_master[x, y, 1]++;
                    else
                        board_array_master[x, y, 1] = 0;

                    attempt_count++;
                }
            }
            else if (((x + 1 < _X_tiles) && (x - 1 >= 0)))
            {
                attempt_count = 0;
                while ((attempt_count <= gem_length)
                       && ((board_array_master[x, y, 1] == board_array_master[x + 1, y, 1]) && (board_array_master[x, y, 1] == board_array_master[x - 1, y, 1])))
                {
                    //horizontal check
                    if (board_array_master[x, y, 1] + 1 < gem_length)
                        board_array_master[x, y, 1]++;
                    else
                        board_array_master[x, y, 1] = 0;

                    attempt_count++;
                }
            }
            else if (((y + 1 < _Y_tiles) && (y - 1 >= 0)))
            {
                attempt_count = 0;
                while ((attempt_count <= gem_length)
                       && ((board_array_master[x, y, 1] == board_array_master[x, y + 1, 1]) && (board_array_master[x, y, 1] == board_array_master[x, y - 1, 1])))
                {
                    //Debug.Log("while verticale");
                    if (board_array_master[x, y, 1] + 1 < gem_length)
                        board_array_master[x, y, 1]++;
                    else
                        board_array_master[x, y, 1] = 0;

                    attempt_count++;
                }
            }
            if ((x - 2 >= 0) && (x - 1 >= 0))
                if ((board_array_master[x, y, 1] == board_array_master[x - 1, y, 1]) && (board_array_master[x, y, 1] == board_array_master[x - 2, y, 1]))
                {
                    if (board_array_master[x, y, 1] + 1 < gem_length)
                        board_array_master[x, y, 1]++;
                    else
                        board_array_master[x, y, 1] = 0;
                }
            if ((y - 2 >= 0) && (y - 1 >= 0))
                if ((board_array_master[x, y, 1] == board_array_master[x, y - 1, 1]) && (board_array_master[x, y, 1] == board_array_master[x, y - 2, 1]))
                {
                    if (board_array_master[x, y, 1] + 1 < gem_length)
                        board_array_master[x, y, 1]++;
                    else
                        board_array_master[x, y, 1] = 0;
                }
        }
    }

    void Search_max_bonus_values_for_charge_bonus()//call from Create_new_board()
    {

    }


}
