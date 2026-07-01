using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class Monster : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 100f;
    private float health;

    [Header("Movement & Patrol")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float waitTimeAtPoint = 1.0f;

    [Header("Combat & Detection")]
    [SerializeField] private float detectionRange = 5f;
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float attackCooldown = 1.5f;
    [SerializeField] private int damageValue = 20;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip attackSound;
    [SerializeField] private AudioClip hurtSound;

    private Vector3 pointA;
    private Vector3 pointB;
    private Vector3 targetPoint;

    private bool isFacingRight = true;
    private bool isAttacking = false;
    private bool isWaiting = false;
    private bool isStunned = false;
    private float nextAttackTime;

    private Rigidbody2D rb;
    private Collider2D bodyCollider;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private Coroutine activeStateCoroutine;

    public Bounds BodyBounds
    {
        get { return bodyCollider != null ? bodyCollider.bounds : new Bounds(); }
    }

    public bool IsDead { get; private set; } = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        bodyCollider = GetComponent<Collider2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        health = maxHealth; // Initialize health state

        if (transform.childCount >= 2)
        {
            pointA = transform.GetChild(0).position;
            pointB = transform.GetChild(1).position;

            transform.GetChild(1).parent = null;
            transform.GetChild(0).parent = null;
        }
        else
        {
            Debug.LogError($"[{gameObject.name}] Can 2 object o vi tri dau tien de trien khai waypoint!");
            pointA = transform.position - Vector3.right * 3f;
            pointB = transform.position + Vector3.right * 3f;
        }

        targetPoint = pointA;
    }

    private void Update()
    {
        if (IsDead || isStunned) return;

        Transform player = DetectPlayer();

        if (player != null)
        {
            if (isWaiting)
            {
                InterruptActiveState();
                isWaiting = false;
            }
            HandleAggroState(player);
        }
        else if (!isAttacking && !isWaiting)
        {
            HandlePatrolState();
        }
    }

    private void HandlePatrolState()
    {
        Vector2 currentPos = transform.position;
        Vector2 targetPos = targetPoint;

        float directionX = Mathf.Sign(targetPos.x - currentPos.x);
        rb.linearVelocity = new Vector2(directionX * moveSpeed, rb.linearVelocity.y);

        UpdateFacingDirection(rb.linearVelocity.x);

        if (animator != null) animator.CrossFade("monster_walk", 0);

        if (Vector2.Distance(new Vector2(currentPos.x, 0), new Vector2(targetPos.x, 0)) < 0.2f)
        {
            activeStateCoroutine = StartCoroutine(WaitAtWaypointRoutine());
        }
    }

    private IEnumerator WaitAtWaypointRoutine()
    {
        isWaiting = true;
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);

        if (animator != null) animator.CrossFade("monster_idle", 0);

        yield return new WaitForSeconds(waitTimeAtPoint);

        targetPoint = (targetPoint == pointA) ? pointB : pointA;
        isWaiting = false;
    }

    private void HandleAggroState(Transform player)
    {
        float distanceToPlayer = Mathf.Abs(player.position.x - transform.position.x);

        UpdateFacingDirection(player.position.x - transform.position.x);

        if (distanceToPlayer <= attackRange)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);

            if (Time.time >= nextAttackTime && !isAttacking)
            {
                activeStateCoroutine = StartCoroutine(ExecuteAttack(player));
            }
        }
        else if (!isAttacking)
        {
            float chaseDirection = Mathf.Sign(player.position.x - transform.position.x);
            rb.linearVelocity = new Vector2(chaseDirection * moveSpeed, rb.linearVelocity.y);
            if (animator != null) animator.CrossFade("monster_walk", 0);
        }
    }

    private Transform DetectPlayer()
    {
        Vector2 direction = isFacingRight ? Vector2.right : Vector2.left;
        Vector2 origin = transform.position;

        RaycastHit2D hit = Physics2D.Linecast(origin, origin + (direction * detectionRange), playerLayer);
        Debug.DrawLine(origin, origin + (direction * detectionRange), hit.collider ? Color.red : Color.green);

        return hit.collider != null ? hit.transform : null;
    }

    private IEnumerator ExecuteAttack(Transform player)
    {
        isAttacking = true;
        nextAttackTime = Time.time + attackCooldown;

        if (animator != null) animator.CrossFade("monster_attack", 0);

        Vector2 dashDirection = isFacingRight ? transform.right : -transform.right;

        yield return StartCoroutine(SpringAttack.SpringMovementRoutine(rb, dashDirection, 0.2f, 0.25f));

        yield return new WaitForSeconds(0.25f);

        if (player != null && Mathf.Abs(player.position.x - transform.position.x) <= attackRange + 0.3f)
        {
            PlayerHealth health = player.GetComponent<PlayerHealth>();
            if (health != null)
            {
                health.TakeDamage(damageValue, dashDirection);
            }
        }

        yield return new WaitForSeconds(0.35f);
        isAttacking = false;
    }

    public void PlayAttackSound()
    {
        audioSource.PlayOneShot(attackSound);
    }

    private void UpdateFacingDirection(float horizontalMovement)
    {
        if (Mathf.Approximately(horizontalMovement, 0f)) return;

        if (horizontalMovement > 0f && !isFacingRight)
        {
            isFacingRight = true;
            spriteRenderer.flipX = false;
        }
        else if (horizontalMovement < 0f && isFacingRight)
        {
            isFacingRight = false;
            spriteRenderer.flipX = true;
        }
    }

    private void InterruptActiveState()
    {
        if (activeStateCoroutine != null)
        {
            StopCoroutine(activeStateCoroutine);
            activeStateCoroutine = null;
        }
    }

    public void TakeDamage(int damage, Vector2 attackDirection)
    {
        if (IsDead) return;

        health -= damage;
        audioSource.PlayOneShot(hurtSound);

        InterruptActiveState();
        isAttacking = false;
        isWaiting = false;

        if (health <= 0)
        {
            Die();
            return;
        }

        UpdateFacingDirection(-attackDirection.x);
        StartCoroutine(HitStunRoutine(attackDirection.normalized));
    }

    private IEnumerator HitStunRoutine(Vector2 pushDirection)
    {
        isStunned = true;

        if (animator != null) animator.CrossFade("monster_hit", 0);
        Vector2 horizontalPush = new Vector2(Mathf.Sign(pushDirection.x), 0f);

        yield return StartCoroutine(SpringAttack.SpringMovementRoutine(rb, horizontalPush, 0.8f, 0.2f));

        isStunned = false;
    }

    private void Die()
    {
        IsDead = true;

        rb.linearVelocity = Vector2.zero;

        if (bodyCollider != null)
        {
            bodyCollider.gameObject.layer = LayerMask.NameToLayer("IgnorePlayer");
        }

        if (animator != null)
        {
            animator.CrossFade("monster_die", 0);
        }
    }
}