using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.SceneManagement;
using static ThiefTargetCalculator;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game States")]
    public bool isPaused = false;

    [Header("UI References")]
    [SerializeField] private GameObject pauseMenuUI;

    [SerializeField]
    private SpriteManager spriteManager;

    public BusGrid BusGrid { get; private set; }

    private List<ThiefActionRule> CurrentRules = new List<ThiefActionRule>();

    [SerializeField]
    private RulesPanelUI rulesPanelUI;

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
    private void Start()
    {
        BusGrid = new BusGrid(4, 5);

        SpawnRandomPassengers(3);

        //Passenger thief = new Passenger("T", 1, 5, 0, 0);
        //thief.Status = PassengerStatus.Thief;

        //Passenger p1 = new Passenger("P1", 0, 4, 0, 0);
        //Passenger p2 = new Passenger("P2", 2, 6, 0, 0);
        //Passenger p3 = new Passenger("P3", 1, 9, 0, 0);
        //Passenger p4 = new Passenger("P4", 3, 5, 0, 0);
        //Passenger p5 = new Passenger("P5", 2, 7, 0, 0);
        //Passenger p6 = new Passenger("P6", 1, 8, 0, 0);
        //p6.Status = PassengerStatus.Robbed;

        //BusGrid.AddPassenger(thief, 1, 5);
        //BusGrid.AddPassenger(p1, 0, 4);
        //BusGrid.AddPassenger(p2, 2, 6);
        //BusGrid.AddPassenger(p3, 1, 9);
        //BusGrid.AddPassenger(p4, 3, 5);
        //BusGrid.AddPassenger(p5, 2, 7);
        //BusGrid.AddPassenger(p6, 1, 8);

        //foreach (ThiefActionRule rule in
        // System.Enum.GetValues(typeof(ThiefActionRule)))
        //{
        //    Debug.Log($"--- {rule} ---");

        //    List<Passenger> targets =
        //        ThiefTargetCalculator.CalculatePossibleTargets(
        //            BusGrid,
        //            new Vector2Int(thief.GridX, thief.GridY),
        //            rule);

        //    foreach (Passenger p in targets)
        //    {
        //        Debug.Log($"{p.Id}");
        //    }
        //}

        //CurrentRules = new List<ThiefActionRule>()
        //{
        //    ThiefActionRule.Area,
        //    ThiefActionRule.KnightMove,
        //    ThiefActionRule.Random
        //};

        //rulesPanelUI.ShowRules(
        //    CurrentRules[0],
        //    CurrentRules[1],
        //    CurrentRules[2]
        //);
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

    public Passenger CreateRandomPassenger(int x, int y)
    {
        return new Passenger(
            System.Guid.NewGuid().ToString(),
            x,
            y,
            spriteManager.GetRandomSpriteId(),
            spriteManager.GetRandomColorId()
        );
    }

    private void SpawnRandomPassengers(int amount)
    {
        HashSet<Vector2Int> occupiedSeats = new();

        for (int i = 0; i < amount; i++)
        {
            Vector2Int seatPosition =
                GetRandomFreeSeat(occupiedSeats);

            CreatePassengerAtSeat(i, seatPosition);

            occupiedSeats.Add(seatPosition);
        }
    }

    private Vector2Int GetRandomFreeSeat(
        HashSet<Vector2Int> occupiedSeats)
    {
        List<Vector2Int> availableSeats = new();

        for (int x = 0; x < 4; x++)
        {
            for (int y = 0; y < 5; y++)
            {
                Vector2Int seat = new(x, y);

                if (!occupiedSeats.Contains(seat))
                {
                    availableSeats.Add(seat);
                }
            }
        }

        return availableSeats[
            Random.Range(0, availableSeats.Count)
        ];
    }

    private void CreatePassengerAtSeat(
        int passengerIndex,
        Vector2Int seatPosition)
    {
        Passenger passenger =
            CreateRandomPassenger(passengerIndex, 0);

        passenger.GridX = seatPosition.x;
        passenger.GridY = seatPosition.y;

        BusGrid.AddPassenger(
            passenger,
            seatPosition.x,
            seatPosition.y);

        GameObject chair =
            GameObject.Find(
                $"chair_{seatPosition.x}_{seatPosition.y}");

        if (chair == null)
        {
            Debug.LogWarning(
                $"Cadeira chair_{seatPosition.x}_{seatPosition.y} não encontrada.");

            return;
        }

        Transform personTransform =
            chair.transform.Find("PersonSprite");

        if (personTransform == null)
        {
            Debug.LogWarning(
                $"PersonSprite não encontrado em {chair.name}");

            return;
        }

        personTransform.gameObject.SetActive(true);

        PassengerView view = personTransform.GetComponent<PassengerView>();

        view.Initialize(passenger, spriteManager);

        SpriteRenderer personRenderer =
            personTransform.GetComponent<SpriteRenderer>();

        personRenderer.sprite =
            spriteManager.GetSprite(passenger.SpriteId);

        personRenderer.color =
            spriteManager.GetColor(passenger.ClothingColorId);
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