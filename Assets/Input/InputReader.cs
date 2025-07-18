using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using static InputActions;

namespace DungeonRun.Input
{
    [CreateAssetMenu(fileName = "InputReader", menuName = "Input/New Input Reader", order = 0)]
    public class InputReader : ScriptableObject, IPlayerActions
    {
        public InputActions InputActions = null;
        
        // Events
        public event UnityAction<Vector2> MoveAction = delegate { };
        
        // Polls
        public Vector2 Direction => InputActions.Player.Move.ReadValue<Vector2>();
        
        public void EnablePlayerActions()
        {
            if (InputActions is null)
            {
                InputActions = new InputActions();
                InputActions.Player.SetCallbacks(this);
            }
            
            InputActions.Enable();
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            MoveAction.Invoke(context.ReadValue<Vector2>());
        }
    }
}