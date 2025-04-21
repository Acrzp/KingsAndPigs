using UnityEngine;

public class DeadArea : MonoBehaviour
{
    [SerializeField] private PlayerController player;

    
    void OnTriggerEnter2D(Collider2D collision)
    {
        if(!collision.CompareTag("Player")) return;
        player = collision.gameObject.GetComponent<PlayerController>();
        player.Die();
        GameManager.Instance.RespawnPlayer();
    }
}
