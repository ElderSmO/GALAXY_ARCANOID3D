using Assets.Game.Core;
using Assets.Game.GameManagerFolder;
using Assets.Game.Player;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Game.UI
{
    /// <summary>
    /// Manages all UI elements and screens
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        [Header("Game UI")]
        [SerializeField] TextMeshProUGUI scoreText;
        [SerializeField] TextMeshProUGUI livesText;

        [Header("Screens")]
        [SerializeField] GameObject menuScreen;
        [SerializeField] GameObject gameScreen;
        [SerializeField] GameObject pauseScreen;
        [SerializeField] GameObject gameOverScreen;
        [SerializeField] GameObject victoryScreen;

        [Header("Buttons")]
        [SerializeField] Button startButton;
        [SerializeField] Button pauseButton;
        [SerializeField] Button resumeButton;
        [SerializeField] Button restartButton;
        [SerializeField] Button menuButton;

        [Header("Control Buttons")]
        [SerializeField] Button leftButton;
        [SerializeField] Button rightButton;

        private GameManager gameController;
        private Paddle playerPaddle;

        private bool isLeftButtonPressed = false;
        private bool isRightButtonPressed = false;

        private void Awake()
        {
            FindReferences();
            SetupButtonListeners();
        }

        private void Start()
        {
            InitializeUI();
        }

        private void Update()
        {
            ProcessInput();
        }

        private void OnEnable()
        {
            SubscribeToEvents();
        }

        private void OnDisable()
        {
            UnsubscribeFromEvents();
        }

        /// <summary>
        /// Find required references
        /// </summary>
        private void FindReferences()
        {
            gameController = FindFirstObjectByType<GameManager>();
            playerPaddle = FindFirstObjectByType<Paddle>();
        }

        /// <summary>
        /// Setup button click listeners
        /// </summary>
        private void SetupButtonListeners()
        {
            startButton.onClick.AddListener(HandleStartGame);
            pauseButton.onClick.AddListener(HandlePauseGame);
            resumeButton.onClick.AddListener(HandleResumeGame);
            restartButton.onClick.AddListener(HandleRestartGame);
            menuButton.onClick.AddListener(HandleReturnToMenu);

          
            SetupControlButton(leftButton, -1f);
            SetupControlButton(rightButton, 1f);
        }

        /// <summary>
        /// Setup paddle control button with hold support
        /// </summary>
        private void SetupControlButton(Button button, float direction) 
        {
            var trigger = button.gameObject.AddComponent<UnityEngine.EventSystems.EventTrigger>();

            // Pointer down event
            var pointerDown = new UnityEngine.EventSystems.EventTrigger.Entry();
            pointerDown.eventID = UnityEngine.EventSystems.EventTriggerType.PointerDown;
            pointerDown.callback.AddListener((data) => { OnControlButtonPressed(direction); });
            trigger.triggers.Add(pointerDown);

            // Pointer up event
            var pointerUp = new UnityEngine.EventSystems.EventTrigger.Entry();
            pointerUp.eventID = UnityEngine.EventSystems.EventTriggerType.PointerUp;
            pointerUp.callback.AddListener((data) => { OnControlButtonReleased(direction); });
            trigger.triggers.Add(pointerUp);

            // Pointer exit event 
            var pointerExit = new UnityEngine.EventSystems.EventTrigger.Entry();
            pointerExit.eventID = UnityEngine.EventSystems.EventTriggerType.PointerExit;
            pointerExit.callback.AddListener((data) => { OnControlButtonReleased(direction); });
            trigger.triggers.Add(pointerExit);
        }

        /// <summary>
        /// Handle control button press
        /// </summary>
        private void OnControlButtonPressed(float direction)
        {
            if (direction < 0)
            {
                isLeftButtonPressed = true;
                Debug.Log("Left button pressed");
            }
            else if (direction > 0)
            {
                isRightButtonPressed = true;
                Debug.Log("Right button pressed");
            }
        }

        /// <summary>
        /// Handle control button release
        /// </summary>
        private void OnControlButtonReleased(float direction)
        {
            if (direction < 0)
            {
                isLeftButtonPressed = false;
                Debug.Log("Left button released");
            }
            else if (direction > 0)
            {
                isRightButtonPressed = false;
                Debug.Log("Right button released");
            }
        }

        /// <summary>
        /// Process input from control buttons every frame
        /// </summary>
        private void ProcessInput()
        {
            if (playerPaddle != null)
            {
                float moveInputX = 0f;
                float moveInputZ = 0f;

                if (isLeftButtonPressed) moveInputX -= 1f;
                if (isRightButtonPressed) moveInputX += 1f;

                playerPaddle.SetMoveInput(moveInputX, moveInputZ);
            }
        }

        /// <summary>
        /// Subscribe to game events
        /// </summary>
        private void SubscribeToEvents()
        {
            if (gameController != null)
            {
                gameController.OnStateChanged += HandleGameStateChanged;
                gameController.OnScoreChanged += UpdateScoreDisplay;
                gameController.OnLivesChanged += UpdateLivesDisplay;
            }
        }

        /// <summary>
        /// Unsubscribe from events
        /// </summary>
        private void UnsubscribeFromEvents()
        {
            if (gameController != null)
            {
                gameController.OnStateChanged -= HandleGameStateChanged;
                gameController.OnScoreChanged -= UpdateScoreDisplay;
                gameController.OnLivesChanged -= UpdateLivesDisplay;
            }
        }

        /// <summary>
        /// Initialize UI state
        /// </summary>
        private void InitializeUI()
        {
            ShowScreen(menuScreen);
            UpdateScoreDisplay(0);
            UpdateLivesDisplay(3);
        }

        /// <summary>
        /// Handle game state changes
        /// </summary>
        private void HandleGameStateChanged(GameState newState)
        {
            switch (newState)
            {
                case GameState.Menu:
                    ShowMenuScreen();
                    break;
                case GameState.Playing:
                    ShowGameScreen();
                    break;
                case GameState.Paused:
                    ShowPauseScreen();
                    break;
                case GameState.GameOver:
                    ShowGameOverScreen();
                    break;
                case GameState.Victory:
                    ShowVictoryScreen();
                    break;
            }
        }


        /// <summary>
        /// Show menu screen
        /// </summary>
        private void ShowMenuScreen()
        {
            ShowScreen(menuScreen);
            SetControlsEnabled(false);
        }

        /// <summary>
        /// Show game screen
        /// </summary>
        private void ShowGameScreen()
        {
            ShowScreen(gameScreen);
            SetControlsEnabled(true);
        }

        /// <summary>
        /// Show pause screen
        /// </summary>
        private void ShowPauseScreen()
        {
            ShowScreen(pauseScreen);


            SetControlsEnabled(false);
        }

        /// <summary>
        /// Show game over screen
        /// </summary>
        private void ShowGameOverScreen()
        {
                ShowScreen(gameOverScreen);
            SetControlsEnabled(false);

            
        }

        /// <summary>
        /// Show victory screen
        /// </summary>
        private void ShowVictoryScreen()
        {
            ShowScreen(victoryScreen);
            SetControlsEnabled(false);
        }

        /// <summary>
        /// Show specific screen and hide others
        /// </summary>
        private void ShowScreen(GameObject screen)
        {
            HideAllScreens();

            if (screen != null)
            {
                screen.SetActive(true);
            }
        }

        /// <summary>
        /// Hide all UI screens
        /// </summary>
        private void HideAllScreens()
        {
            GameObject[] screens = { menuScreen, gameScreen, pauseScreen, gameOverScreen, victoryScreen };

            foreach (GameObject screen in screens)
            {
                if (screen != null)
                {
                    screen.SetActive(false);
                }
            }
        }

        /// <summary>
        /// Update score display
        /// </summary>
        public void UpdateScoreDisplay(int score)
        {
            if (scoreText != null)
            {
                scoreText.text = $"SCORE: {score.ToString("D6")}";
            }
        }

        /// <summary>
        /// Update lives display
        /// </summary>
        public void UpdateLivesDisplay(int lives)
        {
            if (livesText != null)
            {
                livesText.text = $"LIVES: {lives}";
            }
        }

        /// <summary>
        /// Handle start game button
        /// </summary>
        private void HandleStartGame()
        {
            if (gameController != null)
            {
                gameController.StartGame();
            }
        }

        /// <summary>
        /// Handle pause game button
        /// </summary>
        private void HandlePauseGame()
        {
            if (gameController != null)
            {
                gameController.PauseGame();
            }
        }

        /// <summary>
        /// Handle resume game button
        /// </summary>
        private void HandleResumeGame()
        {
            if (gameController != null)
            {
                gameController.ResumeGame();
            }
        }

        /// <summary>
        /// Handle restart game button
        /// </summary>
        private void HandleRestartGame()
        {
            if (gameController != null)
            {
                gameController.RestartGame();
            }
        }

        /// <summary>
        /// Handle return to menu button
        /// </summary>
        private void HandleReturnToMenu()
        {
            if (gameController != null)
            {
                gameController.ChangeState(GameState.Menu);
            }
        }

        /// <summary>
        /// Enable or disable control buttons
        /// </summary>
        public void SetControlsEnabled(bool enabled)
        {
            if (leftButton != null) leftButton.interactable = enabled;
            if (rightButton != null) rightButton.interactable = enabled;

            if (!enabled)
            {
                isLeftButtonPressed = false;
                isRightButtonPressed = false;

                if (playerPaddle != null)
                {
                    playerPaddle.SetMoveInput(0f);
                }
            }
        }

        /// <summary>
        /// Update level display
        /// </summary>
        public void UpdateLevelDisplay(int level)
        {
            
        }
    }
}