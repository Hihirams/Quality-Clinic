using UnityEngine;
using System.IO;

public class MachineDataLoader : MonoBehaviour
{
    public string fileName = "layout.json"; // en StreamingAssets

    void Start()
    {
        string path = Path.Combine(Application.streamingAssetsPath, fileName);
        if (!File.Exists(path))
        {
            Debug.LogError("No existe: " + path);
            return;
        }

        string json = File.ReadAllText(path);
        var layout = JsonUtility.FromJson<LayoutDTO>(json);
        if (layout == null || layout.machines == null)
        {
            Debug.LogError("JSON inválido o sin 'machines'");
            return;
        }

        // Busca MachineMarker por ID en el nombre del GameObject: Machine_A, Machine_B, etc.
        ApplyTo("A", layout);
        ApplyTo("B", layout);
        ApplyTo("C", layout);
    }

    void ApplyTo(string id, LayoutDTO layout)
    {
        // 1) Encuentra el GO por convención "Machine_<ID>"
        var go = GameObject.Find("Machine_" + id);
        if (go == null)
        {
            Debug.LogWarning("No encontré Machine_" + id + " en escena");
            return;
        }

        // 2) Toma el componente MachineMarker
        var marker = go.GetComponent<MachineMarker>();
        if (marker == null)
        {
            Debug.LogWarning("MachineMarker faltante en " + go.name);
            return;
        }

        // 3) Busca los datos del JSON
        var dto = layout.machines.Find(m => m.id == id);
        if (dto == null)
        {
            Debug.LogWarning("No hay datos para id " + id + " en JSON");
            return;
        }

        // 4) Aplica datos del JSON
        marker.machineName = dto.name;
        marker.status = dto.status;
        marker.output = dto.output;

        // 5) Asegura markerRenderer (si no fue asignado en el Inspector)
        if (marker.markerRenderer == null)
        {
            var markerObj = marker.transform.Find("Marker/Ball"); // ajusta ruta si lo nombraste distinto
            if (markerObj != null)
                marker.markerRenderer = markerObj.GetComponent<Renderer>();

            if (marker.markerRenderer == null)
                marker.markerRenderer = marker.GetComponentInChildren<Renderer>(); // respaldo
        }

        // 6) Actualiza color del marcador según el nuevo output
        marker.UpdateMarkerColor();
    }
}
