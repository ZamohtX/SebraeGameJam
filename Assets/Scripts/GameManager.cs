using System.Collections.Generic;
using UnityEngine;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set;}

    [Header("Configurações do Tabuleiro")]
    [SerializeField] private int gridWidth = 4;
    [SerializeField] private int gridHeight = 5;


    [Header("Referencia para o SpriteManager")]
    [SerializeField] private SpriteManager spriteManager;

    public BusGrid BusGrid { get; private set; }
    public bool isPaused { get; private set; } = false;


    // Lista de todas as cadeiras do ônibus para fácil acesso
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

    // Mettodo chamado automaticamente pelas cadeiras ao iniciarem
    public void RegisterSeat(BusSeatView seat)
    {
        if (!registeredSeats.Contains(seat))
        {
            registeredSeats.Add(seat);
        }
    }


    private void Start()
    {
        // Inicializa a matriz logica do onibus
        BusGrid = new BusGrid(gridWidth, gridHeight);
        // Preenche o onibus baseado nas cadeiras fisicas que se registraram
        PopulateBus();
    }


    private void PopulateBus()
    {
        // Escolhe uma cadeira aleatoria para ser o ladrao
        int thiefIndex = Random.Range(0, registeredSeats.Count);


        for (int i = 0; i < registeredSeats.Count; i++)
        {
            BusSeatView seat = registeredSeats[i];

            // Cria o passageiro logico com a posição correta da poltrona
            string uniqueId = System.Guid.NewGuid().ToString().Substring(0, 5);
            Passenger passenger = new Passenger(
                uniqueId,
                seat.GridX,
                seat.GridY,
                spriteManager.GetRandomSpriteId(),
                spriteManager.GetRandomSpriteId()
            );

            // Definição do ladrão
            if ( i == thiefIndex)
            {
                passenger.Status = PassengerStatus.Thief;
                Debug.Log($"Ladrão definido: {passenger.Id} na cadeira ({passenger.GridX}, {passenger.GridY})");
            }

            // Adiciona na matriz logica (BusGrid)
            BusGrid.AddPassenger(passenger, seat.GridX, seat.GridY);

            // Liga a parte logica com a parte visual (PassangerView)
            if (seat.PassengerView != null)
            {
                seat.PassengerView.gameObject.SetActive(true); // Ativa o GameObject do passageiro
                seat.PassengerView.Initialize(passenger, spriteManager);
            }
        }
    }

    #region Sistema de Pause Tradional
    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f; // Congela o tempo do jogo
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f; // Retoma o tempo do jogo
    }
    #endregion


}