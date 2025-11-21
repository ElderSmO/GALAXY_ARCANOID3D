using Assets.Game.Core;
using UnityEngine;

namespace Assets.Game.Bricks
{
    /// <summary>
    /// Base class for all brick types
    /// </summary>
    public class Brick : MonoBehaviour, IDamageable
    {
        [Header("Brick Settings")]
        [SerializeField] int hitPoints = 1;
        [SerializeField] int scoreValue = 100;
        [SerializeField] BrickType brickType = BrickType.Standard;
        [SerializeField] GameObject destriyEffect;

        protected int currentHitPoints = 0;
        protected bool isDestroyed = false;

        /// <summary>
        /// Type of this brick
        /// </summary>
        public BrickType Type
        {
            get { return brickType; }
        }

        /// <summary>
        /// Score value when destroyed
        /// </summary>
        public int ScoreValue
        {
            get { return scoreValue; }
        }

        /// <summary>
        /// Check if brick is destroyed
        /// </summary>
        public bool IsDestroyed
        {
            get { return isDestroyed; }
        }

        /// <summary>
        /// Event called when brick is destroyed
        /// </summary>
        public static event System.Action<Brick> BrickDestroyed;

        private void Start()
        {
            currentHitPoints = hitPoints;
            UpdateVisualState();
        }

        private void OnCollisionEnter(Collision collision)
        {
            // Check if ball hit the brick
            if (collision.gameObject.CompareTag("Ball"))
            {
                TakeDamage(1);
                Debug.Log($"Brick hit by ball! HP: {currentHitPoints}/{hitPoints}");
            }
        }

        /// <summary>
        /// Apply damage to brick
        /// </summary>
        public virtual void TakeDamage(int damage)
        {
            if (isDestroyed || brickType == BrickType.Indestructible)
            {
                return;
            }

            currentHitPoints -= damage;
            OnDamageTaken();

            if (currentHitPoints <= 0)
            {
                DestroyBrick();
            }
            else
            {
                UpdateVisualState();
            }
        }

        /// <summary>
        /// Handle damage taken
        /// </summary>
        protected virtual void OnDamageTaken()
        {
            // Visual or audio feedback
            Debug.Log($"Brick took damage! Remaining HP: {currentHitPoints}");
        }

        /// <summary>
        /// Destroy this brick
        /// </summary>
        protected virtual void DestroyBrick()
        {
            isDestroyed = true;

            // Notify about brick destruction
            BrickDestroyed?.Invoke(this);

            // Visual effects before destruction
            PlayDestroyEffects();

            // Destroy the game object
            Destroy(gameObject);

            Instantiate(destriyEffect, transform.position, Quaternion.identity);

            Debug.Log("Brick destroyed!");
        }

        /// <summary>
        /// Update brick visual appearance based on health
        /// </summary>
        protected virtual void UpdateVisualState()
        {
            // Change color based on health
            if (TryGetComponent<Renderer>(out Renderer renderer))
            {
                float healthRatio = (float)currentHitPoints / hitPoints;
                Color currentColor = renderer.material.color;

                // Make brick darker as it takes damage
                renderer.material.color = new Color(
                    currentColor.r * healthRatio,
                    currentColor.g * healthRatio,
                    currentColor.b * healthRatio
                );
            }
        }

        /// <summary>
        /// Play destruction effects
        /// </summary>
        protected virtual void PlayDestroyEffects()
        {
            // You can add particle effects, sounds, etc. here
            // For now, just log the destruction
            Debug.Log($"Brick destroyed! Score: {scoreValue}");
        }

        /// <summary>
        /// Set brick properties
        /// </summary>
        public void SetBrickProperties(int hitPoints, int scoreValue, BrickType type)
        {
            this.hitPoints = hitPoints;
            this.scoreValue = scoreValue;
            this.brickType = type;
            this.currentHitPoints = hitPoints;

            UpdateVisualState();
        }

        /// <summary>
        /// Reset brick to initial state
        /// </summary>
        public void ResetBrick()
        {
            isDestroyed = false;
            currentHitPoints = hitPoints;
            UpdateVisualState();
        }
    }
}