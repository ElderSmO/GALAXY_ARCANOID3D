using Assets.Game.Player;
using UnityEngine;

namespace Assets.Game.UI
{
    /// <summary>
    /// Handles UI button input for paddle control
    /// </summary>
    public class UIController : MonoBehaviour
    {
        [Header("Control References")]
        [SerializeField] Paddle playerPaddle;
        [SerializeField] GameObject leftButton;
        [SerializeField] GameObject rightButton;

        private bool isLeftPressed = false;
        private bool isRightPressed = false;

        private void Update()
        {
            if (playerPaddle == null) return;

            float moveInput = 0f;
            if (isLeftPressed) moveInput -= 1f;
            if (isRightPressed) moveInput += 1f;

            playerPaddle.SetMoveInput(moveInput);
        }

        /// <summary>
        /// Called when left button is pressed
        /// </summary>
        public void OnLeftButtonDown()
        {
            isLeftPressed = true;
            Debug.Log("Left button pressed");
        }

        /// <summary>
        /// Called when left button is released
        /// </summary>
        public void OnLeftButtonUp()
        {
            isLeftPressed = false;
            Debug.Log("Left button released");
        }

        /// <summary>
        /// Called when right button is pressed
        /// </summary>
        public void OnRightButtonDown()
        {
            isRightPressed = true;
            Debug.Log("Right button pressed");
        }

        /// <summary>
        /// Called when right button is released
        /// </summary>
        public void OnRightButtonUp()
        {
            isRightPressed = false;
            Debug.Log("Right button released");
        }
    }
        
}


