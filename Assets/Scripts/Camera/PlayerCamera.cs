using System;
using Unity.Cinemachine;
using UnityEngine;

public class PlayerCamera : MonoBehaviour {
    CinemachineCamera _cameraCineMachine;

    void Start() {
        _cameraCineMachine = GetComponent<CinemachineCamera>();
    }

    private void OnEnable() {
        PlayerSetUp.OnPlayerSpawned += SetFollowTarget;
    }
    private void OnDisable() {
        PlayerSetUp.OnPlayerSpawned -= SetFollowTarget;
    }

    private void SetFollowTarget(GameObject target) {
        _cameraCineMachine.Follow = target.transform;
    }
}
