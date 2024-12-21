using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Teste : MonoBehaviour {

    // Movimento
    public float CharacterMoveSpeed;
    public float RotationSpeed;
    public PlayerInput playerInput;
    float _rotationY;
    bool _isMoving;
    Vector2 _moveInput;
    Vector2 _rotationInput;

    // Camera
    public CinemachineCamera Camera;
    CinemachineInputAxisController _cameraInputController;
    CinemachineOrbitalFollow _cameraOrbital;
    

    private void Start() {
        _cameraInputController = Camera.GetComponent<CinemachineInputAxisController>();
        _cameraOrbital = Camera.GetComponent<CinemachineOrbitalFollow>();
    }

    public void InputMove(Vector2 input) {
        _moveInput = input;
    }
    public void InputRotate(Vector2 input) {
        _rotationInput = input;
    }

    private void FixedUpdate() {

        if (_moveInput.magnitude >= 0.1f) {
            transform.Translate(CharacterMoveSpeed * Time.deltaTime * _moveInput);
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

            _rotationY += _rotationInput.x * RotationSpeed * Time.deltaTime;

            transform.rotation = Quaternion.Euler(0, _rotationY, 0);

        }
    }
}
