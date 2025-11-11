using TMPro;
using UnityEngine;

public class Computer : MonoBehaviour
{
    public bool hasLoggedIn = false;

    public TMP_InputField inputField;
    public InteractableObject interactableObject;

    public void ClickCallback()
    {
        if (!interactableObject.hasInteractedOnce)
            return;
        if (!hasLoggedIn)
        {
            inputField.gameObject.SetActive(true);
            Debug.Log("Computer clicked, user not logged in.");
        }
        else
        {
            Debug.Log("Computer clicked, user is logged in.");
            inputField.gameObject.SetActive(true);
            inputField.gameObject.GetComponent<Password>().LogIn();
        }
    }
}
