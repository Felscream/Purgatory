using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupScore : MonoBehaviour {

    [SerializeField] private float displayTime = 1.5f;
    [SerializeField] private Vector2 offset = new Vector2(1.0f, 0.4f);
    private Animator anim;
    private Text score;
    private Champion target;
    private void Start()
    {
        Destroy(gameObject, displayTime);
    }
    
    public void SetText(int s)
    {
        anim = transform.GetComponentInChildren<Animator>();
        StringBuilder sb = new StringBuilder();
        if (score == null)
        {
            score = GetComponentInChildren<Text>();
        }
        if(s < 0)
        {
            score.color = Color.red;
        }
        else
        {
            sb.Append("+");
            score.color = Color.white;
        }
        sb.Append(s.ToString());
        score.text = sb.ToString();
        Vector3 reference = target.transform.position;
        transform.position = Camera.main.WorldToScreenPoint(reference + (Vector3)offset);
        anim.Play("scoreDisplay", -1, 0.0f);
    }

    public Champion Target
    {
        set
        {
            target = value;
        }
    }
}
