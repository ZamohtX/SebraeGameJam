using UnityEngine;

public static class SaveManager
{
    // Chaves para salvar os dados
    private const string LEVEL_KEY = "CurrentLevel";
    private const string SCORE_KEY = "HighScore";

    // Salva o progresso básico
    public static void SaveGameProgress(int levelIndex, int currentScore)
    {
        PlayerPrefs.SetInt(LEVEL_KEY, levelIndex);
        PlayerPrefs.SetInt(SCORE_KEY, currentScore);
        PlayerPrefs.Save(); // Força a gravação no disco
        Debug.Log("Jogo Salvo com Sucesso!");
    }

    // Carrega o level salvo (retorna 1 se não achar nada)
    public static int LoadLevelProgress()
    {
        return PlayerPrefs.GetInt(LEVEL_KEY, 1); 
    }

    // Carrega o Score salvo (retorna 0 se não achar nada)
    public static int LoadHighScore()
    {
        return PlayerPrefs.GetInt(SCORE_KEY, 0);
    }

    // Apaga os dados (útil para botão de "Novo Jogo")
    public static void ClearSaveData()
    {
        PlayerPrefs.DeleteAll();
    }
}