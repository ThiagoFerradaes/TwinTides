using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShoulderObject : MonoBehaviour {

    [SerializeField] float rotationSpeed;

    bool _canRotate = true;
    float _rotationY;
    Coroutine rotationCoroutine;
    Vector2 _rotationInput;

    private void OnEnable() {
        PlayerController.OnMove += RotateOff;
        PlayerController.OnStop += RotateOn;
    }
    private void OnDisable() {
        PlayerController.OnMove -= RotateOff;
        PlayerController.OnStop -= RotateOn;
    }
    void RotateOff() {
        _canRotate = false;
        if (rotationCoroutine != null) {
            StopCoroutine(rotationCoroutine);
            rotationCoroutine = null;
        }
        rotationCoroutine = StartCoroutine(RotateBack());
    }
    void RotateOn() {
        _rotationInput = Vector2.zero;
        _canRotate = true;
    }
    IEnumerator RotateBack() {
        Quaternion inicialRotation = transform.localRotation;
        float timePassed = 0f;

        while (timePassed < 0.5f) {
            timePassed += Time.deltaTime;
            float time = timePassed / 0.5f;
            transform.localRotation = Quaternion.Lerp(inicialRotation, Quaternion.Euler(0, 0, 0), time);
            yield return null;
        }

        rotationCoroutine = null;
        transform.localRotation = Quaternion.Euler(0, 0, 0);
        _rotationY = 0f;
    }
    public void InputRotate(InputAction.CallbackContext context) {
        if (context.phase == InputActionPhase.Performed && _canRotate) {
            _rotationInput = context.ReadValue<Vector2>();

            if (rotationCoroutine == null) {
                Rotate();
            }

        }
        else if (context.phase == InputActionPhase.Canceled) {
            _rotationInput = Vector2.zero;
        }
    }
    private void Rotate() {
        _rotationY += _rotationInput.x * rotationSpeed * Time.deltaTime;
        transform.localRotation = Quaternion.Euler(0, _rotationY, 0);
    }
}
