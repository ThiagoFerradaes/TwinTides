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
    [SerializeField] float walkingAnimationBaseSpeed = 1.1f;

    [Header("Aim")]
    public LayerMask FloorLayer;
    public Vector3 mousePos;
    public Texture2D aimCursorTexture;
    public Texture2D normalCursorTexture;

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
    [HideInInspector] public bool CanInteract;
    [HideInInspector] public Vector2 _rotationInput;

    // Componentes
    Vector2 _moveInput;
    Vector3 aimDirection;
    MovementManager _mManager;
    Rigidbody _rb;
    Animator anim;

    // eventos 
    public static event Action OnMove; // Esses dois aqui são pra camera
    public static event Action OnStop;
    public static event Action OnPause;
    public event Action<SkillType, float> OnDashCooldown;
    public event System.EventHandler OnInteractInGame;
    public static event System.EventHandler OnInteractOutGame;

    #endregion

    #region Initialize and Update
    void Start() {
        _mManager = GetComponent<MovementManager>();
        _rb = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();

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
            if (Time.timeScale == 0 && LocalWhiteBoard.Instance.IsAiming) ChangeMouseSprite(true);
            else ChangeMouseSprite(false);
            OnPause?.Invoke();
        }
    }
    public void InputMove(InputAction.CallbackContext context) {

        if (context.phase == InputActionPhase.Performed && _canWalk && CanDetectInputs()) {
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

    public void InputDash(InputAction.CallbackContext context) {

        if (!CanDetectInputs()) return;

        if (context.phase == InputActionPhase.Performed) {
            Dash();
        }
    }
    public void InputAimMode(InputAction.CallbackContext context) {

        if (!CanDetectInputs()) return;

        if (context.phase == InputActionPhase.Started) {
            LocalWhiteBoard.Instance.IsAiming = !LocalWhiteBoard.Instance.IsAiming;

            ChangeMouseSprite(LocalWhiteBoard.Instance.IsAiming);
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
        if (_rb.linearVelocity.y < 0) { // Gravidade
            _rb.linearVelocity += (2) * Physics.gravity.y * Time.deltaTime * Vector3.up;
        }

        if (_mManager.ReturnStunnedValue()) { // Stunado
            _rb.linearVelocity = new(0f, _rb.linearVelocity.y, 0f);
            anim.SetBool("IsWalking", false);
            return; 
        }

        Moving();

        Rotate();

    }

    void Moving() {
        if (_moveInput == Vector2.zero || !_canWalk) { // parado
            if (!_inDash) { // verificando se está em dash
                _rb.linearVelocity = new(0f, _rb.linearVelocity.y, 0f);
            }
            anim.SetBool("IsWalking", false);
            return;
        }

        // andando
        Vector3 moveDirection = new Vector3(_moveInput.x, 0, _moveInput.y).normalized;
        Vector3 velocity = moveDirection * _mManager.ReturnMoveSpeed();
        velocity.y = _rb.linearVelocity.y;
        _rb.linearVelocity = velocity;

        AnimatorWalkingSpeed();
    }

    public void Rotate() {
        if (!_canRotate) return;

        if (LocalWhiteBoard.Instance.IsAiming) {

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray , out RaycastHit hit, Mathf.Infinity, FloorLayer)){

                mousePos = new(hit.point.x, transform.position.y, hit.point.z);
                aimDirection = (mousePos - transform.position);

                if (aimDirection.sqrMagnitude > 0.01f) {
                    Quaternion direction = Quaternion.LookRotation(aimDirection);
                    transform.rotation = Quaternion.Slerp(transform.rotation, direction, Time.deltaTime * rotationSpeed);
                }
            }
        }
        else {
            if (_moveInput.sqrMagnitude > 0.01f) {
                float angle = Mathf.Atan2(_moveInput.x, _moveInput.y) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0, angle, 0);
            }
        }
    }

    void AnimatorWalkingSpeed() { // Ajustando a velocidade da animação
        float currentSpeed = _mManager.ReturnMoveSpeed();
        float originalSpeed = _mManager.ReturnOriginalMoveSpeed();

        float speedRatio = currentSpeed / originalSpeed;

        float animationSpeed = speedRatio * walkingAnimationBaseSpeed;

        if (LocalWhiteBoard.Instance.IsAiming && _moveInput != Vector2.zero) { // Parte que verifica se ela ta andando para trás
            Vector3 moveDir = new Vector3(_moveInput.x, 0, _moveInput.y).normalized;
            Vector3 lookDir = new Vector3(aimDirection.x, 0, aimDirection.z).normalized;

            float dot = Vector3.Dot(moveDir, lookDir);

            // Se dot < 0, o ângulo entre eles é maior que 90 graus, ou seja, andando para trás
            if (dot < 0) {
                animationSpeed *= -1;
            }
        }

        anim.SetFloat("Speed", animationSpeed);
        anim.SetBool("IsWalking", true);
    }

    #endregion

    #region Aim
    public void ChangeMouseSprite(bool isAim) {
        if (isAim) Cursor.SetCursor(aimCursorTexture, Vector2.zero, CursorMode.Auto);
        else Cursor.SetCursor(normalCursorTexture, Vector2.zero, CursorMode.Auto);
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
