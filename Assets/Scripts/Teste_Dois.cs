using System.Collections;
using System.Threading;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class Teste_Dois : MonoBehaviour
{
    [Header("Movimento")]
    public float CharacterMoveSpeed;
    public float SprintSpeed;
    public float RotationSpeed;
    bool _canWalk;

    [Header("Pulo")]
    public float JumpForce;
    public LayerMask FloorLayer;
    public int MaxJumps;
    public float FallForce;

    [Header("Dash")]
    public float DashForce;
    public float DashCooldown;
    public float DashDuration;
    bool _inDash;


    float _currentCharacterMoveSpeed;
    float _rotationY;
    Vector2 _moveInput;
    Vector2 _rotationInput;
    
    int _currentJumpsAlowed;
    Rigidbody rb;

    // Camera
    public CinemachineCamera Camera;
    CinemachineInputAxisController _cameraInputController;
    CinemachineOrbitalFollow _cameraOrbital;


    private void Start() {
        _cameraInputController = Camera.GetComponent<CinemachineInputAxisController>();
        _cameraOrbital = Camera.GetComponent<CinemachineOrbitalFollow>();
        rb = GetComponent<Rigidbody>();

        _currentCharacterMoveSpeed = CharacterMoveSpeed;
    }

    public void InputMove(InputAction.CallbackContext context) {
        if (context.phase == InputActionPhase.Performed && _canWalk) 
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
    public void InputJump(InputAction.CallbackContext context) {
        if (context.phase == InputActionPhase.Performed && _currentJumpsAlowed > 0) {
            _currentJumpsAlowed--;
            rb.linearVelocity = new(rb.linearVelocity.x, 0, rb.linearVelocity.z);
            rb.AddForce(Vector3.up * JumpForce, ForceMode.Impulse);
        }
    }
    public void InputSprint(InputAction.CallbackContext context) {
        if (context.phase == InputActionPhase.Started) {
            _currentCharacterMoveSpeed = SprintSpeed;
        }
        else if (context.phase == InputActionPhase.Canceled){
            _currentCharacterMoveSpeed = CharacterMoveSpeed;
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

        while(Time.time - startTime < DashDuration) {
            if (_moveInput.magnitude >= 0.1f) {
                Vector3 moveDirection = new Vector3(_moveInput.x, 0, _moveInput.y).normalized;
                transform.Translate(DashForce * Time.deltaTime * moveDirection);
                yield return null;
            }
            else {
                transform.Translate(DashForce * Time.deltaTime * Vector3.forward);
                yield return null;
            }
        }

        _canWalk = true;

        yield return new WaitForSeconds(DashCooldown);
        _inDash = false;

    }
    private void FixedUpdate() {
        if (_moveInput.magnitude >= 0.1f) {
            // Movimentação
            Vector3 moveDirection = new Vector3(_moveInput.x, 0, _moveInput.y).normalized;
            transform.Translate(_currentCharacterMoveSpeed * Time.deltaTime * moveDirection);

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

        
        if (rb.linearVelocity.y != 0) { // Durante o pulo aumenta a gravidade, serve para regular a duração do pulo
            rb.AddForce(Vector3.down * FallForce, ForceMode.Acceleration);
        }

    }

    private void OnCollisionEnter(Collision collision) {
        if (((1 << collision.gameObject.layer) & FloorLayer.value) != 0) {
            _currentJumpsAlowed = MaxJumps;
        }
    }
}
