using UnityEngine;
using System.Collections;

public class AnimRacahdura : MonoBehaviour
{
    public Material materialOriginal; // Material AtaqueBasicoMaevis
    private Material matInstancia;

    // Parâmetros da animação
    public float angleInicial = 0f;
    public float angleFinal = 180f;

    public float duracao = 1f;

    private Coroutine animacaoAtual;

    private void Awake()
    {
        // Criar uma instância do material para não alterar o material original
        matInstancia = new Material(materialOriginal);
        GetComponent<Renderer>().material = matInstancia;
    }

    private void OnEnable()
    {
        if (animacaoAtual != null) StopCoroutine(animacaoAtual);
        animacaoAtual = StartCoroutine(AnimarValores(angleInicial, angleFinal));
    }

    private void OnDisable()
    {
        if (animacaoAtual != null) StopCoroutine(animacaoAtual);
        animacaoAtual = StartCoroutine(AnimarValores(angleFinal, angleInicial));
    }

    private IEnumerator AnimarValores(float aStart, float aEnd)
    {
        float tempo = 0f;

        while (tempo < duracao)
        {
            float t = tempo / duracao;

            float angle = Mathf.Lerp(aStart, aEnd, t);


            matInstancia.SetFloat("_mask", angle);


            tempo += Time.deltaTime;
            yield return null;
        }

        // Garante que os valores finais sejam aplicados
        matInstancia.SetFloat("_mask", aEnd);

    }
}
