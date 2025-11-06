using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class DialoguePanel : MonoBehaviour
{
    public static DialoguePanel Instance;

    [Header("UI Elements")]
    public GameObject panel;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descriptionText;
    public float fadeDuration = 0.5f;

    private bool _isVisible;
    private bool _isFading;
    private InputAction _clickAction;
    private CanvasGroup _canvasGroup;

    private void Awake()
    {
        Instance = this;
        _clickAction = InputSystem.actions.FindAction("Click");
        _clickAction?.Enable();

        _canvasGroup = panel.GetComponent<CanvasGroup>();
        panel.SetActive(false);
        _canvasGroup.alpha = 0f;
    }

    private void Update()
    {
        if (_isVisible && !_isFading && _clickAction.WasPressedThisFrame())
        {
            StartCoroutine(FadeOut());
        }
    }

    public void Show(string title, string description)
    {
        if (_isVisible || _isFading) return;

        titleText.text = title;
        descriptionText.text = description;

        panel.SetActive(true);
        StartCoroutine(FadeIn());

        PlayerController.IsInputBlocked = true;
    }

    private IEnumerator FadeIn()
    {
        _isVisible = true;
        _isFading = true;

        var elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            _canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeDuration);
            yield return null;
        }

        _canvasGroup.alpha = 1f;
        _isFading = false;
    }

    private IEnumerator FadeOut()
    {
        _isFading = true;

        var elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            _canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            yield return null;
        }

        _canvasGroup.alpha = 0f;
        _isFading = false;
        _isVisible = false;

        panel.SetActive(false);
        PlayerController.IsInputBlocked = false;
    }

    public bool IsVisible => _isVisible;
}
