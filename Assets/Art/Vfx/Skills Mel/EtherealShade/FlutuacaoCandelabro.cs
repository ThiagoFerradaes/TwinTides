using UnityEngine;

public class FlutuacaoCandelabro : MonoBehaviour
{
    public float amplitude = 1f; // altura do movimento
    public float frequency = 1f; // frequência do movimento

    private float startY;
    private float startRotationY;

    void Start()
    {
        startY = transform.position.y;
        startRotationY = transform.rotation.y;
    }

    void Update()
    {
        float newY = startY + Mathf.Sin(Time.time * frequency * 2 * Mathf.PI) * amplitude;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }
}
