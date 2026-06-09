using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class GhostRider1 : MonoBehaviour
{
    public float speed = 2f;
    public Transform groundCheck;
    public bool movingRight = true;

    [SerializeField]
    private int damage = 10;

    [SerializeField]
    private float attackRange = 0.85f;

    [SerializeField]
    private float attackCooldown = 1f;

    [HideInInspector]
    public Rigidbody2D rb;

    [SerializeField]
    private LayerMask groundLayer = ~0;

    [SerializeField]
    private float groundCheckDistance = 0.25f;

    [SerializeField]
    private float wallCheckDistance = 0.15f;

    private Collider2D bodyCollider;
    private PlayerHealth health;
    private PlayerHealthBar healthBar;
    private PlayerHealth playerHealth;
    private Transform player;
    private float nextAttackTime;
    private bool isDying;

    public bool IsDead => health != null && health.IsDead;
    public Bounds BodyBounds => bodyCollider.bounds;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        bodyCollider = GetComponent<Collider2D>();
        health = GetComponent<PlayerHealth>();
        if (health == null)
        {
            health = gameObject.AddComponent<PlayerHealth>();
        }

        healthBar = GetComponent<PlayerHealthBar>();
        if (healthBar == null)
        {
            healthBar = gameObject.AddComponent<PlayerHealthBar>();
        }

        healthBar.Configure(
            new Vector3(0f, 1.05f, 0f),
            new Vector2(0.75f, 0.08f),
            20,
            new Color(0.95f, 0.18f, 0.08f, 1f));

        health.HealthDepleted += Die;
    }

    private void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject == null)
        {
            playerObject = GameObject.Find("Player");
        }

        if (playerObject != null)
        {
            player = playerObject.transform;
            playerHealth = playerObject.GetComponent<PlayerHealth>();
        }
    }

    private void OnDestroy()
    {
        if (health != null)
        {
            health.HealthDepleted -= Die;
        }
    }

    private void Update()
    {
        TryDamagePlayer();
    }

    private void FixedUpdate()
    {
        if (health.IsDead)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            return;
        }

        float direction = movingRight ? 1f : -1f;
        rb.linearVelocity = new Vector2(direction * speed, rb.linearVelocity.y);

        if (ShouldTurnAround(direction))
        {
            Flip();
        }
    }

    private bool ShouldTurnAround(float direction)
    {
        Bounds bounds = bodyCollider.bounds;
        Vector2 wallOrigin = new Vector2(
            bounds.center.x + direction * bounds.extents.x,
            bounds.center.y);

        if (HasHitOtherCollider(wallOrigin, Vector2.right * direction, wallCheckDistance))
        {
            return true;
        }

        Vector2 floorOrigin = groundCheck != null
            ? groundCheck.position
            : new Vector2(bounds.center.x + direction * bounds.extents.x, bounds.min.y);

        return !HasHitOtherCollider(floorOrigin, Vector2.down, groundCheckDistance);
    }

    private bool HasHitOtherCollider(Vector2 origin, Vector2 direction, float distance)
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(origin, direction, distance, groundLayer);
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider != null && hit.collider != bodyCollider && !hit.collider.isTrigger)
            {
                return true;
            }
        }

        return false;
    }

    private void Flip()
    {
        movingRight = !movingRight;

        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * (movingRight ? 1f : -1f);
        transform.localScale = scale;
    }

    public void TakeDamage(int amount)
    {
        health.TakeDamage(amount);

        if (health.IsDead)
        {
            Die();
        }
    }

    private void TryDamagePlayer()
    {
        if (health.IsDead || player == null || playerHealth == null || playerHealth.IsDead || Time.time < nextAttackTime)
        {
            return;
        }

        float distance = Vector2.Distance(transform.position, player.position);
        if (distance > attackRange)
        {
            return;
        }

        nextAttackTime = Time.time + attackCooldown;
        playerHealth.TakeDamage(damage);
    }

    private void Die()
    {
        if (isDying)
        {
            return;
        }

        isDying = true;
        Destroy(gameObject);
    }
}
