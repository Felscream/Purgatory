using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupDamage : MonoBehaviour {

    private float displayTime = 0.5f;
    [SerializeField] private Vector2 offset = new Vector2(1.0f, 0.4f);
    private Animator anim;
    private Text damageText;
    private float timer = 0.0f;
    private Champion target;
    private float totalDamage = 0;
    private Vector2 finalOffset;
    private Coroutine disableDisplayCoroutine;
    private void Start()
    {
        anim = transform.GetComponentInChildren<Animator>();
        displayTime += anim.GetCurrentAnimatorClipInfo(0)[0].clip.length;
        damageText = anim.GetComponent<Text>();
        finalOffset = new Vector2(Random.Range(-offset.x, offset.x), Random.Range(-offset.y, offset.y));
        if(Random.Range(0, 1) < 0.5f)
        {
            anim.SetBool("popRight", true);
        }
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
        if(damage > 0.01f)
        {
            totalDamage += damage;
            timer = 0.0f;

            if (anim == null)
            {
                anim = GetComponentInChildren<Animator>();
            }
            anim.Play("popUp", -1, 0.0f);
            if (damageText == null)
            {
                damageText = GetComponentInChildren<Text>();
            }
            if (totalDamage > 20f)
            {
                damageText.color = Color.yellow;
                damageText.fontSize = 12;
            }
            if (totalDamage > 40f)
            {
                damageText.color = Color.red;
                damageText.fontSize = 16;
            }
            damageText.text = Mathf.Round(totalDamage).ToString();
            if (disableDisplayCoroutine != null)
            {
                CancelInvoke("DestroyObject");
                StopCoroutine(disableDisplayCoroutine);
            }
            disableDisplayCoroutine = StartCoroutine(DisableDisplay());
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
        AnimatorClipInfo[] clipInfo = anim.GetCurrentAnimatorClipInfo(0);

        Invoke("DestroyObject", displayTime*1.2f);
    }

    public void DestroyObject()
    {
        Destroy(gameObject);
    }
}
