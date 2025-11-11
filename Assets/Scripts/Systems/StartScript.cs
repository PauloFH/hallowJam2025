using UnityEngine;
using UnityEngine.UI;

public class StartScript : MonoBehaviour {

    public Button startButton;
    public Button exitButton;
    
    private void Start() {
        startButton.onClick.AddListener(OnStartButtonClicked);
        exitButton.onClick.AddListener(OnExitButtonClicked);
    }
    
    private void OnStartButtonClicked() {
        var gameDataReset = FindAnyObjectByType<GameDataReset>();
        if (gameDataReset != null) {
            gameDataReset.NewGame();
        }
        SceneTransitionManager.Instance.StartCoroutine(
            SceneTransitionManager.Instance.TransitionToNextScene(1)
        );
    }
    
    private void OnExitButtonClicked() {
        Application.Quit();
    }
}