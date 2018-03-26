using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Champion champ = collision.GetComponent<Champion>();
        Transform platform = transform;
        if ( champ != null)
        {
            if ((!champ.IsFalling() && !champ.IsGrounded()) || champ.IgnorePlatforms)
            {
                Physics2D.IgnoreCollision(collision.GetComponent<Collider2D>(), platform.GetComponent<BoxCollider2D>(), true);
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        Champion champ = collision.GetComponent<Champion>();
        Transform platform = transform;
        if (champ != null)
        {
            Physics2D.IgnoreCollision(collision.GetComponent<Collider2D>(), platform.GetComponent<BoxCollider2D>(), false);
            champ.IgnorePlatforms = false;
        }
    }
}
