using UnityEngine;

public class SpriteManager : MonoBehaviour
{
    [SerializeField]
    private Sprite[] passengerSprites;

    [SerializeField]
    private Color[] clothingColors;

    public int GetRandomSpriteId()
    {
        return Random.Range(0, passengerSprites.Length);
    }

    public int GetRandomColorId()
    {
        return Random.Range(0, clothingColors.Length);
    }

    public Sprite GetSprite(int spriteId)
    {
        return passengerSprites[spriteId];
    }

    public Color GetColor(int colorId)
    {
        return clothingColors[colorId];
    }
}