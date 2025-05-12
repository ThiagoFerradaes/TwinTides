using UnityEngine;

public class AlterarExplosao : MonoBehaviour
{
    private Material material;
    public Vector4 teste;
    
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
            //teste = !teste;
            //material.SetFloat("_NoisePadrao", teste ? 1.0f : 0.0f);

            //teste += 1;
            //material.SetFloat("_disttortionScale", teste);

            //teste = 
            //material.SetFloat("_color_01_normal", teste);


        }
    }
}
