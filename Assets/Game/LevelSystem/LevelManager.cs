using Assets.Game.Bricks;
using Assets.Game.Core;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Game.LevelSystem
{
    /// <summary>
    /// Manages level generation with random bricks
    /// </summary>
    public class LevelManager : MonoBehaviour
    {
        [Header("Level Generation Settings")]
        [SerializeField] Brick brickPrefab;
        [SerializeField] Transform brickContainer;
        [SerializeField] int rows = 3;       
        [SerializeField] int columns = 8;       
        [SerializeField] float XaxisIndentation = 0.02f;
        [SerializeField] float ZaxisIndentation = 0.02f;

        [Header("Brick Layout Settings")]
        [SerializeField] float brickWidth = 1.8f;
        [SerializeField] float brickHeight = 0.6f;
        [SerializeField] Vector2 gridStartOffset = new Vector2(-7f, 3f);

        [Header("Brick Probabilities")]
        [SerializeField] float standardBrickChance = 0.7f;
        [SerializeField] float strongBrickChance = 0.2f;
        [SerializeField] float indestructibleBrickChance = 0.1f;

        private List<Brick> spawnedBricks;
        private int totalDestroyableBricks;

        /// <summary>
        /// Total destroyable bricks in current level
        /// </summary>
        public int TotalDestroyableBricks
        {
            get { return totalDestroyableBricks; }
        }

        /// <summary>
        /// Remaining destroyable bricks
        /// </summary>
        public int RemainingBricks
        {
            get { return GetRemainingBricks(); }
        }

        private void Awake()
        {
          
            spawnedBricks = new List<Brick>();
            
            if (brickContainer == null)
            {
                CreateBrickContainer();
            }
        }

        /// <summary>
        /// Create brick container if not assigned
        /// </summary>
        private void CreateBrickContainer()
        {
            
            GameObject container = new GameObject("BrickContainer");
            
            brickContainer = container.transform;
            brickContainer.position = Vector3.zero;
        }

        /// <summary>
        /// Generate random level layout
        /// </summary>
        public void GenerateRandomLevel()
        {
            ClearCurrentLevel();
            totalDestroyableBricks = 0;

            Debug.Log($"Generating level: {rows} rows x {columns} columns");

            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < columns; col++)
                {
                 
                    if (Random.Range(0f, 1f) < 0.1f) continue;

                    Vector3 spawnPosition = CalculateGridPosition(row, col);
                    SpawnRandomBrick(spawnPosition, row, col);
                }
            }

            Debug.Log($"Level generated: {spawnedBricks.Count} bricks");
        }

        /// <summary>
        /// Calculate grid position for brick
        /// </summary>
        private Vector3 CalculateGridPosition(int row, int col)
        {
            Vector3 containerPosition = brickContainer.position;
            
            
            float x = containerPosition.x + gridStartOffset.x + col * XaxisIndentation;

           

            float y = brickContainer.position.y;
            
            float z = containerPosition.z + gridStartOffset.y - row  * ZaxisIndentation;

            return new Vector3(x, y, z);
        }

        /// <summary>
        /// Spawn brick with random type at grid position
        /// </summary>
        private void SpawnRandomBrick(Vector3 position, int row, int col)
        {
            if (brickPrefab == null)
            {
                Debug.LogError("Brick prefab is not assigned!");
                return;
            }

            Brick newBrick = Instantiate(brickPrefab, position, Quaternion.identity, brickContainer);
            
             newBrick.transform.localScale = Vector3.one;
            
            SetupRandomBrickType(newBrick);
            spawnedBricks.Add(newBrick);

            if (newBrick.Type != BrickType.Indestructible)
            {
                totalDestroyableBricks++;
            }

            Debug.Log($"Spawned brick at row {row}, col {col}, position: ({position.x:F1}, {position.y:F1})");
        }

        /// <summary>
        /// Setup brick type based on probabilities
        /// </summary>
        private void SetupRandomBrickType(Brick brick)
        {
            float randomValue = Random.Range(0f, 1f);
            
            if (randomValue < standardBrickChance)
            {
                SetupAsStandardBrick(brick);
            }
            else if (randomValue < standardBrickChance + strongBrickChance)
            {
                SetupAsStrongBrick(brick);
            }
            else
            {
                SetupAsIndestructibleBrick(brick);
            }
        }

        /// <summary>
        /// Configure brick as standard type
        /// </summary>
        private void SetupAsStandardBrick(Brick brick)
        {
            Renderer renderer = brick.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = Color.red;
            }
            if (brick is Brick brickComponent)
            {
                brickComponent.SetBrickProperties(1, 100, BrickType.Standard);
            }
        }

        /// <summary>
        /// Configure brick as strong type
        /// </summary>
        private void SetupAsStrongBrick(Brick brick)
        {
            Renderer renderer = brick.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = Color.blue;
            }
            if (brick is Brick brickComponent)
            {
                brickComponent.SetBrickProperties(2, 200, BrickType.Strong);
            }
        }

        /// <summary>
        /// Configure brick as indestructible type
        /// </summary>
        private void SetupAsIndestructibleBrick(Brick brick)
        {
            Renderer renderer = brick.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = Color.gray;
            }
            if (brick is Brick brickComponent)
            {
                brickComponent.SetBrickProperties(999, 0, BrickType.Indestructible);
            }
        }

        /// <summary>
        /// Clear current level bricks
        /// </summary>
        private void ClearCurrentLevel()
        {
            if (spawnedBricks == null) return;

            foreach (Brick brick in spawnedBricks)
            {
                if (brick != null)
                {
                    Destroy(brick.gameObject);
                }
            }
            spawnedBricks.Clear();
        }

        /// <summary>
        /// Get count of remaining destroyable bricks
        /// </summary>
        private int GetRemainingBricks()
        {
            int count = 0;
            foreach (Brick brick in spawnedBricks)
            {
                if (brick != null && !brick.IsDestroyed && brick.Type != BrickType.Indestructible)
                {
                    count++;
                }
            }
            return count;
        }

        /// <summary>
        /// Check if level is cleared
        /// </summary>
        public bool IsLevelCleared()
        {
            return GetRemainingBricks() <= 0;
        }

        /// <summary>
        /// Draw grid in scene view for debugging
        /// </summary>
        private void OnDrawGizmos()
        {
            if (brickContainer == null) return;

            Gizmos.color = Color.cyan;
            
            
            
                Vector3 containerPos = brickContainer.position;
            
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < columns; col++)
                {
                    Vector3 position = CalculateGridPosition(row, col);
                            

                    Gizmos.DrawWireCube(position, new Vector3(brickWidth * 0.9f, brickHeight * 0.9f, 0.5f));
                }
            }
        }
    }
}