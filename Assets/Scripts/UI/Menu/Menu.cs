using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("Main");
    }

    public void ExitGame()
    {
        //Application.Quit();

        // Apenas para testes no Editor
        Debug.Log("Fechar jogo");
    }
}