using UnityEngine;

[RequireComponent(typeof(PlayerHealth))]
public class PlayerHealthBar : MonoBehaviour
{
    [SerializeField]
    private Vector3 offset = new Vector3(0f, 1.4f, 0f);

    [SerializeField]
    private Vector2 size = new Vector2(1.3f, 0.14f);

    [SerializeField]
    private int sortingOrder = 20;

    [SerializeField]
    private Color fillColor = new Color(0.12f, 0.86f, 0.22f, 1f);

    private PlayerHealth playerHealth;
    private Transform barRoot;
    private Transform fill;
    private SpriteRenderer fillRenderer;

    private void Awake()
    {
        EnsureHealthReference();

        if (barRoot == null)
        {
            CreateBar();
        }
    }

    private void OnEnable()
    {
        EnsureHealthReference();
        playerHealth.HealthChanged += UpdateBar;
    }

    private void Start()
    {
        UpdateBar(playerHealth.CurrentHealth, playerHealth.MaxHealth);
    }

    private void LateUpdate()
    {
        if (barRoot == null)
        {
            return;
        }

        barRoot.position = transform.position + offset;
        barRoot.rotation = Quaternion.identity;
        barRoot.localScale = new Vector3(
            SafeInverse(transform.lossyScale.x),
            SafeInverse(transform.lossyScale.y),
            1f);
    }

    private void OnDisable()
    {
        if (playerHealth != null)
        {
            playerHealth.HealthChanged -= UpdateBar;
        }
    }

    public void Configure(Vector3 newOffset, Vector2 newSize, int newSortingOrder, Color newFillColor)
    {
        EnsureHealthReference();

        offset = newOffset;
        size = newSize;
        sortingOrder = newSortingOrder;
        fillColor = newFillColor;

        if (barRoot != null)
        {
            Destroy(barRoot.gameObject);
        }

        CreateBar();
        UpdateBar(playerHealth.CurrentHealth, playerHealth.MaxHealth);
    }

    private void CreateBar()
    {
        Sprite sprite = CreatePixelSprite();

        barRoot = new GameObject("Player Health Bar").transform;
        barRoot.SetParent(transform);
        barRoot.position = transform.position + offset;
        barRoot.rotation = Quaternion.identity;
        barRoot.localScale = new Vector3(
            SafeInverse(transform.lossyScale.x),
            SafeInverse(transform.lossyScale.y),
            1f);

        Transform background = CreatePart("Background", sprite, new Color(0.12f, 0.02f, 0.02f, 0.85f), sortingOrder);
        background.SetParent(barRoot);
        background.localPosition = Vector3.zero;
        background.localScale = new Vector3(size.x + 0.08f, size.y + 0.08f, 1f);

        Transform empty = CreatePart("Empty", sprite, new Color(0.42f, 0.04f, 0.04f, 0.95f), sortingOrder + 1);
        empty.SetParent(barRoot);
        empty.localPosition = Vector3.zero;
        empty.localScale = new Vector3(size.x, size.y, 1f);

        fill = CreatePart("Fill", sprite, fillColor, sortingOrder + 2);
        fill.SetParent(barRoot);
        fill.localPosition = new Vector3(-size.x * 0.5f, 0f, 0f);
        fill.localScale = new Vector3(size.x, size.y, 1f);
        fillRenderer = fill.GetComponent<SpriteRenderer>();
    }

    private Transform CreatePart(string partName, Sprite sprite, Color color, int order)
    {
        GameObject part = new GameObject(partName);
        SpriteRenderer renderer = part.AddComponent<SpriteRenderer>();
        renderer.sprite = sprite;
        renderer.color = color;
        renderer.sortingOrder = order;
        return part.transform;
    }

    private Sprite CreatePixelSprite()
    {
        Texture2D texture = Texture2D.whiteTexture;
        return Sprite.Create(texture, new Rect(0f, 0f, 1f, 1f), new Vector2(0.5f, 0.5f), 1f);
    }

    private void UpdateBar(int currentHealth, int maxHealth)
    {
        if (fill == null)
        {
            return;
        }

        float percent = maxHealth <= 0 ? 0f : Mathf.Clamp01((float)currentHealth / maxHealth);
        fill.localScale = new Vector3(size.x * percent, size.y, 1f);
        fill.localPosition = new Vector3(-size.x * 0.5f + fill.localScale.x * 0.5f, 0f, 0f);

        if (fillRenderer != null)
        {
            fillRenderer.color = Color.Lerp(new Color(0.9f, 0.08f, 0.02f, 1f), fillColor, percent);
        }
    }

    private void EnsureHealthReference()
    {
        if (playerHealth == null)
        {
            playerHealth = GetComponent<PlayerHealth>();
        }
    }

    private float SafeInverse(float value)
    {
        if (Mathf.Approximately(value, 0f))
        {
            return 1f;
        }

        return 1f / value;
    }
}
