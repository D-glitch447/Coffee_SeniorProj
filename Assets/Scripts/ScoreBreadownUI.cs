using UnityEngine;
using TMPro;
using System.Text;

public class ScoreBreadownUI : MonoBehaviour
{
    public TextMeshProUGUI finalScoreText;
    public TextMeshProUGUI breakdownText;

    private void Start()
    {
        CoffeeRuntime rt = CoffeeRuntime.Instance;

        if (rt == null)
        {
            Debug.LogError("[ScoreBoardUI] CoffeeRuntime not found!");
            return;
        }
        
        finalScoreText.text =
        $@"FINAL SCORE
        {Mathf.Round(rt.finalScore)}%";

        breakdownText.text =
        "Scaling:   " + Mathf.Round(rt.scoreScale) + "%\n" +
        "Grinding:  " + Mathf.Round(rt.scoreGrind) + "%\n" +
        "Heating:   " + Mathf.Round(rt.scoreHeat) + "%\n" +
        "Brewing:   " + Mathf.Round(rt.scoreBrew) + "%";

        // breakdownText.text =
        // $@"Scaling:   {Mathf.Round(rt.scoreScale)}%
        // Grinding:  {Mathf.Round(rt.scoreGrind)}%
        // Heating:   {Mathf.Round(rt.scoreHeat)}%
        // Brewing:   {Mathf.Round(rt.scoreBrew)}%";


        // // Safety: avoid null crashes if runtime isn't present
        // if (rt == null || scoreText == null)
        //     return;

        // StringBuilder sb = new StringBuilder();

        // // Left-style emphasis using size tags
        // sb.AppendLine("<size=150%><b>FINAL SCORE</b></size>");
        // sb.AppendLine($"<size=220%><b>{Mathf.Round(rt.finalScore)}%</b></size>");
        // sb.AppendLine("");
        // sb.AppendLine("──────────────");
        // sb.AppendLine($"Scaling:   {Mathf.Round(rt.scoreScale)}%");
        // sb.AppendLine($"Grinding:  {Mathf.Round(rt.scoreGrind)}%");
        // sb.AppendLine($"Heating:   {Mathf.Round(rt.scoreHeat)}%");
        // sb.AppendLine($"Brewing:   {Mathf.Round(rt.scoreBrew)}%");

        // scoreText.text = sb.ToString();
    }
}
