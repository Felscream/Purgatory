using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour {

    [SerializeField] private Vector2 positionOffset = new Vector2(0.5f, -1);
    // Use this for initialization
    private List<Champion> hits = new List<Champion>();
    private Champion owner;
    private float facing;
    private Collider2D colliderBox;
    private void Awake()
    {
        owner = transform.parent.GetComponent<Champion>();
        if (owner == null)
        {
            Debug.LogError("Owner not assigned to HitBox component of " + gameObject.name);
        }
    }

    private void Update()
    {
        if (!owner.Dead)
        {
            facing = owner.Facing;
            Vector2 pos = new Vector2(Mathf.Abs(positionOffset.x) * facing, positionOffset.y);
            transform.localPosition = pos;
        }
        
    }
    private void OnEnable()
    {
        if (!owner.Dead)
        {
            hits.Clear(); //reset list
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if(owner != null && !owner.Dead)
        {
            Champion foe = collider.GetComponent<Champion>();
            if (foe != null && foe != owner)
            {
                foreach(Champion h in hits)
                {
                    if(h == foe) //if we already hit this player, we do not deal damage
                    {
                        return;
                    }
                }
                Debug.Log(foe.gameObject.name);
                foe.ApplyDamage(owner.GuardBreakDamage, owner.Facing, owner.GuardBreakStunLock, owner.GuardBreakRecoilForce, true);
                hits.Add(foe);
            }
        }
        
    }
}
