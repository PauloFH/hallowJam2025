using UnityEngine;

public class GameDataReset : MonoBehaviour
{
    public void NewGame()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }
}