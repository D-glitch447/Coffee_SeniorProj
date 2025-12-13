using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class FadeController : MonoBehaviour
{
    public static FadeController Instance;

    [Header("Fade Settings")]
    public Image fadeImage;
    public float fadeDuration = 1f;

    [Header("Startup")]
    public bool startBlack = false;

    public AudioSource transitionAudio;

    private void Awake()
    {
        // Singleton pattern (one FadeController across scenes)
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (fadeImage == null)
            fadeImage = GetComponent<Image>();

        // Set initial alpha
        SetAlpha(startBlack ? 1f : 0f);
    }

    public IEnumerator FadeIn()
    {
        yield return Fade(0f, 1f);
    }

    public IEnumerator FadeOut()
    {
        yield return Fade(1f, 0f);
    }

    public IEnumerator FadeIn(float duration)
    {
        yield return Fade(0f, 1f, duration);
    }

    public IEnumerator FadeOut(float duration)
    {
        yield return Fade(1f, 0f, duration);
    }

    // Backward-compatible version (uses default fadeDuration)
    private IEnumerator Fade(float startAlpha, float endAlpha)
    {
        yield return Fade(startAlpha, endAlpha, fadeDuration);
    }

    // Core fade logic with explicit duration
    private IEnumerator Fade(float startAlpha, float endAlpha, float duration)
    {
        float elapsed = 0f;
        Color color = fadeImage.color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
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
    public IEnumerator FadeOutWithCallback(System.Action onComplete)
    {
        yield return Fade(1f, 0f);
        onComplete?.Invoke();
    }
    public IEnumerator FadeOutWithCallback(float duration, System.Action onComplete)
    {
        yield return Fade(1f, 0f, duration);
        onComplete?.Invoke();
    }
 
    // Fade → load scene → fade back
    public void FadeToScene(string sceneName)
    {
        StartCoroutine(FadeToSceneRoutine(sceneName));
    }

    private IEnumerator FadeToSceneRoutine(string sceneName)
    {
        // Fade to black
        yield return FadeIn();

        // Load scene
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        while (!op.isDone)
        {
            yield return null;
        }

        // Fade back to clear
        yield return FadeOut();
    }
    public IEnumerator FadeToSceneAfterAudio(string sceneName,AudioClip clip, float audioVolume = 1f)
    {
        // Play transition audio FIRST
        if (transitionAudio != null && clip != null)
        {
            transitionAudio.volume = audioVolume;
            transitionAudio.PlayOneShot(clip);
        }

        // Fade to black while audio is playing
        yield return FadeIn();

        //WAIT until the audio finishes
        if (clip != null)
            yield return new WaitForSeconds(clip.length);

        //Load the new scene
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        while (!op.isDone)
            yield return null;

        // Fade back in
        yield return FadeOut();
    }


}
