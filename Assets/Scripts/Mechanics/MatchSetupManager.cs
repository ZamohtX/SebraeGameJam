using System.Collections.Generic;
using UnityEngine;
using static ThiefTargetCalculator;

public class MatchSetupManager : MonoBehaviour
{
    public static MatchSetupManager Instance { get; private set; }

    [Header("Regras da Partida")]
    private List<ThiefActionRule> activeRules = new List<ThiefActionRule>();
    
    // Armazena o passageiro que é o ladrão para acesso rápido da IA
    private Passenger thiefPassenger;

    private void Awake()
    {
        if (Instance == null) 
        {
            Instance = this;
        }
        else 
        {
            Destroy(gameObject);
        }
    }

    // Inicializa a partida escolhendo as regras e identificando o ladrão
    public void InitializeMatch(List<Passenger> allPassengers, RulesPanelUI rulesPanel)
    {
        // 1. Encontra quem é o ladrão na lista de passageiros gerados
        thiefPassenger = allPassengers.Find(p => p.Status == PassengerStatus.Thief);

        if (thiefPassenger == null)
        {
            Debug.LogError("[CRÍTICO] Nenhum ladrão foi definido no Spawn de passageiros!");
            return;
        }

        // 2. Sorteia 3 regras únicas das 8 disponíveis (excluindo defeitos ou repetidas)
        SelectRandomRules();

        // 3. Atualiza a UI para mostrar as regras sorteadas ao jogador
        if (rulesPanel != null && activeRules.Count >= 3)
        {
            rulesPanel.ShowRules(activeRules[0], activeRules[1], activeRules[2]);
        }
    }

    private void SelectRandomRules()
    {
        activeRules.Clear();
        List<ThiefActionRule> allAvailableRules = new List<ThiefActionRule>(
            (ThiefActionRule[])System.Enum.GetValues(typeof(ThiefActionRule))
        );

        // Sorteia 3 regras distintas
        while (activeRules.Count < 3 && allAvailableRules.Count > 0)
        {
            int randomIndex = Random.Range(0, allAvailableRules.Count);
            activeRules.Add(allAvailableRules[randomIndex]);
            allAvailableRules.RemoveAt(randomIndex); // Evita repetir a mesma regra
        }
    }

    // Executado a cada parada do ônibus (Turno do Ladrão)
    public void ExecuteThiefTurn(BusGrid busGrid)
    {
        if (thiefPassenger == null) return;

        // Regra Especial (*): 15% de chance de fingir que foi roubado (se já não tiver sido)
        if (Random.value <= 0.15f && thiefPassenger.Status != PassengerStatus.Robbed)
        {
            thiefPassenger.Status = PassengerStatus.Robbed;
            Debug.Log("<color=yellow>[BLEFE]</color> O Ladrão (" + thiefPassenger.Id + ") fingiu ser uma vítima nesta rodada!");
            // TODO: Disparar feedback visual na UI de que o "Ladrão" foi roubado
            return;
        }

        // Caso não blefe, ele escolhe uma regra ativa aleatoriamente para cometer o roubo
        ThiefActionRule ruleToUse = activeRules[Random.Range(0, activeRules.Count)];
        
        // Calcula os alvos possíveis usando a classe estática que vocês criaram
        Vector2Int thiefPos = new Vector2Int(thiefPassenger.GridX, thiefPassenger.GridY);
        List<Passenger> possibleTargets = ThiefTargetCalculator.CalculatePossibleTargets(busGrid, thiefPos, ruleToUse);

        // Filtra para garantir que ele não roube quem já foi roubado ou a si mesmo
        possibleTargets.RemoveAll(p => p.Status == PassengerStatus.Robbed || p.Status == PassengerStatus.Thief);

        if (possibleTargets.Count > 0)
        {
            // Escolhe uma vítima aleatória entre os alvos válidos da regra
            Passenger victim = possibleTargets[Random.Range(0, possibleTargets.Count)];
            victim.Status = PassengerStatus.Robbed;

            Debug.Log("<color=red>[ROUBO]</color> O ladrão usou a regra " + ruleToUse + " e roubou o passageiro " + victim.Id + " na posição (" + victim.GridX + ", " + victim.GridY + ")");
            // TODO: Atualizar o visual do passageiro roubado na tela
        }
        else
        {
            Debug.Log("[TURNO] O ladrão tentou usar a regra " + ruleToUse + ", mas não encontrou nenhuma vítima válida disponível.");
        }
    }
}