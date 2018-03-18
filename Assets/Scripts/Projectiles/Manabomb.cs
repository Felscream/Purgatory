using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manabomb : Projectile {
    [SerializeField] private Vector2 altForce;
    [SerializeField] private GameObject travelEffect;
    [SerializeField] private GameObject explosionEffect;
    public Vector2 AltForce
    {
        get
        {
            return altForce;
        }
    }

    public override void DestroyProjectile()
    {
        ParticleSystem.EmissionModule temp = ps.emission;
        temp.enabled = false;
        GetComponent<Collider2D>().enabled = false;
        rb.velocity = Vector2.zero;
        sr.sortingOrder = -3;
        sr.enabled = false;
        GameObject newps = Instantiate(explosionEffect, this.transform);
        newps.transform.localPosition = newps.transform.localPosition * direction;
        Destroy(gameObject, timeToDestroy);
    }

    protected override void SetImpact()
    {
        base.SetImpact();
        GameObject newps = Instantiate(explosionEffect, this.transform);
        newps.transform.localPosition = newps.transform.localPosition * direction;
        Destroy(gameObject, timeToDestroy);
    }
}
