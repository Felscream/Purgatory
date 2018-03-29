using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;

public class PopupDamage : MonoBehaviour {

    private float displayTime = 0.5f;
    [SerializeField] private Vector2 offset = new Vector2(1.0f, 0.4f);
    [SerializeField] private int minFontSize = 8;
    [SerializeField] private int maxFontSize = 18;
    [SerializeField] private float firstTierValue = 20.0f;
    [SerializeField] private float lastTierValue = 40.0f;
    [SerializeField] private Color firstTier;
    [SerializeField] private Color lastTier;
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

            StringBuilder sb = new StringBuilder();
            float inter = totalDamage / target.BaseHealth;
            if (damageText == null)
            {
                damageText = GetComponentInChildren<Text>();
            }
            if (!(target.Health <= 0.0f))
            {
                sb.Append(Mathf.Round(totalDamage).ToString());
                if (totalDamage > lastTierValue)
                {
                    
                    if (inter > 0.60f)
                    {
                        damageText.color = lastTier;
                        sb.Append(" !!!");
                    }
                    else
                    {
                        sb.Append(" !!");
                    }
                }
                else if (totalDamage > firstTierValue)
                {
                    damageText.color = firstTier;
                    sb.Append(" !");
                }
            }
            else
            {
                sb.Append("EXECUTED");
            }

            if (anim == null)
            {
                anim = GetComponentInChildren<Animator>();
            }
            
            
            
            
            damageText.fontSize = (int)Mathf.Round(inter * maxFontSize) > minFontSize ? (int)Mathf.Round(inter * maxFontSize) : minFontSize;
            
            damageText.text = sb.ToString();
            anim.Play("popUp", -1, 0.0f);
            if (disableDisplayCoroutine != null)
            {
                CancelInvoke("DestroyObject");
                StopCoroutine(disableDisplayCoroutine);
            }
            if (gameObject.activeSelf)
            {
                disableDisplayCoroutine = StartCoroutine(DisableDisplay());
            }
            else
            {
                Destroy(gameObject);
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
        AnimatorClipInfo[] clipInfo = anim.GetCurrentAnimatorClipInfo(0);

        Invoke("DestroyObject", displayTime * 1.2f);
    }

    public void DestroyObject()
    {
        Destroy(gameObject);
    }
}
