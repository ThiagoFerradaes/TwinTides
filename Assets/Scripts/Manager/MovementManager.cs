using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class MovementManager : NetworkBehaviour {

    [SerializeField] NetworkVariable<float> baseMoveSpeed = new();
    [SerializeField] NetworkVariable<float> adicionalMoveSpeed = new();
    private NetworkVariable<bool> _isStunned = new(false);

    /// <summary>
    /// Retorna o valor da velocidade base/minima + o valor da velocidade adicional, se o objeto estiver stunado retorna 0
    /// </summary>
    /// <returns></returns>
    public float ReturnMoveSpeed() {
        if (!_isStunned.Value) {
            return adicionalMoveSpeed.Value + baseMoveSpeed.Value;
        }
        else {
            return 0f;
        }
    }

    /// <summary>
    /// Retorna se o objeto está stunado
    /// </summary>
    /// <returns></returns>
    public bool ReturnStunnedValue() {
        return _isStunned.Value;
    }

    /// <summary>
    /// Multiplica a velocidade adicional por um valor
    /// </summary>
    /// <param name="speedMultiplier"></param>
    [Rpc(SendTo.Server)]
    public void IncreaseMoveSpeedRpc(float speedMultiplier) {
        adicionalMoveSpeed.Value *= speedMultiplier;
    }

    /// <summary>
    /// Divide a velocidade adicional por um valor
    /// </summary>
    /// <param name="speedMultiplier"></param>
    [Rpc(SendTo.Server)]
    public void DecreaseMoveSpeedRpc(float speedMultiplier) {
        adicionalMoveSpeed.Value /= speedMultiplier;
    }

    /// <summary>
    /// Multiplica a velocidade adicional por um valor por um periodo de tempo
    /// </summary>
    /// <param name="speedMultiplier"></param>
    /// <param name="duration"></param>
    [Rpc(SendTo.Server)]
    public void IncreaseMoveSpeedWithTimeRpc(float speedMultiplier, float duration) {
        StartCoroutine(IncreaseSpeedCoroutine(speedMultiplier, duration));
    }

    /// <summary>
    /// Divide a velocidade adicional por um valor por um tempo
    /// </summary>
    /// <param name="speedMultiplier"></param>
    /// <param name="duration"></param>
    [Rpc(SendTo.Server)]
    public void DecreaseBaseSpeedWithTimeRpc(float speedMultiplier, float duration) {
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
    [Rpc(SendTo.Server)]
    public void StunRpc() {
        _isStunned.Value = true;
    }


    /// <summary>
    /// Stuna o objeto por um periodo de tempo
    /// </summary>
    /// <param name="stunDuration"></param>
    [Rpc(SendTo.Server)]
    public void StunWithTimeRpc(float stunDuration) {
        StartCoroutine(StunWithTimeCoroutine(stunDuration));
    }

    IEnumerator StunWithTimeCoroutine(float stunDuration) {
        _isStunned.Value = true;
        yield return new WaitForSeconds(stunDuration);
        _isStunned.Value = false;
    }

}
