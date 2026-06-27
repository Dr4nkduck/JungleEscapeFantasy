using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]

public class PlayerController : MonoBehaviour
{
    public float walkSpeed = 5f;
    public float runSpeed = 8f;
    public float jumpPower = 10f;

    [SerializeField]
    private int attackDamage = 25;

    [SerializeField]
    private float attackRange = 1.25f;

    [SerializeField]
    private float attackCooldown = 0.35f;

    [SerializeField]
    private float fallThreshold = -10f;

    Vector2 moveInput;
    float nextAttackTime;

    public float CurrentMoveSpeed
    {
        get
        {
            if (IsMoving)
            {
                if (IsRunning)
                {
                    return runSpeed;

                }
                else
                {
                    return walkSpeed;
                }
            }
            else
            {
                return 0;
            }


        }
    }

    [SerializeField]
    private bool _isMoving = false;

    public bool IsMoving { get { return _isMoving; } private set { _isMoving = value; animator.SetBool(AnimationStrings.isMoving, value); } }


    [SerializeField]
    private bool _isRunning = false;

    public bool IsRunning { get { return _isRunning; } private set { _isRunning = value; animator.SetBool(AnimationStrings.isRunning, value); } }


    public bool _isFacingRight = true;
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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (playerHealth != null)
        {
            playerHealth.HealthDepleted += OnPlayerDeath;
        }
    }

    private void OnDestroy()
    {
        if (playerHealth != null)
        {
            playerHealth.HealthDepleted -= OnPlayerDeath;
        }
    }

    private void OnPlayerDeath()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.LoseGame();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            TryAttack();
        }

        if (transform.position.y < fallThreshold && playerHealth != null && !playerHealth.IsDead)
        {
            playerHealth.SetHealth(0);
        }
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(moveInput.x * CurrentMoveSpeed * Time.fixedDeltaTime, rb.linearVelocity.y);
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
        }
    }

    private void TryAttack()
    {
        if (Time.time < nextAttackTime || playerHealth.IsDead)
        {
            return;
        }

        nextAttackTime = Time.time + attackCooldown;
        GhostRider1 target = FindAttackTarget();
        if (target != null)
        {
            target.TakeDamage(attackDamage);
        }
    }

    private GhostRider1 FindAttackTarget()
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
    }
}
