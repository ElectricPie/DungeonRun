using System;
using System.Collections.Generic;
using UnityEngine;

namespace ElectricPie.AStar
{
    public class AStarGrid : MonoBehaviour
    {
        [SerializeField] private LayerMask m_unwalkableMask = new LayerMask();
        [SerializeField] private Vector2 m_worldSize = new Vector2(10, 10);
        [SerializeField] private float m_nodeRadius = 0.5f;
        [SerializeField] private AStarTerrainType[] m_walkableRegions = Array.Empty<AStarTerrainType>();
        
        [Header("Debug")]
        [SerializeField] private bool m_showGridGizmos = false;
        
        private AStarNode[,] m_grid = null;

        private float m_nodeDiameter = 0.0f;
        private Vector2Int m_gridSize = Vector2Int.zero;

        private LayerMask m_walkableMask = new LayerMask();
        private readonly Dictionary<int, int> m_walkableRegionDictionary = new Dictionary<int, int>();
        
        public int MaxSize => m_gridSize.x * m_gridSize.y;

        public AStarNode NodeFromWorldPosition(Vector3 worldPosition)
        {
            float percentX = Mathf.Clamp01((worldPosition.x + m_worldSize.x / 2.0f) / m_worldSize.x);
            float percentY = Mathf.Clamp01((worldPosition.z + m_worldSize.y / 2.0f) / m_worldSize.y);

            int x = Mathf.RoundToInt((m_gridSize.x - 1) * percentX);
            int y = Mathf.RoundToInt((m_gridSize.y - 1) * percentY);

            return m_grid[x, y];
        }

        public List<AStarNode> GetNeighbours(AStarNode node)
        {
            List<AStarNode> neighbours = new List<AStarNode>();

            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0)
                        continue;

                    int checkX = node.GridPosition.x + x;
                    int checkY = node.GridPosition.y + y;

                    if (checkX >= 0 && checkX < m_gridSize.x && checkY >= 0 && checkY < m_gridSize.y)
                    {
                        neighbours.Add(m_grid[checkX, checkY]);
                    }
                }
            }
            
            return neighbours;
        }
        
        private void Awake()
        {
            InitializeGrid();
        }

        private void InitializeGrid()
        {
            m_nodeDiameter = m_nodeRadius * 2.0f;
            m_gridSize.x = Mathf.RoundToInt(m_worldSize.x / m_nodeDiameter);
            m_gridSize.y = Mathf.RoundToInt(m_worldSize.y / m_nodeDiameter);
            
            m_walkableRegionDictionary.Clear();
            foreach (AStarTerrainType region in m_walkableRegions)
            {
                m_walkableMask |= region.TerrainMask;
                m_walkableRegionDictionary.Add((int) Mathf.Log(region.TerrainMask.value, 2.0f), region.MovementPenalty);
            }
            
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
                    
                    // Calculate movement penalty based on terrain type
                    int movementPenalty = 0;
                    if (isWalkable)
                    {
                        Ray ray = new Ray(worldPosition + Vector3.up * 50.0f, Vector3.down);
                        if (Physics.Raycast(ray, out RaycastHit hit, 100.0f, m_walkableMask))
                        {
                            m_walkableRegionDictionary.TryGetValue(hit.collider.gameObject.layer, out movementPenalty);
                        }
                    }
                        
                    m_grid[x, y] = new AStarNode(worldPosition, isWalkable, new Vector2Int(x, y), movementPenalty);
                }
            }
        }

        private void OnDrawGizmos()
        {
            // Draw the grid bounds
            Gizmos.DrawWireCube(transform.position, new Vector3(m_worldSize.x, 1.0f, m_worldSize.y));
            
            if (m_showGridGizmos is false)
                return;
            
            Vector3 cubeSize = Vector3.one * m_nodeDiameter * 0.9f;
            cubeSize.y = 0.1f; // Make the cubes flat
            for (int x = 0; x < m_gridSize.x; x++)
            {
                for (int y = 0; y < m_gridSize.y; y++)
                {
                    AStarNode node = m_grid[x, y];
                    Gizmos.color = node.IsWalkable ? Color.green : Color.red;
                    Gizmos.DrawCube(node.WorldPosition, cubeSize);
                }
            }
        }
        
        private void OnValidate()
        {
            InitializeGrid();
        }
    }

    [Serializable]
    public class AStarTerrainType
    {
        public LayerMask TerrainMask = new LayerMask();
        public int MovementPenalty = 0;
    }
}