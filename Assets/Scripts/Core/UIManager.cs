using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("Panel lateral")]
    public GameObject sidePanel;
    public TMP_Text titleText;
    public TMP_Text nameText;
    public TMP_Text statusText;
    public TMP_Text outputText;

    void Awake()
    {
        if (sidePanel != null)
            sidePanel.SetActive(true); // siempre visible
    }

    public void ShowMachine(string title, string name, string status, string output)
    {
        if (titleText) titleText.text = title;
        if (nameText) nameText.text = "Name: " + name;
        if (statusText) statusText.text = "Status: " + status;
        if (outputText) outputText.text = "Output: " + output;
    }
}
