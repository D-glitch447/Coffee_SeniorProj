using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class ScoreButtonHandler : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Button scoreButton;

    [Header("Scene Config")]
    [SerializeField] private string scoringSceneName = "ScoringScene";

    [Header("Optional")]
    [SerializeField] private FadeController fade;

    private void Start()
    {
        // 🔒 Lock button unless brewing is complete
        if (scoreButton != null)
        {
            scoreButton.interactable =
                CoffeeRuntime.Instance != null &&
                CoffeeRuntime.Instance.hasCompletedBrewing;
        }
    }

    public void OnScoreClicked()
    {
        // Safety check (prevents edge cases)
        if (CoffeeRuntime.Instance == null ||
            !CoffeeRuntime.Instance.hasCompletedBrewing)
            return;

        StartCoroutine(GoToScoringScene());
    }

    private IEnumerator GoToScoringScene()
    {
        if (scoreButton != null)
            scoreButton.interactable = false;

        if (fade != null)
            yield return fade.FadeIn();

        SceneManager.LoadScene(scoringSceneName);
    }
}
