
using UnityEngine.UI;
using UnityEngine;
using System.Collections;
using UnityEditor;

using System.IO;
using System;

[CustomEditor(typeof(Board_C))]
internal class match_controller : Editor
{


    int total_gem_to_collect;

    #region temp AI
    public enum enemy_AI_gem_collect
    {
        random,
        collect_gems_from_less_to_more,
        collect_gems_from_more_to_less,
        by_hand_setup
    }
    public enemy_AI_gem_collect enemy_AI_gem_collect_select;

    public enum enemy_AI_battle
    {
        random,
        dynamic_battle,
        by_hand_setup
    }
    public enemy_AI_battle enemy_AI_battle_select;

    public enum enemy_AI_target_score
    {
        random,
        by_hand_setup
    }
    public enemy_AI_target_score enemy_AI_target_score_select;
    #endregion

    #region temp bonus
    public enum charge_bonus
    {
        none = 0,
        destroy_one = 1,
        switch_gem_teleport = 2,
        destroy_3x3 = 3,
        destroy_horizontal = 4,
        destroy_vertical = 5,
        destroy_horizontal_and_vertical = 6,
        destroy_all_gem_with_this_color = 7,
        give_more_time = 8,
        give_more_moves = 9,
        heal_hp = 10
    }
    public charge_bonus[] charge_bonus_select;
    public charge_bonus[] enemy_charge_bonus_select;

    public enum on_board_bonus
    {
        none = 0,
        //destroy_one = 1, 
        //switch_gem_teleport = 2,
        destroy_3x3 = 3,
        destroy_horizontal = 4,
        destroy_vertical = 5,
        destroy_horizontal_and_vertical = 6,
        //destroy_all_gem_with_this_color = 7,
        give_more_time = 8,
        give_more_moves = 9,
        heal_hp = 10
    }
    public on_board_bonus[] on_board_bonus_select;


    #endregion


    void OnEnable()
    {
        Board_C my_target = (Board_C)target;

    }

    public override void OnInspectorGUI()
    {

        Stage();
        Rules();
        Camera_setup();
        Assign_sprites();
        Audio_sfx();
        Visual_enhancements();
        Advanced();
        if (!EditorApplication.isPlaying)
            Show_gui();

    }

    void Stage()
    {
        Board_C my_target = (Board_C)target;
        EditorGUI.BeginChangeCheck();
        Undo.RecordObject(my_target, "Stage");

        if (my_target.Stage_uGUI_obj == null)
            my_target.current_level = EditorGUILayout.IntField("Stage n", my_target.current_level);

        if (!my_target.file_txt_asset)
            GUI.color = Color.red;
        else
            GUI.color = Color.white;
        my_target.file_txt_asset = EditorGUILayout.ObjectField("Stage file", my_target.file_txt_asset, typeof(TextAsset), true) as TextAsset;
        my_target.pivot_board = EditorGUILayout.ObjectField("Pivot", my_target.pivot_board, typeof(Transform), true) as Transform;

        my_target.tile_obj = EditorGUILayout.ObjectField("Tile prefab", my_target.tile_obj, typeof(GameObject), true) as GameObject;
        my_target.tile_content = EditorGUILayout.ObjectField("Tile prefab", my_target.tile_content, typeof(GameObject), true) as GameObject;

        GUI.color = Color.white;

        my_target.start_after_selected = (Board_C.start_after)EditorGUILayout.EnumPopup("Start condition", my_target.start_after_selected);
        if (my_target.start_after_selected == Board_C.start_after.time)
        {
            EditorGUI.indentLevel++;
            if (my_target.start_after_n_seconds < 0)
                GUI.color = Color.red;
            else
                GUI.color = Color.white;

            my_target.start_after_n_seconds = EditorGUILayout.FloatField("start after n seconds", my_target.start_after_n_seconds);

            GUI.color = Color.white;
            EditorGUI.indentLevel--;
        }

        if (EditorGUI.EndChangeCheck())
            EditorUtility.SetDirty(my_target);
    }

