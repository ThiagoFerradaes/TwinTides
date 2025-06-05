using FMODUnity;
using System.Collections;
using System.Threading;
using Unity.Netcode;
using UnityEngine;

public class MovementManager : NetworkBehaviour {

    [SerializeField] NetworkVariable<float> baseMoveSpeed = new(3);
    NetworkVariable<float> adicionalMoveSpeed = new(1);
    private NetworkVariable<bool> _isStunned = new(false);

    [SerializeField] EventReference stunSound;
    [SerializeField] float cooldownStunSound;
    Coroutine stunSoundRooutine;
    Coroutine stunRoutine;

    /// <summary>
    /// Retorna o valor da velocidade base/minima + o valor da velocidade adicional, se o objeto estiver stunado retorna 0
    /// </summary>
    /// <returns></returns>
    public float ReturnMoveSpeed() {
        if (!_isStunned.Value) {
            return Mathf.Clamp(baseMoveSpeed.Value * (1 + adicionalMoveSpeed.Value * 2), baseMoveSpeed.Value, baseMoveSpeed.Value * 4) ;
        }
        else {
            return 0f;
        }
    }

    /// <summary>
    /// Retorna se o objeto está stunado
    /// </summary>
    /// <returns></returns>
    public bool ReturnStunnedValue() => _isStunned.Value;

    /// <summary>
    /// Multiplica a velocidade adicional por uma porcentam - o valor passado tem que estar entre 0 e 100
    /// </summary>
    /// <param name="speedPercent"></param>
    public void IncreaseMoveSpeed(float speedPercent) {
        if (!IsServer) return;
        speedPercent = Mathf.Clamp(speedPercent, 0, 100);
        adicionalMoveSpeed.Value *= (1 + speedPercent/100);
    }

    /// <summary>
    /// Divide a velocidade adicional por uma porcentam - o valor passado tem que estar entre 0 e 100
    /// </summary>
    /// <param name="speedPercent"></param>
    public void DecreaseMoveSpeed(float speedPercent) {
        if (!IsServer) return;
        speedPercent = Mathf.Clamp(speedPercent, 0, 100);
        adicionalMoveSpeed.Value /= (1 + speedPercent/100);
    }

    /// <summary>
    /// Multiplica a velocidade adicional por um valor por um periodo de tempo
    /// </summary>
    /// <param name="speedMultiplier"></param>
    /// <param name="duration"></param>
    public void IncreaseMoveSpeedWithTime(float speedMultiplier, float duration) {
        if (!IsServer) return;
        StartCoroutine(IncreaseSpeedCoroutine(speedMultiplier, duration));
    }

    /// <summary>
    /// Divide a velocidade adicional por um valor por um tempo
    /// </summary>
    /// <param name="speedMultiplier"></param>
    /// <param name="duration"></param>
    public void DecreaseBaseSpeedWithTime(float speedMultiplier, float duration) {
        if (!IsServer) return;
        StartCoroutine(DecreaseSpeedCoroutine(speedMultiplier, duration));
    }

    IEnumerator IncreaseSpeedCoroutine(float speedMultiplier, float duration) {
        adicionalMoveSpeed.Value *= speedMultiplier;
        yield return new WaitForSeconds(duration);
        adicionalMoveSpeed.Value /= speedMultiplier;
    }
    IEnumerator DecreaseSpeedCoroutine(float speeMultiplier, float duration) {
        adicionalMoveSpeed.Value /= speeMultiplier;
        yield return new WaitForSeconds(duration);
        adicionalMoveSpeed.Value *= speeMultiplier;
    }


    /// <summary>
    /// Stuna o objeto, ou seja, faz o retorno da velocidade total desse objeto ser 0
    /// </summary>
    public void Stun() {
        if (!IsServer) return;

        stunSoundRooutine ??= StartCoroutine(StunSoundRoutine());

        _isStunned.Value = true;
    }

    public void UnStun() {
        if (!IsServer) return;
        _isStunned.Value = false;
    }

    IEnumerator StunSoundRoutine() {
        if (!stunSound.IsNull) RuntimeManager.PlayOneShot(stunSound, transform.position);

        yield return new WaitForSeconds(cooldownStunSound);

        stunSoundRooutine = null;
    }


    /// <summary>
    /// Stuna o objeto por um periodo de tempo
    /// </summary>
    /// <param name="stunDuration"></param>
    public void StunWithTime(float stunDuration) {
        if (!IsServer) return;
        if(stunRoutine == null) stunRoutine = StartCoroutine(StunWithTimeCoroutine(stunDuration));
        else {
            StopCoroutine(stunRoutine);
            stunRoutine = null;
            stunRoutine = StartCoroutine(StunWithTimeCoroutine(stunDuration));
        }
    }

    IEnumerator StunWithTimeCoroutine(float stunDuration) {
        Stun();
        yield return new WaitForSeconds(stunDuration);
        UnStun();

        stunRoutine = null;
    }

}
