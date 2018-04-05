using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuPause : MonoBehaviour {

    private bool onDisplay = false;
    private Transform pauseMenu;

	// Use this for initialization
	void Start () {
        pauseMenu = transform.GetChild(0);
        pauseMenu.gameObject.SetActive(false);
    }
	
	// Update is called once per frame
	void Update () {

		if (Input.GetButtonDown("Start"))
        {
            if (!onDisplay)
            {
                OnPause();
            }
            else
            {
                OnUnpause();
            }
        }
	}

    private void OnPause()
    {
        onDisplay = true;
        pauseMenu.gameObject.SetActive(true);

        Time.timeScale = 0.00001f;
        Cursor.visible = true;
    }

    private void OnUnpause()
    {
        onDisplay = false;
        pauseMenu.gameObject.SetActive(false);

        Time.timeScale = 1f;
        Cursor.visible = false;
    }
}
