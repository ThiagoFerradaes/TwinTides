using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class teste : MonoBehaviour {
    public float CharacterMoveSpeed;
    public CinemachineCamera Camera;
    public float RotationSpeed;
    float _rotationY;
    bool _isMoving;

    CinemachineInputAxisController _cameraInputController;
    CinemachineOrbitalFollow _cameraOrbital;
    

    private void Start() {
        _cameraInputController = Camera.GetComponent<CinemachineInputAxisController>();
        _cameraOrbital = Camera.GetComponent<CinemachineOrbitalFollow>();
    }

    private void FixedUpdate() {

        if (Keyboard.current.wKey.isPressed) {
            transform.Translate(CharacterMoveSpeed * Time.deltaTime * Vector3.forward.normalized);
            _isMoving = true;
        }
        else if (Keyboard.current.sKey.isPressed) {
            transform.Translate(CharacterMoveSpeed * Time.deltaTime * Vector3.back.normalized);
            _isMoving = true;
        }
        else if (Keyboard.current.dKey.isPressed) {
            transform.Translate(CharacterMoveSpeed * Time.deltaTime * Vector3.right.normalized);
            _isMoving = true;
        }
        else if (Keyboard.current.aKey.isPressed) {
            transform.Translate(CharacterMoveSpeed * Time.deltaTime * Vector3.left.normalized);
            _isMoving = true;
        }
        else {
            _isMoving = false;
        }

        if (!_isMoving) { // ta parado
            _cameraInputController.enabled = true;
        }
        else { // ta se movendo
            _cameraInputController.enabled = false;
            _cameraOrbital.HorizontalAxis.TriggerRecentering();

            float mouseX = Input.GetAxis("Mouse X");

            _rotationY += mouseX * RotationSpeed * Time.deltaTime;

            transform.rotation = Quaternion.Euler(0, _rotationY, 0);

        }
    }
}
