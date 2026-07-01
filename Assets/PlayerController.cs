using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]

public static class SpringAttack
{
    public static IEnumerator SpringMovementRoutine(Rigidbody2D rb, Vector2 direction, float distance, float duration)
    {
        Vector2 startPosition = rb.position;
        Vector2 targetPosition = startPosition + (direction.normalized * distance);
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;

            float springT = Mathf.SmoothStep(0f, 1f, t);

            rb.MovePosition(Vector2.Lerp(startPosition, targetPosition, springT));
            yield return null;
        }

        rb.MovePosition(targetPosition);
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
    }
}
public class PlayerController : MonoBehaviour
{
    public float walkSpeed = 5f;
    public float runSpeed = 8f;
    public float jumpPower = 10f;

    [SerializeField] private int attackDamage = 25;
    [SerializeField] private float attackRange = 1.25f;
    [SerializeField] private float attackDashForce = 1.25f;
    [SerializeField] private float attackCooldown = 0.35f;
    [SerializeField] private float fallThreshold = -10f;
    [SerializeField] private LayerMask groundLayer;

    [SerializeField] private LayerMask enemyLayer;
    [SerializeField]private Vector2 attackBoxSize = new Vector2(1.5f, 1f);


    [SerializeField] private AudioSource playerAudio;
    [SerializeField] private AudioClip hurtSound;
    [SerializeField] private AudioClip attackSound;
    [SerializeField] private AudioClip dieSound;

    Vector2 moveInput;
    float nextAttackTime;
    private bool canMove = true;
    private bool isAttacking = false;
    private bool isHit = false;

    // Check if the player is moving and return the current move speed

    public bool MoveAble
    {
        get { return canMove; }
        set { canMove = value; }
    }
    public float CurrentMoveSpeed
    {
        get
        {
            if (IsMoving)
            {
                return IsRunning ? runSpeed : walkSpeed;
            }
            return 0f;
        }
    }

    [SerializeField] private bool _isMoving = false;
    public bool IsMoving
    {
        get { return _isMoving; }
        private set
        {
            if (_isMoving != value)
            {
                _isMoving = value;
                UpdateAnimationState();
            }
        }
    }

    [SerializeField] private bool _isRunning = false;
    public bool IsRunning
    {
        get { return _isRunning; }
        private set
        {
            if (_isRunning != value)
            {
                _isRunning = value;
                UpdateAnimationState();
            }
        }
    }

    [SerializeField] private bool _isFacingRight = true;
    public bool IsFacingRight
    {
        get { return _isFacingRight; }
        private set
        {
            if (_isFacingRight != value)
            {
                _isFacingRight = value;
                GetComponent<SpriteRenderer>().flipX = !value;
            }
        }
    }

    [SerializeField] private bool _isGrounded = true;
    public bool IsGrounded
    {
        get { return _isGrounded; }
        set
        {
            if (_isGrounded != value)
            {
                _isGrounded = value;
                UpdateAnimationState();
            }
        }
    }

    Rigidbody2D rb;
    Collider2D bodyCollider;
    Animator animator;
    PlayerHealth playerHealth;
    PlayerHealthBar playerHealthBar;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        bodyCollider = GetComponent<Collider2D>();
        animator = GetComponent<Animator>();
        playerHealth = GetComponent<PlayerHealth>();

        if (playerHealth == null)
        {
            playerHealth = gameObject.AddComponent<PlayerHealth>();
        }

