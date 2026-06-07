using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

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

    [SerializeField] private GameObject messagePanel; 
    [SerializeField] private TextMeshProUGUI messageText;

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
            DontDestroyOnLoad(gameObject);
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
        currentRound = 1;
        BusGrid = new BusGrid(gridWidth, gridHeight);

        PopulateBus();

        if (matchSetupManager == null)
        {
            matchSetupManager = FindFirstObjectByType<MatchSetupManager>();
        }

        if (matchSetupManager != null)
        {
            matchSetupManager.InitializeMatch(BusGrid.GetAllPassengers(), rulesPanelUI);
            
            // Primeiro roubo do jogo (Round 1)
            matchSetupManager.ExecuteThiefTurn(BusGrid);
        }

        if (AudioManager.Instance != null)  AudioManager.Instance.StartBus();
    }

    private void PopulateBus()
    {
        if (registeredSeats.Count == 0)
        {
            Debug.LogWarning("Nenhuma cadeira foi registrada! Verifique se as cadeiras possuem o componente BusSeatView.");
            return;
        }

        int totalSeats = registeredSeats.Count;
        int thiefIndex = Random.Range(0, totalSeats);

        for (int i = 0; i < totalSeats; i++)
        {
            BusSeatView seat = registeredSeats[i];
            string uniqueId = System.Guid.NewGuid().ToString().Substring(0, 5);
            Passenger passenger = new Passenger(
                uniqueId,
                seat.GridX,
                seat.GridY,
                spriteManager.GetRandomSpriteId()
            );

            if (i == thiefIndex)
            {
                passenger.Status = PassengerStatus.Thief;
                // CORRIGIDO: Tag 'cyan' digitada corretamente
                Debug.Log($"<color=cyan>[DEV]</color> Ladrão gerado no ID: {uniqueId} na posição ({seat.GridX}, {seat.GridY})");
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

            UpdateExpelledPassengersVisual();
            
            currentRound++;

            if (currentRound > MAX_ROUNDS)
            {
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
            // CORRIGIDO: Adicionada checagem defensiva extra para evitar quebras
            if (seat.PassengerView != null && seat.PassengerView.gameObject.activeSelf && seat.PassengerView.Passenger != null)
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
        int spriteIdDoExpulso = expelled.SpriteId;
        string mensagem = ObterNomePorSpriteId(spriteIdDoExpulso);

        displayMessage(mensagem);
        Debug.Log("[FADE] Entrando em tela preta... Onibus parando no ponto.");
        yield return new WaitForSeconds(1.0f);

        if (gameResultUI != null)
        {
            gameResultUI.ShowRoundReport(currentRound);
        }

        // Executa o roubo da próxima rodada de forma segura
        if (matchSetupManager != null)
        {
            matchSetupManager.ExecuteThiefTurn(BusGrid);
        }

        yield return new WaitForSeconds(2.5f);

        Debug.Log("[FADE] Saindo da tela preta... Proximo round começou.");
        // TODO: Desativar painel de round report ou disparar animação de fade out aqui
    }

    private void ProcessWin()
    {
        int savedHighScore = SaveManager.LoadHighScore();

        if (savedHighScore == 0 || currentRound < savedHighScore)
        {
            SaveManager.SaveGameProgress(1, currentRound);
            Debug.Log($"[HIGHSCORE] Novo recorde estabelecido: {currentRound} rodadas!");
        }

        if (gameResultUI != null)
        {
            gameResultUI.ShowFinalResult(true, currentRound);
        }
    }

    private void ProcessLoss(bool ranOutRounds)
    {
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
                return $"{nome} diz: Tá repreendido em nome do Senhor";
            case 1:
                nome = clt[Random.Range(0, clt.Count)];
                return $"{nome} diz: Não acredito nisso, vou ter que pegar outro ônibus!!!";
            case 2:
                nome  = menina[Random.Range(0, menina.Count)];
                return $"{nome} diz: Eita bixiga!!";
            case 3:
                nome = menino[Random.Range(0, menino.Count)];
                return $"{nome} diz: Vou contar pra minha mãee";
            default:
                Debug.LogError("Lista de nomes não encontrada");
                return null;
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
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
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