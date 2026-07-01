using UnityEngine;

public class DeadZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            PlayerHealth health = collision.GetComponent<PlayerHealth>();

            if (!health.IsDead) {
                health.TakeDamage(999);
            }
        }
    }
}
