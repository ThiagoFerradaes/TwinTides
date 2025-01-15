using System;
using Unity.Cinemachine;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    CinemachineCamera _cameraCineMachine;
    CinemachineInputAxisController _cameraInputController;
    CinemachineOrbitalFollow _cameraOrbital;
    void Start()
    {
        _cameraCineMachine = GetComponent<CinemachineCamera>();
        _cameraInputController = GetComponent<CinemachineInputAxisController>();
        _cameraOrbital = GetComponent<CinemachineOrbitalFollow>();
    }

    private void OnEnable() {
        PlayerSetUp.OnPlayerSpawned += SetFollowTarget;
    }
    private void OnDisable() {
        PlayerSetUp.OnPlayerSpawned -= SetFollowTarget;
    }

    private void SetFollowTarget(GameObject target) {
        Debug.Log("Set follow to: " + target.name);
        _cameraCineMachine.Follow = target.transform;
    }
}
