using TMPro;
using UnityEngine;

public class Password : MonoBehaviour
{
    private TMP_InputField inputField;
    private Computer computer;
    private AudioSource audioSource;
    public GameObject hint;

    public AudioClip correctSound;
    public AudioClip incorrectSound;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        inputField = GetComponent<TMP_InputField>();
        computer = GameObject.Find("Computer").GetComponent<Computer>();
        audioSource = computer.gameObject.GetComponent<AudioSource>();
    }

    public void ValidatePassword()
    {
        if (inputField.text.Length < 4)
            return;

        string correctPassword = "1234";
        if (inputField.text.ToLower() == correctPassword)
        {
            Debug.Log("Password correct!");
            audioSource.PlayOneShot(correctSound);
            InventoryManager.Instance.AddItem("InfoChave");

            LogIn();
        }
        else
        {
            Debug.Log("Incorrect password. Try again.");
            inputField.text = "";
            audioSource.PlayOneShot(incorrectSound);
            gameObject.SetActive(false);
        }
    }
    
    public void LogIn()
    {
        Debug.Log("User logged in to the computer.");
        computer.hasLoggedIn = true;
        hint.SetActive(true);
        Invoke(nameof(HideHint), 5f);
    }
    
    private void HideHint()
    {
        hint.SetActive(false);
        gameObject.SetActive(false);
    }
}
