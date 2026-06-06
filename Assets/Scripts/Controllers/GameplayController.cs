using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    Setup,
    BusMoving,
    ThiefTurn,
    PlayerTurn,
    Victory,
    GameOver
}

public class GameplayController : MonoBehaviour
{
    [Header("Grid Config")]
    [SerializeField] private int busWidth = 4;
    [SerializeField] private int busHeight = 10;
    [SerializeField] private int minPassengers = 35;

    private BusGrid busGrid;
    private MatchManager matchManager;
    private GameState currentState;
    
    private int currentStopIndex = 0;
    private int maxStops = 5;
    
    private List<string> maceioRoute = new List<string>
    {
        "Terminal UFAL",
        "Centro Histórico (Teatro Deodoro)",
        "Jaraguá",
        "Praia da Pajuçara (Feirinha)",
        "Ponta Verde (Marco dos Corais)"
    };

    private void Start()
    {
        InitializeGame();
    }

    public void InitializeGame()
    {
        currentState = GameState.Setup;
        busGrid = new BusGrid(busWidth, busHeight);
        matchManager = new MatchManager();
        
        matchManager.SetupMatch(busGrid, minPassengers);
        
        currentStopIndex = 0;
        Debug.Log("Ônibus saindo do: " + maceioRoute[currentStopIndex]);
        StartNextTurn();
    }

    public void StartNextTurn()
    {
        if (currentStopIndex >= maxStops - 1)
        {
            EndGame(false);
            return;
        }

        currentState = GameState.BusMoving;
        currentStopIndex++;
        
        Debug.Log("Próxima parada: " + maceioRoute[currentStopIndex]);
        ProcessThiefAction();
    }

    private void ProcessThiefAction()
    {
        currentState = GameState.ThiefTurn;
        currentState = GameState.PlayerTurn;
    }

    public void AccusePassenger(Passenger targetPassenger)
    {
        if (currentState != GameState.PlayerTurn || targetPassenger == null) return;

        if (targetPassenger.Status == PassengerStatus.Thief)
        {
            EndGame(true);
        }
        else
        {
            Debug.Log(targetPassenger.Id + " diz: 'Tá maluco, motorista? Eu sou trabalhador!'");
            StartNextTurn();
        }
    }

    private void EndGame(bool isVictory)
    {
        if (isVictory)
        {
            currentState = GameState.Victory;
            Debug.Log("VITÓRIA! Você prendeu o ladrão!");
        }
        else
        {
            currentState = GameState.GameOver;
            Debug.Log("GAME OVER... O ladrão escapou em Ponta Verde.");
        }
    }

    public string GetCurrentStopName()
    {
        return maceioRoute[currentStopIndex];
    }
}
