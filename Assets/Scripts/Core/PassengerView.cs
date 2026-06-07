using UnityEngine;
using UnityEngine.EventSystems;

public class PassengerView : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private BoxCollider2D boxCollider;
     [SerializeField] private GameObject robbedNotice;

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

        gameObject.SetActive(true);
        
        UpdateVisual();
        // UpdateCollider();
    }

    // CORRIGIDO: Implementação do UpdateVisual que estava faltando
    public void UpdateVisual()
    {
        if (Passenger == null || spriteManager == null || spriteRenderer == null || robbedNotice == null) return;

        if (Passenger.Status == PassengerStatus.Intact) {
            spriteRenderer.sprite = spriteManager.GetSprite(Passenger.SpriteId);
            robbedNotice.SetActive(false);
        }
        else if (Passenger.Status == PassengerStatus.Robbed)
        {
            spriteRenderer.sprite = spriteManager.GetRobbedSprite(Passenger.SpriteId);
            robbedNotice.SetActive(true);
        }
        //spriteRenderer.color = spriteManager.GetColor(Passenger.ClothingColorId);
    }

    //Atualização dinâmica do collider para o clique funcionar no tamanho certo do sprite
    private void UpdateCollider()
    {
        if (boxCollider == null || spriteRenderer.sprite == null) return;
        
        boxCollider.size = spriteRenderer.sprite.bounds.size;
        boxCollider.offset = spriteRenderer.sprite.bounds.center;
    }

    // CORRIGIDO: Retornado o clique para abrir a UI de acusação
    public void OnPointerClick(PointerEventData eventData)
    {
        if (PassengerActionUI.Instance != null)
        {
            PassengerActionUI.Instance.Open(this);
        }
    }

    public void SetHighlight(bool active)
    {
        if (spriteRenderer == null) return;

        if (active)
        {
            Color hdrWhite = new Color(1.25f, 1.25f, 1.25f, 1f);
            spriteRenderer.color = hdrWhite;
        }
        else
        {
            // Volta para a cor padrão do sprite (branco normal)
            spriteRenderer.color = Color.white;
        }
    }

    public void RobPassenger()
    {
        if (Passenger == null) return;

        UpdateVisual();
    }
}