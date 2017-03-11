using UnityEngine;
using System.Collections;

public partial class Board_C : MonoBehaviour{

    public enum gem_emitter
    {
        off,
        normal,
        special
    }
    public gem_emitter gem_emitter_rule = gem_emitter.normal;
    public int create_a_special_element_each_n_gems_created;
    public int count_gems_created;
    public int chance_to_create_a_special_element;
    public int[] bonus_creation_chances_weight;
    int[] creation_weight_chances_deck;//the sum of token + junk + bonuses
    public int token_creation_chance_weight;
    public bool emit_token_only_after_all_tiles_are_destroyed;
    public bool allow_token_emission;
    public int junk_creation_chance_weight;

    void Initiate_emitter_variables()// call from: Board_C.Initiate_board.Initiate_variables()
    {
        if (gem_emitter_rule == gem_emitter.special)
        {
            int total_weight = token_creation_chance_weight + junk_creation_chance_weight;
            for (int i = 0; i < bonus_creation_chances_weight.Length; i++)
                total_weight += bonus_creation_chances_weight[i];

            //put all the bonus weight in one array
            creation_weight_chances_deck = new int[total_weight];
            int count = 0;
            for (int i = 0; i < bonus_creation_chances_weight.Length; i++)
            {
                if (bonus_creation_chances_weight[i] > 0)
                {
                    for (int b = 0; b < bonus_creation_chances_weight[i]; b++)
                    {
                        creation_weight_chances_deck[count] = i;
                        count++;
                    }
                }
            }
            //add the token weight
            if (token_creation_chance_weight > 0)
            {
                for (int i = 0; i < token_creation_chance_weight; i++)
                {
                    creation_weight_chances_deck[count] = -200; //-200 = token
                    count++;
                }
            }
            //add the junk weight 
            if (junk_creation_chance_weight > 0)
            {
                for (int i = 0; i < junk_creation_chance_weight; i++)
                {
                    creation_weight_chances_deck[count] = -100; //-100 = junk
                    count++;
                }
            }
            for (int i = 0; i < creation_weight_chances_deck.Length; i++)
                print(creation_weight_chances_deck[i]);

        }
        if (token_creation_chance_weight > 0)
        {
            if (emit_token_only_after_all_tiles_are_destroyed)
                allow_token_emission = false;
            else
                allow_token_emission = true;
        }
    }

    public bool Emit_special_element()
    {
        if (gem_emitter_rule == gem_emitter.special && count_gems_created >= create_a_special_element_each_n_gems_created)
        {
            count_gems_created = 0;
            return true;
        }
        else
            return false;
    }

    public int Random_choose_special_element_to_create()
    {
        int temp = 0;

        temp = creation_weight_chances_deck[Random.Range(0, creation_weight_chances_deck.Length)];

        if (temp == -200 && (!allow_token_emission || number_of_token_collected >= number_of_token_to_collect))//if you have pick a token but cant emit it now, ignore it
            temp = 0;

        //print("Emit: " + temp);
        return temp;  
    }
}
