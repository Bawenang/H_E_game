using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour {

    GameObject current_obj;
    GameObject previous_obj;

    Board_C board;

    float cameraZ;

    //[SerializeField]
    //private bool _isUsingTouch = true;

    // Use this for initialization
    void Start () {
        board = GetComponent<Board_C>();
        cameraZ = Camera.main.transform.position.z - board.pivot_board.transform.position.z;
    }
	
	// Update is called once per frame
	void Update() {

        if (board.game_end)
            return;

        //if (Input.GetMouseButtonUp(0))
        //    MouseUp();

        if (EventSystem.current.IsPointerOverGameObject())//don't click through UI
            return;

        bool isTouch = false;
        bool isMouse = false;
        Vector2 inputScreenPos = Vector2.zero;
        Vector3 inputWorldPos = Vector3.zero;

        //if (_isUsingTouch && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began )
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            isTouch = true;

        //else if (!_isUsingTouch && Input.GetMouseButtonDown(0))
        else if (Input.GetMouseButtonDown(0))
            isMouse = true;

        if (isTouch || isMouse)
        {
            if (isTouch)
            {
                inputScreenPos= Input.GetTouch(0).position;
                inputWorldPos = new Vector3(inputScreenPos.x, inputScreenPos.y, cameraZ);
                inputWorldPos = Camera.main.ScreenToWorldPoint(inputWorldPos);
            }
            else
            {
                inputScreenPos = Input.mousePosition;
                inputWorldPos = new Vector3(inputScreenPos.x, inputScreenPos.y, cameraZ);
                inputWorldPos = Camera.main.ScreenToWorldPoint(inputWorldPos);
            }

            if (IsOnBoard(inputWorldPos))
            {
                tile_C tile = GetTileFromBoardAt(inputWorldPos);

                tile.MyOnMouseDown();
                //tile.my_gem_renderer.color = Color.red;
            }
        }

        //Ray ray3d = Camera.main.ScreenPointToRay(Input.mousePosition);
        //RaycastHit hitInfo3d;
        //if (Physics.Raycast(ray3d, out hitInfo3d))
        //{
        //    GameObject thisObj = hitInfo3d.collider.transform.gameObject;

        //    if (thisObj.GetComponent<tile_C>() != null)
        //        MouseOver_Tile(thisObj);

        //}

       

}
    void MouseUp()
    {
        board.touch_number--;
        if (board.touch_number < 0)
            board.touch_number = 0;
    }

    void MouseOver_Tile(GameObject thisObj)
    {
        
        //mouse enter and exit
        current_obj = thisObj;
        if (current_obj != previous_obj)
        {
            if (previous_obj != null)
                previous_obj.GetComponent<tile_C>().MyOnMouseExit();

            current_obj.GetComponent<tile_C>().MyOnMouseEnter();

            previous_obj = current_obj;
        }

        //mouse click
        if (Input.GetMouseButtonDown(0))
            thisObj.GetComponent<tile_C>().MyOnMouseDown();

    }

    bool IsOnBoard(Vector3 position)
    {
        if (position.x >= board.topLeftBoardPoint.x && position.x <= board.bottomRightBoardPoint.x &&
            position.y <= board.topLeftBoardPoint.y && position.y >= board.bottomRightBoardPoint.y)
        {
            return true;
        }

        return false;

    }

    tile_C GetTileFromBoardAt(Vector3 position)
    {
        int x = Mathf.FloorToInt(position.x - board.topLeftBoardPoint.x);
        int y = Mathf.FloorToInt(board.topLeftBoardPoint.y - position.y);

        return board.tiles_array[x, y];
    }
}
