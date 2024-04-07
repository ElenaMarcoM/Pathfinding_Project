using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Catched_byShark : MonoBehaviour
{
    Transform myTransform;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Shark")
        {
            Debug.Log("Enemy got you");
            myTransform.position = new Vector3(1.03f, 0f, 3.57f);
        }
    }
}
