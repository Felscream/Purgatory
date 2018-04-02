using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RefreshControllers : MonoBehaviour {

    void Update()
    {
        ControllerManager.Instance.Refresh();
    }
}
