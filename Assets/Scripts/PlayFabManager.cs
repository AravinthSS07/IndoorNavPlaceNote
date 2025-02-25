using System;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

public class PlayFabManager : MonoBehaviour
{
    // Ensure you authenticate the user before making PlayFab API calls.
    // This example uses a Custom ID login. Replace with your preferred authentication method.
    private void Start()
    {
        Login();
    }

    private void Login()
    {
        var request = new LoginWithCustomIDRequest
        {
            CustomId = SystemInfo.deviceUniqueIdentifier,
            CreateAccount = true
        };

        PlayFabClientAPI.LoginWithCustomID(request,
            result => { Debug.Log("PlayFab login successful."); },
            error => { Debug.LogError("Error logging into PlayFab: " + error.GenerateErrorReport()); });
    }

    public void SaveMapData(string mapName, string cloudAnchorId, Action<bool> callback)
    {
        var data = new Dictionary<string, string>
        {
            { "CloudAnchorId", cloudAnchorId }
        };

        // Use a unique key such as "Map_" + mapName.
        var request = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>(data),
            Permission = UserDataPermission.Public
        };

        PlayFabClientAPI.UpdateUserData(request,
            result =>
            {
                Debug.Log($"Map data for '{mapName}' saved successfully.");
                callback?.Invoke(true);
            },
            error =>
            {
                Debug.LogError("Error saving map data: " + error.GenerateErrorReport());
                callback?.Invoke(false);
            });
    }

    public void LoadMapData(string mapName, Action<string> callback)
    {
        string dataKey = "Map_" + mapName;

        var request = new GetUserDataRequest
        {
            Keys = new List<string> { dataKey }
        };

        PlayFabClientAPI.GetUserData(request,
            result =>
            {
                if (result.Data != null && result.Data.ContainsKey(dataKey) && !string.IsNullOrEmpty(result.Data[dataKey].Value))
                {
                    string cloudAnchorId = result.Data[dataKey].Value;
                    Debug.Log($"Map data for '{mapName}' loaded successfully.");
                    callback?.Invoke(cloudAnchorId);
                }
                else
                {
                    Debug.LogError("Map data not found for: " + mapName);
                    callback?.Invoke(null);
                }
            },
            error =>
            {
                Debug.LogError("Error loading map data: " + error.GenerateErrorReport());
                callback?.Invoke(null);
            });
    }

    public void DeleteMapData(string mapName, Action<bool> callback)
    {
        // PlayFab does not support deleting individual keys; update the key to an empty string.
        string dataKey = "Map_" + mapName;
        var updateRequest = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string> { { dataKey, "" } }
        };
        PlayFabClientAPI.UpdateUserData(updateRequest,
            result =>
            {
                Debug.Log($"Map data for '{mapName}' deleted successfully.");
                callback?.Invoke(true);
            },
            error =>
            {
                Debug.LogError("Error deleting map data: " + error.GenerateErrorReport());
                callback?.Invoke(false);
            });
    }
}