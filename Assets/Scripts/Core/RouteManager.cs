using UnityEngine;
using UnityEngine.UI;

public class RouteManager : MonoBehaviour
{
    public static RouteManager Instance { get; private set; }

    [Header("Cenários das Paradas de Maceió")]
    [SerializeField] private Sprite[] stationBackgrounds; 
    
    [Header("Referências de UI da Parada")]
    [SerializeField] private Image stationBackgroundImageComponent; 
    [SerializeField] private GameObject stationPanel; 

    private int currentStationIndex = 0;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
<<<<<<< HEAD

        if (stationPanel != null) stationPanel.SetActive(false);
=======
>>>>>>> 79db131acaf8a656b2997f5e3a9684222c022b24
    }

    public void ShowCurrentStation()
    {
        if (stationBackgrounds == null || stationBackgrounds.Length == 0) return;
        if (stationBackgroundImageComponent == null || stationPanel == null) return;

        int index = currentStationIndex % stationBackgrounds.Length;
        stationBackgroundImageComponent.sprite = stationBackgrounds[index];
<<<<<<< HEAD

=======
>>>>>>> 79db131acaf8a656b2997f5e3a9684222c022b24
        stationPanel.SetActive(true);
    }

    public void AdvanceToNextStation()
    {
        currentStationIndex++;
    }
<<<<<<< HEAD
    
=======

>>>>>>> 79db131acaf8a656b2997f5e3a9684222c022b24
    public void HideStation()
    {
        if (stationPanel != null) stationPanel.SetActive(false);
    }
}