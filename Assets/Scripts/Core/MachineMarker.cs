using UnityEngine;

[RequireComponent(typeof(Collider))]
public class MachineMarker : MonoBehaviour
{
    [Header("Datos de máquina")]
    public string machineName = "Machine A";
    public string status = "RUN";
    public int output = 120;

    [Header("UI")]
    public UIManager ui;

    [Header("Marker Prefab (auto)")]
    public GameObject markerPrefab;   // arrastra Marker.prefab aquí
    public float markerHeight = 2f;   // altura del indicador

    [Header("Marker Runtime Refs")]
    public Transform markerRoot;      // se autollenará
    public Renderer markerRenderer;   // se autollenará (DEBE ser Marker/Ball)

    [SerializeField] private string ballTag = "MarkerBall";

    void Awake()
    {
        if (ui == null) ui = FindObjectOfType<UIManager>();
        BindMarkerRenderer(); // asegura que NO toma renderers de la máquina
    }

    void Start()
    {
        // Si no hay Marker en la jerarquía y tenemos prefab, lo instanciamos
        if (markerRoot == null && markerPrefab != null)
        {
            var markerInstance = Instantiate(markerPrefab, transform);
            markerRoot = markerInstance.transform;
            markerRoot.localPosition = new Vector3(0f, markerHeight, 0f);
        }

        BindMarkerRenderer();          // vuelve a atar tras instanciar
        SetIndicatorVisible(false);    // oculto por defecto
        UpdateMarkerColor();           // color listo
    }

    void OnMouseDown() => HandleClick();

    public void HandleClick()
    {
        Debug.Log("CLICK en " + name);

        if (ui != null)
            ui.ShowMachine("Máquina", machineName, status, output.ToString());
        else
            Debug.LogError("UIManager no asignado/encontrado para " + name);

        UpdateMarkerColor();   // color según datos actuales
        HideAllIndicators();   // oculta los demás
        SetIndicatorVisible(true);
    }

    public void UpdateMarkerColor()
    {
        if (markerRenderer == null)
        {
            Debug.LogWarning($"[{name}] markerRenderer es null. ¿Existe Marker/Ball con tag {ballTag}?");
            return;
        }

        Debug.Log($"[{name}] Pintando renderer: {markerRenderer.gameObject.name}");

        Color c = (output >= 100) ? Color.green : (output > 0 ? Color.yellow : Color.red);

        var mpb = new MaterialPropertyBlock();
        markerRenderer.GetPropertyBlock(mpb);

        if (markerRenderer.sharedMaterial != null && markerRenderer.sharedMaterial.HasProperty("_BaseColor"))
            mpb.SetColor("_BaseColor", c); // URP/Lit
        else
            mpb.SetColor("_Color", c);     // fallback Standard

        markerRenderer.SetPropertyBlock(mpb);
    }

    void SetIndicatorVisible(bool visible)
    {
        if (markerRoot != null)
            markerRoot.gameObject.SetActive(visible);
    }

    static void HideAllIndicators()
    {
        var all = FindObjectsOfType<MachineMarker>(includeInactive: true);
        foreach (var m in all)
            m.SetIndicatorVisible(false);
    }

    // ——— SOLO busca dentro de Marker y por tag ———
    void BindMarkerRenderer()
    {
        // 1) Localiza markerRoot si aún no está
        if (markerRoot == null)
        {
            var t = transform.Find("Marker");
            if (t != null) markerRoot = t;
        }
        if (markerRoot == null) return;

        // 2) Busca SOLO renderers bajo markerRoot y con Tag correcto
        markerRenderer = null;
        var renderers = markerRoot.GetComponentsInChildren<Renderer>(true);
        foreach (var r in renderers)
        {
            if (r.gameObject.CompareTag(ballTag))
            {
                markerRenderer = r;
                break;
            }
        }

        // 3) Seguridad: si no encontró la Ball, avisa
        if (markerRenderer == null)
        {
            Debug.LogError($"[{name}] No encontré Renderer con tag '{ballTag}' dentro de Marker.");
        }
    }
}
