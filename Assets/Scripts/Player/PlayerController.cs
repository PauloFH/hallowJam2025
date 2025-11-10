using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour {
    public float moveSpeed = 3f;

    private Rigidbody2D _rb;
    private SpriteRenderer sr;
    private Vector2 _targetPosition;
    private bool _isMoving;
    private Camera _cam;
    private Animator animator;

    private InputAction _clickAction;
    private InputAction _pointAction;

    [Header("Boundaries")]
    public float leftXBoundary = 0f;
    public float rightXBoundary = 0f;

    public static bool IsInputBlocked = false;

    public void SetInputBlocked(bool blocked) {
        IsInputBlocked = blocked;
    }

    private void Awake() {
        _rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        _cam = Camera.main;
        _clickAction = InputSystem.actions.FindAction("Click");
        _pointAction = InputSystem.actions.FindAction("Point");
        animator = GetComponent<Animator>();
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

    private void Update()
    {
        if (IsInputBlocked)
            return;

        if (transform.position.x < leftXBoundary)
        {
            transform.position = new Vector2(leftXBoundary, transform.position.y);
            return;
        }
        if (transform.position.x > rightXBoundary)
        {
            transform.position = new Vector2(rightXBoundary, transform.position.y);
            return;
        }
            

        // MouseMovement();
        KeyboardMovement();
    }

    private void MouseMovement()
    {
        if (_clickAction != null && _clickAction.WasPressedThisFrame())
        {
            var mouseScreenPos = _pointAction.ReadValue<Vector2>();
            Vector2 mouseWorldPos = _cam.ScreenToWorldPoint(mouseScreenPos);

            var hit = Physics2D.Raycast(mouseWorldPos, Vector2.zero);

            if (hit.collider)
            {
                var interactable = hit.collider.GetComponent<InteractableObject>();
                if (interactable)
                {
                    interactable.OnClick();
                    return;
                }
            }

            _targetPosition = new Vector2(mouseWorldPos.x, transform.position.y);
            _isMoving = true;
            animator.SetBool("isMoving", true);
        }

        if (!_isMoving) return;
        var step = moveSpeed * Time.deltaTime;
        transform.position = Vector2.MoveTowards(transform.position, _targetPosition, step);

        if (Vector2.Distance(transform.position, _targetPosition) < 0.05f)
            _isMoving = false;
            animator.SetBool("isMoving", false);
    }
    
    private void KeyboardMovement()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        // no jump
        // float moveY = Input.GetAxisRaw("Vertical");
        float moveY = 0f;

        if (moveX > 0f)
        {
            animator.SetBool("isMoving", true);
            sr.flipX = false;
        }
        else if (moveX < 0f)
        {
            animator.SetBool("isMoving", true);
            sr.flipX = true;
        }
        else {
            animator.SetBool("isMoving", false);
        }

        Vector2 movement = new Vector2(moveX, moveY).normalized;
        _rb.linearVelocity = movement * moveSpeed;
    }
}