using UnityEngine;

public class BusSeatView : MonoBehaviour
{
    [Header("Coordenadas no ônibus")]
    [SerializeField] private int gridX;
    [SerializeField] private int gridY; // CORRIGIDO: SerializeField com minúsculo

    [Header("Referência visual do passageiro")]
    [SerializeField]
    PassengerView passengerView; // PADRONIZADO: com 'e'

    public int GridX => gridX;
    public int GridY => gridY;
    public PassengerView PassengerView => passengerView;

    private void Start()
    {
        string objectName = gameObject.name;
        string[] nameParts = objectName.Split('_');

        if (nameParts.Length >= 3)
        {
            int.TryParse(nameParts[1], out gridX);
            int.TryParse(nameParts[2], out gridY);

            // --- NOVA LÓGICA DE ORDER IN LAYER ---

            // 1. Calcula a ordem do Objeto Pai (este objeto) baseado no gridX
            int paiOrder = 5 + (gridX * 10);

            // 2. Aplica a ordem no SpriteRenderer do Pai
            if (TryGetComponent<SpriteRenderer>(out var paiRenderer))
            {
                paiRenderer.sortingOrder = paiOrder;
            }

            // 3. Aplica a ordem no SpriteRenderer do Filho (PassengerView)
            // Como você já tem a referência ou o objeto filho, podemos buscar nele:
            if (passengerView != null && passengerView.TryGetComponent<SpriteRenderer>(out var filhoRenderer))
            {
                filhoRenderer.sortingOrder = paiOrder + 5;
            }
            else
            {
                // Caso o passengerView ainda não tenha sido arrastado no Inspector,
                // tenta buscar pelo primeiro SpriteRenderer nos filhos como plano de fundo
                var childRenderer = GetComponentInChildren<SpriteRenderer>();
                if (childRenderer != null && childRenderer != paiRenderer)
                {
                    childRenderer.sortingOrder = paiOrder + 5;
                }
            }
        }
        else
        {
            Debug.LogWarning($"O objeto {objectName} não está no formato correto 'chair_x_y'!");
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.RegisterSeat(this);
        }
    }
}