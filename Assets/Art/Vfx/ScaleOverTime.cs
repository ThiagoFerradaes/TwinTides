using UnityEngine;

public class ScaleOverTime : MonoBehaviour
{
    public Vector3 initialScale = Vector3.one;   // Tamanho inicial (X)
    public Vector3 finalScale = Vector3.one * 2; // Tamanho final (Y)
    public float duration = 2f;                  // Duração em segundos (Z)

    private float elapsedTime = 0f;
    private bool isScaling = false;

    void Start()
    {
        transform.localScale = initialScale;
        StartScaling();
    }

    public void StartScaling()
    {
        elapsedTime = 0f;
        isScaling = true;
    }

    void Update()
    {
        if (!isScaling) return;

        elapsedTime += Time.deltaTime;
        float t = Mathf.Clamp01(elapsedTime / duration);
        transform.localScale = Vector3.Lerp(initialScale, finalScale, t);

        if (t >= 1f)
        {
            isScaling = false;
        }
    }
}

