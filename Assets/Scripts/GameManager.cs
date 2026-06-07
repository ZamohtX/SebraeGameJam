using System.Collections.Generic;
using System.Dynamic;
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

    // Função de registro usada pelo BusSeatView
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

        // Inicializa as regras e a IA do Ladrão
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
        int amount = Random.Range(15, 20);
        int thiefIndex = Random.Range(0, amount);

        for (int i = 0; i < amount; i++)
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
                Debug.Log($"<color=cyanq>[DEV]</color> Ladrão gerado no ID: {uniqueId} na posição ({seat.GridX}, {seat.GridY})");
            }

            BusGrid.AddPassenger(passenger, seat.GridX, seat.GridY);

            if (seat.PassengerView != null)
            {
                seat.PassengerView.gameObject.SetActive(true);
                seat.PassengerView.Initialize(passenger, spriteManager);
            }
        }
    }

    // FLuxo chamado quando o jogador clica no botao acusar
   public void CheckAccusation(Passenger accusedPassenger)
    {
        if (accusedPassenger.Status == PassengerStatus.Thief)
        {
            ProcessWin();
        }
        else{
            accusedPassenger.Status = PassengerStatus.Expelled;

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
        foreach ( var seat in registeredSeats)
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

    private System.Collections.IEnumerator TransitionToNextRound()
    {
        // Chamar a animação de Fade In aqui
        Debug.Log("[FADE] Entrando em tela preta... Onibus parando no ponto.");
        yield return new WaitForSeconds(1.0f); // Simula o tempo do Fade

        // Alerta na tela: "Houve outro roubo no onibus"
        if (gameResultUI != null)
        {
            // mostra a tela informando novo roubo
            gameResultUI.ShowRoundReport(currentRound);
        }

        // Executa o proximo roubo do ladrão por tras dos panos
        if (matchSetupManager != null)
        {
            matchSetupManager.ExecuteThiefTurn(BusGrid);
        }

        yield return new WaitForSeconds(2.5f);

        Debug.Log("Saindo da tela preta... Proximo round começou.");
    }


    private void ProcessWin()
    {
        // Salva o Highscore
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


    

    public void RestartScene()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }



    // Chamado na virada de ponto turístico/rodada
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