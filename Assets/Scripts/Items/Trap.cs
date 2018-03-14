using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour {

    public Vector3 startPosition;
    public Vector3 endPosition;
    public float movingSpeed = 1f;
    public float rotationSpeed = 0f;
    public float stayTime = 1f;
    public GameObject lever;

    private Lever leverScript;
    private float timer = 0f;

    // Use this for initialization
    void Start () {

        startPosition = transform.localPosition;
        leverScript = lever.GetComponent<Lever>();

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Engage()
    {
        gameObject.SetActive(true);
        transform.position = startPosition;
        timer = 0.0f;
        
    }

    public void Disengage()
    {

    }
}
