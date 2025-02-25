using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using Google.XR.ARCoreExtensions;

public class CloudAnchorService : MonoBehaviour
{
    // Reference to the ARCloudAnchorManager (from ARCore Extensions) and ARAnchorManager in your scene.
    public ARAnchorManager cloudAnchorManager;
    public ARAnchorManager anchorManager;

    /// <summary>
    /// Hosts an ARAnchor using ARCore Extensions cloud anchors.
    /// Waits until the hosting task completes successfully or fails.
    /// </summary>
    /// <param name="localAnchor">The local ARAnchor to host</param>
    /// <param name="ttlDays">Time-to-live for the anchor (in days)</param>
    /// <returns>The cloud anchor ID on success or null on failure.</returns>
    public async Task<string> HostCloudAnchor(ARAnchor localAnchor, int ttlDays = 1)
    {
        if (localAnchor == null)
        {
            Debug.LogError("Local anchor is null");
            return null;
        }
        
        ARCloudAnchor cloudAnchor = cloudAnchorManager.HostCloudAnchor(localAnchor, ttlDays);
        // Poll until the cloud anchor task is complete or a timeout occurs.
        float timeout = 20f; // seconds
        float elapsed = 0f;
        while (cloudAnchor.cloudAnchorState == CloudAnchorState.TaskInProgress && elapsed < timeout)
        {
            await Task.Delay(500);
            elapsed += 0.5f;
        }
        
        if (cloudAnchor.cloudAnchorState == CloudAnchorState.Success)
        {
            Debug.Log($"Hosted Cloud Anchor with ID: {cloudAnchor.cloudAnchorId}");
            return cloudAnchor.cloudAnchorId;
        }
        else
        {
            Debug.LogError("Failed to host cloud anchor. State: " + cloudAnchor.cloudAnchorState);
            return null;
        }
    }

    /// <summary>
    /// Resolves a cloud anchor by its ID using ARCore Extensions.
    /// Waits until the resolution task completes successfully or fails.
    /// </summary>
    /// <param name="cloudAnchorId">The cloud anchor ID to resolve</param>
    /// <returns>The resolved ARAnchor on success or null on failure.</returns>
    public async Task<ARAnchor> ResolveCloudAnchor(string cloudAnchorId)
    {
        if (string.IsNullOrEmpty(cloudAnchorId))
        {
            Debug.LogError("Cloud anchor ID is null or empty");
            return null;
        }
        
        ARCloudAnchor cloudAnchor = cloudAnchorManager.ResolveCloudAnchorId(cloudAnchorId);
        float timeout = 20f;
        float elapsed = 0f;
        while (cloudAnchor.cloudAnchorState == CloudAnchorState.TaskInProgress && elapsed < timeout)
        {
            await Task.Delay(500);
            elapsed += 0.5f;
        }
        
        if (cloudAnchor.cloudAnchorState == CloudAnchorState.Success)
        {
            Debug.Log("Resolved Cloud Anchor successfully");
            return cloudAnchor.GetComponent<ARAnchor>();
        }
        else
        {
            Debug.LogError("Failed to resolve cloud anchor. State: " + cloudAnchor.cloudAnchorState);
            return null;
        }
    }
}