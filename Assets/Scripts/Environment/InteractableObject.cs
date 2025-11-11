using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(Collider2D))]
public class InteractableObject : MonoBehaviour
{
    [Header("Interaction Info")]
    public string title = "";
    [TextArea(2, 5)] public string description = "";
    public float instabilityIncrease = 10f;

    private SpriteRenderer _sr;
    private Color _originalColor;
    private bool _hasInteracted;

    private void Start()
    {
        _sr = GetComponent<SpriteRenderer>();
        _originalColor = _sr.color;
    }

    public void OnHoverEnter()
    {
        if (!_hasInteracted)
            _sr.color = Color.yellow;
    }

    public void OnHoverExit()
    {
        if (!_hasInteracted)
            _sr.color = _originalColor;
    }

    public void OnClick()
    {
        if (_hasInteracted) return;

        _hasInteracted = true;
        _sr.color = Color.gray;

        DreamStabilityManager.Instance.AddInstability(instabilityIncrease);
        DialoguePanel.Instance.Show(title, description);
    }
}