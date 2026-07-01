using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Vector2 playerPos = Vector2.zero;
    public int playerHealth = 0;
    bool saved = false;

    public AudioSource audio;
    public AudioClip clip;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            if (spriteRenderer != null && !saved)
            {
                spriteRenderer.color = Color.white;
                playerPos = collision.transform.position;
                playerHealth = collision.GetComponent<PlayerHealth>().CurrentHealth;
                saved = true;
                audio.PlayOneShot(clip);
                GameManager.instance.SetCheckPoint(this);
            }
        }
    }
}
