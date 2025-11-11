using UnityEngine;


[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour {
    public float moveSpeed = 3f;

    private Rigidbody2D _rb;
    private SpriteRenderer _sr;
    private AudioSource _audioSource;
    private Animator _animator;

    public AudioClip[] footstepSounds;

    [Header("Boundaries")]
    public float leftXBoundary;
    public float rightXBoundary;

    public static bool IsInputBlocked = false;
    private static readonly int IsMoving = Animator.StringToHash("isMoving");
    public NextRoomUI nextRoomUI;
    
    private bool _atRightBoundary;
    private bool _atLeftBoundary;
    private bool _holdingTransition;
    private float _holdTime;
    public float requiredHoldTime = 1.5f;
    public float boundaryTolerance = 0.05f; 
    private void Awake() {
        _rb = GetComponent<Rigidbody2D>();
        _sr = GetComponent<SpriteRenderer>();
        _audioSource = GetComponent<AudioSource>();
        _animator = GetComponent<Animator>();
    }

    private void Update() {
        if (IsInputBlocked)
            return;

        var moveX = Input.GetAxisRaw("Horizontal");
        CheckBoundaries(moveX);
        
        if (_atRightBoundary)
        {
            nextRoomUI.ShowNextRoomHint(Vector2.right);
        }
        else if (_atLeftBoundary)
        {
            nextRoomUI.ShowNextRoomHint(Vector2.left);
        }
        else
        {
            nextRoomUI.Hide();
            _holdTime = 0f;
        }

        if (!_atRightBoundary && !_atLeftBoundary)
        {
            KeyboardMovement(moveX);
        }
        else
        {
            _rb.linearVelocity = Vector2.zero;
            _animator.SetBool(IsMoving, false);
        }
        
        if ((_atRightBoundary && moveX > 0) || (_atLeftBoundary && moveX < 0))
        {
            _holdingTransition = true;
            _holdTime += Time.deltaTime;
            nextRoomUI.UpdateProgress(_holdTime / requiredHoldTime);

            if (!(_holdTime >= requiredHoldTime)) return;
            _holdTime = 0f;
            _holdingTransition = false;
            IsInputBlocked = true;

            var direction = _atRightBoundary ? 1 : -1;
            StartCoroutine(SceneTransitionManager.Instance.TransitionToNextScene(direction));
        }
        else
        {
            if (_holdingTransition)
            {
                _holdingTransition = false;
                StartCoroutine(nextRoomUI.AnimateCancel());
            }
            _holdTime = 0f;
        }
    }


    private void KeyboardMovement(float moveX) {
        const float moveY = 0f;

        switch (moveX) {
            case > 0f:
                _animator.SetBool(IsMoving, true);
                _sr.flipX = false;
                break;
            case < 0f:
                _animator.SetBool(IsMoving, true);
                _sr.flipX = true;
                break;
            default:
                _animator.SetBool(IsMoving, false);
                break;
        }

        var movement = new Vector2(moveX, moveY).normalized;
        _rb.linearVelocity = movement * moveSpeed;
    }

    public void PlayFootstepSound() {
        if (footstepSounds.Length == 0) return;
        var random = Random.Range(0, footstepSounds.Length);
        _audioSource.PlayOneShot(footstepSounds[random]);
    }
    
    public void ResetBoundaryState()
    {
        _atLeftBoundary = false;
        _atRightBoundary = false;
        _holdingTransition = false;
        _holdTime = 0f;
        _rb.linearVelocity = Vector2.zero;
        nextRoomUI.Hide();
    }
    private void CheckBoundaries(float moveX)
    {
        if (transform.position.x <= leftXBoundary + boundaryTolerance)
        {
            if (moveX < 0f)
            {
                transform.position = new Vector2(leftXBoundary, transform.position.y);
                _rb.linearVelocity = Vector2.zero;
                _atLeftBoundary = true;
            }
            else if (moveX > 0f)
            {
                _atLeftBoundary = false;
            }
        }
        else
        {
            _atLeftBoundary = false;
        }
        
        if (transform.position.x >= rightXBoundary - boundaryTolerance) {
            switch (moveX) {
                case > 0f:
                    transform.position = new Vector2(rightXBoundary, transform.position.y);
                    _rb.linearVelocity = Vector2.zero;
                    _atRightBoundary = true;
                    break;
                case < 0f:
                    _atRightBoundary = false;
                    break;
            }
        }
        else
        {
            _atRightBoundary = false;
        }
    }

}
