using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class DreamStabilityManager : MonoBehaviour
{
    public static DreamStabilityManager Instance;
    public float instability;
    public float maxInstability = 100f;
    public Camera mainCamera;
    private AudioSource backgroundAudio;

    private Vector3 _originalCamPos;
    private GameObject[] windows;

    public AudioClip heartbeat;

    private bool isHeartbeatPlaying = false;

    private bool hasBrokenWindows = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else Destroy(gameObject);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        windows = GameObject.FindGameObjectsWithTag("Window");
        hasBrokenWindows = false;

        mainCamera = Camera.main;
        _originalCamPos = mainCamera.transform.localPosition;

        var dialoguePanel = GetComponent<DialoguePanel>();
        DialoguePanel.Instance = dialoguePanel;
        dialoguePanel.panel = GameObject.Find("DialoguePanel");
        dialoguePanel._canvasGroup = dialoguePanel.panel.GetComponent<CanvasGroup>();
        dialoguePanel.titleText = GameObject.Find("TitleText").GetComponent<TextMeshProUGUI>();
        dialoguePanel.descriptionText = GameObject.Find("DescriptionText").GetComponent<TextMeshProUGUI>();
        GetComponent<HoverManager>()._cam = mainCamera;
        print("DreamStabilityManager: Scene loaded and references updated.");
    }

    private void Start()
    {
        _originalCamPos = mainCamera.transform.localPosition;
        backgroundAudio = GetComponent<AudioSource>();
        windows = GameObject.FindGameObjectsWithTag("Window");
    }

    private void Update()
    {
        if (instability > 0)
        {
            ShakeCamera();

            DistortAudio();
        }

        if (instability >= 20f)
        {
            if (!isHeartbeatPlaying)
            {
                PlayHeartbeatSound();
                isHeartbeatPlaying = true;
            }
        }

        if (instability >= 40f)
        {
            if (!hasBrokenWindows)
            {
                BreakWindows();
                hasBrokenWindows = true;
            }
        }

        if (instability >= maxInstability)
        {
            SceneManager.LoadScene("DreamCollapse");
        }
    }

    public void AddInstability(float value)
    {
        instability = Mathf.Min(instability + value, maxInstability);
    }

    private void ShakeCamera()
    {
        float instabilityPercentage = instability / maxInstability;
        float k = 1f;
        var shakeAmount = Mathf.Log(1 + k * instabilityPercentage) / Mathf.Log(1 + k) * 0.05f;
        var shakePos = Random.insideUnitSphere * shakeAmount;
        shakePos.z = -10f;
        mainCamera.transform.localPosition = _originalCamPos + shakePos;
    }

    private void DistortAudio()
    {
        if (backgroundAudio != null)
        {
            backgroundAudio.pitch = 1f + (instability / maxInstability);
        }
    }

    private void BreakWindows()
    {
        foreach (var window in windows)
        {
            Animator windowAnimator = window.GetComponent<Animator>();
            if (windowAnimator != null)
            {
                windowAnimator.SetBool("isBroken", true);
                AudioSource audioSource = window.GetComponent<AudioSource>();
                if (audioSource != null)
                {
                    audioSource.Play();
                }
            }
        }
    }

    private void PlayHeartbeatSound()
    {
        var audioSource = GetComponent<AudioSource>();
        if (audioSource != null && heartbeat != null)
        {
            audioSource.PlayOneShot(heartbeat);

            var x = instability / maxInstability;
            var delay = Mathf.Lerp(2f, 0.5f, (x - 0.1f) / (1f - 0.1f));
            Invoke(nameof(PlayHeartbeatSound), delay);
        }
    }
}