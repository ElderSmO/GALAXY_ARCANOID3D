using Assets.Game.Core;
using Assets.Game.GameManagerFolder;
using Assets.Game.Player;
using UnityEngine;

namespace Assets.Game.BallFolder
{
    /// <summary>
    /// Main ball that uses physics for movement and collisions
    /// </summary>
    public class Ball : MonoBehaviour, IMovable
    {
        [Header("Physics Settings")]
        [SerializeField] private float initialSpeed = 8f;
        [SerializeField] private float minSpeed = 6f;
        [SerializeField] private float maxSpeed = 15f;

        [Header("References")]
        [SerializeField] private GameManager gameManager;

        private bool isLaunched = false;
        private float currentSpeed = 0f;
        private Rigidbody ballRigidbody;

        /// <summary>
        /// Current movement speed
        /// </summary>
        public float MoveSpeed
        {
            get { return currentSpeed; }
            set { currentSpeed = Mathf.Clamp(value, minSpeed, maxSpeed); }
        }

        /// <summary>
        /// Check if ball is launched
        /// </summary>
        public bool IsLaunched
        {
            get { return isLaunched; }
        }

        private void Awake()
        {
            ballRigidbody = GetComponent<Rigidbody>();
            currentSpeed = initialSpeed;

            if (ballRigidbody == null)
            {
                Debug.LogError("Ball is missing Rigidbody component!");
            }
        }

        private void Start()
        {
            SetupPhysics();
            ResetBall();
        }

        /// <summary>
        /// Setup physics properties for ball
        /// </summary>
        private void SetupPhysics()
        {
            if (ballRigidbody != null)
            {
                ballRigidbody.useGravity = false;
                ballRigidbody.drag = 0f;
                ballRigidbody.angularDrag = 0.05f;
                ballRigidbody.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;
            }
        }

        private void FixedUpdate()
        {
            if (isLaunched && ballRigidbody != null)
            {
                MaintainConstantSpeed();
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("DeadZone"))
            {
                HandleDeathZone();
            }
            else if (collision.gameObject.CompareTag("Paddle")) //Calculate bounce paddle
            {
                
                Debug.Log("Ball hit paddle");
            }
            else if (collision.gameObject.CompareTag("Brick")) //Lofic for collision brick
            {
               
                Debug.Log("Ball hit brick");
            }
            
        }

        /// <summary>
        /// Launch ball with initial force
        /// </summary>
        public void Launch(Vector3 direction)
        {
            if (!isLaunched && ballRigidbody != null)
            {
                isLaunched = true;
                Vector3 launchDirection = new Vector3(direction.x, 0f, direction.z).normalized;
                ballRigidbody.velocity = launchDirection * currentSpeed;
                Debug.Log($"Ball launched! Direction: {launchDirection}, Speed: {currentSpeed}");
            }
        }

        /// <summary>
        /// Launch ball forward
        /// </summary>
        public void Launch()
        {
            Launch(Vector3.forward); // always ball forward vector
        }

        /// <summary>
        /// Move ball by applying force
        /// </summary>
        public void Move(Vector2 direction)
        {
            if (ballRigidbody != null && isLaunched)
            {
                Vector3 moveDirection = new Vector3(direction.x, 0f, direction.y).normalized;
                ballRigidbody.velocity = moveDirection * currentSpeed;
            }
        }

        /// <summary>
        /// Maintain constant ball speed
        /// </summary>
        private void MaintainConstantSpeed()
        {
            if (ballRigidbody.velocity.magnitude != currentSpeed)
            {
                ballRigidbody.velocity = ballRigidbody.velocity.normalized * currentSpeed;
            }
        }

        /// <summary>
        /// Handle ball going out of bounds
        /// </summary>
        private void HandleDeathZone()
        {
            Debug.Log("Ball entered death zone");
            if (gameManager != null)
            {
                gameManager.OnBallLost();
            }
        }

        /// <summary>
        /// Reset ball to initial state
        /// </summary>
        public void ResetBall()
        {
            isLaunched = false;
            currentSpeed = initialSpeed;

            if (ballRigidbody != null)
            {
                ballRigidbody.velocity = Vector3.zero;
                ballRigidbody.angularVelocity = Vector3.zero;
            }
        }

        /// <summary>
        /// Reset ball to paddle position
        /// </summary>
        public void ResetToPaddle(Transform paddleTransform)
        {
            ResetBall();
            if (paddleTransform != null)
            {
                Vector3 paddlePosition = paddleTransform.position;
                transform.position = paddlePosition + Vector3.forward * 1.5f;
            }
        }

        /// <summary>
        /// Set ball speed
        /// </summary>
        public void SetSpeed(float speed)
        {
            currentSpeed = Mathf.Clamp(speed, minSpeed, maxSpeed);
            if (isLaunched && ballRigidbody != null)
            {
                ballRigidbody.velocity = ballRigidbody.velocity.normalized * currentSpeed;
            }
        }

        /// <summary>
        /// Add force to ball
        /// </summary>
        public void AddForce(Vector3 force, ForceMode forceMode = ForceMode.VelocityChange)
        {
            if (ballRigidbody != null)
            {
                ballRigidbody.AddForce(force, forceMode);
            }
        }

        /// <summary>
        /// Get current ball velocity
        /// </summary>
        public Vector3 GetVelocity()
        {
            return ballRigidbody != null ? ballRigidbody.velocity : Vector3.zero;
        }
    }
}