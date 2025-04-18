using Unity.Cinemachine;
using UnityEngine;

//Ejemplo de como mover la camara mediante código con Cinemachine

public class CustomCameraOffset : MonoBehaviour
{
    public CinemachineCamera CinemachineCamera;
    public CinemachinePositionComposer PositionComposer;


    private void Start()
    {
        PositionComposer = CinemachineCamera.GetComponent<CinemachinePositionComposer>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PositionComposer.Damping.x = 2;
        PositionComposer.Damping.y = 2;
        PositionComposer.TargetOffset.y = 1.5f;
        PositionComposer.TargetOffset.x = 5f;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        PositionComposer.TargetOffset.y = 0;
        PositionComposer.TargetOffset.x = 0;
        PositionComposer.Damping.x = 0.5f;
        PositionComposer.Damping.y = 0.8f;
    }
}
