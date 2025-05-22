using UnityEngine;

public class GirarObjeto : MonoBehaviour
{
    public Vector3 rotationSpeed = new Vector3(0f, 100f, 0f); // graus por segundo

    void Update()
    {
        transform.Rotate(rotationSpeed * Time.deltaTime);
    }
}
