using UnityEngine;

public class Coin : MonoBehaviour
{
    public AudioClip audioClip;
    public int pointValue = 1;

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Enter");
        if(other.gameObject.tag == "Player")
        {
            CoinManager.instance.AddPoint(this, pointValue);
            AudioManager.Instance.PlayAudioClip(audioClip);
            Destroy(this.gameObject);
        }
    } 
}
