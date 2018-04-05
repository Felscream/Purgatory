using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Multiplier : MonoBehaviour {
    [SerializeField] private Color[] colors;
    [SerializeField] private Vector2 offset;
    private Animator animator;
    private Text text;
    [NonSerialized] public Champion target;
    private int multiplier = 1;
	// Use this for initialization
	void Start () {
        animator = GetComponentInChildren<Animator>();
        text = GetComponentInChildren<Text>();
	}
    private void FixedUpdate()
    {
        if (target != null)
        {
            Vector3 reference = target.transform.position;
            transform.position = Camera.main.WorldToScreenPoint(reference);
        }
    }
    // Update is called once per frame
    void LateUpdate () {
		if(target != null && !target.Dead)
        {
            if(target.playerScore.multiplier != multiplier)
            {
                StringBuilder sb = new StringBuilder("x").Append(target.playerScore.multiplier.ToString());
                text.text = sb.ToString();
                switch (target.playerScore.multiplier)
                {
                    case 2:
                        text.color = colors[1];
                        break;
                    case 3:
                        text.color = colors[2];
                        break;
                    case 4:
                        text.color = colors[3];
                        break;
                    default:
                        text.color = colors[0];
                        break;
                }

                if(multiplier < target.playerScore.multiplier)
                {
                    animator.Play("multUp", -1, 0.0f);
                }
                else
                {
                    animator.Play("multDown", -1, 0.0f);
                }
                multiplier = target.playerScore.multiplier;
            }
        }
	}
}
