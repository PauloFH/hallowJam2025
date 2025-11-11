using UnityEngine;
using UnityEngine.InputSystem;

public class HoverManager : MonoBehaviour {
    public Camera _cam;
    private InputAction _pointAction;
    private InputAction _clickAction;

    private InteractableObject _hovered;

    private void Awake() {
        _cam = Camera.main;
        _pointAction = InputSystem.actions.FindAction("Point");
        _clickAction = InputSystem.actions.FindAction("Click");

        _pointAction?.Enable();
        _clickAction?.Enable();
    }

    private void OnDisable() {
        _pointAction?.Disable();
        _clickAction?.Disable();
    }

    private void Update() {
        if (PlayerController.IsInputBlocked)
            return;
        var mousePos = _pointAction.ReadValue<Vector2>();
        Vector2 worldPos = _cam.ScreenToWorldPoint(mousePos);

        var hit = Physics2D.Raycast(worldPos, Vector2.zero);

        if (hit.collider && hit.collider.TryGetComponent(out InteractableObject obj)) {
            if (_hovered != obj) {
                _hovered?.OnHoverExit();
                _hovered = obj;
                _hovered.OnHoverEnter();
            }
        }
        else {
            _hovered?.OnHoverExit();
            _hovered = null;
        }

        if (_clickAction.WasPressedThisFrame() && _hovered) {
            _hovered.OnClick();
        }
    }
}