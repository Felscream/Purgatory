using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manabomb : Projectile {
    [SerializeField] private Vector2 altForce;
    public Vector2 AltForce
    {
        get
        {
            return altForce;
        }
    }

}
