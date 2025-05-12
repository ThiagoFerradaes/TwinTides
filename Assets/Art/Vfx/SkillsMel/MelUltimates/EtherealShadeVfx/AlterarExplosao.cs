using UnityEngine;

public class AlterarExplosao : MonoBehaviour
{
    private Material material;
    public float teste;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        material = GetComponent<MeshRenderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //teste = Color(1.75177872, 8.67703533, 6.6664772, 1);
            //bool
            //material.SetFloat("_NoisePadrao", teste ? 1.0f : 0.0f);

            //float
            //material.SetFloat("_NoisePadrao", teste);

            //material.SetFloat("_NoisePadrao", teste);
        }
    }
}
