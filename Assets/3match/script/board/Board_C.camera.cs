using UnityEngine;
using System.Collections;

public partial class Board_C : MonoBehaviour{

    private bool isMoveCamera = false;

    public Camera Board_camera;
    public Vector3 Camera_adjust;
    public float camera_zoom;
    public bool adaptive_zoom;

    Vector3 displacement;
    Vector3 direction;

    public enum camera_position
    {
        centred_to_board,
        centred_to_move,
        centred_to_player_avatar,
        manual
    }
    public camera_position camera_position_choice = camera_position.centred_to_board;
    //centred_to_move:
    public float camera_speed;
    float new_camera_position_x;
    float new_camera_position_y;
    Vector3 new_camera_position;
    public float camera_move_tolerance;
    public Vector2 margin;

    void Setup_camera()//call from Awake
    {
        if (Board_camera)
        {
            if (Stage_uGUI_obj)//avoid 2 AudioListener when use menu kit
                Board_camera.GetComponent<AudioListener>().enabled = false;
            else
                Board_camera.GetComponent<AudioListener>().enabled = true;

            if (camera_position_choice == camera_position.centred_to_board)
            {
                Board_camera.transform.position = new Vector3(pivot_board.transform.position.x + (_X_tiles - 1) * 0.5f + Camera_adjust.x,
                                                               pivot_board.transform.position.y + (_Y_tiles - 1) * -0.5f + Camera_adjust.y,
                                                               pivot_board.transform.position.z + Camera_adjust.z
                                                               );
            }
            else if (camera_position_choice == camera_position.centred_to_move)
            {
                Board_camera.transform.position = (pivot_board.transform.position + Camera_adjust);
            }

            if (adaptive_zoom)
            {
                Board_camera.orthographicSize = (_Y_tiles + (camera_zoom * -2)) * 0.5f;
            }
            else
            {
                Board_camera.orthographicSize = 4 + camera_zoom * -1;
            }

            if (Board_camera.orthographicSize <= 0)
                Board_camera.orthographicSize = 1;
        }
    }


    void Center_camera_to_move()// call from: SwitchingGems()
    {
        if (camera_position_choice == camera_position.centred_to_move)
        {

            if (Vector2.Distance(new Vector2(main_gem_selected_x, main_gem_selected_y), new Vector2(new_camera_position_x, Mathf.Abs(new_camera_position_y))) >= camera_move_tolerance)
            {

                new_camera_position_x = main_gem_selected_x - pivot_board.position.x;
                if (new_camera_position_x < margin.x)
                    new_camera_position_x = margin.x;
                else if (new_camera_position_x > _X_tiles - margin.x)
                    new_camera_position_x = _X_tiles - margin.x;

                new_camera_position_y = pivot_board.position.y - main_gem_selected_y;
                if (new_camera_position_y * -1 < margin.y)
                    new_camera_position_y = margin.y * -1;
                else if (new_camera_position_y * -1 > _Y_tiles - margin.y)
                    new_camera_position_y = (_Y_tiles - margin.y) * -1;


                new_camera_position = new Vector3(new_camera_position_x, new_camera_position_y, Board_camera.transform.position.z);

                SetMoveCamera();
            }
        }
    }

    public void SetMoveCamera()
    {
        displacement = new_camera_position - Board_camera.gameObject.transform.position;
        direction = displacement.normalized;
        isMoveCamera = true;
    }

    void Move_Camera()
    {
        displacement = new_camera_position - Board_camera.gameObject.transform.position;
        float angleDisplacement = Vector3.Angle(direction, displacement.normalized);
        if (angleDisplacement < 1.0f && displacement.sqrMagnitude > accuracySquare * 10)
        {
            Board_camera.gameObject.transform.Translate(direction * camera_speed * Time.deltaTime, Space.World);
        }
        else
        {
            isMoveCamera = false;
        }

    }
}
