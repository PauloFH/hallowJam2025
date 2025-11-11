using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(Collider2D))]
public class InteractableObject : MonoBehaviour
{
    private static readonly int Interact = Animator.StringToHash("Interact");

    [Header("Interaction Info")]
    public string title = "";
    [TextArea(2, 5)] public string description = "";
    public float instabilityIncrease = 10f;

    private static int instanceCounter = 0;
    private bool hasInstantiatedEffect = false;
    private DreamStabilityManager dreamStabilityManager;
    
    [Header("Conditional Interaction")]
    public bool isCollectible;
    public string itemId;
    public string requiredItemId;
    [TextArea(2, 5)] public string firstInteractionMessage = "";
    [TextArea(2, 5)] public string failMessage = "";
    [TextArea(2, 5)] public string successMessage = "";
    public string rewardItemId;
    public string nextSceneName;

    private SpriteRenderer _sr;
    private AudioSource _audioSource;
    private Animator _animator;
    private Color _originalColor;

    private bool _hasInteracted;
    private bool _isOpened;

    private void Start()
    {
        _sr = GetComponent<SpriteRenderer>();
        _audioSource = GetComponent<AudioSource>();
        _animator = GetComponent<Animator>();
        _originalColor = _sr.color;
        dreamStabilityManager = DreamStabilityManager.Instance;

        if (PlayerPrefs.GetInt(GetPrefsKey(), 0) != 1) return;
        _isOpened = true;
        if (isCollectible)
            gameObject.SetActive(false);
    }

    void Update()
    {
        if (dreamStabilityManager.instability >= 80f && instanceCounter < 100 && !hasInstantiatedEffect)
        {
            hasInstantiatedEffect = true;
            instanceCounter++;
            Instantiate(gameObject, new Vector3(transform.position.x + 0.6f, transform.position.y, transform.position.z), Quaternion.identity);
        }
    }

    private string GetPrefsKey() => $"Interactable_{itemId}_{gameObject.scene.name}";

    public void OnHoverEnter()
    {
        if (!_isOpened)
            _sr.color = Color.yellow;
    }

    public void OnHoverExit()
    {
        if (!_isOpened)
            _sr.color = _originalColor;
    }

    public void OnClick()
    {
        _sr.color = _originalColor;
     
        
        if (isCollectible)
        {
            HandleCollectible();
            return;
        }
        
        if (!string.IsNullOrEmpty(requiredItemId) || !string.IsNullOrEmpty(nextSceneName))
        {
            HandleConditionalInteraction();
            return;
        }
        DreamStabilityManager.Instance.AddInstability(instabilityIncrease);
        DialoguePanel.Instance.Show(title, description);
        PlayAudioAnim();
    }


    private void HandleCollectible()
    {
        if (_isOpened) return;

        InventoryManager.Instance.AddItem(itemId);
        DialoguePanel.Instance.Show(title, $"Você coletou {itemId}.");

        _isOpened = true;
        PlayerPrefs.SetInt(GetPrefsKey(), 1);
        PlayerPrefs.Save();

        PlayAudioAnim();
        Destroy(gameObject);
    }

    private void HandleConditionalInteraction()
    {
        if (_isOpened) return;

        if (!_hasInteracted)
        {
            DialoguePanel.Instance.Show(title, firstInteractionMessage);
            _hasInteracted = true;
            PlayAudioAnim();
            return;
        }

        if (!string.IsNullOrEmpty(requiredItemId))
        {
            if (InventoryManager.Instance.HasItem(requiredItemId))
            {
                OnInteractionSuccess();
            }
            else
            {
                OnInteractionFail();
            }
        }
        else
        {
            OnInteractionSuccess();
        }
    }

    private void OnInteractionSuccess()
    {
        _isOpened = true;
        PlayerPrefs.SetInt(GetPrefsKey(), 1);
        PlayerPrefs.Save();

        DialoguePanel.Instance.Show(title, successMessage);
        PlayAudioAnim();

        if (!string.IsNullOrEmpty(rewardItemId))
        {
            InventoryManager.Instance.AddItem(rewardItemId);
        }

        if (string.IsNullOrEmpty(nextSceneName)) return;
        PlayerController.IsInputBlocked = true;
        StartCoroutine(SceneTransitionManager.Instance.TransitionToNextSceneByName(nextSceneName));
    }

    private void OnInteractionFail()
    {
        DialoguePanel.Instance.Show(title, failMessage);
        PlayAudioAnim();
    }

    private void PlayAudioAnim()
    {
        if (_audioSource) _audioSource.Play();
        if (_animator) _animator.SetTrigger(Interact);
    }
}
