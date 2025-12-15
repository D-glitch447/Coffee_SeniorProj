using UnityEngine;
using UnityEngine.SceneManagement;

public class ScorePanelButtonHandler : MonoBehaviour
{
    [Header("Scene Names")]
    [SerializeField] private string kitchenSceneName = "Kitchen";

    // 🟩 CONTINUE BUTTON
    public void OnContinueClicked()
    {
        if (CoffeeRuntime.Instance != null)
        {
            CoffeeRuntime.Instance.ResetCurrentRun();
        }

        SceneManager.LoadScene(kitchenSceneName);
    }

    // 🟥 QUIT BUTTON
    public void OnQuitClicked()
    {
        Debug.Log("Quitting game...");

        Application.Quit();

#if UNITY_EDITOR
        // So it "works" in the editor
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
