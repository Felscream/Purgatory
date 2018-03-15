using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingPlatform : MonoBehaviour {

    public float height = -1.8f; //lowest position
    private bool canDealDamage = true;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        float facing = collision.transform.position.x.CompareTo(0); // -1 if <0, 0 if ==0 and 1 if >0
        // les dommages seront proportionnels à la hauteur de chute
        int damage = (int) (height - collision.transform.position.y);
        Vector2 recoil = new Vector2(facing * 10, 2);
        Champion champion = collision.GetComponent<Champion>();

        if (canDealDamage && champion != null)
        {
            Physics2D.IgnoreCollision(collision, this.GetComponent<Collider2D>());
            Debug.Log(damage + " infligés à " + collision.gameObject.name);
            champion.ApplyDamage(damage, facing, 20, recoil);
        }

        if (collision.name == "ground") // on the ground
        {
            canDealDamage = false;
        }
    }
}
