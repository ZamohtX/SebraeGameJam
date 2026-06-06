using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Configurações do Tabuleiro")]
    [SerializeField] private int gridWidth = 4;
    [SerializeField] private int gridHeight = 5;

    [Header("Referências Técnicas")]
    [SerializeField] private SpriteManager spriteManager;
    [SerializeField] private RulesPanelUI rulesPanelUI;
    [SerializeField] private MatchSetupManager matchSetupManager; // Adicionado para integrar com a IA

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
        }
    }

    private void PopulateBus()
    {
        if (registeredSeats.Count == 0)
        {
            Debug.LogWarning("Nenhuma cadeira foi registrada! Verifique se as cadeiras possuem o componente BusSeatView.");
            return;
        }

        int thiefIndex = Random.Range(0, registeredSeats.Count);

        for (int i = 0; i < registeredSeats.Count; i++)
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