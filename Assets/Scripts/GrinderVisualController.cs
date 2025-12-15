using UnityEngine;

public class GrinderVisualController : MonoBehaviour
{
    [SerializeField] private GameObject emptyGrinder;
    [SerializeField] private GameObject fullGrinder;

    private void Start()
    {
        ShowEmpty();
    }

    public void ShowEmpty()
    {
        if (emptyGrinder != null) emptyGrinder.SetActive(true);
        if (fullGrinder != null) fullGrinder.SetActive(false);
    }

    public void ShowFull()
    {
        if (emptyGrinder != null) emptyGrinder.SetActive(false);
        if (fullGrinder != null) fullGrinder.SetActive(true);
    }
}

