using UnityEngine;
using UnityEngine.Serialization;

namespace ElectricPie.AStar
{
    public class AStarGrid : MonoBehaviour
    {
        [SerializeField] private LayerMask m_unwalkableMask = new LayerMask();
        [SerializeField] private Vector2 m_worldSize = new Vector2(10, 10);
        [SerializeField] private float m_nodeRadius = 0.5f;

        [SerializeField] private Transform m_debugCharacter = null;
        
        private AStarNode[,] m_grid = null;

        private float m_nodeDiameter = 0.0f;
        private Vector2Int m_gridSize = Vector2Int.zero;

        public AStarNode NodeFromWorldPosition(Vector3 worldPosition)
        {
            float percentX = Mathf.Clamp01((worldPosition.x + m_worldSize.x / 2.0f) / m_worldSize.x);
            float percentY = Mathf.Clamp01((worldPosition.z + m_worldSize.y / 2.0f) / m_worldSize.y);

            int x = Mathf.RoundToInt((m_gridSize.x - 1) * percentX);
            int y = Mathf.RoundToInt((m_gridSize.y - 1) * percentY);

            return m_grid[x, y];
        }
        
        private void Start()
        {
            m_nodeDiameter = m_nodeRadius * 2.0f;
            m_gridSize.x = Mathf.RoundToInt(m_worldSize.x / m_nodeDiameter);
            m_gridSize.y = Mathf.RoundToInt(m_worldSize.y / m_nodeDiameter);
            
            CreateGrid();
        }

        private void CreateGrid()
        {
            m_grid = new AStarNode[m_gridSize.x, m_gridSize.y];
            
            Vector3 worldBottomLeft = transform.position - Vector3.right * m_worldSize.x / 2.0f - Vector3.forward * m_worldSize.y / 2.0f;
            for (int x = 0; x < m_gridSize.x; x++)
            {
                for (int y = 0; y < m_gridSize.y; y++)
                {
                    Vector3 worldPosition = worldBottomLeft + Vector3.right * (x * m_nodeDiameter + m_nodeRadius) + Vector3.forward * (y * m_nodeDiameter + m_nodeRadius);
                    bool isWalkable = !Physics.CheckSphere(worldPosition, m_nodeRadius, m_unwalkableMask);
                    m_grid[x, y] = new AStarNode(worldPosition, isWalkable);
                }
            }
        }

        private void OnDrawGizmos()
        {
            // Draw the grid bounds
            Gizmos.DrawWireCube(transform.position, new Vector3(m_worldSize.x, 1.0f, m_worldSize.y));
            
            // Draw the grid nodes
            if (m_grid is not null)
            {
                AStarNode playerNode = NodeFromWorldPosition(m_debugCharacter.position);
                
                foreach (AStarNode node in m_grid)
                {
                    Gizmos.color = node.IsWalkable ? Color.green : Color.red;
                    if (node == playerNode)
                    {
                        Gizmos.color = Color.cyan;
                    }

                    
                    Gizmos.DrawCube(node.WorldPosition, Vector3.one * m_nodeDiameter * 0.9f);
                }
            }
        }
    }
}