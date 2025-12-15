// ============================================
// UiManager.cs (Optimized)
// ============================================

using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UiManager : MonoBehaviour
{
    // OPTIMIZED: Static flag to skip main menu on retry
    public static bool isMainMenuSkipped = false;

    [Header("Panels")]
    public GameObject mainMenu;
    public GameObject resultPanel;
    public GameObject settingPanel;
    public GameObject playerUI;

    [Header("Game References")]
    public GameObject managers;
    public GameObject player;
    public SpaceShipMovement spaceShipMovement;
    public WaveManager waveManager;

    [Header("Result Texts")]
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI resultText;
    public TextMeshProUGUI scoreText;
    //public TextMeshProUGUI highScoreText;
    //public TextMeshProUGUI timeSurvivedText;
    //public TextMeshProUGUI waveClearedText;
    // public TextMeshProUGUI accuracyText;
    // public TextMeshProUGUI totalPowerUpsCollectedText;

    void Start()
    {
        // OPTIMIZED: Load audio settings first
        AudioManager.Instance?.LoadVolumeSettings();

        Time.timeScale = 1f;

        // OPTIMIZED: Handle main menu skip on retry
        if (isMainMenuSkipped)
        {
            StartGame();
            isMainMenuSkipped = false;
        }
        else
        {
            ShowMainMenu();
        }
    }

    // OPTIMIZED: Separated start game logic
    private void StartGame()
    {
        ShowPanel(playerUI);
        mainMenu.SetActive(false);
        resultPanel.SetActive(false);
        player?.SetActive(true);
        managers?.SetActive(true);
    }

    // OPTIMIZED: Separated main menu logic
    private void ShowMainMenu()
    {
        ShowPanel(mainMenu);
        resultPanel.SetActive(false);
        player?.SetActive(false);
        playerUI?.SetActive(false);
        managers?.SetActive(false);
    }

    // ================ GAME STATE EVENTS ================

    public void GameOver()
    {
        Time.timeScale = 0f;
        HidePanel(playerUI);

        // OPTIMIZED: Use coroutine for delay with timeScale = 0
        StartCoroutine(ShowGameOverPanelAfterDelay(0.5f));
    }

    private IEnumerator ShowGameOverPanelAfterDelay(float delay)
    {
        // OPTIMIZED: Use realtime since timeScale = 0
        yield return new WaitForSecondsRealtime(delay);

        ShowPanel(resultPanel);

        // OPTIMIZED: Set game over text
        if (titleText != null) titleText.text = "GAME\nOVER";
        if (resultText != null) resultText.text = "SPACESHIP CRASHED!!!";
        if (scoreText != null && spaceShipMovement != null)
            scoreText.text = $"{spaceShipMovement.GetScore()}";
        // if (waveClearedText != null && waveManager != null)
        //waveClearedText.text = $"{waveManager.currentWave}"; // OPTIMIZED: Fixed off-by-one
    }

    public void GameCompleted()
    {
        Time.timeScale = 0f;
        HidePanel(mainMenu);
        HidePanel(playerUI);

        StartCoroutine(ShowGameCompletedPanelAfterDelay(0.5f));
    }

    private IEnumerator ShowGameCompletedPanelAfterDelay(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);

        ShowPanel(resultPanel);

        // OPTIMIZED: Set completion text
        if (titleText != null) titleText.text = "GAME\nCOMPLETED";
        if (resultText != null) resultText.text = "MISSION ACCOMPLISHED!!!";
        if (scoreText != null && spaceShipMovement != null)
            scoreText.text = $"{spaceShipMovement.GetScore()}";
        //if (waveClearedText != null && waveManager != null)
        //  waveClearedText.text = $"{waveManager.TotalNumberOfWaves()}";
    }

    // ================ BUTTON ACTIONS ================

    public void Retry()
    {
        isMainMenuSkipped = true;
        HidePanel(resultPanel);
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Home()
    {
        Time.timeScale = 1f;
        isMainMenuSkipped = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Play()
    {
        HidePanel(mainMenu);
        ShowPanel(playerUI);
        player?.SetActive(true);
        managers?.SetActive(true);
    }

    public void Exit()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    // ================ PANEL CONTROLS ================

    public void ShowPanel(GameObject panel)
    {
        if (panel != null)
        {
            panel.SetActive(true); // Triggers UiAnimations fade-in
        }
    }

    public void HidePanel(GameObject panel)
    {
        if (panel == null) return;

        UiAnimations anim = panel.GetComponent<UiAnimations>();
        if (anim != null)
        {
            anim.HidePanel(); // Plays fade-out, then disables
        }
        else
        {
            panel.SetActive(false);
        }
    }
}
