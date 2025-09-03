using UnityEngine;

public class FloatingMarker : MonoBehaviour
{
    public float amplitude = 0.2f;  // altura del movimiento
    public float frequency = 2f;    // velocidad del movimiento

    Vector3 startPos;

    void Start()
    {
        startPos = transform.localPosition; // posición inicial
    }

    void Update()
    {
        float y = Mathf.Sin(Time.time * frequency) * amplitude;
        transform.localPosition = startPos + new Vector3(0, y, 0);
        transform.Rotate(0, 30 * Time.deltaTime, 0); // giro leve
    }
}
