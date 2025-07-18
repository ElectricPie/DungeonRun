using ElectricPie.Collections;
using UnityEngine;

namespace ElectricPie.AStar
{
    public class AStarNode : IHeapItem<AStarNode>
    {
        public bool IsWalkable { get; private set; } = false;
        public Vector3 WorldPosition { get; private set; }
        public Vector2Int GridPosition { get; private set; }

        public AStarNode Parent = null;

        public int GCost = 0;
        public int HCost = 0;

        public int FCost
        {
            get { return GCost + HCost; }
        }

        public AStarNode(Vector3 worldPosition, bool isWalkable, Vector2Int gridPosition)
        {
            WorldPosition = worldPosition;
            IsWalkable = isWalkable;
            GridPosition = gridPosition;
        }

        /* IHeapItem Interface */
        public int HeapIndex { get; set; }
        
        public int CompareTo(AStarNode otherNode)
        {
            int compare = FCost.CompareTo(otherNode.FCost);
            
            // Use h cost for tiebreakers
            if (compare == 0)
            {
                compare = HCost.CompareTo(otherNode.HCost);
            }

            return -compare;
        }
        /* IHeapItem Interface end */
    }
}