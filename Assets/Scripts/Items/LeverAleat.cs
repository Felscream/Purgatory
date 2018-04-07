using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeverAleat : Lever {

    public List<GameObject> trapList;
    private GameObject thirdTrap = null;

    public override void Engage()
    {
        trap = trapList[Random.Range(0, trapList.Count)];
        do
        {
            secondTrap = trapList[Random.Range(0, trapList.Count)];
        } while (secondTrap == trap && trapList.Count >= 2);
        do
        {
            thirdTrap = trapList[Random.Range(0, trapList.Count)];
        } while ((thirdTrap == trap || thirdTrap == secondTrap) && trapList.Count >= 3);

        EngageOtherTraps();

        base.Engage();
    }

    public override void Disengage()
    {
        if (secondTrap != null)
            secondTrap.SetActive(false);
        if (thirdTrap != null)
            thirdTrap.SetActive(false);

        base.Disengage();
    }

    private void EngageOtherTraps()
    {

        if (secondTrap != null)
        { 
            secondTrap.GetComponent<Trap>().Engage();
            secondTrap.SetActive(true);
        }
        if (thirdTrap != null)
        {
            thirdTrap.GetComponent<Trap>().Engage();
            thirdTrap.SetActive(true);
        }
    }
}
