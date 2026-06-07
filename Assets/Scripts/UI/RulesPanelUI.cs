using TMPro;
using UnityEngine;
using static ThiefTargetCalculator;

public class RulesPanelUI : MonoBehaviour
{
    [SerializeField] private GameObject panel;

    [SerializeField] private TMP_Text rule1Text;
    [SerializeField] private TMP_Text rule2Text;
    [SerializeField] private TMP_Text rule3Text;

    public void TogglePanel()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlayClick();
        panel.SetActive(!panel.activeSelf);
    }

    public void ShowRules(
        ThiefActionRule rule1,
        ThiefActionRule rule2,
        ThiefActionRule rule3)
    {
        rule1Text.text = FormatRule(rule1);
        rule2Text.text = FormatRule(rule2);
        rule3Text.text = FormatRule(rule3);
    }

    private string FormatRule(ThiefActionRule rule)
    {
        return rule switch
        {
            ThiefActionRule.Adjacent =>
                "Adjacente\nPessoas nos bancos vizinhas.",

            ThiefActionRule.SameRow =>
                "Mesma Fileira\nPessoas na mesma linha.",

            ThiefActionRule.SameColumn =>
                "Mesma Coluna\nPessoas na mesma coluna.",

            ThiefActionRule.KnightMove =>
                "Em L\nMovimento de cavalo do xadrez.",

            ThiefActionRule.Diagonal =>
                "Diagonal\nPessoas que fazem um X.",

            ThiefActionRule.Cross =>
                "Linha e Coluna\nForma uma cruz completa.",

            ThiefActionRule.Area =>
                "Área\nRetângulo 3x3 a partir do suspeito.",

            ThiefActionRule.Random =>
                "Aleatório\nQualquer pessoa válida (Exceto o suspeito).",

            _ => rule.ToString()
        };
    }
}