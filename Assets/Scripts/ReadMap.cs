using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

public class ReadMap : MonoBehaviour
{
    public Text debugText;
    public MapManager mapManager;

    private ARSession arSession;

    void Start()
    {
        arSession = FindObjectOfType<ARSession>();
        debugText.text = "Initializing AR Session for map loading...";
        if (arSession != null)
        {
            arSession.Reset();
        }
        // In production, determine the map name to load (could be user-selected).
        LoadMap("GenericMap");
    }

    public async void LoadMap(string mapName)
    {
        debugText.text = "Loading map...";
        var anchor = await mapManager.LoadMap(mapName);
        if (anchor != null)
        {
            debugText.text = "Map loaded successfully!";
            // Optionally, load shapes from metadata via CustomShapeManager here.
        }
        else
        {
            debugText.text = "Failed to load map.";
        }
    }
}