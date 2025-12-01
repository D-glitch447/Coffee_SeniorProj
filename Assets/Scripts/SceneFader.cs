using UnityEngine;

public class SceneFader : MonoBehaviour
{
    public FadeController fade;

    private void Start()
    {
        StartCoroutine(fade.FadeOut()); //fade from black -> visible 
    }
}
