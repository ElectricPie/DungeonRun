using System.Collections.Generic;
using UnityEngine;

using ElectricPie.Collections;

namespace ElectricPie.AStar
{
    public class AStarPathfinding : MonoBehaviour
    {
        [SerializeField] private AStarGrid m_grid = null;

        [SerializeField] private Transform m_debugSeeker = null;
        [SerializeField] private Transform m_debugTarget;

        private void Awake()
        {
            m_grid ??= GetComponent<AStarGrid>();
        }

        private void Update()
        {
            if (Input.GetButtonDown("Jump"))
            {
                FindPath(m_debugSeeker.position, m_debugTarget.position);
            }
        }

        private void FindPath(Vector3 startPos, Vector3 targetPos)
        {
            AStarNode startNode = m_grid.NodeFromWorldPosition(startPos);
            AStarNode targetNode = m_grid.NodeFromWorldPosition(targetPos);

            Heap<AStarNode> openSet = new Heap<AStarNode>(m_grid.MaxSize);
            openSet.Add(startNode);
            HashSet<AStarNode> closedSet = new HashSet<AStarNode>();

            while (openSet.Count > 0)
            {
                AStarNode currentNode = openSet.RemoveFirst();

                closedSet.Add(currentNode);

                if (currentNode == targetNode)
                {
                    RetracePath(startNode, targetNode);
                    return;
                }

                foreach (AStarNode neighbour in m_grid.GetNeighbours(currentNode))
                {
                    if (neighbour.IsWalkable == false || closedSet.Contains(neighbour))
                        continue;

                    int newMovementCostToNeighbour = currentNode.GCost + GetDistance(currentNode, neighbour);
                    if (newMovementCostToNeighbour < neighbour.GCost || openSet.Contains(neighbour) == false)
                    {
                        neighbour.GCost = newMovementCostToNeighbour;
                        neighbour.HCost = GetDistance(neighbour, targetNode);
                        neighbour.Parent = currentNode;

                        if (openSet.Contains(neighbour) == false)
                        {
                            openSet.Add(neighbour);
                        }
                    }
                }
            }
        }

        private static int GetDistance(AStarNode a, AStarNode b)
        {
            int distX = Mathf.Abs(a.GridPosition.x - b.GridPosition.x);
            int distY = Mathf.Abs(a.GridPosition.y - b.GridPosition.y);

            if (distX > distY)
            {
                return 14 * distY + 10 * (distX - distY);
            }

            return 14 * distX + 10 * (distY - distX);
        }

        private void RetracePath(AStarNode startNode, AStarNode endNode)
        {
            List<AStarNode> path = new List<AStarNode>();
            AStarNode currentNode = endNode;

            while (currentNode != startNode)
            {
                path.Add(currentNode);
                currentNode = currentNode.Parent;
            }
            path.Reverse();
            
            m_grid.Path = path;
        }
    }
}