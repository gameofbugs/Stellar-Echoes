// ============================================
// ScoreSystem.cs (with High Score + PlayerPrefs)
// ============================================

using System;
using UnityEngine; // Needed for PlayerPrefs

public class ScoreSystem
{
    private int currentScore;
    private int highScore;

    public event Action OnScoreAdded;
    public event Action OnHighScoreUpdated;

    private const string HighScoreKey = "HighScore";

    public ScoreSystem()
    {
        LoadHighScore();
    }

    public int GetScore()
    {
        return currentScore;
    }

    public int GetHighScore()
    {
        return highScore;
    }

    public void AddScore(int scoreAmount)
    {
        currentScore += scoreAmount;
        OnScoreAdded?.Invoke();

        // Check and update high score
        if (currentScore > highScore)
        {
            highScore = currentScore;
            SaveHighScore();
            OnHighScoreUpdated?.Invoke();
        }
    }

    public void ResetScore()
    {
        currentScore = 0;
        OnScoreAdded?.Invoke();
    }

    private void SaveHighScore()
    {
        PlayerPrefs.SetInt(HighScoreKey, highScore);
        PlayerPrefs.Save();
    }

    private void LoadHighScore()
    {
        highScore = PlayerPrefs.GetInt(HighScoreKey, 0);
    }
}
