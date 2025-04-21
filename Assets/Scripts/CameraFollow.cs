using UnityEngine;
using Unity.Cinemachine;

public class CameraFollow : MonoBehaviour
{
    public CinemachineCamera CinemachineCam;

    private void Awake()
    {
        CinemachineCam = GetComponent<CinemachineCamera>();
    }

    private void OnEnable()
    {
        GameManager.OnPlayerRespawned += AssignTrackingTarget;
    }

    private void OnDisable()
    {
        GameManager.OnPlayerRespawned -= AssignTrackingTarget;
    }

    private void AssignTrackingTarget(Transform target)
    {
        if (CinemachineCam != null)
        {
            CinemachineCam.Target.TrackingTarget = target;
        }
    }
}

