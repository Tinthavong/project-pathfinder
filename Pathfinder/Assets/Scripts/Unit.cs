using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public Transform target;
    public float speed = 20;
    Vector3[] path;
    int targetIndex;

    private void Start()
    {
        PathRequestManager.RequestPath(transform.position, target.position, OnPathFound);
    }

    public void OnPathFound(Vector3[] newPath, bool pathSuccesssful)
    {
        if(pathSuccesssful)
        {
            path = newPath;
            StopCoroutine("FollowPath"); //Has to be stopped first in case it is already running
            StartCoroutine("FollowPath");
        }
    }

    IEnumerator FollowPath()
    {
        Vector3 currentWayPoint = path[0];

        while (true)
        {
            if(transform.position == currentWayPoint)
            {
                targetIndex++; //Simply advances to the next waypoint
                if(targetIndex >= path.Length)
                {
                    yield break; //breaks out of coroutine after following is done
                }
                currentWayPoint = path[targetIndex]; 
            }
            transform.position = Vector3.MoveTowards(transform.position, currentWayPoint, speed * Time.deltaTime); //current position, target, speed
            yield return null;
        }
    }

    public void OnDrawGizmos()
    {
        if(path != null)
        {
            //don't want to draw waypoints passed
            for(int i = targetIndex; i < path.Length; i++)
            {
                Gizmos.color = Color.black;
                Gizmos.DrawCube(path[i], Vector3.one); //

                if(i == targetIndex)
                {
                    Gizmos.DrawLine(transform.position, path[i]);
                }
                else
                {
                    Gizmos.DrawLine(path[i - 1], path[i]);
                }
            }
        }
    }
}
