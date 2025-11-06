using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour {
    public float moveSpeed = 3f;

    private Rigidbody2D _rb;
    private Vector2 _targetPosition;
    private bool _isMoving;
    private Camera _cam;

    private InputAction _clickAction;
    private InputAction _pointAction;

    public static bool IsInputBlocked = false;

    private void Awake() {
        _rb = GetComponent<Rigidbody2D>();
        _cam = Camera.main;
        _clickAction = InputSystem.actions.FindAction("Click");
        _pointAction = InputSystem.actions.FindAction("Point");
    }

    private void OnEnable() {
        _clickAction?.Enable();
        _pointAction?.Enable();
    }

    private void OnDisable() {
        _clickAction?.Disable();
        _pointAction?.Disable();
    }

    private void Start() {
        _targetPosition = transform.position;
    }

    private void Update() {
        if (IsInputBlocked)
            return;

        if (_clickAction != null && _clickAction.WasPressedThisFrame()) {
            var mouseScreenPos = _pointAction.ReadValue<Vector2>();
            Vector2 mouseWorldPos = _cam.ScreenToWorldPoint(mouseScreenPos);

            var hit = Physics2D.Raycast(mouseWorldPos, Vector2.zero);

            if (hit.collider) {
                var interactable = hit.collider.GetComponent<InteractableObject>();
                if (interactable) {
                    interactable.OnClick();
                    return;
                }
            }

            _targetPosition = new Vector2(mouseWorldPos.x, transform.position.y);
            _isMoving = true;
        }

        if (!_isMoving) return;
        var step = moveSpeed * Time.deltaTime;
        transform.position = Vector2.MoveTowards(transform.position, _targetPosition, step);

        if (Vector2.Distance(transform.position, _targetPosition) < 0.05f)
            _isMoving = false;
    }
}