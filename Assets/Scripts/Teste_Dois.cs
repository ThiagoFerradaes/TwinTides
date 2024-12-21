using System.Threading;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class Teste_Dois : MonoBehaviour
{
    // Movimento
    public float CharacterMoveSpeed;
    public float RotationSpeed;
    float _rotationY;
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

    public void InputMove(InputAction.CallbackContext context) {
        if (context.phase == InputActionPhase.Performed) 
    {
            _moveInput = context.ReadValue<Vector2>();
        }
        else if (context.phase == InputActionPhase.Canceled) 
        {
            _moveInput = Vector2.zero; 
        }
    }
    public void InputRotate(InputAction.CallbackContext context) {
        if (context.phase == InputActionPhase.Performed) {
            _rotationInput = context.ReadValue<Vector2>();
        }
        else if (context.phase == InputActionPhase.Canceled) {
            _rotationInput = Vector2.zero;
        }
    }


    private void FixedUpdate() {
        if (_moveInput.magnitude >= 0.1f) {
            // Movimentação
            Vector3 moveDirection = new Vector3(_moveInput.x, 0, _moveInput.y).normalized;
            transform.Translate(CharacterMoveSpeed * Time.deltaTime * moveDirection);

            // Rotação de camera
            _cameraInputController.enabled = false;
            _cameraOrbital.HorizontalAxis.TriggerRecentering();

            // Rotação do personagem
            _rotationY += _rotationInput.x * RotationSpeed * Time.deltaTime;
            transform.rotation = Quaternion.Euler(0, _rotationY, 0);
        }
        else {
            _cameraInputController.enabled = true;
        }
    }
}
