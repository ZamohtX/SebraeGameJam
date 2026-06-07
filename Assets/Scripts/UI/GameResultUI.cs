using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameResultUI : MonoBehaviour
{
    public static GameResultUI Instance { get; private set; }

    [Header("Painel Principal")]
    [SerializeField] private GameObject resultPanel;




    [Header("Elementos de Texto")]
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text descriptionText;


    [Header("Controle de Interação")]
    [SerializeField] private Button actionButton;
    [SerializeField] private TMP_Text buttonText;


    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // Começa escondido
        if (resultPanel != null) resultPanel.SetActive(false);
    }


    public void ShowRoundReport(int nextRound)
    {
        resultPanel.SetActive(true);
        titleText.text = "<color=yellow>ATENÇÃO MOTORISTA!</color>";
        descriptionText.text = $"O ônibus parou no ponto.\n\n<color=red><b>HOUVE OUTRO ROUBO A BORDO!</b></color>\nEstamos iniciando o Round {nextRound}/5. Observe as pistas!";    
    

        actionButton.gameObject.SetActive(false);
    }


    public void ShowFinalResult(bool won, int roundsUsed, string thiefId = "")
    {
        resultPanel.SetActive(true);
        actionButton.gameObject.SetActive(true);
        buttonText.text= "Jogar Novamente";
        
        int bestScore = SaveManager.LoadHighScore();

        if (won)
        {
            titleText.text = "<color=green>LADRÃO CAPTURADO!</color>";
            descriptionText.text = $"Você descobriu o meliante em <b>{roundsUsed} rodadas</b>!\n\n" +
                                $"🏆 Seu Melhor Recorde: {bestScore} rodadas.";
        }
        else
        {
            titleText.text = "<color=red>FIM DA LINHA!</color>";
            descriptionText.text = $"O limite de 5 rounds acabou e o ônibus chegou ao ponto final!\n" +
                                $"O verdadeiro ladrão era o passageiro <b>{thiefId}</b> e ele escapou com os pertences.";
        }

        actionButton.onClick.RemoveAllListeners();
        actionButton.onClick.AddListener(() => resultPanel.SetActive(false));
        actionButton.onClick.AddListener(() => GameManager.Instance.RestartScene());
    }


    public void ShowResult(bool won, string thiefId, Vector2Int thiefPos)
    {
        if (resultPanel == null) return;

        resultPanel.SetActive(true);

        if (won)
        {
            titleText.text = "<color=green>LADRÃO CAPTURADO!</color>";
            descriptionText.text = $"Parabéns, motorista! Você descobriu o meliante (ID: {thiefId}) antes que ele fizesse a limpa na linha de Maceió.";
        }
        else
        {
            titleText.text = "<color=red>FALHOU CORRIDA!</color>";
            descriptionText.text = $"Você acusou um inocente! O verdadeiro ladrão era o ID {thiefId} na poltrona ({thiefPos.x}, {thiefPos.y}). Ele aproveitou a confusão e levou tudo!";
        }

        // Configura o botão para reiniciar o jogo quando clicado
        actionButton.onClick.RemoveAllListeners();
        actionButton.onClick.AddListener(() => GameManager.Instance.RestartScene());
    }
}