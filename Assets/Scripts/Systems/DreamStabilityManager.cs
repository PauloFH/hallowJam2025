using UnityEngine;
using UnityEngine.SceneManagement;

public class DreamStabilityManager : MonoBehaviour
{
    public static DreamStabilityManager Instance;
    public float instability;
    public float maxInstability = 100f;
    public Camera mainCamera;

    private Vector3 _originalCamPos;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        _originalCamPos = mainCamera.transform.localPosition;
    }

    private void Update()
    {
        if (instability > 0)
        {
            var shakeAmount = Mathf.Clamp(instability / maxInstability, 0f, 0.05f);
            var shakePos = Random.insideUnitSphere * shakeAmount;
            shakePos.z = -10f;
            mainCamera.transform.localPosition = _originalCamPos + shakePos;
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
}