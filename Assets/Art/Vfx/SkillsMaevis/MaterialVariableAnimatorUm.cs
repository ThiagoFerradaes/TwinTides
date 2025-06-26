using UnityEngine;
using System.Collections;

public class MaterialVariableAnimatorUm : MonoBehaviour
{
    public Material materialOriginal; // Material AtaqueBasicoMaevis
    private Material matInstancia;

    // Parâmetros da animação
    public float angleInicial = 0f;
    public float angleFinal = 180f;

    public float aberturaInicial = 0f;
    public float aberturaFinal = 1f;

    public float powerInicial = 0f;
    public float powerFinal = 5f;

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
        animacaoAtual = StartCoroutine(AnimarValores(angleInicial, angleFinal, aberturaInicial, aberturaFinal, powerInicial, powerFinal));
    }

    private void OnDisable()
    {
        if (animacaoAtual != null) StopCoroutine(animacaoAtual);
        animacaoAtual = StartCoroutine(AnimarValores(angleFinal, angleInicial, aberturaFinal, aberturaInicial, powerFinal, powerInicial));
    }

    private IEnumerator AnimarValores(float aStart, float aEnd, float abStart, float abEnd, float pStart, float pEnd)
    {
        float tempo = 0f;

        while (tempo < duracao)
        {
            float t = tempo / duracao;

            float angle = Mathf.Lerp(aStart, aEnd, t);
            float abertura = Mathf.Lerp(abStart, abEnd, t);
            float power = Mathf.Lerp(pStart, pEnd, t);

            matInstancia.SetFloat("_angle", angle);
            matInstancia.SetFloat("_aberturaDoArco", abertura);
            matInstancia.SetFloat("_power", power);

            tempo += Time.deltaTime;
            yield return null;
        }

        // Garante que os valores finais sejam aplicados
        matInstancia.SetFloat("_angle", aEnd);
        matInstancia.SetFloat("_aberturaDoArco", abEnd);
        matInstancia.SetFloat("_power", pEnd);
    }
}
