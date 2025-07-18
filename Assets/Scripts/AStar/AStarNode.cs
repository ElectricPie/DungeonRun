using UnityEngine;

namespace ElectricPie.AStar
{
    public class AStarNode
    {
        public bool IsWalkable { get; private set; }
        public Vector3 WorldPosition { get; private set; }

        public AStarNode(Vector3 worldPosition, bool isWalkable)
        {
            WorldPosition = worldPosition;
            IsWalkable = isWalkable;
        }
    }
}