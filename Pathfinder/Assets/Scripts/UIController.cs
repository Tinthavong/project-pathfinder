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

    //First Implementation of brushes
    public WallBrush wallBrushTool;
    public RoadBrush roadBrushTool;

    AStarLoop aStarLoop;
    LineRenderer[] linesInScene;

    public GameObject playPanel;
    public GameObject brushPanel;
    public GameObject tutorialInfo;
    private void Start()
    {
        unit = FindObjectOfType<Unit>();
        pf = FindObjectOfType<Pathfinding>();
        aStarLoop = GetComponent<AStarLoop>();
    }

    public void TurnOnTutorialInformation()
    {
        if(!tutorialInfo.activeSelf)
        {
            tutorialInfo.SetActive(true);
        }
        else
        {
            tutorialInfo.SetActive(false);
        }
    }

    public void StartUnitPathFind()
    {
        if (!unit.isMovingToTarget && !aStarLoop.paintMode)
        {
            tutorialInfo.SetActive(false);
            StartCoroutine(unit.UpdatePath());
            timeDisplay.text = "Time:" + pf.timeToDestination.ToString() + " m/s";
            SetFlagsInBrushTools(false);
        }
    }

    //Resets unit position to the base position and stops the coroutine
    public void StopAndResetUnit()
    {
        //StopCoroutine(unit.UpdatePath());
        if (!aStarLoop.paintMode)
        {
            unit.StopAndReset();
            SetFlagsInBrushTools(true);
            timeDisplay.text = "Time: 00 m/s";
        }
    }

    public void EnterPaintModeButton()
    {
        //If paintmode then play and stop buttons will be grayed out
        if (!aStarLoop.paintMode) //On
        {
            aStarLoop.paintMode = true;
            SetFlagsInPlayButtons(false);
            //Also needs to enable the other painting options OR every paint brush options enters/exits paint mode by default
        }
    }

    public void ExitPaintModeButton()
    {
        if (aStarLoop.paintMode) //Off
        {
            aStarLoop.paintMode = false;
            SetFlagsInPlayButtons(true);
            wallBrushTool.enabled = false;
            roadBrushTool.enabled = false;
        }
    }

    public void WallBrushButton()
    {
        EnterPaintModeButton();
        //If paintmode then play and stop buttons will be grayed out
        if (!wallBrushTool.enabled) //On
        {
            wallBrushTool.enabled = true;
            roadBrushTool.enabled = false;
        }
        else if (wallBrushTool.enabled) //Off
        {
            wallBrushTool.enabled = false;
        }
    }

    public void RoadBrushButton()
    {
        EnterPaintModeButton();
        //If paintmode then play and stop buttons will be grayed out
        if (!roadBrushTool.enabled) //On
        {
            roadBrushTool.enabled = true;
            wallBrushTool.enabled = false;
        }
        else if (roadBrushTool.enabled) //Off
        {
            roadBrushTool.enabled = false;
        }
    }

    public void GenerateMapButton()
    {
        RecycleGrid();
    }

    public void ClearMapButton()
    {
        linesInScene = FindObjectsOfType<LineRenderer>();

        if (linesInScene != null)
        {
            foreach (LineRenderer wb in linesInScene)
            {
                Destroy(wb.gameObject);
            }
        }
        StopAndResetUnit();
        Invoke("RecycleGrid", 0.1f);
    }

    public void RecycleGrid()
    {
        AStarGrid asg = FindObjectOfType<AStarGrid>();
        asg.ClearMapDictionary();
        asg.RegenerateMapDictionary();
    }

    public void SetFlagsInBrushTools(bool condition)
    {
        Button[] targetButtons = brushPanel.GetComponentsInChildren<Button>();

        foreach (Button b in targetButtons)
        {
            b.interactable = condition;
        }
    }

    void SetFlagsInPlayButtons(bool condition)
    {
        Button[] targetButtons = playPanel.GetComponentsInChildren<Button>();
        if (aStarLoop.paintMode)
        {
            foreach (Button b in targetButtons)
            {
                b.interactable = condition;
            }
        }
        else if (!aStarLoop.paintMode)
        {
            foreach (Button b in targetButtons)
            {
                b.interactable = condition;
            }
        }
    }
}
