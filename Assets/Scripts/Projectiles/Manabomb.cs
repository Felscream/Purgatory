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

    protected override void SetImpact()
    {
        base.SetImpact();
        Destroy(transform.GetChild(0).gameObject);
        GameObject newps = Instantiate(explosionEffect, this.transform);
        newps.transform.localPosition = newps.transform.localPosition * direction;
        sr.enabled = false;
        Destroy(gameObject, 1.2f);
    }
}
