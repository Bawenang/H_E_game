using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public partial class Board_C : MonoBehaviour {


    public string click_sfx_str;
    public string dialog_sfx_str;
    public string button_sfx_str;
    public string sugar_out_sfx_str;
    public string sugar_in_sfx_str;
    public string explosion_sfx_str;
    public string end_fall_sfx_str;
    public string bad_move_sfx_str;
    public string win_sfx_str;
    public string lose_sfx_str;

    public AudioClip click_sfx;
    public AudioClip dialog_sfx;
    public AudioClip button_sfx;
    public AudioClip sugar_out_sfx;
    public AudioClip sugar_in_sfx;
    public AudioClip explosion_sfx;
    public AudioClip end_fall_sfx;
    public float latest_sfx_time;
    public AudioClip bad_move_sfx;
    public AudioClip win_sfx;
    public AudioClip lose_sfx;
    public AudioClip[] bonus_sfx;
    public int play_this_bonus_sfx = -1; //-1 = don't play

    public List<AudioSource> audioSrcList = new List<AudioSource>();

    public void Play_sfx(string my_clip_str)
    {
        Debug.Log("Play sfx = " + my_clip_str);
        if (LoLController.Exists() && LoLController.instance.isUsingLoL)
            LoLSDK.LOLSDK.Instance.PlaySound(my_clip_str);
    }

    public void Play_sfx(AudioClip my_clip)
    {
        if (Stage_uGUI_obj)//use menu kit win screen
        {
            /* if you have the menu kit, DELETE THIS LINE
			if (my_game_master && my_clip)
				my_game_master.Gui_sfx(my_clip);
			//if you have the menu kit, DELETE THIS LINE */
        }
        else
        {
            if (my_clip)
            {
                AudioSource src = audioSrcList.Find(x => !x.isPlaying);
                if (src == null)
                {
                    src = gameObject.AddComponent<AudioSource>();
                    src.priority = 0;
                    src.spatialBlend = 0.0f;
                    src.volume = 1.0f;
                    src.pitch = 1.0f;
                    src.panStereo = 0.0f;
                    src.loop = false;
                    audioSrcList.Add(src);

                    if (AudioController.Exists())
                        src.mute = !AudioController.instance.isSound;
                }
                src.clip = my_clip;
                src.Play();
            }
        }
    }

    public void Play_bonus_sfx(int bonus_id, bool call_from_GUIbutton = false)
    {
        if (bonus_sfx[bonus_id] != null)
        {
            play_this_bonus_sfx = bonus_id;
            Play_sfx(bonus_sfx[bonus_id]);
        }
        else
            play_this_bonus_sfx = -1;

        if (call_from_GUIbutton)
            play_this_bonus_sfx = -1;

    }
}
