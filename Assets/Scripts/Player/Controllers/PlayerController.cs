using System;
using System.Collections;
using Unity.Cinemachine;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class PlayerController : NetworkBehaviour {

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

    // eventos 
    public static event Action OnMove;
    public static event Action OnStop;
    public static event Action OnPause;
    public static event Action<SkillType,float> OnDashCooldown;

    void Start() {
        _rb = GetComponent<Rigidbody>();
        _currentCharacterMoveSpeed = characterMoveSpeed;
        _currentJumpsAlowed = maxJumps;
    }
    public void InputMenuInGame(InputAction.CallbackContext context) {
        if (context.phase == InputActionPhase.Performed) {
            OnPause?.Invoke();
        }
    }
    public void InputMove(InputAction.CallbackContext context) {
        if (context.phase == InputActionPhase.Performed && _canWalk && Time.timeScale == 1) {
            _moveInput = context.ReadValue<Vector2>();
            OnMove?.Invoke();
        }
        else if (context.phase == InputActionPhase.Canceled) {
            _moveInput = Vector2.zero;
            OnStop?.Invoke();
        }
    }
    public void InputRotate(InputAction.CallbackContext context) {
        if (context.phase == InputActionPhase.Performed && Time.timeScale == 1) {
            _rotationInput = context.ReadValue<Vector2>();
        }
        else if (context.phase == InputActionPhase.Canceled) {
            _rotationInput = Vector2.zero;
        }
    }
    public void InputJump(InputAction.CallbackContext context) {
        if (context.phase == InputActionPhase.Performed && _currentJumpsAlowed > 0 && Time.timeScale == 1) {
            _currentJumpsAlowed--;
            _rb.linearVelocity = new(_rb.linearVelocity.x, 0, _rb.linearVelocity.z);
            _rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }
    public void InputSprint(InputAction.CallbackContext context) {
        if (context.phase == InputActionPhase.Started && Time.timeScale == 1) {
            _currentCharacterMoveSpeed = sprintSpeed;
        }
        else if (context.phase == InputActionPhase.Canceled) {
            _currentCharacterMoveSpeed = characterMoveSpeed;
        }
    }
    public void InputDash(InputAction.CallbackContext context) {
        if (context.phase == InputActionPhase.Performed && !_inDash && Time.timeScale == 1) {
            StartCoroutine(DashCoroutine());
        }
    }
    IEnumerator DashCoroutine() {
        _inDash = true;
        _canWalk = false;
        float startTime = Time.time;
        OnDashCooldown?.Invoke(SkillType.Dash, dashCooldown);

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

        yield return new WaitForSeconds(dashCooldown - dashDuration);
        _inDash = false;

    }
    void FixedUpdate() {
        if (!IsOwner) return;
        MoveAndRotate();
    }
    private void MoveAndRotate() {
        if (_moveInput.magnitude != 0) {

            // Movimentação
            Vector3 moveDirection = new Vector3(_moveInput.x, 0, _moveInput.y).normalized;
            transform.Translate(_currentCharacterMoveSpeed * Time.deltaTime * moveDirection);

            // Rotação do personagem
            _rotationY += _rotationInput.x * rotationSpeed * Time.deltaTime;
            transform.rotation = Quaternion.Euler(0, _rotationY, 0);
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
