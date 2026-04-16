using UnityEngine;
using UnityEngine.SceneManagement;

public class StartScreenManager : MonoBehaviour
{
    public void OnPlayClicked()
    {
        SceneManager.LoadScene(1); // LevelSelect
    }

    public void OnQuitClicked()
    {
        Application.Quit();
    }
}