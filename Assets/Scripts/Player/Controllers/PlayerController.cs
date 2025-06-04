using FMOD.Studio;
using FMODUnity;
using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerController : NetworkBehaviour {

    #region Variables
    [Header("Movement")]
    [SerializeField] float rotationSpeed;

    [Header("Aim")]
    public LayerMask FloorLayer;
    [HideInInspector] public Transform aimObject;

    [Header("Dash")]
    [SerializeField] float dashForce;
    [SerializeField] float dashDuration;
    [SerializeField] float dashCooldown;
    [SerializeField] EventReference dashSound;
    EventInstance dashSoundInstance;

    // booleanas
    bool _canWalk = true;
    bool _canRotate = true;
    bool _canDash = true;
    bool _inDash = false;

    [HideInInspector] public bool isRotatingMouse;
    [HideInInspector] public bool isAiming;
    [HideInInspector] public bool CanInteract;
    [HideInInspector] public Vector2 _rotationInput;

    // Componentes
    Vector2 _moveInput;
    MovementManager _mManager;
    Rigidbody _rb;

    // eventos 
    public static event Action OnMove; // Esses dois aqui são pra camera
    public static event Action OnStop;
    public static event Action OnPause;
    public event Action<SkillType, float> OnDashCooldown;
    public event System.EventHandler OnAim;
    public event System.EventHandler OnInteractInGame;
    public static event System.EventHandler OnInteractOutGame;

    #endregion

    #region Initialize and Update
    void Start() {
        _mManager = GetComponent<MovementManager>();
        _rb = GetComponent<Rigidbody>();

    }
    void FixedUpdate() {
        if (!IsOwner) return;
        if (LocalWhiteBoard.Instance.AnimationOn) return;
        MoveAndRotate();

    }
    #endregion

    #region Audio

    #endregion

    #region Inputs
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

        if (!CanDetectInputs()) return;

        if (context.phase == InputActionPhase.Performed && _canWalk) {
            _moveInput = context.ReadValue<Vector2>();
            OnMove?.Invoke();
        }
        else if (context.phase == InputActionPhase.Canceled) {
            _moveInput = Vector2.zero;
            OnStop?.Invoke();
        }
    }
    public void InputRotateMouse(InputAction.CallbackContext context) {

        if (!CanDetectInputs()) return;

        if (context.phase == InputActionPhase.Performed) {
            isRotatingMouse = true;
        }
    }
    public void InputRotateController(InputAction.CallbackContext context) {

        if (!CanDetectInputs()) return;

        if (context.phase == InputActionPhase.Performed) {
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

        if (!CanDetectInputs()) return;

        if (context.phase == InputActionPhase.Performed) {
            Dash();
        }
    }
    public void InputAimMode(InputAction.CallbackContext context) {

        if (!CanDetectInputs()) return;

        if (context.phase == InputActionPhase.Started) {
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

    bool CanDetectInputs() {
        return (!LocalWhiteBoard.Instance.AnimationOn && Time.timeScale == 1);
    }

    #endregion

    #region Dash

    void Dash() {
        if (!_canDash || _inDash) return;

        StartCoroutine(DashCoroutine());
    }
    IEnumerator DashCoroutine() {
        BlockMovement();

        _inDash = true;

        float startTime = Time.time;
        OnDashCooldown?.Invoke(SkillType.Dash, dashCooldown);

        Vector3 moveDirection = (_moveInput.magnitude >= 0.1f) ? new Vector3(_moveInput.x, 0, _moveInput.y).normalized : transform.forward;

        _rb.linearVelocity = moveDirection * dashForce;

        OnDashCooldown?.Invoke(SkillType.Dash, dashCooldown);

        if (!dashSound.IsNull) {
            dashSoundInstance = RuntimeManager.CreateInstance(dashSound);
            RuntimeManager.AttachInstanceToGameObject(dashSoundInstance, this.gameObject);
            dashSoundInstance.start();
        }

        float timer = 0f;
        while (timer < dashDuration && !_mManager.ReturnStunnedValue()) {
            timer += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        if (dashSoundInstance.isValid()) {
            dashSoundInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            dashSoundInstance.release();
        }

        _rb.linearVelocity = new(0f, _rb.linearVelocity.y, 0f);

        AllowMovement();

        yield return new WaitForSeconds(dashCooldown - dashDuration);
        _inDash = false;
    }


    #endregion

    #region MoveAndaRotate


    private void MoveAndRotate() {
        if (_rb.linearVelocity.y < 0) {
            _rb.linearVelocity += Vector3.up * Physics.gravity.y * (2) * Time.deltaTime;
        }

        if (_mManager.ReturnStunnedValue()) { _rb.linearVelocity = new(0f, _rb.linearVelocity.y, 0f); return; }

        Moving();

        Rotate();

    }

    void Moving() {
        if (_moveInput == (Vector2.zero) || !_canWalk) {
            if (!_inDash) _rb.linearVelocity = new(0f, _rb.linearVelocity.y, 0f);
            return;
        }

        Vector3 moveDirection = new Vector3(_moveInput.x, 0, _moveInput.y).normalized;
        Vector3 velocity = moveDirection * _mManager.ReturnMoveSpeed();
        velocity.y = _rb.linearVelocity.y;
        _rb.linearVelocity = velocity;
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

    #endregion

    #region Aim
    public void StartAimMode() {
        if (aimObject == null) return;
        aimObject.gameObject.SetActive(true);
        OnAim?.Invoke(this, EventArgs.Empty);
    }
    public void StopAimMode() {
        if (aimObject == null) return;
        aimObject.gameObject.SetActive(false);
    }
    #endregion

    #region Setters
    public void BlockMovement() {
        _canDash = false;
        _canWalk = false;
        _canRotate = false;
    }

    public void AllowMovement() {
        _canDash = true;
        _canWalk = true;
        _canRotate = true;
    }

    public void BlockRotate(bool canRotate) => _canRotate = canRotate;
    #endregion
}
