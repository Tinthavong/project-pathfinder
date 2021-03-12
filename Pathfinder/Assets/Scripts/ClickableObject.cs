using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickableObject : MonoBehaviour
{
    private Vector3 mOffset;
    private float mZCoord;

    AStarLoop aStarLoop;
    Unit unit;

    Vector3 newUnitPosition;

    private void Start()
    {
        aStarLoop = FindObjectOfType<AStarLoop>();
        unit = FindObjectOfType<Unit>();
    }

    private void OnMouseDown()
    {
        if (!aStarLoop.paintMode && !unit.isMovingToTarget)
        {
            mZCoord = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;

            mOffset = gameObject.transform.position - GetMouseWorldPos();



        }
    }

    private Vector3 GetMouseWorldPos()
    {
        Vector3 mousePoint = Input.mousePosition;

        mousePoint.z = mZCoord;
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }

    private void OnMouseDrag()
    {
        if (!aStarLoop.paintMode && !unit.isMovingToTarget)
        {
            transform.position = GetMouseWorldPos() + mOffset;
            unit.startingPosition = GetMouseWorldPos() + mOffset;
        }
    }
}
