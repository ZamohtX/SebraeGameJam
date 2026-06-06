using System.Collections.Generic;
using UnityEngine;

// Definição dos estados possíveis do jogo
public enum GameState
{
    Setup,          // Preparando o ônibus e passageiros
    BusMoving,      // Ônibus andando em direção à próxima parada
    ThiefTurn,      // Momento em que o ladrão age (oculto)
    PlayerTurn,     // Jogador analisa o ônibus e decide se vai acusar ou avançar
    Victory,        // Jogador descobriu o ladrão
    GameOver        // O ônibus chegou ao fim da linha ou todos foram roubados
}

public class GameplayController : MonoBehaviour
{
    [Header("Grid Config")]
    [SerializeField] private int busWidth = 4;
    [SerializeField] private int busHeight = 10;
    [SerializeField] private int minPassengers = 35;

    private BusGrid busGrid;
    private MatchSetupManager setupManager;
    private GameState currentState;

    private int currentStopIndex = 0;
    private int maxStops = 5; // Quantidade de paradas turísticas até o fim da linha

    // Lista com os nomes das paradas para ambientação alagoana
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

    // Inicializa as classes lógicas e prepara a partida
    public void InitializeGame()
    {
        currentState = GameState.Setup;

        // Instancia a matriz do ônibus e o populador que você criou
        busGrid = new BusGrid(busWidth, busHeight);
        setupManager = new MatchSetupManager();

        // Popula o ônibus e sorteia o ladrão
        setupManager.SetupMatch(busGrid, minPassengers);

        currentStopIndex = 0;
        Debug.Log("Ônibus saindo do: " + maceioRoute[currentStopIndex]);

        // Coloca o ônibus em movimento
        StartNextTurn();
    }

    // Controla a transição de turnos e avança o ônibus
    public void StartNextTurn()
    {
        if (currentStopIndex >= maxStops - 1)
        {
            EndGame(false); // Fim da linha! O ladrão escapou em Ponta Verde
            return;
        }

        currentState = GameState.BusMoving;
        currentStopIndex++;

        Debug.Log("Próxima parada: " + maceioRoute[currentStopIndex]);

        // Dispara o turno do ladrão logo em seguida
        ProcessThiefAction();
    }

    // Executa a lógica oculta do roubo
    private void ProcessThiefAction()
    {
        currentState = GameState.ThiefTurn;

        // 1. Aqui entra a chamada para o script de roubo

        Debug.Log("[Ladrão]: Alguém foi roubado no caminho para " + maceioRoute[currentStopIndex]);

        // Libera o controle para o jogador interagir
        currentState = GameState.PlayerTurn;
    }

    // Função chamada pela UI quando o jogador clica em um passageiro para acusar
    public void AccusePassenger(Passenger targetPassenger)
    {
        if (currentState != GameState.PlayerTurn) return;

        if (targetPassenger == null) return;

        // Verifica se o passageiro clicado é o ladrão
        if (targetPassenger.Status == PassengerStatus.Thief)
        {
            EndGame(true); // Vitória!
        }
        else
        {
            // Punição por acusar um inocente
            Debug.Log(targetPassenger.Id + " diz: 'Tá maluco, motorista? Eu sou trabalhador!'");

            // Avança o jogo para a próxima parada após o erro
            StartNextTurn();
        }
    }

    // Finaliza a partida exibindo o resultado
    private void EndGame(bool isVictory)
    {
        if (isVictory)
        {
            currentState = GameState.Victory;
            Debug.Log("VITÓRIA! Você prendeu o ladrão antes do fim da linha!");
        }
        else
        {
            currentState = GameState.GameOver;
            Debug.Log("GAME OVER... O ônibus chegou na Ponta Verde e o ladrão desceu com o bolso cheio.");
        }
    }

    // Getter para a UI saber em qual parada o ônibus está
    public string GetCurrentStopName()
    {
        return maceioRoute[currentStopIndex];
    }
}