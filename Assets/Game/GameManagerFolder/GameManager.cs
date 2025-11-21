using Assets.Game.BallFolder;
using Assets.Game.Bricks;
using Assets.Game.Core;
using Assets.Game.LevelSystem;
using Assets.Game.Player;
using System.Collections;
using UnityEngine;

namespace Assets.Game.GameManagerFolder
{
    /// <summary>
    /// Main game controller managing states and scoring
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        [Header("Game References")]
        [SerializeField] Paddle playerPaddle;
        [SerializeField] Ball gameBall = null;
        [SerializeField] LevelManager levelManager;

        [Header("Game Settings")]
        [SerializeField] int startingLives = 3;
        [SerializeField] float ballLaunchDelay = 2f;
        [SerializeField] float gameOverRestartDelay = 3f;

        GameState currentGameState = GameState.Menu;
         int currentScore = 0;
        int currentLives = 0;
        bool isBallReadyToLaunch = false;
         Coroutine restartCoroutine;

        /// <summary>
        /// Current game state
        /// </summary>
        public GameState CurrentState
        {
            get { return currentGameState; }
        }

        /// <summary>
        /// Current player score
        /// </summary>
        public int CurrentScore
        {
            get { return currentScore; }
        }

        /// <summary>
        /// Current player lives
        /// </summary>
        public int CurrentLives
        {
            get { return currentLives; }
        }

        /// <summary>
        /// Event called when game state changes
        /// </summary>
        public event System.Action<GameState> OnStateChanged;

        /// <summary>
        /// Event called when score changes
        /// </summary>
        public event System.Action<int> OnScoreChanged;

        /// <summary>
        /// Event called when lives change
        /// </summary>
        public event System.Action<int> OnLivesChanged;

        private void Start()
        {
            InitializeGame();
            ChangeState(GameState.Menu);
        }

        private void OnEnable()
        {
            Brick.BrickDestroyed += HandleBrickDestroyed;
        }

        private void OnDisable()
        {
            Brick.BrickDestroyed -= HandleBrickDestroyed;
        }

        /// <summary>
        /// Initialize game systems
        /// </summary>
        private void InitializeGame()
        {
            currentLives = startingLives;
            if (levelManager != null)
            {
                levelManager.GenerateRandomLevel();
            }
            else
            {
                Debug.LogError("LevelManager reference is missing!");
            }
        }

        /// <summary>
        /// Change game state and handle transitions
        /// </summary>
        public void ChangeState(GameState newState)
        {
            if (currentGameState == newState) return;

            currentGameState  = newState;
             OnStateChanged?.Invoke(newState);
            HandleStateTransition(newState);
        }

        /// <summary>
        /// Handle logic for state transitions
        /// </summary>
        private void HandleStateTransition(GameState state)
        {
            switch (state)
            {
                case GameState.Menu:
                    Time.timeScale = 1f;
                    ResetGame();
                    break;
                case GameState.Playing:
                    Time.timeScale = 1f;
                    PrepareBallForLaunch();
                    break;
                case GameState.Paused:
                    Time.timeScale = 0f;
                    break;
                case GameState.GameOver:
                    HandleGameOver();
                    break;
                case GameState.Victory:
                    HandleVictory();
                    break;
            }
        }

        /// <summary>
        /// Start new game
        /// </summary>
        public void StartGame()
        {
            Debug.Log("GameManager Starting game");
            ResetGame();


                ChangeState(GameState.Playing);
        }

        /// <summary>
        /// Restart game with new level
        /// </summary>
        public void RestartGame()
        {
            if (restartCoroutine != null)
            {
                 StopCoroutine(restartCoroutine);

                restartCoroutine = null;
            }

            Debug.Log("GameManager: Restarting game");

            if (levelManager != null) levelManager.GenerateRandomLevel();




            ResetGame();
            ChangeState(GameState.Playing);
        }

