using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelComplete : MonoBehaviour
{
    public int levelNumber; // set this in the Inspector — 1, 2, 3, or 4

    public void CompleteLevel()
    {
        int current = PlayerPrefs.GetInt("LevelsUnlocked", 1);
        if (levelNumber >= current)
            PlayerPrefs.SetInt("LevelsUnlocked", levelNumber + 1);

        if (levelNumber == 4)
            SceneManager.LoadScene(6); // EndScreen
        else
            SceneManager.LoadScene(1); // LevelSelect
    }
}
