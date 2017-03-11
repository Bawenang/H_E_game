using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class home_buttons : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
    public void OpenThisScene(string thisScene)
    {
        SceneManager.LoadScene(thisScene);
    }
   
}
