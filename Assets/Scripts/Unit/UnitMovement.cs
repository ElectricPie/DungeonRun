using UnityEngine;

namespace DungeonRun.Unit
{
    public class UnitMovement : MonoBehaviour
    {
        [SerializeField] private Vector3 m_moveTarget = Vector3.zero;
        
        public void SetMoveTarget(Vector3 targetPosition)
        {
            m_moveTarget = targetPosition;
        }

        private void Update()
        {
            transform.position = Vector3.MoveTowards(transform.position, m_moveTarget, Time.deltaTime * 5.0f);
        }
    }
}