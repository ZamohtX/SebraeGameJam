using UnityEngine;
using UnityEngine.EventSystems;


public class PassangerView : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private BoxCollider2D boxCollider;

    public Passenger Passenger { get; private set; }
    private SpriteManager spriteManager;

    private void Awake()
    {
        if (boxCollider == null) boxCollider = GetComponent<BoxCollider2D>();
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Initialize(Passenger passenger, SpriteManager manager)
    {
        this.Passenger = passenger;
        this.spriteManager = manager;

        // Ativa o gameObject Caso estivesse com assento vazio
        gameObject.SetActive(true);
        
        UpdateVisual();
    }

}