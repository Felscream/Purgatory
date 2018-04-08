using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dragon : MonoBehaviour {
    public float spawnChance = 0.001f;
    public float speed;
    public Transform[] startingPoints;
    public Transform[] endPoints;
    private float timer;
    private bool spawned = false;
    private AudioVolumeManager audioVolumeManager;
	// Use this for initialization
	void Start () {
        audioVolumeManager = AudioVolumeManager.GetInstance();
        timer = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
		if(Time.time - timer > 1.0f)
        {
            if(Random.Range(0.0f,100.0f) <= spawnChance && !spawned)
            {
                spawned = true;
                transform.position = startingPoints[Random.Range(0, startingPoints.Length)].position;
                StartCoroutine(Move());
            }
        }
	}

    IEnumerator Move()
    {
        Vector3 endPoint = endPoints[Random.Range(0, endPoints.Length)].position;
        Vector3 dir = (endPoint - transform.position).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        while(transform.position.x <= endPoint.x)
        {
            transform.Translate(dir * speed * Time.deltaTime);
            yield return null;
        }
    }

    public void Flight()
    {
        if(transform.position.x > -15 && transform.position.x < 15)
        {
            audioVolumeManager.PlaySoundEffectRandomPitch("DragonFlight");
        }
    }
}
