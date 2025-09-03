using UnityEngine;

public class ClickManager : MonoBehaviour
{
    public Camera cam;
    int machineLayer;

    void Awake()
    {
        if (cam == null) cam = Camera.main;
        machineLayer = LayerMask.NameToLayer("Machine");
        if (machineLayer == -1)
        {
            Debug.LogError("No existe la capa 'Machine'. Crea la capa y asígnala a tus máquinas.");
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (cam == null) return;

            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 2000f))
            {
                // Asegura que pegaste a algo en la capa Machine
                if (hit.collider.gameObject.layer == machineLayer)
                {
                    var marker = hit.collider.GetComponent<MachineMarker>();
                    if (marker != null)
                    {
                        Debug.Log("RAY CLICK en " + hit.collider.name);
                        marker.HandleClick(); // ✅ usamos el método real
                    }
                    else
                    {
                        Debug.LogWarning("No encontré MachineMarker en " + hit.collider.name);
                    }
                }
            }
        }
    }
}
