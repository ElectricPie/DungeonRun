using UnityEngine;

namespace DungeonRun.Unit
{
    [RequireComponent(typeof(UnitMovement))]
    public class UnitBase : MonoBehaviour
    {
        public UnitMovement UnitMovement { get; private set; } = null;
        
        public void Awake()
        {
            UnitMovement = GetComponent<UnitMovement>();
        }
    }
}