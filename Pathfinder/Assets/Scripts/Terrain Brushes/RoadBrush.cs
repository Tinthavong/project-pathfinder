using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadBrush : MonoBehaviour
{
    public GameObject linePrefab;
    public GameObject currentLine;
    private GameObject meshLine;

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

                //Debug.Log(currentLine.GetComponent<LineRenderer>().positionCount);
                currentLine.transform.rotation = Quaternion.LookRotation(Vector3.down);

                meshLine = Instantiate(currentLine, Vector3.zero, Quaternion.identity);
                meshLine.transform.parent = currentLine.transform;
                meshLine.transform.localPosition = Vector3.zero;
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
        currentLine = Instantiate(linePrefab, new Vector3(0, 0.9f, 0), Quaternion.LookRotation(Vector3.down)); //display line
        lineRenderer = currentLine.GetComponent<LineRenderer>();
        mousePositions.Clear();
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
