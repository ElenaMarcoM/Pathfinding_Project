using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishCol : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.name == "MYPenguin")
        {
            ScoreManager.scoreCount += 1;
        }
        Destroy(gameObject, 1f);

        Debug.Log("You found fish");
        Vector3 randomPos = new Vector3(Random.Range(-2f, 2f), Random.Range(2f, 2f), 0);
        Instantiate(gameObject, randomPos, Quaternion.identity);
    }
}
