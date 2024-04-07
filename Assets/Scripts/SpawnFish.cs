using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnFish : MonoBehaviour
{
    public GameObject objectToSpawn;
    private float spawnInterval = 2f;
    private float objectLifetime = 5f;

    private float timer = 0f;

    void Update()
    {
        timer += Time.deltaTime;
        Vector3 randomPos = new Vector3(Random.Range(-1.5f, 1.5f), Random.Range(-1.5f, 1.5f), 0);

        if (timer >= spawnInterval)
        {
            //Debug.Log("Appear fish");
            GameObject newObject = Instantiate(objectToSpawn, randomPos, Quaternion.identity);

            timer = 0f;

            Destroy(newObject, objectLifetime);
        }
    }
}
