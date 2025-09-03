using UnityEngine;

public class SimsCameraController : MonoBehaviour
{
    [Header("Refs")]
    public Transform cam;              // arrastra Main Camera aquí

    [Header("Movimiento")]
    public float moveSpeed = 15f;      // WASD
    public float dragSpeed = 0.15f;    // arrastre con rueda media
    public Vector2 xzMin = new Vector2(-100f, -100f);
    public Vector2 xzMax = new Vector2(100f, 100f);

    [Header("Zoom")]
    public float minZoom = 8f;
    public float maxZoom = 60f;
    public float zoomSpeed = 30f;

    [Header("Rotación")]
    public float rotateSpeed = 120f;

    [Header("Suavizado")]
    public float posSmoothTime = 0.12f;  // menor = más responsivo
    public float zoomSmoothTime = 0.15f;
    public float rotSmoothTime = 0.12f;

    // estado interno (targets + velocidades para SmoothDamp)
    float yaw, targetYaw, yawVel;
    float camDist, targetCamDist, zoomVel;
    Vector3 camDirLocal;
    Vector3 targetPos, posVel;

    void Awake()
    {
        if (cam == null) cam = Camera.main.transform;

        camDirLocal = cam.localPosition.normalized;
        camDist = cam.localPosition.magnitude;
        targetCamDist = camDist;

        yaw = transform.eulerAngles.y;
        targetYaw = yaw;

        targetPos = transform.position;
    }

    void Update()
    {
        // ---------- Entrada ----------
        // WASD
        Vector3 input = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));
        if (input.sqrMagnitude > 0f)
        {
            Vector3 fwd = new Vector3(cam.forward.x, 0f, cam.forward.z).normalized;
            Vector3 right = new Vector3(cam.right.x, 0f, cam.right.z).normalized;
            targetPos += (fwd * input.z + right * input.x) * moveSpeed * Time.deltaTime;
        }

        // Pan con botón medio
        if (Input.GetMouseButton(2))
        {
            float dx = -Input.GetAxis("Mouse X") * dragSpeed * camDist;
            float dy = -Input.GetAxis("Mouse Y") * dragSpeed * camDist;
            Vector3 fwd = new Vector3(cam.forward.x, 0f, cam.forward.z).normalized;
            Vector3 right = new Vector3(cam.right.x, 0f, cam.right.z).normalized;
            targetPos += right * dx + fwd * dy;
        }

        // Zoom con rueda
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.0001f)
        {
            targetCamDist = Mathf.Clamp(targetCamDist - scroll * zoomSpeed, minZoom, maxZoom);
        }

        // Rotación con botón derecho
        if (Input.GetMouseButton(1))
        {
            targetYaw += Input.GetAxis("Mouse X") * rotateSpeed * Time.deltaTime;
        }

        // Límites del plano
        targetPos.x = Mathf.Clamp(targetPos.x, xzMin.x, xzMax.x);
        targetPos.z = Mathf.Clamp(targetPos.z, xzMin.y, xzMax.y);

        // ---------- Suavizado ----------
        transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref posVel, posSmoothTime);
        yaw = Mathf.SmoothDampAngle(yaw, targetYaw, ref yawVel, rotSmoothTime);
        transform.rotation = Quaternion.Euler(0f, yaw, 0f);

        camDist = Mathf.SmoothDamp(camDist, targetCamDist, ref zoomVel, zoomSmoothTime);
        cam.localPosition = camDirLocal * camDist;
    }
}
