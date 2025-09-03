using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;
using System.Threading;

public class CsvDataLoader : MonoBehaviour
{
    // Para modo Editor (archivo dentro del proyecto)
    public string csvFileName = "layout.csv"; // Assets/StreamingAssets/layout.csv

    // Para modo Ejecutable (archivo externo)
    [HideInInspector] public string absoluteCsvPath = ""; // lo fija el watcher

    public void LoadAndApply() // usa StreamingAssets
    {
        string path = Path.Combine(Application.streamingAssetsPath, csvFileName);
        LoadAndApplyAbsolute(path);
    }

    public void LoadAndApplyAbsolute(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            Debug.LogError("CSV path vacío.");
            return;
        }

        // Leer aunque Excel tenga el archivo abierto (FileShare.ReadWrite) + reintentos
        if (!TryReadAllText(path, out string csvText))
        {
            Debug.LogError("No pude leer el CSV (bloqueado). Intenta de nuevo.");
            return;
        }

        // Parseo simple
        var lines = csvText.Replace("\r", "").Split('\n');
        if (lines.Length <= 1) { Debug.LogError("CSV vacío o sin datos."); return; }

        char delim = lines[0].Contains(";") ? ';' : ',';
        var layout = new LayoutDTO { machines = new List<MachineDTO>() };

        for (int i = 1; i < lines.Length; i++)
        {
            var line = lines[i].Trim();
            if (string.IsNullOrWhiteSpace(line)) continue;

            var parts = line.Split(delim);
            if (parts.Length < 4) { Debug.LogWarning("Fila CSV inválida: " + line); continue; }

            string id = parts[0].Trim().Trim('"');
            string name = parts[1].Trim().Trim('"');
            string status = parts[2].Trim().Trim('"');
            int output = 0; int.TryParse(parts[3].Trim().Trim('"'), out output);

            layout.machines.Add(new MachineDTO { id = id, name = name, status = status, output = output });
        }

        ApplyTo("A", layout);
        ApplyTo("B", layout);
        ApplyTo("C", layout);
        Debug.Log("✅ CSV aplicado desde: " + path);
    }

    // Helper: intenta leer con FileShare.ReadWrite y pequeños reintentos
    bool TryReadAllText(string path, out string text)
    {
        const int maxTries = 8;
        const int waitMs = 80;

        for (int i = 0; i < maxTries; i++)
        {
            try
            {
                using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (var sr = new StreamReader(fs))
                {
                    text = sr.ReadToEnd();
                    return true;
                }
            }
            catch (IOException)
            {
                Thread.Sleep(waitMs); // espera breve y vuelve a intentar
            }
        }
        text = null;
        return false;
    }

    void ApplyTo(string id, LayoutDTO layout)
    {
        var go = GameObject.Find("Machine_" + id);
        if (go == null) { Debug.LogWarning("No encontré Machine_" + id); return; }

        var marker = go.GetComponent<MachineMarker>();
        if (marker == null) { Debug.LogWarning("Falta MachineMarker en " + go.name); return; }

        var dto = layout.machines.Find(m => string.Equals(m.id, id, StringComparison.OrdinalIgnoreCase));
        if (dto == null) { Debug.LogWarning("Sin datos para id " + id + " en CSV"); return; }

        marker.machineName = dto.name;
        marker.status = dto.status;
        marker.output = dto.output;
        marker.UpdateMarkerColor();
    }
}
