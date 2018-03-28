using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupDamage : MonoBehaviour {

    [SerializeField] private float displayTime = 0.1f;
    [SerializeField] private Vector2 offset = new Vector2(1.0f, 0.4f);
    private Animator anim;
    private Text damageText;
    private float timer = 0.0f;
    private float direction;
    private Champion target;
    private float totalDamage = 0;
    private Vector2 finalOffset;
    private bool setOffsetOnce = false;
    private Coroutine disableDisplayCoroutine;
    private void Start()
    {
        anim = transform.GetComponentInChildren<Animator>();
        damageText = anim.GetComponent<Text>();
        finalOffset = offset;
        
    }

    private void FixedUpdate()
    {
        if(target != null)
        {
            Vector3 reference = target.transform.position;
            if(target.transform.position.x > 19.5f)
            {
                reference = new Vector3(reference.x - Mathf.Abs(finalOffset.x), reference.y, reference.z);
            }
            if(target.transform.position.x < 19.5f)
            {
                reference = new Vector3(reference.x + Mathf.Abs(finalOffset.x), reference.y, reference.z);
            }
            transform.position = Camera.main.WorldToScreenPoint(reference + (Vector3)finalOffset);
        }
    }
    public void SetText(float damage)
    {
        totalDamage += damage;
        Debug.Log(totalDamage);
        timer = 0.0f;
        
        if(anim == null)
        {
            anim = GetComponentInChildren<Animator>();
        }
        anim.Play("popUp", -1, 0.0f);
        if(damageText == null)
        {
            damageText = GetComponentInChildren<Text>();
        }
        if(totalDamage > 20f)
        {
            damageText.color = Color.yellow;
            damageText.fontSize = 12;
        }
        if(totalDamage > 40f)
        {
            damageText.color = Color.red;
            damageText.fontSize = 16;
        }
        damageText.text = Mathf.Round(totalDamage).ToString();
        if(disableDisplayCoroutine != null)
        {
            CancelInvoke("DestroyObject");
            StopCoroutine(disableDisplayCoroutine);
        }
        disableDisplayCoroutine = StartCoroutine(DisableDisplay());
    }

    public float Direction
    {
        set
        {
            direction = value;
            if(direction >= 0)
            {
                anim.SetBool("popRight", true);
            }
            else
            {
                anim.SetBool("popRight", false);
            }

            if (!setOffsetOnce)
            {
                finalOffset = new Vector2(offset.x * direction, offset.y);
                setOffsetOnce = true;
            }
            
        }
    }

    public Champion Target {
        set
        {
            target = value;
        }
    }

    private IEnumerator DisableDisplay()
    {
        while(timer < displayTime)
        {
            timer += Time.unscaledDeltaTime;
            yield return null;
        }
        anim.SetTrigger("endLoop");
        Invoke("DestroyObject", 0.75f);
    }

    public void DestroyObject()
    {
        Destroy(gameObject);
    }
}
