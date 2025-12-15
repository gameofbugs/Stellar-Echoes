// ============================================
// CameraMovement.cs (Optimized)
// ============================================

using System;
using Unity.Cinemachine;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [Header("Camera Reference")]
    public CinemachineCamera playerFollowCamera; // FIXED TYPO: was playerFallowCamera

    [Header("FOV Settings")]
    public float chaseFov;
    public float defaultFov;
    public float attackFov;
    public float camMovementSmooth; // FIXED TYPO: was camMovementSmoth
    public bool classicCamera = false; // FIXED TYPO: was clasicCamera

    // OPTIMIZED: Private variables
    private float currentFov;

    // OPTIMIZED: Events (currently unused, but kept for future use)
    public event EventHandler OnChase;
    public event EventHandler OnAttack;

    void Awake()
    {
        // OPTIMIZED: Find player and set as follow target
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null && playerFollowCamera != null)
        {
            playerFollowCamera.Follow = player.transform;
        }

        // OPTIMIZED: Invoke events if needed (currently no subscribers)
        OnChase?.Invoke(this, EventArgs.Empty);
        OnAttack?.Invoke(this, EventArgs.Empty);
    }

    public void Start()
    {
        SetCameraOrthographicSize(defaultFov);
    }

    void Update()
    {
        if (playerFollowCamera == null) return;

        // OPTIMIZED: Determine target FOV
        float targetFov = classicCamera ? defaultFov : currentFov;

        // OPTIMIZED: Smoothly lerp to target FOV
        playerFollowCamera.Lens.OrthographicSize = Mathf.Lerp(
            playerFollowCamera.Lens.OrthographicSize,
            targetFov,
            camMovementSmooth * Time.deltaTime
        );
    }

    public void SetCameraOrthographicSize(float size)
    {
        currentFov = size;
    }
}