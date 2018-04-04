using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Shockwave : MonoBehaviour
{
    [SerializeField] protected float chromaticAberDuration = 0.2f;
    public Vector3 startScale;
    public Vector3 targetScale;
    public float duration;
    private Vector3 step;
    private void Start()
    {
        step = (targetScale - startScale) / duration;
        transform.localScale = new Vector3(0.1f,0.1f,0.1f);
        StartCoroutine(ManagerInGame.GetInstance().ChromaticAberration(chromaticAberDuration));
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
