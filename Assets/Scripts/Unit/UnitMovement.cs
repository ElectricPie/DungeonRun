using UnityEngine;
using UnityEngine.AI;

namespace DungeonRun.Unit
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class UnitMovement : MonoBehaviour
    {
        private NavMeshAgent m_navMeshAgent = null;
        
        public void SetMoveTarget(Vector3 targetPosition)
        {
            m_navMeshAgent.SetDestination(targetPosition);
        }

        private void Awake()
        {
            m_navMeshAgent = GetComponent<NavMeshAgent>();
        }
    }
}