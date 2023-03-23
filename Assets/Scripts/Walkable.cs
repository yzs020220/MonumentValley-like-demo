using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Walkable : MonoBehaviour
{
    public List<WalkPath> possiblePaths = new List<WalkPath>();

    public Transform previousBlock;

    [Space] [Header("Booleans")] public bool isStair = false;
    public bool movingPlatform = false;
    public bool isButton;
    public bool notRotate;

    [Space] [Header("Offsets")] public float walkPointOffset = .5f;
    public float stairOffset = .4f;
    public Vector3 walkPoint;
    public Vector3 screenPoint;

    private void Start()
    {
        walkPoint = GetWalkPoint();
        screenPoint = GetScreenPoint();
    }

    public Vector3 GetWalkPoint()
    {
        float stair = isStair ? stairOffset : 0;
        return transform.position + transform.up * (walkPointOffset - stair);
    }

    public Vector3 GetScreenPoint()
    {
        Vector3 sp = Camera.main.WorldToScreenPoint(walkPoint);
        return sp;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.grey;
        float stair = isStair ? stairOffset : 0;
        Gizmos.DrawSphere(walkPoint, .1f);

        if (possiblePaths == null) return;

        foreach (WalkPath p in possiblePaths)
        {
            if (p.target == null) return;
            Gizmos.color = p.active ? Color.black : Color.clear;
            Gizmos.DrawLine(walkPoint, p.target.GetComponent<Walkable>().walkPoint);
        }
    }
}


[System.Serializable]
public class WalkPath
{
    public Transform target;
    public bool active = true;
}