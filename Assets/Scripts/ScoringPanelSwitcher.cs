using UnityEngine;

public class ScoringPanelSwitcher : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject graphPanel;
    [SerializeField] private GameObject scorePanel;

    public void ShowScorePanel()
    {
        if (graphPanel != null)
            graphPanel.SetActive(false);

        if (scorePanel != null)
            scorePanel.SetActive(true);
    }
}
