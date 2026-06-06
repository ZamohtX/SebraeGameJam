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
        selectedPassenger = passengerView;

        Vector3 screenPos =
            Camera.main.WorldToScreenPoint(
                passengerView.transform.position);

        panel.transform.position = screenPos;

        panel.SetActive(true);
    }

    public void Close()
    {
        selectedPassenger = null;

        panel.SetActive(false);
    }

    public void Accuse()
    {
        Debug.Log(
            $"Acusado: {selectedPassenger.Passenger.Id}");

        Close();
    }
}