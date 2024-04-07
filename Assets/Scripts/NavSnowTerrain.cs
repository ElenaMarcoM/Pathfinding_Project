using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class Nav_TerrainModifier : MonoBehaviour
{
    private NavMeshModifier _meshSurface;
    // Start is called before the first frame update
    void Start()
    {
        _meshSurface = GetComponent<NavMeshModifier>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // Debug.Log("InS");
        if (other.TryGetComponent<NavMeshAgent>(out NavMeshAgent agent))
        {
            if (_meshSurface.AffectsAgentType(agent.agentTypeID))
            {
                Debug.Log("Woks inS");
                agent.speed /= NavMesh.GetAreaCost(_meshSurface.area);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Debug.Log("outS");
        if (other.TryGetComponent<NavMeshAgent>(out NavMeshAgent agent))
        {
            if (_meshSurface.AffectsAgentType(agent.agentTypeID))
            {
                Debug.Log("Woks outS");
                agent.speed *= NavMesh.GetAreaCost(_meshSurface.area);
            }
        }
    }
}