        playerHealthBar = GetComponent<PlayerHealthBar>();
        if (playerHealthBar == null)
        {
            playerHealthBar = gameObject.AddComponent<PlayerHealthBar>();
        }
    }

    void Start()
    {
        if (playerHealth != null)
        {
            playerHealth.HealthDepleted += OnPlayerDeath;
            playerHealth.DamageTaken += PushPlayerBack;
            playerHealth.DamageTakenDirection += PushPlayerBack;
        }
    }

    private void OnDestroy()
    {
        if (playerHealth != null)
        {
            playerHealth.HealthDepleted -= OnPlayerDeath;
            playerHealth.DamageTaken -= PushPlayerBack;
            playerHealth.DamageTakenDirection -= PushPlayerBack;
        }
    }

    private void OnPlayerDeath()
    {
        if (GameManager.instance != null)
        {
            canMove = false;
            playerAudio.PlayOneShot(dieSound);
            GameManager.instance.LoseGame();
        }
    }

    void Update()
    {
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            TryAttack();
        }

        if (transform.position.y < fallThreshold && playerHealth != null && !playerHealth.IsDead)
        {
            playerHealth.SetHealth(0);
        }

        bool currentlyGrounded = CheckIfGrounded();
        if (currentlyGrounded != _isGrounded)
        {
            IsGrounded = currentlyGrounded;
        }
    }

    private void FixedUpdate()
    {
        if (canMove)
        {
            rb.linearVelocity = new Vector2(moveInput.x * CurrentMoveSpeed, rb.linearVelocity.y);
        }
    }

    private void UpdateAnimationState()
    {
        if (animator == null) return;

        if (isHit)
        {
            animator.CrossFade("player_hit", 0);
        }
        else if (isAttacking)
        {
            animator.CrossFade(_isGrounded ? "player_attack_ground" : "player_attack_air", 0);
        }
        else if (!_isGrounded)
        {
            animator.CrossFade("player_jump", 0);
        }
        else if (_isMoving)
        {
            animator.CrossFade(_isRunning ? "player_run" : "player_walk", 0);
        }
        else
        {
            animator.CrossFade("player_idle", 0);
        }
    }

    private void PushPlayerBack()
    {
        StartCoroutine(PushPlayerBackIE(_isFacingRight ? -transform.right.normalized * 5 : transform.right.normalized * 5));
        playerAudio.PlayOneShot(hurtSound);
    }
    private void PushPlayerBack(Vector3? direction)
    {
        StartCoroutine(PushPlayerBackIE(direction));
        playerAudio.PlayOneShot(hurtSound);
    }

    IEnumerator PushPlayerBackIE(Vector3? direction)
    {
        canMove = false;
        isHit = true;
        UpdateAnimationState();
        yield return SpringAttack.SpringMovementRoutine(rb, (Vector2)direction, attackDashForce, 1);
        canMove = true;
        isHit = false;
        UpdateAnimationState();
    }
    private bool CheckIfGrounded()
    {
        if (bodyCollider == null) return false;

        if (rb.linearVelocity.y > 0.1f) return false;

        Bounds bounds = bodyCollider.bounds;
        Vector2 raycastOrigin = new Vector2(bounds.center.x, bounds.min.y);
        Vector2 raycastSize = new Vector2(bounds.size.x * 0.9f, 0.1f);

        RaycastHit2D hit = Physics2D.BoxCast(raycastOrigin, raycastSize, 0f, Vector2.down, 0.1f, groundLayer);

        return hit.collider != null;
    }

    private void SetFacingDirection(Vector2 moveInput)
    {
        if (moveInput.x > 0 && !IsFacingRight)
        {
            IsFacingRight = true;
        }
        else if (moveInput.x < 0 && IsFacingRight)
        {
            IsFacingRight = false;
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
        IsMoving = !Mathf.Approximately(moveInput.x, 0f);
        SetFacingDirection(moveInput);
    }

    public void OnRun(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            IsRunning = true;
        }
        else if (context.canceled)
        {
            IsRunning = false;
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started && bodyCollider.IsTouchingLayers())
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpPower);
            IsGrounded = false;
        }
    }

    private void TryAttack()
    {
        if (Time.time < nextAttackTime || playerHealth.IsDead)
        {
            return;
        }

        nextAttackTime = Time.time + attackCooldown;

        isAttacking = true;

        playerAudio.PlayOneShot(attackSound);
        UpdateAnimationState();

        StartCoroutine(ResetAttackFlag(attackCooldown * 0.85f));
    }

    private IEnumerator ResetAttackFlag(float delay)
    {
        canMove = false;

        Vector2 dashDirection = _isFacingRight ? transform.right : -transform.right;

        yield return StartCoroutine(SpringAttack.SpringMovementRoutine(rb, dashDirection, attackDashForce, delay));

        isAttacking = false;
        canMove = true;
        UpdateAnimationState();
    }

    public void ExecuteBoxCastAttack()
    {
        float directionSign = IsFacingRight ? 1f : -1f;
        Vector2 attackOrigin = (Vector2)transform.position + new Vector2(attackRange * directionSign, 0f);

        RaycastHit2D[] hits = Physics2D.BoxCastAll(attackOrigin, attackBoxSize, 0f, Vector2.zero, 0f, enemyLayer);

        foreach (RaycastHit2D hit in hits)
        {
            Monster monster = hit.collider.GetComponent<Monster>();
            if (monster == null)
            {
                monster = hit.collider.GetComponentInParent<Monster>();
            }

            if (monster != null && !monster.IsDead)
            {
                monster.TakeDamage(attackDamage, IsFacingRight ? Vector2.right : Vector2.left);
            }
        }
    }

    public void Respawn(Vector3 position, int health)
    {
        transform.position = position;

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }

        canMove = true;
        isAttacking = false;
        isHit = false;

        if (playerHealth != null)
        {
            playerHealth.SetHealth(health);
        }

        if (bodyCollider != null)
        {
            bodyCollider.enabled = true;
        }

        UpdateAnimationState();
    }
    private void OnDrawGizmosSelected()
    {
        float directionSign = IsFacingRight ? 1f : -1f;
        Vector2 attackOrigin = (Vector2)transform.position + new Vector2(attackRange * directionSign, 0f);

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(attackOrigin, attackBoxSize);
    }


    /*private GhostRider1 FindAttackTarget()
    {
        Vector2 attackOrigin = (Vector2)transform.position + Vector2.right * (IsFacingRight ? attackRange * 0.5f : -attackRange * 0.5f);
        Collider2D[] hits = Physics2D.OverlapCircleAll(attackOrigin, attackRange);

        foreach (Collider2D hit in hits)
        {
            GhostRider1 ghost = hit.GetComponent<GhostRider1>();
            if (ghost == null)
            {
                ghost = hit.GetComponentInParent<GhostRider1>();
            }

            if (ghost != null)
            {
                return ghost;
            }
        }

        GhostRider1[] ghosts = FindObjectsByType<GhostRider1>();
        foreach (GhostRider1 ghost in ghosts)
        {
            if (CanHitGhost(ghost))
            {
                return ghost;
            }
        }

        return null;
    }

    private bool CanHitGhost(GhostRider1 ghost)
    {
        if (ghost == null || ghost.IsDead)
        {
            return false;
        }

        Bounds playerBounds = bodyCollider.bounds;
        Bounds ghostBounds = ghost.BodyBounds;
        float direction = IsFacingRight ? 1f : -1f;
        float horizontalDirection = Mathf.Sign(ghostBounds.center.x - playerBounds.center.x);

        if (!Mathf.Approximately(horizontalDirection, 0f) && horizontalDirection != direction)
        {
            return false;
        }

        float ghostEdge = IsFacingRight ? ghostBounds.min.x : ghostBounds.max.x;
        float horizontalDistance = Mathf.Abs(ghostEdge - playerBounds.center.x);
        float allowedVerticalDistance = playerBounds.extents.y + ghostBounds.extents.y + 0.35f;
        float verticalDistance = Mathf.Abs(ghostBounds.center.y - playerBounds.center.y);

        return horizontalDistance <= attackRange && verticalDistance <= allowedVerticalDistance;
    }*/
}