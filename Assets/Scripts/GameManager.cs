using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game States")]
    public bool isPaused = false;

    [Header("UI References")]
    [SerializeField] private GameObject pauseMenuUI;

    private void Awake()
    {
        // Padrão Singleton para garantir que só exista um GameManager
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Opcional: mantém entre cenas se necessário
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        // Monitora a tecla ESC ou P para pausar
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        if (pauseMenuUI != null) pauseMenuUI.SetActive(true);
        
        // Congela o tempo do jogo e a física
        Time.timeScale = 0f; 
    }

    public void ResumeGame()
    {
        isPaused = false;
        if (pauseMenuUI != null) pauseMenuUI.SetActive(false);
        
        // Descongela o tempo
        Time.timeScale = 1f; 
    }

    public void RestartScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitToMainMenu()
    {
        Time.timeScale = 1f;
        // Substitua pelo index ou nome da sua cena de Menu Principal
        SceneManager.LoadScene("MainMenu"); 
    }
}