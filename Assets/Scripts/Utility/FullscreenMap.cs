using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FullscreenMap : MonoBehaviour {

	// Use this for initialization
	void Start ()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

        float cameraHeight = Camera.main.orthographicSize * 2;
        Vector2 cameraSize = new Vector2(Camera.main.aspect * cameraHeight, cameraHeight);
        Vector2 spriteSize = spriteRenderer.sprite.bounds.size;
        //spriteSize = new Vector2(1920, 1080);
        Vector2 scale = transform.localScale;
        
        scale *= Mathf.Min(cameraSize.x / spriteSize.x, cameraSize.y / spriteSize.y)/2f;

        transform.position = Vector2.zero; // Optional
        transform.localScale = scale;
    }
}
