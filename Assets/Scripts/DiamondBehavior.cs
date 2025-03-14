﻿using UnityEngine;

public class DiamondBehavior : MonoBehaviour
{
    private Vector3 rotate = new Vector3(0, 1, 0);
    public GameObject diamond;

    private void Awake()
    {
        Activate(false);
    }

    void Update()
    {
        transform.eulerAngles += rotate;
    }

    public void Activate(bool active)
    {
        diamond.SetActive(active);
    }
}