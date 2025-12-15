using System;
using UnityEngine;

/// <summary>
/// GameManager - Handles ONLY game state management
/// States: MainMenu, Playing, Paused, WaveTransition, GameOver, Victory
/// </summary>
public class GameManager : MonoBehaviour
{
    /* public static GameManager Instance;

     // ============================================
     // GAME STATE SYSTEM
     // ============================================
     public enum GameState
     {
         MainMenu,       // In main menu
         Playing,        // Actively playing
         Paused,         // Game paused
         WaveTransition, // Between waves
         GameOver,       // Player died
         Victory         // All waves completed
     }

     [Header("Current State")]
     public GameState currentState = GameState.MainMenu;
     private GameState previousState;

     [Header("UI Panel References")]
     public GameObject mainMenuPanel;
     public GameObject gameplayUIPanel;
     public GameObject pauseMenuPanel;
     public GameObject gameOverPanel;
     public GameObject victoryPanel;
     public GameObject waveTransitionPanel;

     [Header("Game Object References")]
     public GameObject player;
     public GameObject managers; // ObjectPool, WaveManager, etc.

     // Events - Subscribe to know when state changes
     public event Action<GameState> OnStateChanged;
     public event Action OnGameStarted;
     public event Action OnGamePaused;
     public event Action OnGameResumed;
     public event Action OnGameOver;
     public event Action OnVictory;

     void Awake()
     {
         // Singleton pattern
         if (Instance == null)
         {
             Instance = this;
         }
         else
         {
             Destroy(gameObject);
             return;
         }
     }

     void Start()
     {
         // Check if we should auto-start the game (after retry)
         if (PlayerPrefs.GetInt("AutoStartGame", 0) == 1)
         {
             PlayerPrefs.DeleteKey("AutoStartGame");
             ChangeState(GameState.Playing);
         }
         else
         {
             // Normal start - show main menu
             ChangeState(GameState.MainMenu);
         }
     }

     void Update()
     {
         // Handle pause input (ESC key)
         if (Input.GetKeyDown(KeyCode.Escape))
         {
             HandlePauseInput();
         }
     }

     // ============================================
     // STATE MANAGEMENT
     // ============================================
     public void ChangeState(GameState newState)
     {
         if (currentState == newState) return;

         // Exit current state
         ExitState(currentState);

         previousState = currentState;
         currentState = newState;

         // Enter new state
         EnterState(newState);

         // Trigger event
         OnStateChanged?.Invoke(newState);
     }

     private void EnterState(GameState state)
     {
         switch (state)
         {
             case GameState.MainMenu:
                 EnterMainMenu();
                 break;
             case GameState.Playing:
                 EnterPlaying();
                 break;
             case GameState.Paused:
                 EnterPaused();
                 break;
             case GameState.WaveTransition:
                 EnterWaveTransition();
                 break;
             case GameState.GameOver:
                 EnterGameOver();
                 break;
             case GameState.Victory:
                 EnterVictory();
                 break;
         }
     }

     private void ExitState(GameState state)
     {
         switch (state)
         {
             case GameState.MainMenu:
                 ExitMainMenu();
                 break;
             case GameState.Playing:
                 ExitPlaying();
                 break;
             case GameState.Paused:
                 ExitPaused();
                 break;
             case GameState.WaveTransition:
                 ExitWaveTransition();
                 break;
             case GameState.GameOver:
                 ExitGameOver();
                 break;
             case GameState.Victory:
                 ExitVictory();
                 break;
         }
     }

     // ============================================
     // STATE: MAIN MENU
     // ============================================
     private void EnterMainMenu()
     {
         Time.timeScale = 1f;

         // Show/Hide panels
         SetPanelActive(mainMenuPanel, true);
         SetPanelActive(gameplayUIPanel, false);
         SetPanelActive(pauseMenuPanel, false);
         SetPanelActive(gameOverPanel, false);
         SetPanelActive(victoryPanel, false);
         SetPanelActive(waveTransitionPanel, false);

         // Disable game objects
         if (player != null) player.SetActive(false);
         if (managers != null) managers.SetActive(false);
     }

     private void ExitMainMenu()
     {
         SetPanelActive(mainMenuPanel, false);
     }

     // ============================================
     // STATE: PLAYING
     // ============================================
     private void EnterPlaying()
     {
         Time.timeScale = 1f;

         // Show/Hide panels
         SetPanelActive(mainMenuPanel, false);
         SetPanelActive(gameplayUIPanel, true);
         SetPanelActive(pauseMenuPanel, false);
         SetPanelActive(gameOverPanel, false);
         SetPanelActive(victoryPanel, false);
         SetPanelActive(waveTransitionPanel, false);

         // Enable game objects
         if (player != null) player.SetActive(true);
         if (managers != null) managers.SetActive(true);

         // Trigger event
         OnGameStarted?.Invoke();
     }

     private void ExitPlaying()
     {
         // Nothing to do
     }

     // ============================================
     // STATE: PAUSED
     // ============================================
     private void EnterPaused()
     {
         Time.timeScale = 0f;
         AudioListener.pause = true;

         // Show pause menu (keep gameplay UI visible but grayed out)
         SetPanelActive(pauseMenuPanel, true);

         // Trigger event
         OnGamePaused?.Invoke();
     }

     private void ExitPaused()
     {
         Time.timeScale = 1f;
         AudioListener.pause = false;

         SetPanelActive(pauseMenuPanel, false);

         // Trigger event
         OnGameResumed?.Invoke();
     }

     // ============================================
     // STATE: WAVE TRANSITION
     // ============================================
     private void EnterWaveTransition()
     {
         Time.timeScale = 0.5f; // Slow motion effect

         SetPanelActive(waveTransitionPanel, true);
     }

     private void ExitWaveTransition()
     {
         Time.timeScale = 1f;

         SetPanelActive(waveTransitionPanel, false);
     }

     // ============================================
     // STATE: GAME OVER
     // ============================================
     private void EnterGameOver()
     {
         Time.timeScale = 0f;

         // Show game over panel
         SetPanelActive(gameOverPanel, true);
         SetPanelActive(gameplayUIPanel, false);

         // Trigger event
         OnGameOver?.Invoke();
     }

     private void ExitGameOver()
     {
         SetPanelActive(gameOverPanel, false);
     }

     // ============================================
     // STATE: VICTORY
     // ============================================
     private void EnterVictory()
     {
         Time.timeScale = 0f;

         // Show victory panel
         SetPanelActive(victoryPanel, true);
         SetPanelActive(gameplayUIPanel, false);

         // Trigger event
         OnVictory?.Invoke();
     }

     private void ExitVictory()
     {
         SetPanelActive(victoryPanel, false);
     }

     // ============================================
     // PUBLIC STATE CONTROL METHODS
     // ============================================

     /// <summary>
     /// Start the game from main menu
     /// </summary>
     public void StartGame()
     {
         ChangeState(GameState.Playing);
     }

     /// <summary>
     /// Pause the game (from Playing state)
     /// </summary>
     public void PauseGame()
     {
         if (currentState == GameState.Playing)
         {
             ChangeState(GameState.Paused);
         }
     }

     /// <summary>
     /// Resume the game (from Paused state)
     /// </summary>
     public void ResumeGame()
     {
         if (currentState == GameState.Paused)
         {
             ChangeState(GameState.Playing);
         }
     }

     /// <summary>
     /// Return to main menu from any state
     /// </summary>
     public void ReturnToMenu()
     {
         ChangeState(GameState.MainMenu);
     }

     /// <summary>
     /// Trigger game over (called when player dies)
     /// </summary>
     public void TriggerGameOver()
     {
         if (currentState == GameState.Playing)
         {
             ChangeState(GameState.GameOver);
         }
     }

     /// <summary>
     /// Trigger victory (called when all waves completed)
     /// </summary>
     public void TriggerVictory()
     {
         if (currentState == GameState.Playing)
         {
             ChangeState(GameState.Victory);
         }
     }

     /// <summary>
     /// Show wave transition screen
     /// </summary>
     public void ShowWaveTransition()
     {
         if (currentState == GameState.Playing)
         {
             ChangeState(GameState.WaveTransition);
         }
     }

     /// <summary>
     /// End wave transition and return to playing
     /// </summary>
     public void EndWaveTransition()
     {
         if (currentState == GameState.WaveTransition)
         {
             ChangeState(GameState.Playing);
         }
     }

     // ============================================
     // PAUSE INPUT HANDLING
     // ============================================
     private void HandlePauseInput()
     {
         if (currentState == GameState.Playing)
         {
             PauseGame();
         }
         else if (currentState == GameState.Paused)
         {
             ResumeGame();
         }
     }

     // ============================================
     // PUBLIC GETTERS
     // ============================================

     /// <summary>
     /// Check if game is currently playing
     /// </summary>
     public bool IsPlaying() => currentState == GameState.Playing;

     /// <summary>
     /// Check if game is paused
     /// </summary>
     public bool IsPaused() => currentState == GameState.Paused;

     /// <summary>
     /// Check if game is over
     /// </summary>
     public bool IsGameOver() => currentState == GameState.GameOver;

     /// <summary>
     /// Check if player won
     /// </summary>
     public bool IsVictory() => currentState == GameState.Victory;

     /// <summary>
     /// Get current game state
     /// </summary>
     public GameState GetCurrentState() => currentState;

     /// <summary>
     /// Get previous game state
     /// </summary>
     public GameState GetPreviousState() => previousState;

     // ============================================
     // HELPER METHODS
     // ============================================
     private void SetPanelActive(GameObject panel, bool active)
     {
         if (panel != null)
         {
             panel.SetActive(active);
         }
    }
 */
}