using UnityEngine;
using UnityEngine.SceneManagement;

public class FinalDialogue : MonoBehaviour
{
    private static int dialogueIndex = 1;

    public void GoToFinalScene()
    {
        SceneManager.LoadScene("endScene");
    }
    
    public void AddFinalInstability()
    {
        var currentInstability = DreamStabilityManager.Instance.instability;

        if (dialogueIndex == 3)
        {
            DreamStabilityManager.Instance.instability = 99f;
            return;
        }

        DreamStabilityManager.Instance.AddInstability((100 - currentInstability) / 3f);

        dialogueIndex++;
    }
}
