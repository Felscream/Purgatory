using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Shockwave : SimpleShockwave
{
    [SerializeField] protected float chromaticAberDuration = 0.2f;
    protected override void Start()
    {
        base.Start();
        ManagerInGame.GetInstance().StartCoroutine(ManagerInGame.GetInstance().ChromaticAberration(chromaticAberDuration));
    }
}
