using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingPlatform : MonoBehaviour {

    public float height = -1.8f; //lowest position
    private bool canDealDamage = true;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        float facing = collision.transform.position.x.CompareTo(0); // -1 if <0, 0 if ==0 and 1 if >0
        int damage = (int) (height - collision.transform.position.y)*10;
        Vector2 recoil = new Vector2(facing * 5, -1);
        Champion champion = collision.GetComponent<Champion>();
        Debug.Log(transform.position.y +" "+ collision.name);

        if (canDealDamage && champion != null)
        {
            Debug.Log(damage + " ici " + facing);
            champion.ApplyDamage(damage, facing, 0, recoil, true);
        }

        if (collision.tag == "obstacle") // on the ground
        {
            canDealDamage = false;
        }
    }
}
