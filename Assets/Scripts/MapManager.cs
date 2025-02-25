using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

[System.Serializable]
public class MapData
{
    public string mapName;
    public string cloudAnchorId;
}

public class MapManager : MonoBehaviour
{
    // Reference to CloudAnchorService and PlayFabManager.
    public CloudAnchorService cloudAnchorService;
    public PlayFabManager playFabManager;
    // Reference to the local ARAnchor placed by the user when capturing a map.
    public ARAnchor userPlacedAnchor;

    /// <summary>
    /// Uploads a map with a unique name.
    /// Hosts the current ARAnchor using a production cloud anchor API and saves the resulting cloud anchor ID to PlayFab.
    /// </summary>
    /// <param name="mapName">Unique name for the map</param>
    /// <returns>True if successful; false otherwise.</returns>
    public async Task<bool> UploadMap(string mapName)
    {
        if (string.IsNullOrEmpty(mapName))
        {
            Debug.LogError("Map name is required.");
            return false;
        }
        if (userPlacedAnchor == null)
        {
            Debug.LogError("No AR Anchor available to host.");
            return false;
        }

        // Host the local anchor using the production cloud anchor API.
        string cloudAnchorId = await cloudAnchorService.HostCloudAnchor(userPlacedAnchor);
        if (string.IsNullOrEmpty(cloudAnchorId))
        {
            Debug.LogError("Failed to host cloud anchor.");
            return false;
        }

        bool saveSuccessful = false;
        var tcs = new TaskCompletionSource<bool>();

        // Save map data permanently via PlayFab.
        playFabManager.SaveMapData(mapName, cloudAnchorId, success =>
        {
            saveSuccessful = success;
            tcs.SetResult(success);
        });
        await tcs.Task;

        if (saveSuccessful)
        {
            Debug.Log($"Map '{mapName}' uploaded successfully with Anchor ID {cloudAnchorId}.");
            return true;
        }
        else
        {
            Debug.LogError("Failed to upload map data to PlayFab.");
            return false;
        }
    }

    /// <summary>
    /// Loads a map based on its unique name.
    /// Resolves its cloud anchor using the stored ID from PlayFab.
    /// </summary>
    /// <param name="mapName">Unique name for the map</param>
    /// <returns>The resolved ARAnchor if successful; otherwise, null.</returns>
    public async Task<ARAnchor> LoadMap(string mapName)
    {
        var tcs = new TaskCompletionSource<string>();

        playFabManager.LoadMapData(mapName, cloudAnchorId =>
        {
            tcs.SetResult(cloudAnchorId);
        });

        string savedCloudAnchorId = await tcs.Task;
        if (string.IsNullOrEmpty(savedCloudAnchorId))
        {
            Debug.LogError("Map data not found for: " + mapName);
            return null;
        }

        ARAnchor resolvedAnchor = await cloudAnchorService.ResolveCloudAnchor(savedCloudAnchorId);
        if (resolvedAnchor != null)
        {
            Debug.Log($"Map '{mapName}' loaded successfully.");
        }
        else
        {
            Debug.LogError("Failed to resolve cloud anchor during load.");
        }
        return resolvedAnchor;
    }

    /// <summary>
    /// Deletes a map by its unique name using PlayFab.
    /// </summary>
    /// <param name="mapName">Unique name of the map to delete</param>
    public void DeleteMap(string mapName)
    {
        playFabManager.DeleteMapData(mapName, success =>
        {
            if (success)
            {
                Debug.Log($"Map '{mapName}' has been deleted from PlayFab storage.");
            }
            else
            {
                Debug.LogError("Failed to delete map data for: " + mapName);
            }
        });
    }
}