using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class NavIcyTerrain : MonoBehaviour
{
    private NavMeshModifier _meshSurface;
    // Start is called before the first frame update
    void Start()
    {
        _meshSurface = GetComponent<NavMeshModifier>();
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("InI");
        if (other.TryGetComponent<NavMeshAgent>(out NavMeshAgent agent))
        {
            if (_meshSurface.AffectsAgentType(agent.agentTypeID))
            {
                Debug.Log("Woks inI");
                agent.speed *= NavMesh.GetAreaCost(_meshSurface.area);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //Debug.Log("outI");
        if (other.TryGetComponent<NavMeshAgent>(out NavMeshAgent agent))
        {
            if (_meshSurface.AffectsAgentType(agent.agentTypeID))
            {
                Debug.Log("Woks outI");
                agent.speed = NavMesh.GetAreaCost(_meshSurface.area);
            }
        }
    }
}
