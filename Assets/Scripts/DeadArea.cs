using UnityEngine;

public class DeadArea : MonoBehaviour
{
    [SerializeField] private PlayerController player;

    
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player")) player.Die();
    }
}
