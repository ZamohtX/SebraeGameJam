using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class InstructionsPanel : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    //void Update()
    //{
    //    if (panel.activeSelf && Input.GetMouseButtonDown(0))
    //    {
    //        if (!EventSystem.current.IsPointerOverGameObject())
    //        {
    //            ClosePanel();
    //        }
    //    }
    //}
    public void OpenPanel()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlayClick();
        panel.SetActive(true);
    }

    public void ClosePanel()
    {
        if (AudioManager.Instance != null && panel.activeInHierarchy) AudioManager.Instance.PlayClick();
        panel.SetActive(false);
    }
}