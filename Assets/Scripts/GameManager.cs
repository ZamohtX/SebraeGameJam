using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement; 

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    [SerializeField] private UnityEngine.UI.GraphicRaycaster canvasRaycaster;

    [Header("Configurações do Tabuleiro")]
    [SerializeField] private int gridWidth = 4;
    [SerializeField] private int gridHeight = 5;

    [Header("Configurações de Turno")]
    private int currentRound = 1;
    private const int MAX_ROUNDS = 5;

    [Header("Referências Técnicas")]
    [SerializeField] private SpriteManager spriteManager;
    [SerializeField] private RulesPanelUI rulesPanelUI;
    [SerializeField] private MatchSetupManager matchSetupManager; 
    [SerializeField] private GameResultUI gameResultUI;

    [Header("Configurações de Mensagens e Paradas")]
    [SerializeField] private GameObject messagePanel; 
    [SerializeField] private TextMeshProUGUI messageText;

    [Header("Pool de Nomes Alagoanos")]
    [SerializeField] private List<string> idosa = new List<string> { "Dona Guilhermina", "Dona Maria josé", " Dona Socorro", "Dona Cida", "Dona Franscisca" };
    [SerializeField] private List<string> clt = new List<string> { "Alexandre", "Ricardo", "Cristóvão", "Celso", "Wellington" };
    [SerializeField] private List<string> menina = new List<string> { "Nicole", "Laura", "Ana", "Júlia", "Lilian" };
    [SerializeField] private List<string> menino = new List<string> { "Pedro", "Davi", "Cauã", "Lucas", "Marquinhos" };

    public BusGrid BusGrid { get; private set; }
    public bool isPaused { get; private set; } = false;

    private List<BusSeatView> registeredSeats = new List<BusSeatView>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // IMPORTANTE: Em Jams, se você mudar de cena e voltar, o DontDestroy pode duplicar Managers.
            // Se o menu for uma cena separada, certifique-se de que o GameManager não quebre o fluxo.
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void RegisterSeat(BusSeatView seat)
    {
        if (!registeredSeats.Contains(seat))
        {
            registeredSeats.Add(seat);
        }
    }

    private void Start()
    {
        // Se voltarmos do menu, resetamos as variáveis
        currentRound = 1;
        isPaused = false;
        Time.timeScale = 1f;

        BusGrid = new BusGrid(gridWidth, gridHeight);
        PopulateBus();

        if (matchSetupManager == null) matchSetupManager = FindFirstObjectByType<MatchSetupManager>();

        if (matchSetupManager != null)
        {
            matchSetupManager.InitializeMatch(BusGrid.GetAllPassengers(), rulesPanelUI);
            matchSetupManager.ExecuteThiefTurn(BusGrid);
        }

        if (AudioManager.Instance != null) AudioManager.Instance.StartBus();
    }

    private void PopulateBus()
    {
        if (registeredSeats.Count == 0) return;

        int totalSeats = registeredSeats.Count;
        int thiefIndex = Random.Range(0, totalSeats);

        for (int i = 0; i < totalSeats; i++)
        {
            BusSeatView seat = registeredSeats[i];
            string uniqueId = System.Guid.NewGuid().ToString().Substring(0, 5);
            Passenger passenger = new Passenger(uniqueId, seat.GridX, seat.GridY, spriteManager.GetRandomSpriteId());

            if (i == thiefIndex)
            {
                passenger.Status = PassengerStatus.Thief;
                Debug.Log($"<color=cyan>[DEV]</color> Ladrão: {uniqueId} em ({seat.GridX}, {seat.GridY})");
            }

            BusGrid.AddPassenger(passenger, seat.GridX, seat.GridY);

            if (seat.PassengerView != null)
            {
                seat.PassengerView.gameObject.SetActive(true);
                seat.PassengerView.Initialize(passenger, spriteManager);
            }
        }
    }

    public void CheckAccusation(Passenger accusedPassenger)
    {
        if (accusedPassenger.Status == PassengerStatus.Thief)
        {
            ProcessWin();
        }
        else
        {
            accusedPassenger.Status = PassengerStatus.Expelled;
            currentRound++;

            if (currentRound > MAX_ROUNDS)
            {
                UpdateExpelledPassengersVisual();
                ProcessLoss(true);  
            }
            else
            {
                StartCoroutine(TransitionToNextRound(accusedPassenger));
            }
        }
    }

    private void UpdateExpelledPassengersVisual()
    {
        foreach (var seat in registeredSeats)
        {
            if (seat.PassengerView != null && seat.PassengerView.Passenger != null)
            {
                if (seat.PassengerView.Passenger.Status == PassengerStatus.Expelled)
                {
                    seat.PassengerView.gameObject.SetActive(false);
                }
            }
        }
    }

    private System.Collections.IEnumerator TransitionToNextRound(Passenger expelled)
    {
        if (canvasRaycaster != null) canvasRaycaster.enabled = false;

        if (AudioManager.Instance != null) AudioManager.Instance.StopBus();
        if (FadeManager.Instance != null) yield return StartCoroutine(FadeManager.Instance.FadeIn(0.8f));

        UpdateExpelledPassengersVisual();
        if (RouteManager.Instance != null) RouteManager.Instance.ShowCurrentStation();

        displayMessage(ObterNomePorSpriteId(expelled.SpriteId));
        if (FadeManager.Instance != null) yield return StartCoroutine(FadeManager.Instance.FadeOut(0.5f));

        yield return new WaitForSeconds(3.5f);

        if (AudioManager.Instance != null) AudioManager.Instance.PlayTheft();
        displayMessage("<color=red><b>ATENÇÃO MOTORISTA!</b></color>\n\nEnquanto o ônibus parou, houve outro roubo!");

        if (matchSetupManager != null) matchSetupManager.ExecuteThiefTurn(BusGrid);
        yield return new WaitForSeconds(3.0f);

        if (FadeManager.Instance != null) yield return StartCoroutine(FadeManager.Instance.FadeIn(0.8f));

        if (RouteManager.Instance != null)
        {
            RouteManager.Instance.HideStation();
            RouteManager.Instance.AdvanceToNextStation();
        }

        if (messagePanel != null) messagePanel.SetActive(false);
        if (AudioManager.Instance != null) AudioManager.Instance.StartBus();
        if (FadeManager.Instance != null) yield return StartCoroutine(FadeManager.Instance.FadeOut(0.6f));

        if (canvasRaycaster != null) canvasRaycaster.enabled = true;
    }

    private void ProcessWin()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlayFanfare();

        // Lógica de HighScore: Menos rodadas é melhor
        int savedHighScore = SaveManager.LoadHighScore();

        // Salva se for o primeiro jogo (0) ou se a pontuação atual for menor (melhor) que a salva
        if (savedHighScore == 0 || currentRound < savedHighScore)
        {
            SaveManager.SaveGameProgress(1, currentRound);
            Debug.Log($"[HIGHSCORE] Novo recorde: {currentRound} rodadas!");
        }

        StartCoroutine(VictoryTransitionCoroutine());
    }

    private System.Collections.IEnumerator VictoryTransitionCoroutine()
    {
        if (canvasRaycaster != null) canvasRaycaster.enabled = false;
        if (AudioManager.Instance != null) AudioManager.Instance.StopBus();
        
        if (FadeManager.Instance != null) yield return StartCoroutine(FadeManager.Instance.FadeIn(0.8f));

        if (RouteManager.Instance != null) RouteManager.Instance.ShowCurrentStation();

        int bestScore = SaveManager.LoadHighScore();
        displayMessage($"<color=green><b>LADRÃO CAPTURADO!</b></color>\n\nVocê descobriu o meliante em <b>{currentRound} rodadas</b>!\n\n🏆 Melhor Recorde: {bestScore} rodadas.");

        if (FadeManager.Instance != null) yield return StartCoroutine(FadeManager.Instance.FadeOut(0.5f));

        yield return new WaitForSeconds(4.5f);

        // Abre o painel de resultado final que tem o botão de "Jogar Novamente" ou "Menu"
        if (gameResultUI != null)
        {
            gameResultUI.ShowFinalResult(true, currentRound);
        }
        else
        {
            // Fallback caso o painel não exista: volta pro menu direto
            ReturnToMenu();
        }
        
        if (canvasRaycaster != null) canvasRaycaster.enabled = true;
    }

    private void ProcessLoss(bool ranOutRounds)
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlayFrustration();

        Passenger trueThief = BusGrid.GetAllPassengers().Find(p => p.Status == PassengerStatus.Thief);
        string thiefId = trueThief != null ? trueThief.Id : "Desconhecido";

        if (gameResultUI != null)
        {
            gameResultUI.ShowFinalResult(false, currentRound, thiefId);
        }
    }
    
    private string ObterNomePorSpriteId(int spriteId)
    {
        string nome = "";
        switch (spriteId)
        {
            case 0:
                nome = idosa[Random.Range(0, idosa.Count)];
                return $"<b>{nome}</b> diz:\n<i>\"Tá repreendido em nome do Senhor!\"</i>";
            case 1:
                nome = clt[Random.Range(0, clt.Count)];
                return $"<b>{nome}</b> diz:\n<i>\"Não acredito nisso, vou perder a hora!\"</i>";
            case 2:
                nome  = menina[Random.Range(0, menina.Count)];
                return $"<b>{nome}</b> diz:\n<i>\"Eita bixiga!!\"</i>";
            case 3:
                nome = menino[Random.Range(0, menino.Count)];
                return $"<b>{nome}</b> diz:\n<i>\"Vou contar tudo pra minha mãee!\"</i>";
            default:
                return "Um passageiro inocente foi expulso.";
        }
    }

    private void displayMessage(string message)
    {
        if (messagePanel == null || messageText == null) return;
        messageText.text = message;
        messagePanel.SetActive(true);
    }

    public void RestartScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // MÉTODO NOVO: Para voltar ao menu principal
    public void ReturnToMenu()
    {
        Time.timeScale = 1f;
        // Certifique-se de que o nome da cena do menu seja exatamente "Menu"
        SceneManager.LoadScene("Menu"); 
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
    }
}