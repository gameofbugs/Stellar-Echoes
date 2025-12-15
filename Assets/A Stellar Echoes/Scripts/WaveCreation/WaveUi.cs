// ============================================
// WaveUi.cs (Optimized)
// ============================================

using UnityEngine;
using TMPro;

public class WaveUi : MonoBehaviour
{
    [Header("References")]
    public WaveManager waveManager;

    [Header("UI Elements")]
    public TextMeshProUGUI enemiesText;
    public TextMeshProUGUI waveText;

    // OPTIMIZED: Use LateUpdate to ensure wave manager has updated
    public void LateUpdate()
    {
        if (waveManager == null) return;

        // OPTIMIZED: Update UI text
        if (enemiesText != null)
        {
            enemiesText.text = $"Enemies: {waveManager.AliveEnemiesCount()}/{waveManager.NumberOfEnemiesInCurrentWave()}";
        }

        if (waveText != null)
        {
            waveText.text = $"Wave: {waveManager.GetWaveNumber() + 1}/{waveManager.TotalNumberOfWaves()}";
        }
    }
}
