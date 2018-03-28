using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapManager : MonoBehaviour {

    public GameObject fallingSpikePrefab;
    public float fallingSpeed = 5;
    public float xMax = 7, yMax = 7;
    public float spawnTimeMin = 15, spawnTimeMax = 20;

    private float timer = 0, aleatTime, aleatPosition;

	// Use this for initialization
	void Start () {
        spawnTimeMin = Mathf.Min(spawnTimeMin, spawnTimeMax);
        spawnTimeMax = Mathf.Max(spawnTimeMin, spawnTimeMax);
        Generate();
	}
	
	// Update is called once per frame
	void Update () {
        timer += Time.deltaTime;
        if (aleatTime != 0 && timer >= aleatTime)
        {
            Fall();
            Generate();
        }
	}

    public void Generate()
    {
        timer = 0;
        aleatPosition = Random.Range(-xMax, xMax);
        aleatTime = Random.Range(spawnTimeMin, spawnTimeMax);
       // Debug.Log("generate" + aleatTime + " " + aleatPosition);
    }

    public void Fall()
    {
        Vector3 poss = new Vector3(aleatPosition, yMax, 0);
        Vector3 pose = new Vector3(aleatPosition, -yMax, 0);
        GameObject spikes = Instantiate(fallingSpikePrefab, transform.position, transform.rotation, transform);
        spikes.transform.localPosition = poss;
        spikes.transform.Rotate(0, 0, 180);
        ParticleSystem particleSystem = spikes.GetComponent<ParticleSystem>();
        particleSystem.GetComponent<Renderer>().sortingLayerName = "Default";
        Trap spikeTrap = spikes.GetComponent<Trap>();
        spikeTrap.startPosition = poss;
        spikeTrap.endPosition = pose;
        spikeTrap.Engage();

        Destroy(spikes, 10);
    }
}