        /// <summary>
        /// Reset game values to initial state
        /// </summary>
        private void ResetGame()
        {
            currentScore = 0;
            currentLives = startingLives;
            isBallReadyToLaunch = false;

            OnScoreChanged?.Invoke(currentScore);
            OnLivesChanged?.Invoke(currentLives);

            if (playerPaddle != null)
            {
                playerPaddle.ResetPosition();
            }
            else
            {
                Debug.LogError("PlayerPaddle reference is missing!");
            }

            if (gameBall != null)
            {
                gameBall.ResetBall();
                gameBall.ResetToPaddle(playerPaddle.transform);
            }
            else
            {
                Debug.LogError("GameBall reference is missing!");
            }
        }

        /// <summary>
        /// Prepare ball for launch after delay
        /// </summary>
        private void PrepareBallForLaunch()
        {
            if (currentGameState != GameState.Playing) return;

            Debug.Log("GameManager: Preparing ball for launch");
            isBallReadyToLaunch = true;
            Invoke("LaunchBall", ballLaunchDelay);
        }

        /// <summary>
        /// Launch ball in chosen direction
        /// </summary>
        private void LaunchBall()
        {
            if (gameBall != null && !gameBall.IsLaunched && isBallReadyToLaunch && currentGameState == GameState.Playing)
            {
                gameBall.Launch();
                Debug.Log("GameManager: Ball launched");
            }
        }

        /// <summary>
        /// Handle brick destruction
        /// </summary>
        private void HandleBrickDestroyed(Brick destroyedBrick)
        {
            AddScore(destroyedBrick.ScoreValue);

            //Winner check
            if (levelManager != null && levelManager.IsLevelCleared())
            {
                ChangeState(GameState.Victory);
            }
        }

        /// <summary>
        /// Handle ball loss
        /// </summary>
        public void OnBallLost()
        {
            if (currentGameState != GameState.Playing) return;

            Debug.Log("GameManager Ball lost");
            currentLives--;
            isBallReadyToLaunch = false;

            OnLivesChanged?.Invoke(currentLives);

            if (currentLives <= 0)
            {
                ChangeState(GameState.GameOver);
            }
            else
            {
                if (playerPaddle != null)
                {
                    playerPaddle.ResetPosition();
                }
                if (gameBall != null)
                {
                    gameBall.ResetBall();
                     gameBall.ResetToPaddle(playerPaddle.transform);
                   PrepareBallForLaunch();
                }
            }
        }

        /// <summary>
        /// Handle game over state
        /// </summary>
        private void HandleGameOver()
        {
            Debug.Log("GameManager: Game Over! Restarting in 3 seconds...");
            isBallReadyToLaunch = false;

            if (gameBall != null)
            {
                gameBall.ResetBall();
            }

            if (restartCoroutine != null)
                StopCoroutine(restartCoroutine);
            restartCoroutine = StartCoroutine(AutoRestartAfterDelay());
        }

        /// <summary>
        /// Automatically restart game after delay
        /// </summary>
        private IEnumerator AutoRestartAfterDelay()
        {
            yield return new WaitForSeconds(gameOverRestartDelay);
            RestartGame();
        }

        /// <summary>
        /// Handle victory state
        /// </summary>
        private void HandleVictory()
        {
            Debug.Log("GameManager: Victory!");
            isBallReadyToLaunch = false;
        }

        /// <summary>
        /// Add points to score
        /// </summary>
        public void AddScore(int points)
        {
            currentScore += points;
            OnScoreChanged?.Invoke(currentScore);
        }

        /// <summary>
        /// Pause the game
        /// </summary>
        public void PauseGame()
        {
            if (currentGameState == GameState.Playing)
            {
                ChangeState(GameState.Paused);
            }
        }

        /// <summary>
        /// Resume paused game
        /// </summary>
        public void ResumeGame()
        {
            if (currentGameState == GameState.Paused)
            {

                ChangeState(GameState.Playing);

              }
        }

        /// <summary>
        /// Manually launch ball for testing
        /// </summary>
        public void ManualLaunchBall()
        {
            if (gameBall != null && !gameBall.IsLaunched)
            {
                gameBall.Launch();
            }
        }
    }
}