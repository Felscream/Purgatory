using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayerScript : MonoBehaviour {

    public Vector3 offset = new Vector3(0,2,0);

    Transform player;   // the parent : Player 1/2/3/4
    Transform champ;    // the champion : Knight, archer, sorcer

	// Use this for initialization
	void Awake () {

        player = this.transform.parent;
        for (int i = 0; i < player.childCount; i++)
        {
            if (player.GetChild(i).tag == "Champion")
            {
                champ = player.GetChild(i);
            }
        }
    }
	
	// Update is called once per frame
	void Update () {
        transform.position = champ.position + offset;
	}
}
