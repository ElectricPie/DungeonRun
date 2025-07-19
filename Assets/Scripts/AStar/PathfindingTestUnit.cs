using System;
using Unity.VisualScripting;
using UnityEngine;

namespace ElectricPie.AStar
{
    public class PathfindingTestUnit : MonoBehaviour
    {
        [SerializeField] private Transform m_target = null;
        [SerializeField] private float m_speed = 5.0f;
        [SerializeField] private float m_waypointThreshold = 0.1f;

        private Vector3[] m_path = Array.Empty<Vector3>();
        private Vector3 m_currentWaypoint = Vector3.zero;
        private int m_targetIndex = 0;

        private void Start()
        {
            PathRequestManager.RequestPath(transform.position, m_target.position, (newPath, foundPath) =>
            {
                if (!foundPath) 
                    return;
                
                m_path = newPath;
                m_targetIndex = 0;
                m_currentWaypoint = m_path[0];
            });
        }

        private void Update()
        {
            MoveToWaypoint();
        }

        private void MoveToWaypoint()
        {
            if (m_path.Length <= 0)
                return;

            // Set next waypoint if at current waypoint
            if ((transform.position - m_currentWaypoint).magnitude < m_waypointThreshold)
            {
                m_targetIndex++;
                // Reached target destination
                if (m_targetIndex >= m_path.Length)
                {
                    m_path = Array.Empty<Vector3>();
                    return;
                }
                m_currentWaypoint = m_path[m_targetIndex];
            }

            transform.position = Vector3.MoveTowards(transform.position, m_currentWaypoint, m_speed);
        }
        
        private void OnDrawGizmos()
        {
            if (m_path.Length <= 0)
                return;

            Gizmos.color = Color.black;
            foreach (Vector3 waypoint in m_path)
            {
                Gizmos.DrawCube(waypoint, Vector3.one * 0.9f);
            }

            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, m_currentWaypoint);
        }
    }
}