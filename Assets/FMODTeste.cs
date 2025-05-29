using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class FMODTeste : MonoBehaviour
{
    [SerializeField] EventReference somTeste;
    void Start()
    {
        //EventInstance isso = RuntimeManager.CreateInstance(somTeste);

        RuntimeManager.PlayOneShot(somTeste);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
