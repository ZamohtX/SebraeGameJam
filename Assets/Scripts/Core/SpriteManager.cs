using UnityEngine;

public class SpriteManager : MonoBehaviour
{
    [SerializeField]
    private Sprite[] passengerSprites;

    [SerializeField]
    private Sprite[] robbedSprites;

    public int GetRandomSpriteId()
    {
        return Random.Range(0, passengerSprites.Length);
    }

    public Sprite GetSprite(int spriteId)
    {
        return passengerSprites[spriteId];
    }

    public Sprite GetRobbedSprite(int spriteId)
    {
        if (spriteId < 0 || spriteId >= robbedSprites.Length) return null;
        return robbedSprites[spriteId];
    }
}