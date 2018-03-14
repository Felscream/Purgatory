using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingPlatform : MonoBehaviour {

    public float height = -1.8f; //lowest position
    private bool canDealDamage = true;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (transform.position.y <= -3.5) // on the ground
        {
            canDealDamage = false;
        }
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        float facing = collision.transform.position.x.CompareTo(0); // -1 if <0, 0 if ==0 and 1 if >0
        int damage = (int) (height - collision.transform.position.y)*10;
        Vector2 recoil = new Vector2(facing * 5, -1);
        Debug.Log(damage);
        if (canDealDamage && collision.gameObject.GetComponent<Champion>() != null)
        {
            collision.gameObject.GetComponent<Champion>().ApplyDamage(damage, facing, 0, recoil, true);
        }
    }
}
