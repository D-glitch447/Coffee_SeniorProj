using UnityEngine;
using System.Collections;

public class RecipeSceneStartFade : MonoBehaviour
{
    public FadeController fade;

    private void Start()
    {
        //fade from black -> show the recipe scene
        StartCoroutine(fade.FadeOut());
    }
}
