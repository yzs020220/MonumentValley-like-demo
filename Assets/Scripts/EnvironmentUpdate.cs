using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.UI;

public class EnvironmentUpdate : MonoBehaviour
{
    public Walkable cube0;
    public Walkable cube1;
    // public Walkable stair0;
    // public Walkable stair1;
    // public Walkable cube2;
    // public Walkable stair2;

    public Walkable[] platform;
    public bool pathFirstSet = false;
    public bool pathUpdated = false;
    public CameraManager cameraManager;

    private float _dist0;
    // private float _dist1;
    // private float _dist2;
    private float _platformDist;

    // Start is called before the first frame update
    void Start()
    {
        platform = GetComponentsInChildren<Walkable>();

        PathDistSet();
    }

    // Update is called once per frame
    void Update()
    {
        if (!pathFirstSet)
        {
            PathDistSet();
            PathUpdate();
            pathFirstSet = true;
        }

        if (cameraManager.isBusy)
        {
            pathUpdated = false;
        }

        if (!cameraManager.isBusy && !pathUpdated)
        {
            PathClear();
            Debug.Log("path update");
            PathUpdate();
            pathUpdated = true;
        }
    }

    void PathDistSet()
    {
        _dist0 = (cube0.screenPoint - cube1.screenPoint).magnitude;
        // _dist1 = (stair0.screenPoint - stair1.screenPoint).magnitude;
        // _dist2 = (cube2.screenPoint - stair2.screenPoint).magnitude;
        // Debug.Log(_dist0);
        // Debug.Log(_dist1);
        // Debug.Log(_dist2);
    }

    public void PathUpdate()
    {
        for (int i = 0; i < platform.Length - 1; i++)
        {
            for (int j = i + 1; j < platform.Length; j++)
            {
                platform[i].GetScreenPoint();
                platform[j].GetScreenPoint();
                Vector2 iScreenPos = new Vector2(platform[i].screenPoint.x, platform[i].screenPoint.y);
                Vector2 jScreenPos = new Vector2(platform[j].screenPoint.x, platform[j].screenPoint.y);
                _platformDist = (iScreenPos - jScreenPos).magnitude;

                if (Mathf.Abs(_platformDist - _dist0) < .5f)
                {
                    WalkPath walkPath0 = new WalkPath();
                    WalkPath walkPath1 = new WalkPath();

                    walkPath0.target = platform[i].transform;
                    walkPath1.target = platform[j].transform;
                    walkPath0.active = true;
                    walkPath1.active = true;

                    platform[i].possiblePaths.Add(walkPath1);
                    platform[j].possiblePaths.Add(walkPath0);

                    // Debug.Log("path added");
                }
                //
                // else
                // {
                //     Debug.Log((i, j));
                //     Debug.Log("dist:" + _platformDist);
                // }
                
            }
        }
    }

    public void PathClear()
    {
        for (int i = 0; i < platform.Length; i++)
        {
            platform[i].possiblePaths.Clear();
            platform[i].screenPoint = platform[i].GetScreenPoint();
        }
    }
}