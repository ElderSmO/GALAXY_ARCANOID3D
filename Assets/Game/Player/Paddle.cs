using Assets.Game.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Game.Player
{
    /// <summary>
    /// Player controlled paddle for ball reflection in XZ plane
    /// </summary>
    public class Paddle : MonoBehaviour, IMovable
    {
        [Header("Movement Settings")]
        [SerializeField] float moveSpeed = 10f;
        [SerializeField] float horizontalLimitX = 3.5f; // Ограничение по X
        [SerializeField] float horizontalLimitZ = 3.5f; // Ограничение по Z

        [Header("Size Settings")]
        [SerializeField] float defaultSize = 2f;

        [Header("Visual Settings")]
        [SerializeField] Renderer paddleRenderer;

        private float currentMoveInputX = 0f;
        private float currentMoveInputZ = 0f;
        private Vector3 initialPosition = Vector3.zero;

        /// <summary>
        /// Current movement speed
        /// </summary>
        public float MoveSpeed
        {
            get { return moveSpeed; }
            set { moveSpeed = Mathf.Max(0f, value); }
        }

        private void Start()
        {
            initialPosition = transform.position;
            ResetPosition();
        }

        private void Update()
        {
            // Process movement every frame based on current input
            if (currentMoveInputX != 0 || currentMoveInputZ != 0)
            {
                Move(new Vector2(currentMoveInputX, currentMoveInputZ));
            }
        }

        /// <summary>
        /// Move paddle in XZ direction
        /// </summary>
        public void Move(Vector2 direction)
        {
            // Calculate new position relative to initial position
            float newX = transform.position.x + direction.x * moveSpeed * Time.deltaTime;
            float newZ = transform.position.z + direction.y * moveSpeed * Time.deltaTime;

            // Clamp relative to initial position (center)
            float minX = initialPosition.x - horizontalLimitX;
            float maxX = initialPosition.x + horizontalLimitX;
            float minZ = initialPosition.z - horizontalLimitZ;
            float maxZ = initialPosition.z + horizontalLimitZ;

            newX = Mathf.Clamp(newX, minX, maxX);
            newZ = Mathf.Clamp(newZ, minZ, maxZ);

            transform.position = new Vector3(newX, transform.position.y, newZ);
        }

        /// <summary>
        /// Set movement input for X and Z axes
        /// </summary>
        public void SetMoveInput(float inputX, float inputZ)
        {
            currentMoveInputX = inputX;
            currentMoveInputZ = inputZ;
        }

        /// <summary>
        /// Set movement input (for IMovable interface)
        /// </summary>
        public void SetMoveInput(float input)
        {
            // For single input, use it for both axes or just X
            currentMoveInputX = input;
            currentMoveInputZ = 0f;
        }

        /// <summary>
        /// Reset paddle to center position
        /// </summary>
        public void ResetPosition()
        {
            currentMoveInputX = 0f;
            currentMoveInputZ = 0f;
            transform.position = initialPosition;
        }

        /// <summary>
        /// Draw debug gizmos in scene view
        /// </summary>
        private void OnDrawGizmosSelected()
        {
            Vector3 center = Application.isPlaying ? initialPosition : transform.position;

            // Draw movement limits
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(center, new Vector3(horizontalLimitX * 2, 0.1f, horizontalLimitZ * 2));
        }
    }
}