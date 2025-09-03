using UnityEngine;
using System.Collections;
using System.IO;

public class CsvWatcher : MonoBehaviour
{
    public CsvDataLoader loader;
    public string csvFileName = "layout.csv";
    public float pollSeconds = 2f;

    string path;
    System.DateTime lastWrite;

    void Start()
    {
        path = Path.Combine(Application.streamingAssetsPath, csvFileName);
        lastWrite = File.Exists(path) ? File.GetLastWriteTimeUtc(path) : System.DateTime.MinValue;
        StartCoroutine(Poll());
    }

    IEnumerator Poll()
    {
        while (true)
        {
            yield return new WaitForSeconds(pollSeconds);

            if (!File.Exists(path)) continue;

            var t = File.GetLastWriteTimeUtc(path);
            if (t > lastWrite)
            {
                lastWrite = t;
                if (loader != null) loader.LoadAndApply();
                else Debug.LogError("CsvWatcher: asigna CsvDataLoader en el Inspector.");
            }
        }
    }
}
