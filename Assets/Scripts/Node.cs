﻿using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public Vector3 pos;

    [Header("A*")]
    public List<Node> neighbors = new List<Node>();
    public float FCost { get { return GCost + HCost; } }
    public float HCost { get; set; }
    public float GCost { get; set; }
    public float Cost { get; set; }
    public Node Parent { get; set; }
    public Node NextInList { get; set; }

    private Vector3 scale;
    private bool isDestination = false;

    private void Awake()
    {
        transform.GetChild(0).gameObject.SetActive(false);
        scale = transform.localScale;
#if UNITY_EDITOR
        pos = transform.position;
#endif
        if (GetComponent<DiamondBehavior>() != null)
            isDestination = true;
    }

    public void Activate(bool active)
    {
        transform.GetChild(0).gameObject.SetActive(active);
        if (NextInList != null)
        {
            transform.LookAt(NextInList.transform);
        }
    }

    void Update()
    {
        if (!isDestination)
            transform.localScale = scale * (1 + Mathf.Sin(Mathf.PI * Time.time) * 0.2f);
    }

    public void FindNeighbors(float maxDistance)
    {
        foreach (Node node in FindObjectsOfType<Node>())
        {
            if (Vector3.Distance(node.pos, this.pos) < maxDistance)
            {
                neighbors.Add(node);
            }
        }
    }
}