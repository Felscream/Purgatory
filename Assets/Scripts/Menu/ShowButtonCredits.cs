using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowButtonCredits : MonoBehaviour {

    public Button button;

	// Use this for initialization
	void Start () {
        button.gameObject.SetActive(false);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void ShowButton()
    {
        button.gameObject.SetActive(true);
    }
}
