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

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        _originalCamPos = mainCamera.transform.localPosition;
        backgroundAudio = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (instability > 0)
        {
            ShakeCamera();

            DistortAudio();
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

    private void ShakeCamera ()
    {
        var shakeAmount = instability / maxInstability * 0.05f;
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
}