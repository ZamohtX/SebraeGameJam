using UnityEngine;

public class PassengerActionUI : MonoBehaviour
{
    public static PassengerActionUI Instance;

    [SerializeField]
    private GameObject panel;

    private PassengerView selectedPassenger;
    public float yOffset = 255f;

    private void Awake()
    {
        Instance = this;
        panel.SetActive(false);
    }

    public void Open(PassengerView passengerView)
    {
        if (AudioManager.Instance != null)  AudioManager.Instance.PlayClick();

        if (selectedPassenger != null)
        {
            selectedPassenger.SetHighlight(false);
        }

        selectedPassenger = passengerView;

        // Ativa o highlight no passageiro atual
        selectedPassenger.SetHighlight(true);

        Vector3 screenPos = Camera.main.WorldToScreenPoint(passengerView.transform.position);
        // Vector3 screenPosOffset = new Vector3(screenPos.x, screenPos.y + yOffset, screenPos.z);
        screenPos.y += yOffset;
        panel.transform.position = screenPos;

        panel.SetActive(true);
    }

    public void Close()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlayClick();

        // Remove o highlight do passageiro antes de limpar a sele��o
        if (selectedPassenger != null)
        {
            selectedPassenger.SetHighlight(false);
        }

        selectedPassenger = null;
        panel.SetActive(false);
    }

    public void Accuse()
    {
        if (selectedPassenger != null && selectedPassenger.Passenger != null)
        {
            // Envia o passageiro selecionado para o GameManager julgar
            GameManager.Instance.CheckAccusation(selectedPassenger.Passenger);
        }
        if (AudioManager.Instance != null) AudioManager.Instance.PlayClick();

        if (selectedPassenger != null)
        {
            Debug.Log($"Acusado: {selectedPassenger.Passenger.Id}");
        }

        // O Close() j� vai cuidar de desligar o highlight e fechar o painel
        Close();

    }
}