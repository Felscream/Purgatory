using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleShockwave : MonoBehaviour {
    
    public Vector3 startScale;
    public Vector3 targetScale;
    public float duration;
    protected Vector3 step;
    protected virtual void Start()
    {
        step = (targetScale - startScale) / duration;
        transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
    }
    void Update()
    {
        transform.localScale += step * Time.unscaledDeltaTime;
        if (transform.localScale.x > targetScale.x)
        {
            Destroy(gameObject);
        }
    }
}
