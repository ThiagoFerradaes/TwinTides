using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerController : NetworkBehaviour {

    [Header("Movement")]
    [SerializeField] float rotationSpeed;
    bool _canWalk = true;
    bool _canRotate = true;
    Vector2 _moveInput;
    [HideInInspector] public Vector2 _rotationInput;
    public Transform aimObject;
    [HideInInspector] public bool isRotatingMouse;
    [HideInInspector] public bool isAiming;
    MovementManager _mManager;
    public LayerMask FloorLayer;
    public bool CanInteract;

    [Header("Dash")]
    [SerializeField] float dashForce;
    [SerializeField] float dashDuration;
    [SerializeField] float dashCooldown;
    bool _inDash;

    // eventos 
    public static event Action OnMove;
    public static event Action OnStop;
    public static event Action OnPause;
    public event Action<SkillType, float> OnDashCooldown;
    public event EventHandler OnAim;
    public event EventHandler OnInteractInGame;
    public static event EventHandler OnInteractOutGame;

    void Start() {
        _mManager = GetComponent<MovementManager>();
    }
    public void InputInteract(InputAction.CallbackContext context) {
        if (LocalWhiteBoard.Instance.AnimationOn) return;

        if (context.phase == InputActionPhase.Started) {
            if (Time.timeScale == 1 && CanInteract) {
                OnInteractInGame?.Invoke(this, EventArgs.Empty);
            }
            else if (LocalWhiteBoard.Instance.AnimationOn) {
                OnInteractOutGame?.Invoke(this, EventArgs.Empty);
            }
        }
    }
    public void InputMenuInGame(InputAction.CallbackContext context) {
        if (LocalWhiteBoard.Instance.AnimationOn) return;

        if (context.phase == InputActionPhase.Performed) {
            OnPause?.Invoke();
        }
    }
    public void InputMove(InputAction.CallbackContext context) {

        if (LocalWhiteBoard.Instance.AnimationOn) return;

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

        if (LocalWhiteBoard.Instance.AnimationOn) return;

        if (context.phase == InputActionPhase.Performed && Time.timeScale == 1) {
            isRotatingMouse = true;
        }
    }
    public void InputRotateController(InputAction.CallbackContext context) {

        if (LocalWhiteBoard.Instance.AnimationOn) return;

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

    public void InputDash(InputAction.CallbackContext context) {

        if (LocalWhiteBoard.Instance.AnimationOn) return;

        if (context.phase == InputActionPhase.Performed && !_inDash && Time.timeScale == 1) {
            StartCoroutine(DashCoroutine());
        }
    }
    public void InputAimMode(InputAction.CallbackContext context) {

        if (LocalWhiteBoard.Instance.AnimationOn) return;

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
        _canRotate = false;
        float startTime = Time.time;
        OnDashCooldown?.Invoke(SkillType.Dash, dashCooldown);

        while (Time.time - startTime < dashDuration && !_mManager.ReturnStunnedValue()) {
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
        _canRotate = true;

        yield return new WaitForSeconds(dashCooldown - dashDuration);
        _inDash = false;

    }
    void FixedUpdate() {
        if (!IsOwner) return;
        if (LocalWhiteBoard.Instance.AnimationOn) return;
        MoveAndRotate();
    }
    private void MoveAndRotate() {
        if (_mManager.ReturnStunnedValue()) return;
        if (_moveInput.magnitude != 0) {

            Moving();

        }

        Rotate();

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
    }

    public void AllowMovement() {
        _inDash = false;
        _canWalk = true;
        _canRotate = true;
    }

    public void BlockRotate(bool canRotate) {
        _canRotate = canRotate;
    }
}
