using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{

    public float xMin = -6.5f, xMax = 6.5f;
    public float speed = 2;
    private bool forward = true;
    private Vector2 translation;

    // Use this for initialization
    void Start()
    {
        if (transform.localPosition.x > xMax)
            transform.localPosition = new Vector3(xMax, transform.position.y);
    }

    // Update is called once per frame
    void Update()
    {
        if (forward)
        {
            translation = new Vector2(speed * Time.deltaTime, 0);
            transform.Translate(speed * Time.deltaTime, 0, 0);
        }
        else
        {
            translation = new Vector2(-speed * Time.deltaTime, 0);
            transform.Translate(-speed * Time.deltaTime, 0, 0);
        }

        if (transform.localPosition.x >= xMax)
            forward = false;
        if (transform.localPosition.x <= xMin)
            forward = true;    
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.GetComponent<Champion>() != null)
        {
            collision.transform.SetParent(transform);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        Champion champ = collision.gameObject.GetComponent<Champion>();
        if (champ != null)
        {
            collision.transform.SetParent(champ.originalParent);
        }
    }
}


