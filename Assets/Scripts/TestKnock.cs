using UnityEngine;

public class TestKnock : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            collision.GetComponent<PlayerController>().Knockback();
        }
        
    }
}
