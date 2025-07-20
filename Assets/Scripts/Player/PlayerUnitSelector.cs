using DungeonRun.Input;
using DungeonRun.Unit;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerUnitSelector : MonoBehaviour
{
    [SerializeField] private InputReader m_inputReader = null;
    [SerializeField] private Camera m_camera = null;

    private UnitMovement m_selectedUnitMovement = null;
    
    private void Start()
    {
        m_inputReader.SelectAction += OnSelect;
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
                m_selectedUnitMovement?.SetMoveTarget(hitInfo.point);
            }
        }
    } 
}
