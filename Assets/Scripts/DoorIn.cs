using UnityEngine;

public class DoorIn : MonoBehaviour
{
    private Animator MyAnimator => GetComponent<Animator>();
    private static readonly int IdOpenDoor = Animator.StringToHash("OpenDoor");
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(!GameManager.Instance.hasCheckpointActive) return; //Si no hay checkpoint activo no entra en la puerta
        if(!collision.CompareTag("Player"))return;
        MyAnimator.SetTrigger(IdOpenDoor);
        collision.GetComponent<PlayerController>().DoorIn();
        collision.transform.position = new Vector3(transform.position.x, collision.transform.position.y, collision.transform.position.z); //Centrar al player en x en la puerta
    }
}
