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
    public Vector2 recoilForce;
    public int damage = 15;

    private Lever leverScript;
    private float timer = 0f;
    private bool isEngaged = false;
    private Quaternion startRotation;

    // Use this for initialization
    void Start () {

        startPosition = transform.localPosition;
        leverScript = lever.GetComponent<Lever>();
        startRotation = transform.localRotation;
    }
	
	// Update is called once per frame
	void Update () {
		
        if (isEngaged)
        {
            if (rotationSpeed > 0)
            {
                transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
            }
            float step = movingSpeed * Time.deltaTime;
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, endPosition, step);
            if (transform.localPosition == endPosition)
            {
                Wait();
            }
        }
    }

    public void Engage()
    {
        gameObject.SetActive(true);
        transform.localPosition = startPosition;
        timer = 0.0f;
        isEngaged = true;
    }

    public void Wait()
    {
        timer += Time.deltaTime;
        if(timer > stayTime)
        {
            Disengage();
        }
    }

    public void Disengage()
    {
        leverScript.Disengage();
        transform.localPosition = startPosition;
        transform.localRotation = startRotation;
        isEngaged = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        float facing = 1; // -1 if <0, 0 if ==0 and 1 if >0
        // les dommages seront proportionnels à la hauteur de chute
        Champion champion = collision.GetComponent<Champion>();

        Debug.Log(damage + "  à " + collision.gameObject.name);

        if (champion != null)
        {
            Debug.Log(damage + " infligés à " + collision.gameObject.name);
            champion.ApplyDamage(damage, facing, 20, recoilForce);
        }   
    }

}
