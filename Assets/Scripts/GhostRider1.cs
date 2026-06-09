using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class GhostRider1 : MonoBehaviour
{
    public float speed = 2f;
    public Transform groundCheck;
    public bool movingRight = true;

    [HideInInspector]
    public Rigidbody2D rb;

    [SerializeField]
    private LayerMask groundLayer = ~0;

    [SerializeField]
    private float groundCheckDistance = 0.25f;

    [SerializeField]
    private float wallCheckDistance = 0.15f;

    private Collider2D bodyCollider;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        bodyCollider = GetComponent<Collider2D>();
    }

    private void FixedUpdate()
    {
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
}
