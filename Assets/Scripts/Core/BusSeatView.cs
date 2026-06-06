using UnityEngine;

public class BusSeatView : MonoBehaviour
{
    [Header("Coordenadas no onibus")]
    [SerializeField] private int gridX;
    [SerializeFIeld] private int gridY;

    [Header("Referencia visual do passageiro")]
    [SerializeField] private PassangerView passangerView;

    public int GridX => gridX;
    public int GridY => gridY;
    public PassangerView PassangerView => passangerView;

    private void Start()
    {
        // Registra automaticamente esta cadeira no GameManager assim que a cena carrega
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RegisterBusSeat(this);
        }
    }

}