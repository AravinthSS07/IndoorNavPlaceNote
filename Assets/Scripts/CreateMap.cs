using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using System.Threading.Tasks;

[RequireComponent(typeof(CustomShapeManager))]
public class CreateMap : MonoBehaviour
{
    public Text debugText;
    public MapManager mapManager;
    public CustomShapeManager shapeManager;

    private bool shouldRecordWaypoints = false;
    private bool shouldSaveMap = true;

    private ARSession arSession;

    void Start()
    {
        shapeManager = GetComponent<CustomShapeManager>();
        arSession = FindObjectOfType<ARSession>();
        debugText.text = "Initializing AR Session...";
        shouldRecordWaypoints = true;
    }

    void Update()
    {
        if (shouldRecordWaypoints)
        {
            Transform player = Camera.main.transform;
            // Add a new waypoint if none are within 1m of the player.
            Collider[] hitColliders = Physics.OverlapSphere(player.position, 1f);
            bool waypointNearby = false;
            foreach (var collider in hitColliders)
            {
                if (collider.CompareTag("waypoint"))
                {
                    waypointNearby = true;
                    break;
                }
            }
            if (!waypointNearby)
            {
                Vector3 pos = player.position;
                pos.y = -0.5f;
                shapeManager.AddShape(pos, Quaternion.identity, false);
            }
        }
    }

    public void CreateDestination()
    {
        shapeManager.AddDestinationShape();
    }

    public void OnStartNewClick()
    {
        if (arSession != null)
        {
            arSession.Reset();
            debugText.text = "AR Session restarted.";
        }
        shouldRecordWaypoints = true;
    }

    public async void OnSaveMapClick()
    {
        // In production, replace with user-input for a unique map name.
        string mapName = "GenericMap";
        debugText.text = "Uploading map...";
        bool success = await mapManager.UploadMap(mapName);
        if (success)
        {
            debugText.text = "Map upload complete!";
        }
        else
        {
            debugText.text = "Map upload failed!";
        }
        shouldRecordWaypoints = false;
    }
}