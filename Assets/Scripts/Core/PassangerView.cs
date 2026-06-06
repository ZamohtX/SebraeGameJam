using UnityEngine;
using UnityEngine.EventSystems;

public class PassengerView : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    private SpriteRenderer spriteRenderer;

    public Passenger Passenger { get; private set; }

    [SerializeField]
    private BoxCollider2D boxCollider;


    private void Awake()
    {
        if (boxCollider == null)
            boxCollider = GetComponent<BoxCollider2D>();

        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
    }
    public void Initialize(Passenger passenger, SpriteManager spriteManager)
    {
        this.Passenger = passenger;

        spriteRenderer.sprite =
            spriteManager.GetSprite(passenger.SpriteId);

        spriteRenderer.color =
            spriteManager.GetColor(passenger.ClothingColorId);

        transform.position = new Vector3(
            passenger.GridX,
            passenger.GridY,
            0
        );

        UpdateCollider();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log($"Clique em {Passenger.Id}");

        PassengerActionUI.Instance.Open(this);
    }

    public Passenger GetPassenger()
    {
        return Passenger;
    }

    private void UpdateCollider()
    {
        if (boxCollider == null || spriteRenderer.sprite == null)
            return;
        boxCollider.size =
            spriteRenderer.sprite.bounds.size;

        boxCollider.offset =
            spriteRenderer.sprite.bounds.center;
    }
}