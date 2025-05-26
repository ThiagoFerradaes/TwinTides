using UnityEngine;

public class DesitroyObjectTeste : MonoBehaviour
{
    public float tempoDeVida = 5f;

    void Start()
    {
        Invoke(nameof(TurnOff), tempoDeVida);
    }

    void TurnOff() {
        gameObject.SetActive(false);
    }
}
