using UnityEngine;
using System.Collections;  // IEnumerator
using System.IO;           // FileSystemWatcher

public class CsvAutoWatcher : MonoBehaviour
{
    public CsvDataLoader loader;

    [Header("Editor (Play en Unity)")]
    public string streamingCsvFileName = "layout.csv"; // Assets/StreamingAssets

    [Header("Ejecutable (.exe)")]
    public string absoluteCsvPath = "";  // ej: C:\\QualityClinic\\data\\layout.csv o \\SERVIDOR\share\layout.csv

    public float pollSecondsEditor = 1.0f; // editor: polling por mtime

    FileSystemWatcher fsw;
    string activePath;
    System.DateTime lastWrite;
    float nextReload = -1f;          // debounce
    const float debounce = 0.5f;     // 500 ms

    void Start()
    {
        // 1) Decide la ruta activa
#if UNITY_EDITOR
        activePath = Path.Combine(Application.streamingAssetsPath, streamingCsvFileName);
#else
        activePath = !string.IsNullOrEmpty(absoluteCsvPath) ? absoluteCsvPath
                   : Path.Combine(Application.streamingAssetsPath, streamingCsvFileName);
#endif
        // 2) Carga inicial
        loader.LoadAndApplyAbsolute(activePath);

        // 3) Arranca vigilancia
#if UNITY_EDITOR
        StartCoroutine(PollEditor());
#else
        StartFSW();
#endif
    }

#if UNITY_EDITOR
    IEnumerator PollEditor()
    {
        lastWrite = File.Exists(activePath) ? File.GetLastWriteTimeUtc(activePath) : System.DateTime.MinValue;
        while (true)
        {
            yield return new WaitForSeconds(pollSecondsEditor);
            if (!File.Exists(activePath)) continue;

            var t = File.GetLastWriteTimeUtc(activePath);
            if (t > lastWrite)
            {
                lastWrite = t;
                nextReload = Time.time + debounce;
            }

            if (nextReload > 0 && Time.time >= nextReload)
            {
                nextReload = -1f;
                loader.LoadAndApplyAbsolute(activePath);
            }
        }
    }
#else
    void StartFSW()
    {
        var dir = Path.GetDirectoryName(activePath);
        var file = Path.GetFileName(activePath);
        if (string.IsNullOrEmpty(dir) || string.IsNullOrEmpty(file))
        {
            Debug.LogError("CsvAutoWatcher: ruta absoluta inválida para ejecutable.");
            return;
        }

        fsw = new FileSystemWatcher(dir, file);
        fsw.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size | NotifyFilters.FileName | NotifyFilters.CreationTime;
        fsw.Changed += (_, __) => nextReload = Time.time + debounce;
        fsw.Created += (_, __) => nextReload = Time.time + debounce;
        fsw.Renamed += (_, __) => nextReload = Time.time + debounce;
        fsw.EnableRaisingEvents = true;
    }

    void Update()
    {
        if (nextReload > 0 && Time.time >= nextReload)
        {
            nextReload = -1f;
            loader.LoadAndApplyAbsolute(activePath);
        }
    }

    void OnDestroy()
    {
        if (fsw != null) { fsw.EnableRaisingEvents = false; fsw.Dispose(); }
    }
#endif
}
