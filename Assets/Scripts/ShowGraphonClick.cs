using Unity.VisualScripting;
using UnityEngine;

public class ShowGraphonClick : MonoBehaviour
{
   public GameObject graphPanel;

   public void ShowGraph()
    {
        graphPanel.SetActive(true);
    }
}
