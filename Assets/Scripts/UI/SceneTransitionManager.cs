using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager Instance;
    public Image fadeImage;
    public float fadeDuration = 1f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        StartCoroutine(FadeIn());
    }

    private void Nullify()
    {
        DialoguePanel.Instance = null;
    }

    public IEnumerator TransitionToNextScene(int direction)
    {
        yield return StartCoroutine(FadeOut());

        var currentIndex = SceneManager.GetActiveScene().buildIndex;
        var nextIndex = currentIndex + direction;

        if (nextIndex < 1)
        {
            yield return StartCoroutine(FadeIn());
            var player = FindAnyObjectByType<PlayerController>();
            if (player)
                player.ResetBoundaryState();
            PlayerController.IsInputBlocked = false;
            yield break;
        }

        if (nextIndex >= SceneManager.sceneCountInBuildSettings)
            nextIndex = 0;

        Nullify();
        SceneManager.LoadScene(nextIndex);

        yield return new WaitForSeconds(0.1f);
        yield return StartCoroutine(FadeIn());
        PlayerController.IsInputBlocked = false;
    }


    private IEnumerator FadeOut()
    {
        var elapsed = 0f;
        var color = fadeImage.color;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            color.a = Mathf.Clamp01(elapsed / fadeDuration);
            fadeImage.color = color;
            yield return null;
        }
    }

    private IEnumerator FadeIn()
    {
        var elapsed = 0f;
        var color = fadeImage.color;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            color.a = 1f - Mathf.Clamp01(elapsed / fadeDuration);
            fadeImage.color = color;
            yield return null;
        }

        PlayerController.IsInputBlocked = false;
    }
    public IEnumerator TransitionToNextSceneByName(string sceneName)
    {
        yield return StartCoroutine(FadeOut());
        Nullify();
        SceneManager.LoadScene(sceneName);
        yield return new WaitForSeconds(0.1f);
        yield return StartCoroutine(FadeIn());
        PlayerController.IsInputBlocked = false;
    }


}