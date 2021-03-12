using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallBrush : MonoBehaviour
{
    public GameObject linePrefab;
    public GameObject currentLine;
    private GameObject meshLine; //Mesh created for the line

    public LineRenderer lineRenderer;
    public List<Vector2> mousePositions;

    public Camera paintCamera;

    public MeshCollider meshCollider;
    public float lineLength;

    Vector3 initialClickPos;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            initialClickPos = Input.mousePosition;
            CreateLine();
        }
        if (Input.GetMouseButtonUp(0))
        {
            Vector3 mouseUpPosition = Input.mousePosition;
            lineLength = Vector3.Distance(initialClickPos, mouseUpPosition);
            Debug.Log(lineLength);

            if (lineLength > 2 && currentLine.GetComponent<LineRenderer>().positionCount > 2)
            {
                meshCollider = lineRenderer.gameObject.AddComponent<MeshCollider>();
                Mesh mesh = new Mesh();
                lineRenderer.BakeMesh(mesh, true);
                meshCollider.sharedMesh = mesh;

                currentLine.transform.rotation = Quaternion.LookRotation(Vector3.down);

                meshLine = Instantiate(currentLine, Vector3.zero, Quaternion.identity);
                meshLine.transform.parent = currentLine.transform;
                Destroy(meshLine.GetComponent<LineRenderer>());
                Destroy(currentLine.GetComponent<MeshCollider>());

                Invoke("RefreshMapDictionary", 0.5f); //This might depend on the machine but the mapdictionary needs time to render for roads

            }
            else
            {
                Destroy(currentLine);
            }
        }

        if (Input.GetMouseButton(0) && lineLength > 0)
        {
            Vector2 tempMousePos = paintCamera.ScreenToWorldPoint(Input.mousePosition);
            if (Vector2.Distance(tempMousePos, mousePositions[mousePositions.Count - 1]) > 0.1f)
            {
                UpdateLine(tempMousePos);
            }
        }
    }
    private void RefreshMapDictionary()
    {

        AStarGrid asg = FindObjectOfType<AStarGrid>();
        asg.ClearMapDictionary();
        asg.RegenerateMapDictionary();
    }

    void CreateLine()
    {
        // currentLine = Instantiate(linePrefab, Vector3.zero, Quaternion.LookRotation(-Vector3.up));
        //currentLine = Instantiate(linePrefab, Vector3.zero, Quaternion.identity);
        currentLine = Instantiate(linePrefab, Vector3.up, Quaternion.LookRotation(Vector3.down)); //display line
        lineRenderer = currentLine.GetComponent<LineRenderer>();
        // edgeCollider = currentLine.GetComponent<EdgeCollider2D>();
        mousePositions.Clear();
        // mousePositions.Add(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        //mousePositions.Add(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        mousePositions.Add(paintCamera.ScreenToWorldPoint(Input.mousePosition));
        mousePositions.Add(paintCamera.ScreenToWorldPoint(Input.mousePosition));
        lineRenderer.SetPosition(0, mousePositions[0]);
        lineRenderer.SetPosition(1, mousePositions[1]);
        lineLength++;
    }

    void UpdateLine(Vector2 newMousePos)
    {
        mousePositions.Add(newMousePos);
        lineRenderer.positionCount++;
        lineRenderer.SetPosition(lineRenderer.positionCount - 1, newMousePos);
    }
}
