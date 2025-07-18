using System;
using DungeonRun.Input;
using UnityEngine;

namespace DungeonRun.Player
{
    public class PlayerCameraControl : MonoBehaviour
    {
        [SerializeField] private InputReader m_inputReader = null;
        
        [SerializeField] private Transform m_cameraTransform = null;
        
        [SerializeField] private float m_movementSpeed = 10.0f;
        
        private Vector3 m_movementDirection = Vector3.zero;

        private void Start()
        {
            m_inputReader.MoveAction += OnMove;
            
            m_inputReader.EnablePlayerActions();
        }
        
        private void Update()
        {
            Move();
        }

        private void OnMove(Vector2 direction)
        {
            m_movementDirection = new Vector3(direction.x, 0, direction.y).normalized;
        }

        private void Move()
        {
            if (m_movementDirection.sqrMagnitude <= 0.01f) 
                return;
            
            // Gets the movement direction relative to the camera's orientation ignoring its pitch
            Vector3 relativeDirection = Quaternion.Euler(0, m_cameraTransform.eulerAngles.y, 0) * m_movementDirection;
            relativeDirection.y = 0f;
                
            transform.Translate(relativeDirection * (m_movementSpeed * Time.deltaTime));
        } 
    }
}