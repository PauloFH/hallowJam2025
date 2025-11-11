using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class NextRoomUI : MonoBehaviour
{
    public GameObject root;
    public Image arrowIcon;
    public Image progressCircle;

    private bool _visible;

    private void Start()
    {
        Hide();
    }

    public void ShowNextRoomHint(Vector2 direction)
    {
        if (!_visible)
        {
            root.SetActive(true);
            progressCircle.fillAmount = 0f;
            _visible = true;
        }
        
        var rotationZ = (direction == Vector2.right) ? 0f : 180f;
        arrowIcon.transform.rotation = Quaternion.Euler(0, 0, rotationZ);
    }

    public void UpdateProgress(float progress)
    {
        progressCircle.fillAmount = Mathf.Clamp01(progress);
    }

    public IEnumerator AnimateCancel()
    {
        var start = progressCircle.fillAmount;
        var t = 0f;

        while (t < 0.3f)
        {
            t += Time.deltaTime;
            progressCircle.fillAmount = Mathf.Lerp(start, 0f, t / 0.3f);
            yield return null;
        }

        progressCircle.fillAmount = 0f;
    }

    public void Hide()
    {
        if (root)
            root.SetActive(false);
        _visible = false;
    }
}