using System.Collections.Generic;
using UnityEngine;
using TMPro;

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

    [Header("Pool de Nomes Alagoanos (Mecânica de Identidade)")]
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
            
            currentRound++;

            if (currentRound > MAX_ROUNDS)
            {
                // Inocente some imediatamente se o jogo acabou
                UpdateExpelledPassengersVisual();
                ProcessLoss(true);  
            }
            else
            {
                // Dispara a transição passando o passageiro acusado
                StartCoroutine(TransitionToNextRound(accusedPassenger));
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
private System.Collections.IEnumerator TransitionToNextRound(Passenger expelled)
    {
        // Pega o Raycaster do Canvas automaticamente se não tiver sido arrastado no Inspector
        UnityEngine.UI.GraphicRaycaster raycaster = GetComponentInParent<UnityEngine.UI.GraphicRaycaster>();
        if (raycaster == null) raycaster = FindFirstObjectByType<UnityEngine.UI.GraphicRaycaster>();

        // 🚨 BLOQUEIO TOTAL: Desativa qualquer clique ou interação na tela inteira
        if (raycaster != null) raycaster.enabled = false;

        // 1. FADE IN (A tela começa a escurecer enquanto o ônibus para no ponto)
        if (AudioManager.Instance != null) AudioManager.Instance.StopBus();
        
        if (FadeManager.Instance != null)
        {
            yield return StartCoroutine(FadeManager.Instance.FadeIn(0.8f));
        }
        else
        {
            yield return new WaitForSeconds(1.0f);
        }

        // 2. SUMIR COM O INOCENTE
        UpdateExpelledPassengersVisual();

        // 3. ATIVA A IMAGEM DO PONTO DE ÔNIBUS NO FUNDO
        if (RouteManager.Instance != null)
        {
            RouteManager.Instance.ShowCurrentStation();
        }

        // Prepara a fala de indignação do passageiro que saiu do ônibus
        string mensagemDoInocente = ObterNomePorSpriteId(expelled.SpriteId);
        displayMessage(mensagemDoInocente);

        // 4. FADE OUT (Revela o cenário do ponto com a mensagem da pessoa expulsa)
        if (FadeManager.Instance != null)
        {
            yield return StartCoroutine(FadeManager.Instance.FadeOut(0.5f));
        }

        // Tempo de leitura da reclamação do passageiro (Cliques continuam bloqueados aqui!)
        yield return new WaitForSeconds(3.5f);

        // 5. ATUALIZA A CAIXA PARA A SEGUNDA MENSAGEM
        if (AudioManager.Instance != null) AudioManager.Instance.PlayTheft();
        displayMessage("<color=red><b>ATENÇÃO MOTORISTA!</b></color>\n\nEnquanto o ônibus estava parado neste ponto, houve outro roubo a bordo!");

        // Executa o cálculo matemático do roubo da IA secretamente em background
        if (matchSetupManager != null)
        {
            matchSetupManager.ExecuteThiefTurn(BusGrid);
        }

        // Tempo para leitura do alerta de roubo
        yield return new WaitForSeconds(3.5f);

        // 6. FADE IN FINAL (Escurece a tela para sair do ponto turístico)
        if (FadeManager.Instance != null)
        {
            yield return StartCoroutine(FadeManager.Instance.FadeIn(0.8f));
        }
        else
        {
            yield return new WaitForSeconds(0.8f);
        }

        // 7. DESATIVAR PAINÉIS E AVANÇAR A FOTO PARA A PRÓXIMA ESTAÇÃO
        if (RouteManager.Instance != null)
        {
            RouteManager.Instance.HideStation();
            RouteManager.Instance.AdvanceToNextStation();
        }

        if (messagePanel != null) messagePanel.SetActive(false);

        if (AudioManager.Instance != null) AudioManager.Instance.StartBus();

        // 8. FADE OUT FINAL (Volta para dentro do ônibus)
        if (FadeManager.Instance != null)
        {
            yield return StartCoroutine(FadeManager.Instance.FadeOut(0.6f));
        }

        // 🔓 LIBERAÇÃO: Toda a animação acabou e o ônibus voltou a andar, agora o jogador pode clicar de novo
        if (raycaster != null) raycaster.enabled = true;

        Debug.Log("[FLUXO SEGURO] Parada finalizada. Cliques reativados para o gameplay.");
    }
    private void ProcessWin()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlayFanfare();

        int savedHighScore = SaveManager.LoadHighScore();

        if (savedHighScore == 0 || currentRound < savedHighScore)
        {
            SaveManager.SaveGameProgress(1, currentRound);
            Debug.Log($"[HIGHSCORE] Novo recorde estabelecido: {currentRound} rodadas!");
        }

        StartCoroutine(VictoryTransitionCoroutine());
    }

    private System.Collections.IEnumerator VictoryTransitionCoroutine()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.StopBus();
        yield return new WaitForSeconds(1.0f);

        if (RouteManager.Instance != null)
        {
            RouteManager.Instance.ShowCurrentStation();
        }

        int bestScore = SaveManager.LoadHighScore();
        displayMessage($"<color=green><b>LADRÃO CAPTURADO!</b></color>\n\nVocê descobriu o meliante em <b>{currentRound} rodadas</b>!\n\n🏆 Seu Melhor Recorde: {bestScore} rodadas.");

        yield return new WaitForSeconds(3.5f);

        if (gameResultUI != null)
        {
            gameResultUI.ShowFinalResult(true, currentRound);
        }
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
                return $"<b>{nome}</b> diz:\n<i>\"Não acredito nisso, vou perder a hora e ter que pegar outro ônibus!!!\"</i>";
            case 2:
                nome  = menina[Random.Range(0, menina.Count)];
                return $"<b>{nome}</b> diz:\n<i>\"Eita bixiga!!\"</i>";
            case 3:
                nome = menino[Random.Range(0, menino.Count)];
                return $"<b>{nome}</b> diz:\n<i>\"Vou contar tudo pra minha mãee!\"</i>";
            default:
                Debug.LogError("Lista de nomes não encontrada");
                return "Um passageiro inocente foi expulso do ônibus.";
        }
    }

    private void displayMessage(string message)
    {
        if (messagePanel == null || messageText == null) return;
        
        messageText.text = message;
        messagePanel.SetActive(true);
    }

    public BusSeatView GetSeatAtPosition(int x, int y)
    {
        return registeredSeats.Find(seat => seat.GridX == x && seat.GridY == y);
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