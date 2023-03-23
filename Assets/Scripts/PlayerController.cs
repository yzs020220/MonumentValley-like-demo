using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using Sequence = DG.Tweening.Sequence;

public class PlayerController : MonoBehaviour
{
    public Transform curCube;
    public Transform clickedCube;
    public List<Transform> finalPath;
    public bool walking = false;
    public Vector3 characterOffset = new Vector3(0, .5f, 0);

    private Vector2 _clickScreenPos;

    void Start()
    {
        RayCastDown();              
    }

    private void Update()
    {
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            RayCastDown();
            Ray mouseRay = Camera.main.ScreenPointToRay(_clickScreenPos);
            RaycastHit mouseHit;
            if (Physics.Raycast(mouseRay, out mouseHit))
            {
                if (mouseHit.transform.GetComponent<Walkable>() != null)
                {
                    clickedCube = mouseHit.transform;
                    DOTween.Kill(gameObject.transform);
                    finalPath.Clear();
                    
                    FindPath();
                }
            }
        }
    }
    
    public void OnGetScreenPosition(InputAction.CallbackContext context)
    {
        _clickScreenPos = context.ReadValue<Vector2>();
    }

    void FindPath()
    {
        List<Transform> nextCubes = new List<Transform>();
        List<Transform> pastCubes = new List<Transform>();

        foreach (WalkPath path in curCube.GetComponent<Walkable>().possiblePaths)
        {
            if (path.active)
            {
                nextCubes.Add(path.target);
                path.target.GetComponent<Walkable>().previousBlock = curCube;
            }
        }
        
        pastCubes.Add(curCube);
        
        ExploreCube(nextCubes, pastCubes);
        BuildPath();
    }

    void ExploreCube(List<Transform> nextCubes, List<Transform> visitedCubes)
    {
        Transform current = nextCubes.First();
        nextCubes.Remove(current);
        
        if(current == clickedCube) return;

        foreach (WalkPath path in current.GetComponent<Walkable>().possiblePaths)
        {
            if (!visitedCubes.Contains(path.target) && path.active)
            {
                nextCubes.Add(path.target);
                path.target.GetComponent<Walkable>().previousBlock = current;
            }
        }
        
        visitedCubes.Add(current);

        if (nextCubes.Any())
        {
            ExploreCube(nextCubes, visitedCubes);
        }
    }

    void BuildPath()
    {
        Transform cube = clickedCube;
        while (cube != curCube)
        {
            finalPath.Add(cube);
            if (cube.GetComponent<Walkable>().previousBlock != null)
                cube = cube.GetComponent<Walkable>().previousBlock;
            else return;
        }
        finalPath.Insert(0, clickedCube);
        FollowPath();
    }

    void FollowPath()
    {
        Sequence s = DOTween.Sequence();
        walking = true;
        for (int i = finalPath.Count - 1; i > 0; i--)
        {
            s.Append(transform.DOMove(finalPath[i].GetComponent<Walkable>().GetWalkPoint() + characterOffset, .2f).SetEase(Ease
            .Linear));
        }

        s.Append(transform.DOMove(clickedCube.GetComponent<Walkable>().GetWalkPoint() + characterOffset, .2f).SetEase(Ease.Linear));

        s.AppendCallback(() => Clear());
    }

    void Clear()
    {
        foreach (Transform t in finalPath)
        {
            t.GetComponent<Walkable>().previousBlock = null;
        }
        finalPath.Clear();
        walking = false;
    }
    
    void RayCastDown()
    {
        Ray playerRay = new Ray(transform.GetChild(0).position, -transform.up);
        RaycastHit playerHit;

        if (Physics.Raycast(playerRay, out playerHit))
        {
            if (playerHit.transform.GetComponent<Walkable>() != null)
            {
                curCube = playerHit.transform;
            }
        }
    }
}
