using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ElectricPie.Collections;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

namespace ElectricPie.AStar
{
    [RequireComponent(typeof(AStarGrid), typeof(PathRequestManager))]
    public class AStarPathfinding : MonoBehaviour
    {
        private PathRequestManager m_requestManager;
        
        private AStarGrid m_grid = null;

        public void StartFindPath(Vector3 startPos, Vector3 endPos)
        {
            StartCoroutine(FindPath(startPos, endPos));
        }
        
        private void Awake()
        {
            m_grid = GetComponent<AStarGrid>();
            m_requestManager = GetComponent<PathRequestManager>();
        }

        private IEnumerator FindPath(Vector3 startPos, Vector3 targetPos)
        {
            Vector3[] waypoints = Array.Empty<Vector3>();
            bool foundPath = false;
            
            AStarNode startNode = m_grid.NodeFromWorldPosition(startPos);
            AStarNode targetNode = m_grid.NodeFromWorldPosition(targetPos);

            // Exit early if path is not possible
            if (!startNode.IsWalkable || !targetNode.IsWalkable)
            {
                m_requestManager.FinishedProcessingPath(waypoints, false);
                yield break;
            }

            Heap<AStarNode> openSet = new Heap<AStarNode>(m_grid.MaxSize);
            openSet.Add(startNode);
            HashSet<AStarNode> closedSet = new HashSet<AStarNode>();

            while (openSet.Count > 0)
            {
                AStarNode currentNode = openSet.RemoveFirst();

                closedSet.Add(currentNode);

                if (currentNode == targetNode)
                {
                    foundPath = true;
                    break;
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

            yield return null;
            if (foundPath)
            {
                waypoints = RetracePath(startNode, targetNode);
            }

            m_requestManager.FinishedProcessingPath(waypoints, foundPath);
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

        private static Vector3[] SimplifyPath(List<AStarNode> path)
        {
            List<Vector3> waypoints = new List<Vector3>();
            Vector2 directionOld = Vector2.zero;

            for (int i = 1; i < path.Count; i++)
            {
                Vector2 directionNew = path[i - 1].GridPosition - path[i].GridPosition;
                if (directionNew != directionOld)
                {
                    waypoints.Add(path[i].WorldPosition);
                }

                directionOld = directionNew;
            }

            return waypoints.ToArray();
        }
        
        private static Vector3[] RetracePath(AStarNode startNode, AStarNode endNode)
        {
            List<AStarNode> path = new List<AStarNode>();
            AStarNode currentNode = endNode;

            while (currentNode != startNode)
            {
                path.Add(currentNode);
                currentNode = currentNode.Parent;
            }
            path.Reverse();
            
            return SimplifyPath(path);
        }
    }
}