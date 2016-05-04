using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class WelcomeMenu : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    // Load Scenes
    public void LoadScene(int level)
    {
        SceneManager.LoadScene(level);
    }
}
