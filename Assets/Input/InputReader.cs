using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using static InputActions;

namespace DungeonRun.Input
{
    [CreateAssetMenu(fileName = "InputReader", menuName = "Input/New Input Reader", order = 0)]
    public class InputReader : ScriptableObject, IPlayerActions
    {
        private InputActions m_inputActions = null;
        
        // Events
        public event UnityAction<Vector2> MoveAction = delegate { };
        public event UnityAction SelectAction = delegate { };
        
        // Polls
        public Vector2 Direction => m_inputActions.Player.Move.ReadValue<Vector2>();
        
        public void EnablePlayerActions()
        {
            if (m_inputActions is null)
            {
                m_inputActions = new InputActions();
                m_inputActions.Player.SetCallbacks(this);
            }
            
            m_inputActions.Enable();
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            MoveAction.Invoke(context.ReadValue<Vector2>());
        }

        public void OnSelect(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                SelectAction.Invoke();
            }
        }
    }
}