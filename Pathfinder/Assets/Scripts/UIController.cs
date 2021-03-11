using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIController : MonoBehaviour
{
    //Reference to the unit
    //Single unit for now
    Unit unit;
    Pathfinding pf;
    public TMP_Text timeDisplay;
    //TODO: Store the units within the scene in an array
    //"Foreach unit in scenearray": play path, reset positions, etc

    private void Start()
    {
        unit = FindObjectOfType<Unit>();
        pf = FindObjectOfType<Pathfinding>();
    }

    public void StartUnitPathFind()
    {
        if(!unit.isMovingToTarget)
        {
            StartCoroutine(unit.UpdatePath());
            timeDisplay.text = "Time:" + pf.timeToDestination.ToString() + " m/s";
        }
    }

    //Resets unit position to the base position and stops the coroutine
    public void StopAndResetUnit()
    {
        //StopCoroutine(unit.UpdatePath());
        unit.StopAndReset();
        timeDisplay.text = "Time: 00 m/s";
    }
}
