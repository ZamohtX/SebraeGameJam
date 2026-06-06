using UnityEngine;

public class PassengerActionUI : MonoBehaviour
{
    public static PassengerActionUI Instance;

    [SerializeField]
    private GameObject panel;

    private PassengerView selectedPassenger;

    private void Awake()
    {
        Instance = this;
        panel.SetActive(false);
    }

    public void Open(PassengerView passengerView)
    {
        // Se o jogador clicar em OUTRO passageiro com a UI já aberta,
        // limpamos o highlight do anterior antes de aplicar no novo.
        if (selectedPassenger != null)
        {
            selectedPassenger.SetHighlight(false);
        }

        selectedPassenger = passengerView;

        // Ativa o highlight no passageiro atual
        selectedPassenger.SetHighlight(true);

        Vector3 screenPos = Camera.main.WorldToScreenPoint(passengerView.transform.position);
        panel.transform.position = screenPos;

        panel.SetActive(true);
    }

    public void Close()
    {
        // Remove o highlight do passageiro antes de limpar a seleção
        if (selectedPassenger != null)
        {
            selectedPassenger.SetHighlight(false);
        }

        selectedPassenger = null;
        panel.SetActive(false);
    }

    public void Accuse()
    {
        if (selectedPassenger != null)
        {
            Debug.Log($"Acusado: {selectedPassenger.Passenger.Id}");
        }

        // O Close() já vai cuidar de desligar o highlight e fechar o painel
        Close();
    }
}