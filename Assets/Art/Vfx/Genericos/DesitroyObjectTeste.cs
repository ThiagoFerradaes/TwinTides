using UnityEngine;

public class DesitroyObjectTeste : MonoBehaviour
{
    public float tempoDeVida = 5f;

    void Start()
    {
        Destroy(gameObject, tempoDeVida);
    }
}
