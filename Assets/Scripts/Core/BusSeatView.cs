using UnityEngine;

public class BusSeatView : MonoBehaviour
{
    [Header("Coordenadas no ônibus")]
    [SerializeField] private int gridX;
    [SerializeField] private int gridY; // CORRIGIDO: SerializeField com minúsculo

    [Header("Referência visual do passageiro")]
    [SerializeField] private PassengerView passengerView; // PADRONIZADO: com 'e'

    public int GridX => gridX;
    public int GridY => gridY;
    public PassengerView PassengerView => passengerView;

    private void Start()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RegisterSeat(this);
        }
    }
}