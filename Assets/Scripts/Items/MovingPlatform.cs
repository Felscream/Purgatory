using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour {

    public float xMin = -6.5f, xMax = 6.5f;
    public float speed = 2;

    private bool forward = true;

	// Use this for initialization
	void Start () {
        if (transform.localPosition.x > xMax)
            transform.localPosition = new Vector3(xMin, transform.position.y, 1);
	}
	
	// Update is called once per frame
	void Update () {
		if (forward)
        {
            transform.Translate(speed * Time.deltaTime, 0, 0);
        } else
        {
            transform.Translate(- speed * Time.deltaTime, 0, 0);
        }

        if (transform.localPosition.x >= xMax)
            forward = false;
        if (transform.localPosition.x <= xMin)
            forward = true;
	}
}
