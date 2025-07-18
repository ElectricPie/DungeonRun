using System;
using System.Collections.Generic;
using UnityEngine;

namespace ElectricPie.AStar
{
    public class AStarPathfinding : MonoBehaviour
    {
        [SerializeField] private AStarGrid m_grid = null;

        [SerializeField] private Transform DebugSeeker = null;
        [SerializeField] private Transform DebugTarget;

        private void Awake()
        {
            m_grid ??= GetComponent<AStarGrid>();
        }

        private void Update()
        {
            FindPath(DebugSeeker.position, DebugTarget.position);
        }

        private void FindPath(Vector3 startPos, Vector3 targetPos)
        {
            AStarNode startNode = m_grid.NodeFromWorldPosition(startPos);
            AStarNode targetNode = m_grid.NodeFromWorldPosition(targetPos);

            List<AStarNode> openSet = new List<AStarNode> { startNode };
            HashSet<AStarNode> closedSet = new HashSet<AStarNode>();

            while (openSet.Count > 0)
            {
                AStarNode currentNode = openSet[0];
                for (int i = 0; i < openSet.Count; i++)
                {
                    if (openSet[i].FCost < currentNode.FCost || openSet[i].FCost == currentNode.FCost && openSet[i].HCost < currentNode.HCost)
                    {
                        currentNode = openSet[i];
                    }
                }

                openSet.Remove(currentNode);
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

        private int GetDistance(AStarNode a, AStarNode b)
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