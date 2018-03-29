using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeverAleat : Lever {

    public List<GameObject> trapList;

    public override void Engage()
    {
        trap = trapList[Random.Range(0, trapList.Count)];
        base.Engage();
    }
}
