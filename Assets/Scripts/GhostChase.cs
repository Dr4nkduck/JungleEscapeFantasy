using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class GhostChase : MonoBehaviour
{
    public Transform player;
    public float speed = 3f;
    public float chaseDistance = 5f;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        if (player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject == null)
            {
                playerObject = GameObject.Find("Player");
            }

            if (playerObject != null)
            {
                player = playerObject.transform;
            }
        }
    }

    private void FixedUpdate()
    {
        if (player == null)
        {
            return;
        }

        float distance = Vector2.Distance(transform.position, player.position);
        if (distance > chaseDistance)
        {
            return;
        }

        float direction = Mathf.Sign(player.position.x - transform.position.x);
        rb.linearVelocity = new Vector2(direction * speed, rb.linearVelocity.y);

        if (!Mathf.Approximately(direction, 0f))
        {
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x) * direction;
            transform.localScale = scale;
        }
    }
}
