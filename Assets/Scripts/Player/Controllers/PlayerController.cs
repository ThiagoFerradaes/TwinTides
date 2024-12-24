using System;
using System.Collections;
using Unity.Cinemachine;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour {

    [Header("Camera")]
    [SerializeField] CinemachineCamera cameraCineMachine;
    CinemachineInputAxisController _cameraInputController;
    CinemachineOrbitalFollow _cameraOrbital;

    [Header("Movement")]
    [SerializeField] float characterMoveSpeed;
    [SerializeField] float sprintSpeed;
    [SerializeField] float rotationSpeed;
    float _currentCharacterMoveSpeed;
    bool _canWalk = true;
    float _rotationY;
    Vector2 _moveInput;
    Vector2 _rotationInput;

    [Header("Jump")]
    [SerializeField] float jumpForce;
    [SerializeField] float fallForce;
    [SerializeField] int maxJumps;
    [SerializeField] LayerMask floorLayer;
    int _currentJumpsAlowed;

    [Header("Dash")]
    [SerializeField] float dashForce;
    [SerializeField] float dashDuration;
    [SerializeField] float dashCooldown;
    bool _inDash;

    Rigidbody _rb;
    void Start() {
        _cameraInputController = cameraCineMachine.GetComponent<CinemachineInputAxisController>();
        _cameraOrbital = cameraCineMachine.GetComponent<CinemachineOrbitalFollow>();
        _rb = GetComponent<Rigidbody>();
        _currentCharacterMoveSpeed = characterMoveSpeed;
    }
    public void InputMove(InputAction.CallbackContext context) {
        if (context.phase == InputActionPhase.Performed && _canWalk) {
            _moveInput = context.ReadValue<Vector2>();
        }
        else if (context.phase == InputActionPhase.Canceled) {
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
    public void InputJump(InputAction.CallbackContext context) {
        if (context.phase == InputActionPhase.Performed && _currentJumpsAlowed > 0) {
            _currentJumpsAlowed--;
            _rb.linearVelocity = new(_rb.linearVelocity.x, 0, _rb.linearVelocity.z);
            _rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }
    public void InputSprint(InputAction.CallbackContext context) {
        if (context.phase == InputActionPhase.Started) {
            _currentCharacterMoveSpeed = sprintSpeed;
        }
        else if (context.phase == InputActionPhase.Canceled) {
            _currentCharacterMoveSpeed = characterMoveSpeed;
        }
    }
    public void InputDash(InputAction.CallbackContext context) {
        if (context.phase == InputActionPhase.Performed && !_inDash) {
            StartCoroutine(DashCoroutine());
        }
    }
    IEnumerator DashCoroutine() {
        _inDash = true;
        _canWalk = false;
        float startTime = Time.time;

        while (Time.time - startTime < dashDuration) {
            if (_moveInput.magnitude >= 0.1f) {
                Vector3 moveDirection = new Vector3(_moveInput.x, 0, _moveInput.y).normalized;
                transform.Translate(dashForce * Time.deltaTime * moveDirection);
                yield return null;
            }
            else {
                transform.Translate(dashForce * Time.deltaTime * Vector3.forward);
                yield return null;
            }
        }

        _canWalk = true;

        yield return new WaitForSeconds(dashCooldown);
        _inDash = false;

    }
    void FixedUpdate() {
        MoveAndRotate();
    }
    private void MoveAndRotate() {
        if (_moveInput.magnitude >= 0.1f) {
            // Movimentação
            Vector3 moveDirection = new Vector3(_moveInput.x, 0, _moveInput.y).normalized;
            transform.Translate(_currentCharacterMoveSpeed * Time.deltaTime * moveDirection);

            // Rotação de camera
            _cameraInputController.enabled = false;
            _cameraOrbital.HorizontalAxis.TriggerRecentering();

            // Rotação do personagem
            _rotationY += _rotationInput.x * rotationSpeed * Time.deltaTime;
            transform.rotation = Quaternion.Euler(0, _rotationY, 0);
        }
        else {
            _cameraInputController.enabled = true;
        }


        if (_rb.linearVelocity.y != 0) { // Durante o pulo aumenta a gravidade, serve para regular a duração do pulo
            _rb.AddForce(Vector3.down * fallForce, ForceMode.Acceleration);
        }
    }
    private void OnCollisionEnter(Collision collision) {
        if (((1 << collision.gameObject.layer) & floorLayer.value) != 0) {
            _currentJumpsAlowed = maxJumps;
        }
    }
}