    void Rules()
    {
        Board_C my_target = (Board_C)target;
        EditorGUI.BeginChangeCheck();
        Undo.RecordObject(my_target, "rules");

        my_target.show_rules = EditorGUILayout.Foldout(my_target.show_rules, "Rules");
        if (my_target.show_rules)
        {
            EditorGUI.indentLevel = 1;

            if (my_target.win_requirement_selected == Board_C.win_requirement.play_until_lose && my_target.lose_requirement_selected == Board_C.lose_requirement.relax_mode)
                GUI.color = Color.red;
            else
                GUI.color = Color.white;

            my_target.win_requirement_selected = (Board_C.win_requirement)EditorGUILayout.EnumPopup("Win requirement", my_target.win_requirement_selected);
            GUI.color = Color.white;

            EditorGUI.indentLevel++;
            if (my_target.win_requirement_selected == Board_C.win_requirement.collect_gems)
            {
                total_gem_to_collect = 0;
                for (int i = 0; i < my_target.gem_length; i++)
                {
                    if (my_target.number_of_gems_to_destroy_to_win[i] < 0)
                        GUI.color = Color.red;
                    else
                        GUI.color = Color.white;
                    my_target.number_of_gems_to_destroy_to_win[i] = EditorGUILayout.IntField("gem " + i + " needful", my_target.number_of_gems_to_destroy_to_win[i]);
                    total_gem_to_collect += my_target.number_of_gems_to_destroy_to_win[i];
                }
                GUI.color = Color.white;
                if (total_gem_to_collect == 0)
                    EditorGUILayout.LabelField("WARNING! The total number of gem to collect can't be zero!");


            }
            else if (my_target.win_requirement_selected == Board_C.win_requirement.reach_target_score)
            {
                my_target.target_score = EditorGUILayout.IntField("target score", my_target.target_score);

            }
            else if (my_target.win_requirement_selected == Board_C.win_requirement.take_all_tokens)
            {
                EditorGUI.indentLevel++;
                my_target.show_token_after_all_tiles_are_destroyed = EditorGUILayout.Toggle("Show token only after all tiles are destroyed", my_target.show_token_after_all_tiles_are_destroyed);
                EditorGUI.indentLevel--;
            }

            if (my_target.win_requirement_selected != Board_C.win_requirement.play_until_lose && my_target.win_requirement_selected != Board_C.win_requirement.enemy_hp_is_zero)
                my_target.continue_to_play_after_win_until_lose_happen = EditorGUILayout.Toggle("continue to play after win", my_target.continue_to_play_after_win_until_lose_happen);
            else
                my_target.continue_to_play_after_win_until_lose_happen = false;

            GUI.color = Color.white;
            EditorGUI.indentLevel--;

            my_target.lose_requirement_selected = (Board_C.lose_requirement)EditorGUILayout.EnumPopup("Lose requirement", my_target.lose_requirement_selected);
            EditorGUI.indentLevel++;
            if (my_target.lose_requirement_selected == Board_C.lose_requirement.timer)
            {
                if (my_target.timer <= 0)
                    GUI.color = Color.red;
                else
                    GUI.color = Color.white;
                my_target.timer = EditorGUILayout.FloatField("Time in seconds", my_target.timer);

                EditorGUILayout.LabelField("time bonus for:");
                EditorGUI.indentLevel++;
                if (my_target.time_bonus_for_gem_explosion < 0)
                    GUI.color = Color.red;
                else
                    GUI.color = Color.white;
                my_target.time_bonus_for_gem_explosion = EditorGUILayout.FloatField("gem explosion", my_target.time_bonus_for_gem_explosion);

                if (my_target.time_bonus_for_secondary_explosion < 0)
                    GUI.color = Color.red;
                else
                    GUI.color = Color.white;
                my_target.time_bonus_for_secondary_explosion = EditorGUILayout.FloatField("secondary explosion", my_target.time_bonus_for_secondary_explosion);
                EditorGUI.indentLevel--;

            }
            else if (my_target.lose_requirement_selected == Board_C.lose_requirement.player_have_zero_moves)
            {
                my_target.max_moves = EditorGUILayout.IntField("Moves", my_target.max_moves);
                EditorGUI.indentLevel++;
                my_target.show_move_reward_system = EditorGUILayout.Foldout(my_target.show_move_reward_system, "move rewards");
                if (my_target.show_move_reward_system)
                {
                    my_target.lose_turn_if_bad_move = EditorGUILayout.Toggle("lost a move if bad move", my_target.lose_turn_if_bad_move);
                    EditorGUILayout.LabelField("gain moves if explode:");
                    EditorGUI.indentLevel++;

                    my_target.gain_turn_if_secondary_explosion = EditorGUILayout.Toggle("secondary explosion", my_target.gain_turn_if_secondary_explosion);
                    if (my_target.gain_turn_if_secondary_explosion)
                    {
                        EditorGUI.indentLevel++;
                        if (my_target.seconday_explosion_maginiture_needed_to_gain_a_turn < 3)
                            GUI.color = Color.red;
                        else
                            GUI.color = Color.white;
                        my_target.seconday_explosion_maginiture_needed_to_gain_a_turn = EditorGUILayout.IntField("minimum magnitude requested", my_target.seconday_explosion_maginiture_needed_to_gain_a_turn);
                        GUI.color = Color.white;

                        if (my_target.combo_lenght_needed_to_gain_a_turn < 0)
                            GUI.color = Color.red;
                        else
                            GUI.color = Color.white;
                        my_target.combo_lenght_needed_to_gain_a_turn = EditorGUILayout.IntField("minimum combo lenght requested", my_target.combo_lenght_needed_to_gain_a_turn);
                        GUI.color = Color.white;
                        EditorGUI.indentLevel--;
                    }

                    my_target.gain_turn_if_explode_same_color_of_previous_move = EditorGUILayout.Toggle("same color of your previous move", my_target.gain_turn_if_explode_same_color_of_previous_move);
                    if (my_target.gain_turn_if_explode_same_color_of_previous_move)
                    {
                        EditorGUI.indentLevel++;
                        if (my_target.move_gained_for_explode_same_color_in_two_adjacent_turn < 0)
                            GUI.color = Color.red;
                        else
                            GUI.color = Color.white;
                        my_target.move_gained_for_explode_same_color_in_two_adjacent_turn = EditorGUILayout.IntField("give n moves:", my_target.move_gained_for_explode_same_color_in_two_adjacent_turn);
                        GUI.color = Color.white;
                        EditorGUI.indentLevel--;
                    }
                    my_target.gain_turn_if_explode_more_than_3_gems = EditorGUILayout.Toggle("more than 3 gems", my_target.gain_turn_if_explode_more_than_3_gems);
                    if (my_target.gain_turn_if_explode_more_than_3_gems)
                    {
                        EditorGUI.indentLevel++;
                        for (int i = 0; i < 4; i++)
                        {
                            if (my_target.move_gained_when_explode_more_than_3_gems[i] < 0)
                                GUI.color = Color.red;
                            else
                                GUI.color = Color.white;
                            my_target.move_gained_when_explode_more_than_3_gems[i] = EditorGUILayout.IntField("explode " + (4 + i) + " gems give " + my_target.move_gained_when_explode_more_than_3_gems[i] + " moves", my_target.move_gained_when_explode_more_than_3_gems[i]);
                        }
                        GUI.color = Color.white;
                        EditorGUI.indentLevel--;
                    }

                    if ((my_target.gain_turn_if_explode_same_color_of_previous_move) || (my_target.gain_turn_if_explode_more_than_3_gems))
                    {
                        EditorGUILayout.BeginHorizontal();
                        my_target.chain_turns_limit = EditorGUILayout.Toggle("chain limit", my_target.chain_turns_limit);
                        if (my_target.chain_turns_limit)
                        {
                            if (my_target.max_chain_turns <= 0)
                                GUI.color = Color.red;
                            else
                                GUI.color = Color.white;
                            my_target.max_chain_turns = EditorGUILayout.IntField("max", my_target.max_chain_turns);
                            GUI.color = Color.white;
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                    EditorGUI.indentLevel--;

                }
                EditorGUI.indentLevel--;
            }

            GUI.color = Color.white;
            EditorGUI.indentLevel--;

            if (my_target.win_requirement_selected == Board_C.win_requirement.destroy_all_tiles)
                my_target.explosions_damages_tiles = true;
            else if ((my_target.win_requirement_selected == Board_C.win_requirement.take_all_tokens) && my_target.show_token_after_all_tiles_are_destroyed)
                my_target.explosions_damages_tiles = true;
            else
                my_target.explosions_damages_tiles = EditorGUILayout.Toggle("Explosion damages tiles", my_target.explosions_damages_tiles);


            EditorGUI.indentLevel++;
            if (my_target.explosions_damages_tiles)
            {
                my_target.tile_give = (Board_C.tile_destroyed_give)EditorGUILayout.EnumPopup("Damage a tile give", my_target.tile_give);
                EditorGUI.indentLevel++;
                if (my_target.tile_give == Board_C.tile_destroyed_give.more_hp)
                {
                    if (my_target.tile_gift_int <= 0)
                        GUI.color = Color.red;
                    else
                        GUI.color = Color.white;
                    my_target.tile_gift_int = EditorGUILayout.IntField("How much HP?", my_target.tile_gift_int);
                }
                else if (my_target.tile_give == Board_C.tile_destroyed_give.more_time)
                {
                    if (my_target.tile_gift_float <= 0)
                        GUI.color = Color.red;
                    else
                        GUI.color = Color.white;
                    my_target.tile_gift_float = EditorGUILayout.FloatField("How much seconds?", my_target.tile_gift_float);
                }
                else if (my_target.tile_give == Board_C.tile_destroyed_give.more_moves)
                {
                    if (my_target.tile_gift_int <= 0)
                        GUI.color = Color.red;
                    else
                        GUI.color = Color.white;
                    my_target.tile_gift_int = EditorGUILayout.IntField("How much moves?", my_target.tile_gift_int);
                }
                GUI.color = Color.white;
                EditorGUI.indentLevel--;


            }
            EditorGUI.indentLevel--;


            EditorGUILayout.BeginHorizontal();
            my_target.show_hint = EditorGUILayout.Toggle("Show hint", my_target.show_hint);
            if (my_target.show_hint)
                my_target.show_hint_after_n_seconds = EditorGUILayout.FloatField("after n seconds", my_target.show_hint_after_n_seconds);
            EditorGUILayout.EndHorizontal();


            my_target.no_more_moves_rule_selected = (Board_C.no_more_moves_rule)EditorGUILayout.EnumPopup("If no more moves", my_target.no_more_moves_rule_selected);

            my_target.show_gem_emiter = EditorGUILayout.Foldout(my_target.show_gem_emiter, "Gems emitters");
            if (my_target.show_gem_emiter)
                {
                EditorGUI.indentLevel++;
                my_target.gem_emitter_rule = (Board_C.gem_emitter)EditorGUILayout.EnumPopup("gem emitters", my_target.gem_emitter_rule);
                if (my_target.gem_emitter_rule == Board_C.gem_emitter.special)
                    {
                    EditorGUI.indentLevel++;
                    if (my_target.create_a_special_element_each_n_gems_created < 1)
                        GUI.color = Color.red;
                    else
                        GUI.color = Color.white;

                    my_target.create_a_special_element_each_n_gems_created = EditorGUILayout.IntField("create a special element each n gems created", my_target.create_a_special_element_each_n_gems_created);
                    GUI.color = Color.white;

                    my_target.chance_to_create_a_special_element = EditorGUILayout.IntSlider("chance to create a special element", my_target.chance_to_create_a_special_element, 1, 100);
                    EditorGUILayout.LabelField("creation chances weights:");
                        EditorGUI.indentLevel++;
                        my_target.token_creation_chance_weight = EditorGUILayout.IntField("token", my_target.token_creation_chance_weight);
                        if (my_target.token_creation_chance_weight > 0)
                            {
                            EditorGUI.indentLevel++;
                            my_target.emit_token_only_after_all_tiles_are_destroyed = EditorGUILayout.Toggle("emit token only after all tiles are_destroyed", my_target.emit_token_only_after_all_tiles_are_destroyed);
                            EditorGUI.indentLevel--;
                            }
                        my_target.junk_creation_chance_weight = EditorGUILayout.IntField("junk", my_target.junk_creation_chance_weight);

                        EditorGUI.indentLevel--;
                    EditorGUI.indentLevel--;
                    }
                EditorGUI.indentLevel--;
                }

            my_target.show_score = EditorGUILayout.Foldout(my_target.show_score, "Score rewards");
            if (my_target.show_score)
            {
                EditorGUI.indentLevel++;

                if (my_target.score_reward_for_damaging_tiles < 0)
                    GUI.color = Color.red;
                else
                    GUI.color = Color.white;
                my_target.score_reward_for_damaging_tiles = EditorGUILayout.IntField("damaging a tile", my_target.score_reward_for_damaging_tiles);
                GUI.color = Color.white;

                EditorGUILayout.Space();
                for (int i = 0; i < my_target.gem_length; i++)
                {
                    if (my_target.score_reward_for_explode_gems == null || my_target.score_reward_for_explode_gems.Length != my_target.gem_length)
                        my_target.score_reward_for_explode_gems = new int[my_target.gem_length];

                    if (my_target.score_reward_for_explode_gems[i] < 0)
                        GUI.color = Color.red;
                    else
                        GUI.color = Color.white;
                    my_target.score_reward_for_explode_gems[i] = EditorGUILayout.IntField("explode " + (3 + i) + " gems with a move", my_target.score_reward_for_explode_gems[i]);
                }
                GUI.color = Color.white;

                EditorGUILayout.Space();

                if (my_target.score_reward_for_each_explode_gems_in_secondary_explosion < 0)
                    GUI.color = Color.red;
                else
                    GUI.color = Color.white;
                my_target.score_reward_for_each_explode_gems_in_secondary_explosion = EditorGUILayout.IntField("for each exploded gems in secondary explosion", my_target.score_reward_for_each_explode_gems_in_secondary_explosion);
                GUI.color = Color.white;

                EditorGUILayout.Space();

                EditorGUI.indentLevel--;
                EditorGUILayout.Space();
            }

            if (my_target.win_requirement_selected == Board_C.win_requirement.destroy_all_gems)
                my_target.gem_emitter_rule = Board_C.gem_emitter.off;


            if (my_target.gem_emitter_rule == Board_C.gem_emitter.off)
                my_target.no_more_moves_rule_selected = Board_C.no_more_moves_rule.lose;


            if ( //(my_target.lose_requirement_selected == Board_C.lose_requirement.player_hp_is_zero)  ||
                 (my_target.win_requirement_selected == Board_C.win_requirement.enemy_hp_is_zero)
                || (my_target.lose_requirement_selected == Board_C.lose_requirement.enemy_collect_gems)
                || (my_target.lose_requirement_selected == Board_C.lose_requirement.enemy_reach_target_score))
            {
                my_target.versus = true;
                my_target.show_versus = EditorGUILayout.Foldout(my_target.show_versus, "Verus Rules:");
                if (my_target.show_versus)
                {
                    EditorGUI.indentLevel++;

                    my_target.lose_turn_if_bad_move = EditorGUILayout.Toggle("lost turn if bad move", my_target.lose_turn_if_bad_move);
                    EditorGUILayout.LabelField("gain a turn if explode:");
                    EditorGUI.indentLevel++;

                    my_target.gain_turn_if_secondary_explosion = EditorGUILayout.Toggle("secondary explosion", my_target.gain_turn_if_secondary_explosion);
                    if (my_target.gain_turn_if_secondary_explosion)
                    {
                        EditorGUI.indentLevel++;
                        if (my_target.seconday_explosion_maginiture_needed_to_gain_a_turn < 3)
                            GUI.color = Color.red;
                        else
                            GUI.color = Color.white;
                        my_target.seconday_explosion_maginiture_needed_to_gain_a_turn = EditorGUILayout.IntField("minimum magnitude requested", my_target.seconday_explosion_maginiture_needed_to_gain_a_turn);
                        GUI.color = Color.white;

                        if (my_target.combo_lenght_needed_to_gain_a_turn < 0)
                            GUI.color = Color.red;
                        else
                            GUI.color = Color.white;
                        my_target.combo_lenght_needed_to_gain_a_turn = EditorGUILayout.IntField("minimum combo lenght requested", my_target.combo_lenght_needed_to_gain_a_turn);
                        GUI.color = Color.white;

                        EditorGUI.indentLevel--;
                    }

                    my_target.gain_turn_if_explode_same_color_of_previous_move = EditorGUILayout.Toggle("same color of your previous move", my_target.gain_turn_if_explode_same_color_of_previous_move);
                    my_target.gain_turn_if_explode_more_than_3_gems = EditorGUILayout.Toggle("more than 3 gems", my_target.gain_turn_if_explode_more_than_3_gems);

                    if ((my_target.gain_turn_if_explode_same_color_of_previous_move) || (my_target.gain_turn_if_explode_more_than_3_gems))
                    {
                        EditorGUILayout.BeginHorizontal();
                        my_target.chain_turns_limit = EditorGUILayout.Toggle("chain limit", my_target.chain_turns_limit);
                        if (my_target.chain_turns_limit)
                        {
                            if (my_target.max_chain_turns <= 0)
                                GUI.color = Color.red;
                            else
                                GUI.color = Color.white;
                            my_target.max_chain_turns = EditorGUILayout.IntField("max", my_target.max_chain_turns);
                            GUI.color = Color.white;
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                    EditorGUI.indentLevel--;
                    if ((my_target.lose_requirement_selected == Board_C.lose_requirement.player_hp_is_zero) ||
                        (my_target.win_requirement_selected == Board_C.win_requirement.enemy_hp_is_zero))
                    {
                        if (my_target.gem_damage_value <= 1)
                            GUI.color = Color.red;
                        else
                            GUI.color = Color.white;
                        my_target.gem_damage_value = EditorGUILayout.IntField("base hit damage", my_target.gem_damage_value);
                        GUI.color = Color.white;
                    }

                    EditorGUI.indentLevel++;
                    if (my_target.lose_requirement_selected == Board_C.lose_requirement.player_hp_is_zero)
                    {
                        //PLAYER
                        my_target.show_player = EditorGUILayout.Foldout(my_target.show_player, "Player");
                        if (my_target.show_player)
                        {
                            EditorGUI.indentLevel++;
                            if (my_target.lose_requirement_selected == Board_C.lose_requirement.player_hp_is_zero)
                            {


                                //armor mode:
                                if (my_target.use_armor)
                                {
                                    EditorGUI.indentLevel++;
                                    for (int i = 0; i < my_target.gem_length; i++)
                                    {
                                        my_target.player_armor[i] = (Board_C.armor)EditorGUILayout.EnumPopup("armor vs gem " + i, my_target.player_armor[i]);
                                    }
                                    EditorGUI.indentLevel--;

                                }

                            }

                            EditorGUI.indentLevel--;
                        }
                    }
                    //ENEMY
                    my_target.show_enemy = EditorGUILayout.Foldout(my_target.show_enemy, "Enemy");
                    if (my_target.show_enemy)
                    {
                        EditorGUI.indentLevel++;
                        if (my_target.win_requirement_selected == Board_C.win_requirement.enemy_hp_is_zero)
                        {

                            GUI.color = Color.white;


                            //armor mode:
                            if (my_target.use_armor)
                            {
                                EditorGUI.indentLevel++;
                                for (int i = 0; i < my_target.gem_length; i++)
                                {
                                    my_target.enemy_armor[i] = (Board_C.armor)EditorGUILayout.EnumPopup("armor vs gem " + i, my_target.enemy_armor[i]);
                                }
                                EditorGUI.indentLevel--;

                            }

                        }


                            my_target.use_armor = false;

                        EditorGUI.indentLevel--;
                        EditorGUILayout.Space();
                    }

                    EditorGUI.indentLevel--;
                }
            }
            else
                my_target.versus = false;



            EditorGUI.indentLevel = 0;

        }

        if (EditorGUI.EndChangeCheck())
        {
            EditorUtility.SetDirty(my_target);
        }
    }

    void Camera_setup()
    {
        Board_C my_target = (Board_C)target;

        EditorGUI.BeginChangeCheck();
        Undo.RecordObject(my_target, "Camera_setup");

        my_target.show_camera_setup = EditorGUILayout.Foldout(my_target.show_camera_setup, "Camera");
        if (my_target.show_camera_setup)
        {
            EditorGUI.indentLevel = 1;


            my_target.Board_camera = EditorGUILayout.ObjectField("Board camera: ", my_target.Board_camera, typeof(Camera), true) as Camera;

            EditorGUILayout.BeginHorizontal();
            my_target.camera_zoom = EditorGUILayout.FloatField("zoom", my_target.camera_zoom);
            if (my_target.camera_position_choice != Board_C.camera_position.centred_to_move)
                my_target.adaptive_zoom = EditorGUILayout.Toggle("adaptive zoom", my_target.adaptive_zoom);
            else
                my_target.adaptive_zoom = false;
            EditorGUILayout.EndHorizontal();

            my_target.Camera_adjust = EditorGUILayout.Vector3Field("adjust position", my_target.Camera_adjust);


            my_target.camera_position_choice = (Board_C.camera_position)EditorGUILayout.EnumPopup("Behavior", my_target.camera_position_choice);
            if (my_target.camera_position_choice == Board_C.camera_position.centred_to_move)
            {
                EditorGUI.indentLevel++;
                my_target.camera_speed = EditorGUILayout.FloatField("speed", my_target.camera_speed);
                my_target.camera_move_tolerance = EditorGUILayout.FloatField("tollerance", my_target.camera_move_tolerance);
                my_target.margin = (Vector2)EditorGUILayout.Vector2Field("margin", my_target.margin);
                EditorGUI.indentLevel--;
            }


            EditorGUI.indentLevel = 0;
            EditorGUILayout.Space();
        }
        if (EditorGUI.EndChangeCheck())
            EditorUtility.SetDirty(my_target);
    }

    void Assign_sprites()
    {
        Board_C my_target = (Board_C)target;
        EditorGUI.BeginChangeCheck();
        Undo.RecordObject(my_target, "Assign_sprites");

        my_target.show_sprites = EditorGUILayout.Foldout(my_target.show_sprites, "Sprites");
        if (my_target.show_sprites)
        {
            EditorGUI.indentLevel++;

            my_target.show_s_gems = EditorGUILayout.Foldout(my_target.show_s_gems, "Gems");
            if (my_target.show_s_gems)
            {
                EditorGUI.indentLevel++;
                if (my_target.gem_colors == null || my_target.gem_colors.Length != my_target.gem_length)
                    my_target.gem_colors = new Sprite[my_target.gem_length];

                for (int i = 0; i < my_target.gem_length; i++)
                {
                    if (!my_target.gem_colors[i])
                        GUI.color = Color.red;
                    else
                        GUI.color = Color.white;
                    my_target.gem_colors[i] = EditorGUILayout.ObjectField("gem n. " + i, my_target.gem_colors[i], typeof(Sprite), false) as Sprite;
                }
                GUI.color = Color.white;
                EditorGUI.indentLevel--;
            }

            my_target.show_s_padlocks = EditorGUILayout.Foldout(my_target.show_s_padlocks, "Padlock");
            if (my_target.show_s_padlocks)
            {
                EditorGUI.indentLevel++;
                for (int i = 0; i < 3; i++)
                {
                    if (!my_target.lock_gem_hp[i])
                        GUI.color = Color.red;
                    else
                        GUI.color = Color.white;
                    my_target.lock_gem_hp[i] = EditorGUILayout.ObjectField("padlock hp " + (i + 1), my_target.lock_gem_hp[i], typeof(Sprite), false) as Sprite;
                }
                GUI.color = Color.white;
                EditorGUI.indentLevel--;
            }

            my_target.show_s_blocks = EditorGUILayout.Foldout(my_target.show_s_blocks, "Blocks");
            if (my_target.show_s_blocks)
            {
                EditorGUI.indentLevel++;
                for (int i = 0; i < 3; i++)
                {
                    if (!my_target.block_hp[i])
                        GUI.color = Color.red;
                    else
                        GUI.color = Color.white;
                    my_target.block_hp[i] = EditorGUILayout.ObjectField("block hp " + (i + 1), my_target.block_hp[i], typeof(Sprite), false) as Sprite;
                }
                GUI.color = Color.white;
                EditorGUI.indentLevel--;
            }
            /*
				my_target.show_s_falling_blocks = EditorGUILayout.Foldout(my_target.show_s_falling_blocks, "Falling Blocks");
					if (my_target.show_s_falling_blocks)
						{
						EditorGUI.indentLevel++;
							for (int i = 0; i < 3 ; i ++)
							{
							if (!my_target.falling_block_hp[i])
									GUI.color = Color.red;
								else
									GUI.color = Color.white;
							my_target.falling_block_hp[i] = EditorGUILayout.ObjectField("falling block hp " + (i+1),my_target.falling_block_hp[i], typeof(Sprite), false) as Sprite;
							}
							EditorGUI.indentLevel--;
						}
*/
            my_target.show_s_misc = EditorGUILayout.Foldout(my_target.show_s_misc, "Misc");
            if (my_target.show_s_misc)
            {
                EditorGUI.indentLevel++;

                if (!my_target.junk)
                    GUI.color = Color.red;
                else
                    GUI.color = Color.white;
                my_target.junk = EditorGUILayout.ObjectField("junk", my_target.junk, typeof(Sprite), false) as Sprite;
                GUI.color = Color.white;

                if (!my_target.token)
                    GUI.color = Color.red;
                else
                    GUI.color = Color.white;
                my_target.token = EditorGUILayout.ObjectField("token", my_target.token, typeof(Sprite), false) as Sprite;
                GUI.color = Color.white;
                EditorGUI.indentLevel--;
            }

            EditorGUI.indentLevel--;
            EditorGUILayout.Space();
        }

        if (EditorGUI.EndChangeCheck())
            EditorUtility.SetDirty(my_target);
    }

    void Audio_sfx()
    {
        Board_C my_target = (Board_C)target;
        EditorGUI.BeginChangeCheck();
        Undo.RecordObject(my_target, "Audio_sfx");

        my_target.show_audio = EditorGUILayout.Foldout(my_target.show_audio, "Audio sfx");
        if (my_target.show_audio)
        {
            EditorGUI.indentLevel++;

            my_target.click_sfx_str = EditorGUILayout.TextField("click string", my_target.click_sfx_str);
            my_target.dialog_sfx_str = EditorGUILayout.TextField("dialog show string", my_target.dialog_sfx_str);
            my_target.button_sfx_str = EditorGUILayout.TextField("button string", my_target.button_sfx_str);
            my_target.sugar_out_sfx_str = EditorGUILayout.TextField("glucose out string", my_target.sugar_out_sfx_str);
            my_target.sugar_in_sfx_str = EditorGUILayout.TextField("glucose in string", my_target.sugar_in_sfx_str);
            my_target.explosion_sfx_str = EditorGUILayout.TextField("explosion string", my_target.explosion_sfx_str);
            my_target.end_fall_sfx_str = EditorGUILayout.TextField("end fall string", my_target.end_fall_sfx_str);
            my_target.bad_move_sfx_str = EditorGUILayout.TextField("bad move string", my_target.bad_move_sfx_str);
            my_target.win_sfx_str = EditorGUILayout.TextField("win string", my_target.win_sfx_str);
            my_target.lose_sfx_str = EditorGUILayout.TextField("lose string", my_target.lose_sfx_str);


            my_target.click_sfx = EditorGUILayout.ObjectField("click", my_target.click_sfx, typeof(AudioClip), false) as AudioClip;
            my_target.dialog_sfx = EditorGUILayout.ObjectField("dialog show", my_target.dialog_sfx, typeof(AudioClip), false) as AudioClip;
            my_target.button_sfx = EditorGUILayout.ObjectField("button", my_target.button_sfx, typeof(AudioClip), false) as AudioClip;
            my_target.sugar_out_sfx = EditorGUILayout.ObjectField("glucose out", my_target.sugar_out_sfx, typeof(AudioClip), false) as AudioClip;
            my_target.sugar_in_sfx = EditorGUILayout.ObjectField("glucose in", my_target.sugar_in_sfx, typeof(AudioClip), false) as AudioClip;
            my_target.explosion_sfx = EditorGUILayout.ObjectField("explosion", my_target.explosion_sfx, typeof(AudioClip), false) as AudioClip;
            my_target.end_fall_sfx = EditorGUILayout.ObjectField("end fall", my_target.end_fall_sfx, typeof(AudioClip), false) as AudioClip;
            my_target.bad_move_sfx = EditorGUILayout.ObjectField("bad move", my_target.bad_move_sfx, typeof(AudioClip), false) as AudioClip;
            my_target.win_sfx = EditorGUILayout.ObjectField("win", my_target.win_sfx, typeof(AudioClip), false) as AudioClip;
            my_target.lose_sfx = EditorGUILayout.ObjectField("lose", my_target.lose_sfx, typeof(AudioClip), false) as AudioClip;

            EditorGUI.indentLevel--;
        }

        if (EditorGUI.EndChangeCheck())
            EditorUtility.SetDirty(my_target);
    }

    void Visual_enhancements()
    {
        Board_C my_target = (Board_C)target;
        EditorGUI.BeginChangeCheck();
        Undo.RecordObject(my_target, "Visual_enhancements");

        my_target.show_visual_fx = EditorGUILayout.Foldout(my_target.show_visual_fx, "Visual enhancements");
        if (my_target.show_visual_fx)
        {
            EditorGUI.indentLevel++;

            my_target.gem_explosion_fx_rule_selected = (Board_C.gem_explosion_fx_rule)EditorGUILayout.EnumPopup("Gem explosion fx", my_target.gem_explosion_fx_rule_selected);
            EditorGUI.indentLevel++;
            if (my_target.gem_explosion_fx_rule_selected == Board_C.gem_explosion_fx_rule.for_each_gem)
            {
                EditorGUILayout.LabelField("gem explosion fx by color:");
                EditorGUI.indentLevel++;
                for (int i = 0; i < my_target.gem_length; i++)
                {
                    if (my_target.gem_explosion_fx[i] == null)
                        GUI.color = Color.red;
                    else
                        GUI.color = Color.white;
                    my_target.gem_explosion_fx[i] = EditorGUILayout.ObjectField("gem " + i, my_target.gem_explosion_fx[i], typeof(explosion_fx), true) as explosion_fx;
                }
                GUI.color = Color.white;
                EditorGUI.indentLevel--;
            }
            else if (my_target.gem_explosion_fx_rule_selected == Board_C.gem_explosion_fx_rule.only_for_big_explosion)
            {
                EditorGUILayout.LabelField("gem explosion fx by explosion magnitude:");
                EditorGUI.indentLevel++;
                for (int i = 0; i < 4; i++)
                {
                    if (my_target.gem_big_explosion_fx[i] == null)
                        GUI.color = Color.red;
                    else
                        GUI.color = Color.white;
                    my_target.gem_big_explosion_fx[i] = EditorGUILayout.ObjectField("explosion " + (i + 4), my_target.gem_big_explosion_fx[i], typeof(Transform), true) as Transform;
                }
                GUI.color = Color.white;
                EditorGUI.indentLevel--;
            }
            EditorGUI.indentLevel--;

            my_target.bonus_have_explosion_fx = EditorGUILayout.Toggle("Bonus fx", my_target.bonus_have_explosion_fx);
            if (my_target.bonus_have_explosion_fx)
            {
                EditorGUI.indentLevel++;

                if (my_target.destroy_one_fx == null)
                    GUI.color = Color.red;
                else
                    GUI.color = Color.white;
                my_target.destroy_one_fx = EditorGUILayout.ObjectField("destroy one", my_target.destroy_one_fx, typeof(Transform), true) as Transform;
                GUI.color = Color.white;

                if (my_target.destroy_3x3_fx == null)
                    GUI.color = Color.red;
                else
                    GUI.color = Color.white;
                my_target.destroy_3x3_fx = EditorGUILayout.ObjectField("destroy 3x3", my_target.destroy_3x3_fx, typeof(Transform), true) as Transform;
                GUI.color = Color.white;

                if (my_target.destroy_horizontal_fx == null)
                    GUI.color = Color.red;
                else
                    GUI.color = Color.white;
                my_target.destroy_horizontal_fx = EditorGUILayout.ObjectField("destroy horizontal", my_target.destroy_horizontal_fx, typeof(Transform), true) as Transform;
                GUI.color = Color.white;

                if (my_target.destroy_vertical_fx == null)
                    GUI.color = Color.red;
                else
                    GUI.color = Color.white;
                my_target.destroy_vertical_fx = EditorGUILayout.ObjectField("destroy vertical", my_target.destroy_vertical_fx, typeof(Transform), true) as Transform;
                GUI.color = Color.white;

                if (my_target.destroy_horizontal_and_vertical_fx == null)
                    GUI.color = Color.red;
                else
                    GUI.color = Color.white;
                my_target.destroy_horizontal_and_vertical_fx = EditorGUILayout.ObjectField("destroy horizontal and vertical", my_target.destroy_horizontal_and_vertical_fx, typeof(Transform), true) as Transform;
                GUI.color = Color.white;

                EditorGUI.indentLevel--;
            }

            my_target.praise_the_player = EditorGUILayout.Toggle("Praise the player", my_target.praise_the_player);
            if (my_target.praise_the_player)
            {
                EditorGUI.indentLevel++;

                my_target.for_big_explosion = EditorGUILayout.Toggle("for big explosion", my_target.for_big_explosion);
                my_target.for_secondary_explosions = EditorGUILayout.Toggle("for secondary explosions", my_target.for_secondary_explosions);
                my_target.for_explode_same_color_again = EditorGUILayout.Toggle("for explode same color again", my_target.for_explode_same_color_again);
                my_target.for_gain_a_turn = EditorGUILayout.Toggle("for gain a turn", my_target.for_gain_a_turn);
                if (my_target.continue_to_play_after_win_until_lose_happen && my_target.Stage_uGUI_obj)
                    my_target.for_gain_a_star = EditorGUILayout.Toggle("for gain a star", my_target.for_gain_a_star);
                else
                    my_target.for_gain_a_star = false;

                EditorGUI.indentLevel--;
            }


            EditorGUI.indentLevel--;
        }

        if (EditorGUI.EndChangeCheck())
            EditorUtility.SetDirty(my_target);
    }

    void Advanced()
    {
        Board_C my_target = (Board_C)target;
        EditorGUI.BeginChangeCheck();
        Undo.RecordObject(my_target, "Advanced");

        my_target.show_advanced = EditorGUILayout.Foldout(my_target.show_advanced, "Advanced");
        if (my_target.show_advanced)
        {
            EditorGUI.indentLevel++;

            my_target.gem_length = EditorGUILayout.IntSlider("gem colors", my_target.gem_length, 3, 7);
            my_target.diagonal_falling = EditorGUILayout.Toggle("diagonal falling", my_target.diagonal_falling);
            if (my_target.versus || my_target.win_requirement_selected == Board_C.win_requirement.destroy_all_gems)
               my_target.player_can_move_when_gem_falling = false;
            else
               my_target.player_can_move_when_gem_falling = EditorGUILayout.Toggle("player can move when gem falling", my_target.player_can_move_when_gem_falling);
 

            my_target.show_advanced_timing = EditorGUILayout.Foldout(my_target.show_advanced_timing, "animations timing");
            if (my_target.show_advanced_timing)
            {
                EditorGUI.indentLevel++;
                //my_target.duration_switch_gems_move_in_seconds = EditorGUILayout.FloatField("switch duration", my_target.duration_switch_gems_move_in_seconds );

                if ((my_target.switch_speed <= 0) || (my_target.switch_speed > 25))
                    GUI.color = Color.red;
                else
                    GUI.color = Color.white;
                my_target.switch_speed = EditorGUILayout.FloatField("switch speed", my_target.switch_speed);
                GUI.color = Color.white;

                if ((my_target.falling_speed <= 0) || (my_target.falling_speed > 50))
                    GUI.color = Color.red;
                else
                    GUI.color = Color.white;
                my_target.falling_speed = EditorGUILayout.FloatField("falling speed", my_target.falling_speed);
                GUI.color = Color.white;

                if ((my_target.falling_accel <= 0) || (my_target.falling_accel > 50))
                    GUI.color = Color.red;
                else
                    GUI.color = Color.white;
                my_target.falling_accel = EditorGUILayout.FloatField("falling acceleration", my_target.falling_accel);
                GUI.color = Color.white;
                

                if ((my_target.accuracy <= 0) || (my_target.accuracy >= 1))
                    GUI.color = Color.red;
                else
                    GUI.color = Color.white;
                my_target.accuracy = EditorGUILayout.FloatField("accuracy", my_target.accuracy);
                GUI.color = Color.white;

                if (GUILayout.Button("Reset to default values"))
                {
                    my_target.switch_speed = 5f;
                    my_target.falling_speed = 11f;
                    my_target.accuracy = 0.2f;
                }
                EditorGUI.indentLevel--;
            }


            my_target.show_advanced_ugui = EditorGUILayout.Foldout(my_target.show_advanced_ugui, "gui elements");
            if (my_target.show_advanced_ugui)
            {
                EditorGUI.indentLevel++;
                my_target.gui_info_screen = EditorGUILayout.ObjectField("info screen", my_target.gui_info_screen, typeof(GameObject), true) as GameObject;

                my_target.gui_win_screen = EditorGUILayout.ObjectField("win screen", my_target.gui_win_screen, typeof(GameObject), true) as GameObject;
                my_target.gui_lose_screen = EditorGUILayout.ObjectField("lose screen", my_target.gui_lose_screen, typeof(GameObject), true) as GameObject;


                my_target.gui_timer = EditorGUILayout.ObjectField("time bar", my_target.gui_timer, typeof(GameObject), true) as GameObject;
                my_target.cursor = EditorGUILayout.ObjectField("cursor", my_target.cursor, typeof(Transform), true) as Transform;
                my_target.my_hint = EditorGUILayout.ObjectField("hint", my_target.my_hint, typeof(Transform), true) as Transform;



                EditorGUILayout.LabelField("player");
                EditorGUI.indentLevel++;
                my_target.gui_player_score = EditorGUILayout.ObjectField("score", my_target.gui_player_score, typeof(Text), true) as Text;

                EditorGUI.indentLevel--;

                EditorGUI.indentLevel--;

                //EditorGUI.indentLevel--;
            }

            my_target.show_garbages = EditorGUILayout.Foldout(my_target.show_garbages, "garbage");
            if (my_target.show_garbages)
            {
                EditorGUI.indentLevel++;
                if (my_target.garbage_recycle == null)
                    GUI.color = Color.red;
                else
                    GUI.color = Color.white;
                my_target.garbage_recycle = EditorGUILayout.ObjectField("gem garbage", my_target.garbage_recycle, typeof(Transform), true) as Transform;
                GUI.color = Color.white;
                EditorGUILayout.Space();


                EditorGUI.indentLevel--;
            }

            my_target.show_menu_kit_setup = EditorGUILayout.Foldout(my_target.show_menu_kit_setup, "Menu kit");
            if (my_target.show_menu_kit_setup)
            {
                my_target.Stage_uGUI_obj = EditorGUILayout.ObjectField("Stage_uGUI", my_target.Stage_uGUI_obj, typeof(GameObject), true) as GameObject;
                if (my_target.Stage_uGUI_obj != null)
                {
                    if (my_target.continue_to_play_after_win_until_lose_happen)
                    {
                        if (my_target.win_requirement_selected == Board_C.win_requirement.reach_target_score)
                            my_target.use_star_progress_bar = EditorGUILayout.Toggle("use star progress bar", my_target.use_star_progress_bar);
                        else
                        {
                            if (my_target.win_requirement_selected == Board_C.win_requirement.collect_gems
                                && my_target.two_stars_target_score == 0 && my_target.three_stars_target_score == 0)
                                my_target.use_star_progress_bar = EditorGUILayout.Toggle("use star progress bar", my_target.use_star_progress_bar);
                            else
                                my_target.use_star_progress_bar = false;
                        }
                    }
                    else
                        my_target.use_star_progress_bar = false;


                    EditorGUILayout.LabelField("2 stars target:");
                    EditorGUI.indentLevel++;
                    my_target.two_stars_target_score = EditorGUILayout.IntField("score", my_target.two_stars_target_score);
                    if (my_target.continue_to_play_after_win_until_lose_happen)
                    {
                        if (my_target.win_requirement_selected == Board_C.win_requirement.collect_gems)
                            my_target.two_stars_target_additional_gems_collected = EditorGUILayout.IntField("additional gems collected", my_target.two_stars_target_additional_gems_collected);
                        else
                            my_target.two_stars_target_additional_gems_collected = 0;
                    }
                    else
                    {
                        my_target.two_stars_target_additional_gems_collected = 0;

                        if (my_target.lose_requirement_selected == Board_C.lose_requirement.enemy_reach_target_score)
                            my_target.two_stars_target_score_advantage_vs_enemy = EditorGUILayout.IntField("score advantage on enemy", my_target.two_stars_target_score_advantage_vs_enemy);

                        if (my_target.lose_requirement_selected == Board_C.lose_requirement.player_hp_is_zero)
                            my_target.two_stars_target_player_hp_spared = EditorGUILayout.IntField("HP spared", my_target.two_stars_target_player_hp_spared);

                        if (my_target.lose_requirement_selected == Board_C.lose_requirement.timer)
                            my_target.two_stars_target_time_spared = EditorGUILayout.FloatField("time spared", my_target.two_stars_target_time_spared);

                        if (my_target.lose_requirement_selected == Board_C.lose_requirement.player_have_zero_moves)
                            my_target.two_stars_target_moves_spared = EditorGUILayout.IntField("moves spared", my_target.two_stars_target_moves_spared);

                        if (my_target.lose_requirement_selected == Board_C.lose_requirement.enemy_collect_gems)
                            my_target.two_stars_target_gems_collect_advantage_vs_enemy = EditorGUILayout.IntField("gems collected advantage on enemy", my_target.two_stars_target_gems_collect_advantage_vs_enemy);
                    }
                    EditorGUI.indentLevel--;


                    EditorGUILayout.LabelField("3 stars target:");
                    EditorGUI.indentLevel++;

                    my_target.three_stars_target_score = EditorGUILayout.IntField("score", my_target.three_stars_target_score);

                    if (my_target.continue_to_play_after_win_until_lose_happen)
                    {
                        if (my_target.win_requirement_selected == Board_C.win_requirement.collect_gems)
                            my_target.three_stars_target_additional_gems_collected = EditorGUILayout.IntField("additional gems collected", my_target.three_stars_target_additional_gems_collected);
                        else
                            my_target.three_stars_target_moves_spared = 0;
                    }
                    else
                    {
                        my_target.three_stars_target_moves_spared = 0;


                        if (my_target.lose_requirement_selected == Board_C.lose_requirement.enemy_reach_target_score)
                            my_target.three_stars_target_score_advantage_vs_enemy = EditorGUILayout.IntField("score advantage on enemy", my_target.three_stars_target_score_advantage_vs_enemy);

                        if (my_target.lose_requirement_selected == Board_C.lose_requirement.player_hp_is_zero)
                            my_target.three_stars_target_player_hp_spared = EditorGUILayout.IntField("HP spared", my_target.three_stars_target_player_hp_spared);

                        if (my_target.lose_requirement_selected == Board_C.lose_requirement.timer)
                            my_target.three_stars_target_time_spared = EditorGUILayout.FloatField("time spared", my_target.three_stars_target_time_spared);

                        if (my_target.lose_requirement_selected == Board_C.lose_requirement.player_have_zero_moves)
                            my_target.three_stars_target_moves_spared = EditorGUILayout.IntField("moves spared", my_target.three_stars_target_moves_spared);

                    }

                    if (my_target.lose_requirement_selected == Board_C.lose_requirement.enemy_collect_gems)
                        my_target.three_stars_target_gems_collect_advantage_vs_enemy = EditorGUILayout.IntField("gems collected advantage on enemy", my_target.three_stars_target_gems_collect_advantage_vs_enemy);

                    EditorGUI.indentLevel--;
                }
                else
                    my_target.use_star_progress_bar = false;

            }
        }

        if (EditorGUI.EndChangeCheck())
            EditorUtility.SetDirty(my_target);
    }

    void Show_gui()
    {
        Board_C my_target = (Board_C)target;

        #region lose requirements
        if (my_target.lose_requirement_selected == Board_C.lose_requirement.timer)
            my_target.gui_timer.SetActive(true);
        else
            my_target.gui_timer.SetActive(false);
        #endregion

        #region win requirements
        #endregion


    }
}
