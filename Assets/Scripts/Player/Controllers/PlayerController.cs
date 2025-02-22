using System;
using System.Collections;
using Unity.Cinemachine;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class PlayerController : NetworkBehaviour {

    [Header("Movement")]
    [SerializeField] float rotationSpeed;
    bool _canWalk = true;
    bool _canRotate = true;
    Vector2 _moveInput;
    [HideInInspector] public Vector2 _rotationInput;
    [HideInInspector] public Transform aimObject;
    [HideInInspector] public bool isRotatingMouse;
    [HideInInspector] public bool isAiming;
    MovementManager _mManager;

    [Header("Jump")]
    [SerializeField] float jumpForce;
    [SerializeField] float fallForce;
    [SerializeField] int maxJumps;
    public LayerMask floorLayer;
    int _currentJumpsAlowed;
    bool _canJump = true;

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
    public event Action<SkillType, float> OnDashCooldown;
    public event EventHandler OnAim;

    void Start() {
        _rb = GetComponent<Rigidbody>();
        _mManager = GetComponent<MovementManager>();
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
    public void InputRotateMouse(InputAction.CallbackContext context) {
        if (context.phase == InputActionPhase.Performed && Time.timeScale == 1) {
            isRotatingMouse = true;
        }
    }
    public void InputRotateController(InputAction.CallbackContext context) {
        if (context.phase == InputActionPhase.Performed && Time.timeScale == 1) {
            isRotatingMouse = false;
            isAiming = true;
            _rotationInput = context.ReadValue<Vector2>();

            if (!aimObject.gameObject.activeSelf && aimObject != null) {
                StartAimMode();
            }
        }
        else if (context.phase == InputActionPhase.Canceled) {
            isAiming = false;
            _rotationInput = Vector2.zero;
        }
    }
    public void InputJump(InputAction.CallbackContext context) {
        if (context.phase == InputActionPhase.Performed && _currentJumpsAlowed > 0 && Time.timeScale == 1 && _canJump) {
            _currentJumpsAlowed--;
            _rb.linearVelocity = new(_rb.linearVelocity.x, 0, _rb.linearVelocity.z);
            _rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }
    public void InputDash(InputAction.CallbackContext context) {
        if (context.phase == InputActionPhase.Performed && !_inDash && Time.timeScale == 1) {
            StartCoroutine(DashCoroutine());
        }
    }
    public void InputAimMode(InputAction.CallbackContext context) {
        if (context.phase == InputActionPhase.Started && Time.timeScale == 1) {
            if (isAiming) {
                isAiming = false;
                StopAimMode();
            }
            else {
                StartAimMode();
                isAiming = true;
            }
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
                transform.position += (dashForce * Time.deltaTime * moveDirection);
                yield return null;
            }
            else {
                transform.position += (dashForce * Time.deltaTime * transform.forward);
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

            Moving();

        }

        Rotate();

        if (_rb.linearVelocity.y != 0) { 
            _rb.AddForce(Vector3.down * fallForce, ForceMode.Acceleration);
        }
    }
    private void OnCollisionEnter(Collision collision) {
        if (((1 << collision.gameObject.layer) & floorLayer.value) != 0) {
            _currentJumpsAlowed = maxJumps;
        }
    }

    void Moving() {
        if (!_canWalk) return;
        Vector3 moveDirection = new Vector3(_moveInput.x, 0, _moveInput.y).normalized;
        transform.position += (_mManager.ReturnMoveSpeed() * Time.deltaTime * moveDirection);
    }

    public void Rotate() {
        if (!_canRotate) return;
        if (aimObject != null && aimObject.gameObject.activeInHierarchy) {
            Vector3 objectPosition = new(aimObject.position.x, transform.position.y, aimObject.position.z);
            Vector3 aimDirection = (objectPosition - transform.position);

            if (aimDirection.sqrMagnitude > 0.01f) {
                Quaternion direction = Quaternion.LookRotation(aimDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, direction, Time.deltaTime * rotationSpeed);
            }
        }
        else {
            if (_moveInput.sqrMagnitude > 0.01f) {
                float angle = Mathf.Atan2(_moveInput.x, _moveInput.y) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0, angle, 0);
            }
        }
    }

    public void StartAimMode() {
        if (aimObject == null) return;
        aimObject.gameObject.SetActive(true);
        OnAim?.Invoke(this, EventArgs.Empty);
    }
    public void StopAimMode() {
        if (aimObject == null) return;
        aimObject.gameObject.SetActive(false);
    }

    public void BlockMovement() {
        _inDash = true;
        _canWalk = false;
        _canRotate = false;
        _canJump = false;
    }

    public void AllowMovement() {
        _inDash = false;
        _canWalk = true;
        _canRotate = true;
        _canJump = true;
    }
}
