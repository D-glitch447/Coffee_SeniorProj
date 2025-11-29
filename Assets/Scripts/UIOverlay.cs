using System;
using UnityEditor;
using UnityEngine;

public class UIOverlay : MonoBehaviour
{
    [Header("PopUp")]
    [SerializeField] private GameObject pausePopup;

    private bool isPaused = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (pausePopup != null) pausePopup.SetActive(false);
        SetTimeScale(1f);
        Debug.Log("[UI] UIOverlay initialized");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isPaused) Pause();
            else ContinueGame();
        }
    }

    public void OnBackPressed()
    {
        Debug.Log("[UI] Back pressed");
    }

    public void OnPausePressed()
    {
        Pause();
    }

    public void OnContinuePressed()
    {
        ContinueGame();
    }

    public void OnExitPressed()
    {
        ExitGame();
    }

    private void Pause()
    {
        isPaused = true;
        if (pausePopup != null) pausePopup.SetActive(true);
        SetTimeScale(0f);
        Debug.Log("[UI] Game Paused");
    }

    private void ContinueGame()
    {
        isPaused = false;
        if (pausePopup != null) pausePopup.SetActive(false);
        SetTimeScale(1f);
        Debug.Log("[UI] Continue pressed, game resumed");
    }

    private void ExitGame()
    {
        Debug.Log("[UI] Exit pressed. Quitting application...");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
    
    private void SetTimeScale(float value)
    {
        if(!Mathf.Approximately(Time.timeScale, value))
        {
            Time.timeScale = value;
        }

    }
}
