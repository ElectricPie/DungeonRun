using DungeonRun.Input;
using DungeonRun.Unit;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerUnitSelector : MonoBehaviour
{
    [SerializeField] private InputReader m_inputReader = null;
    [SerializeField] private Camera m_camera = null;

    [SerializeField] private Grid m_grid = null;
    
    [SerializeField] private Transform m_cursor = null;
    
    private UnitMovement m_selectedUnitMovement = null;
    private Vector3 m_cursorPosition = Vector3.zero;
    
    private readonly Collider[] m_results = new Collider[10];

    private void Start()
    {
        m_inputReader.SelectAction += OnSelect;
    }

    private void Update()
    {
        GetMouseGridPosition();
    }

    private void GetMouseGridPosition()
    {
        // show mouse position in world space and grid space
        if (m_cursor)
        {
            Vector2 mouseScreenPosition = Mouse.current.position.ReadValue();
            Ray ray = m_camera.ScreenPointToRay(mouseScreenPosition);

            if (Physics.Raycast(ray, out RaycastHit hitInfo))
            {
                Vector3 cellOffset = m_grid.cellSize * 0.5f;
                
                Vector3Int gridPos = m_grid.WorldToCell(hitInfo.point);
                m_cursorPosition = m_grid.CellToWorld(gridPos) + cellOffset;
                m_cursorPosition.y = hitInfo.point.y;
                m_cursor.position = m_cursorPosition;
            }
        }
    } 
    

    private void OnSelect()
    {
        int size = Physics.OverlapSphereNonAlloc(m_cursorPosition, 1.0f, m_results);
        if (size == 0)
            return;

        // Check if any of the results are units
        for (int i = 0; i < size; i++)
        {
            if (m_results[i].TryGetComponent(out UnitBase unit))
            {
                m_selectedUnitMovement = unit.UnitMovement;
                return;
            }
        }

        if (m_selectedUnitMovement)
        {
            m_selectedUnitMovement.SetMoveTarget(m_cursorPosition);
        }
    }
}
