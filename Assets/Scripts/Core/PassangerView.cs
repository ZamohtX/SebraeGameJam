using UnityEngine;

public class PassengerView : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer spriteRenderer;

    private Passenger passenger;

    public void Initialize(Passenger passenger, SpriteManager spriteManager)
    {
        this.passenger = passenger;

        spriteRenderer.sprite =
            spriteManager.GetSprite(passenger.SpriteId);

        spriteRenderer.color =
            spriteManager.GetColor(passenger.ClothingColorId);

        transform.position = new Vector3(
            passenger.GridX,
            passenger.GridY,
            0
        );
    }

    public Passenger GetPassenger()
    {
        return passenger;
    }
}