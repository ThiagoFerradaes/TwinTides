using System;
using Unity.Cinemachine;
using UnityEngine;

public class PlayerCamera : MonoBehaviour {
    CinemachineCamera _cameraCineMachine;
    CinemachineInputAxisController _cameraInputController;
    CinemachineOrbitalFollow _cameraOrbital;

    void Start() {
        _cameraCineMachine = GetComponent<CinemachineCamera>();
        _cameraInputController = GetComponent<CinemachineInputAxisController>();
        _cameraOrbital = GetComponent<CinemachineOrbitalFollow>();
    }

    private void OnEnable() {
        PlayerSetUp.OnPlayerSpawned += SetFollowTarget;
        PlayerController.OnMove += PlayerMoving;
        PlayerController.OnStop += PlayerStoped;
    }
    private void OnDisable() {
        PlayerSetUp.OnPlayerSpawned -= SetFollowTarget;
        PlayerController.OnMove -= PlayerMoving;
        PlayerController.OnStop -= PlayerStoped;
    }

    private void SetFollowTarget(GameObject target) {
        _cameraCineMachine.Follow = target.transform;
    }
    private void PlayerMoving() {
        _cameraInputController.enabled = false;
        _cameraOrbital.HorizontalAxis.TriggerRecentering();
    }
    void PlayerStoped() {
        _cameraInputController.enabled = true;
        _cameraOrbital.HorizontalAxis.TriggerRecentering();
    }
}
