using System;
using DungeonRun.Input;
using DungeonRun.Unit;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerUnitSelector : MonoBehaviour
{
    [SerializeField] private InputReader m_inputReader = null;
    [SerializeField] private Camera m_camera = null;

    [SerializeField] private Grid m_grid = null;
    
    private UnitMovement m_selectedUnitMovement = null;
    
    // Debug
    [Header("Debug")]
    [SerializeField] private Transform m_debugCursor = null;
    [SerializeField] private Transform m_debugGridCursor = null;

    private void Start()
    {
        m_inputReader.SelectAction += OnSelect;
    }

    private void Update()
    {
        // Debug to show mouse position in world space and grid space
        if (m_debugCursor is not null)
        {
            Vector2 mouseScreenPosition = Mouse.current.position.ReadValue();
            Ray ray = m_camera.ScreenPointToRay(mouseScreenPosition);

            if (Physics.Raycast(ray, out RaycastHit hitInfo))
            {
                m_debugCursor.position = hitInfo.point;
                
                Vector3 cellOffset = m_grid.cellSize * 0.5f;
                
                Vector3Int gridPos = m_grid.WorldToCell(hitInfo.point);
                Vector3 gridWorldPosition = m_grid.CellToWorld(gridPos) + cellOffset;
                gridWorldPosition.y = hitInfo.point.y;
                m_debugGridCursor.position = gridWorldPosition;
            }
        }
    }

    private void OnSelect()
    {
        Vector2 mouseScreenPosition = Mouse.current.position.ReadValue();
        Ray ray = m_camera.ScreenPointToRay(mouseScreenPosition);

        if (Physics.Raycast(ray, out RaycastHit hitInfo))
        {
            GameObject hoveredObject = hitInfo.collider.gameObject;
            if (hoveredObject.TryGetComponent(out UnitBase unit))
            {
                m_selectedUnitMovement = unit.UnitMovement;
            }
            else
            {
                Vector3Int gridPos = m_grid.WorldToCell(hitInfo.point);
                
                Vector3 cellOffset = m_grid.cellSize * 0.5f;
                Vector3 gridWorldPosition = m_grid.CellToWorld(gridPos) + cellOffset;
                
                m_selectedUnitMovement?.SetMoveTarget(gridWorldPosition);
            }
        }
    }
}
