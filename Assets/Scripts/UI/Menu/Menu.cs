using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        AudioManager.Instance.PlayClick();
        SceneManager.LoadScene("Main");
    }

    public void ExitGame()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlayClick();

        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #else
                Application.Quit();
        #endif
    }
}