using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Configurações do Tabuleiro")]
    [SerializeField] private int gridWidth = 4;
    [SerializeField] private int gridHeight = 5;

    [Header("Configurações de Turno")]
    private int currentRound = 1;
    private const int MAX_ROUNDS = 5;

    [Header("Referências de UI Internas")]
    [SerializeField] private GameObject messagePanel; 
    [SerializeField] private TMPro.TMP_Text messageText;

    [Header("Referências Técnicas")]
    [SerializeField] private SpriteManager spriteManager;
    [SerializeField] private RulesPanelUI rulesPanelUI;
    [SerializeField] private MatchSetupManager matchSetupManager; 
    [SerializeField] private GameResultUI gameResultUI;

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

    // Função de registro usada pelo BusSeatView ao iniciar a cena
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

        // Instancia e posiciona os passageiros logicamente e visualmente
        PopulateBus();

        // Tenta buscar o gerenciador de partidas se não foi arrastado no Inspector
        if (matchSetupManager == null)
        {
            matchSetupManager = FindFirstObjectByType<MatchSetupManager>();
        }

        // Inicializa as regras e a IA do Ladrão para o Round 1
        if (matchSetupManager != null)
        {
            matchSetupManager.InitializeMatch(BusGrid.GetAllPassengers(), rulesPanelUI);
            matchSetupManager.ExecuteThiefTurn(BusGrid);
        }
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
                spriteManager.GetRandomSpriteId(),
                spriteManager.GetRandomColorId()
            );

            if (i == thiefIndex)
            {
                passenger.Status = PassengerStatus.Thief;
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

    // Exibe textos informativos na sua caixa de mensagens nativa
    private void displayMessage(string message)
    {
        if (messagePanel == null || messageText == null) return;
        
        messageText.text = message;
        messagePanel.SetActive(true);
    }

    // Fluxo acionado pelo botão "Acusar" da interface do passageiro
    public void CheckAccusation(Passenger accusedPassenger)
    {
        if (accusedPassenger.Status == PassengerStatus.Thief)
        {
            ProcessWin();
        }
        else
        {
            // Errou: O inocente assume o status de expulso
            accusedPassenger.Status = PassengerStatus.Expelled;

            // Remove o sprite do passageiro expulso do ônibus antes do fade voltar
            UpdateExpelledPassengersVisual();
            
            currentRound++;

            if (currentRound > MAX_ROUNDS)
            {
                ProcessLoss(true);  
            }
            else
            {
                StartCoroutine(TransitionToNextRound());
            }
        }
    }

    private void UpdateExpelledPassengersVisual()
    {
        foreach (var seat in registeredSeats)
        {
            if (seat.PassengerView != null && seat.PassengerView.gameObject.activeSelf && seat.PassengerView.Passenger != null)
            {
                if (seat.PassengerView.Passenger.Status == PassengerStatus.Expelled)
                {
                    seat.PassengerView.gameObject.SetActive(false);
                }
            }
        }
    }

    // Coroutine para rounds normais (Erro de acusação)
    private System.Collections.IEnumerator TransitionToNextRound()
    {
        // 1. FADE IN (Transição simulada para a tela preta)
        Debug.Log("[FADE] Entrando em tela preta... Ônibus parando no ponto.");
        yield return new WaitForSeconds(1.0f);

        // 2. CONFIGURA O CENÁRIO (Injeta o PNG do ponto de ônibus atual no fundo)
        if (RouteManager.Instance != null)
        {
            RouteManager.Instance.ShowCurrentStation();
        }

        // 3. EXIBE A MENSAGEM NA CAIXA DE TEXTO NATIVA
        displayMessage("Houve outro roubo no ônibus!");

        // 4. TEMPO DE LEITURA (Jogador visualiza a parada de Maceió e lê o texto)
        yield return new WaitForSeconds(3.5f);

        // 5. PROCESSA O PRÓXIMO ROUBO (Acontece escondido antes do fade out)
        if (matchSetupManager != null)
        {
            matchSetupManager.ExecuteThiefTurn(BusGrid);
        }

        // 6. SEGUNDO FADE IN (Preparando para voltar ao ônibus)
        Debug.Log("[FADE] Entrando em tela preta para voltar para dentro do ônibus.");
        yield return new WaitForSeconds(1.0f);

        // 7. DESATIVA O CENÁRIO E AVANÇA A IMAGEM PARA A PRÓXIMA PARADA
        if (RouteManager.Instance != null)
        {
            RouteManager.Instance.HideStation(); 
            RouteManager.Instance.AdvanceToNextStation(); 
        }

        if (messagePanel != null)
        {
            messagePanel.SetActive(false);
        }

        Debug.Log("[FADE] Saindo da tela preta... Próximo round começou.");
    }

    private void ProcessWin()
    {
        int savedHighScore = SaveManager.LoadHighScore();

        // Se for menor número de rodadas, atualiza o recorde
        if (savedHighScore == 0 || currentRound < savedHighScore)
        {
            SaveManager.SaveGameProgress(1, currentRound);
            Debug.Log($"[HIGHSCORE] Novo recorde estabelecido: {currentRound} rodadas!");
        }  

        StartCoroutine(VictoryTransitionCoroutine());
    }

    // Coroutine para o fluxo de acerto (Vitória)
    private System.Collections.IEnumerator VictoryTransitionCoroutine()
    {
        Debug.Log("[FADE] Entrando em tela preta... Transicionando para a vitória.");
        yield return new WaitForSeconds(1.0f);

        if (RouteManager.Instance != null)
        {
            RouteManager.Instance.ShowCurrentStation();
        }

        int bestScore = SaveManager.LoadHighScore();
        displayMessage($"<color=green><b>LADRÃO CAPTURADO!</b></color>\n\nVocê descobriu o meliante em {currentRound} rodadas nesta parada.\n🏆 Seu melhor recorde: {bestScore} rodadas.");

        yield return new WaitForSeconds(3.0f);

        // Acende o painel final com o botão de reiniciar
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

    public void RestartScene()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }

    public void OnBusArrivedAtStation()
    {
        if (matchSetupManager != null)
        {
            matchSetupManager.ExecuteThiefTurn(BusGrid);
        }
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