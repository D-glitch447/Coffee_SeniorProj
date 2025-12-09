using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class FadeController : MonoBehaviour
{
    public static FadeController Instance;

    public Image fadeImage;
    public float fadeDuration = 1f;

    public bool startBlack = false;

    private void Awake()
    {   

        // Singleton pattern so we only have ONE FadeController ever
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  // persists between scenes
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (fadeImage == null)
            fadeImage = GetComponent<Image>();

        // Start fully transparent in the very first scene
        SetAlpha(startBlack ? 1f: 0f);
    }

    // Fade from transparent -> black
    public IEnumerator FadeIn()
    {
        yield return Fade(0f, 1f);
    }

    // Fade from black -> transparent
    public IEnumerator FadeOut()
    {
        yield return Fade(1f, 0f);
    }

    private IEnumerator Fade(float startAlpha, float endAlpha)
    {
        float elapsed = 0f;
        Color color = fadeImage.color;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / fadeDuration);
            color.a = Mathf.Lerp(startAlpha, endAlpha, t);
            fadeImage.color = color;
            yield return null;
        }

        SetAlpha(endAlpha);
    }

    private void SetAlpha(float alpha)
    {
        Color color = fadeImage.color;
        color.a = alpha;
        fadeImage.color = color;
    }

    // 🚀 Call this to: fade to black -> load scene -> fade back in
    public void FadeToScene(string sceneName)
    {
        StartCoroutine(FadeToSceneRoutine(sceneName));
    }

    private IEnumerator FadeToSceneRoutine(string sceneName)
    {
        // 1) Fade to black
        yield return FadeIn();

        // 2) Load next scene
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        while (!op.isDone)
        {
            yield return null;
        }

        // 3) Fade back out to show the new scene
        yield return FadeOut();
    }
}
