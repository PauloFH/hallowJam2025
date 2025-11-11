using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FailScript : MonoBehaviour
{
    public Button restartButton;
    public Button exitButton;

    private void Start()
    {
        restartButton.onClick.AddListener(OnRestartButtonClicked);
        exitButton.onClick.AddListener(OnExitButtonClicked);
    }

    private void OnRestartButtonClicked()
    {
        SceneManager.LoadScene(0);
    }

    private void OnExitButtonClicked()
    {
        Application.Quit();
    }
}