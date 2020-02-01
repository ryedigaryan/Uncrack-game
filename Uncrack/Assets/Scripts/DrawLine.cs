using System.Collections.Generic;
using UnityEngine;

public class DrawLine : MonoBehaviour
{
    public static List<LineRenderer> drawnLines = new List<LineRenderer>();
    private LineRenderer curentLineRenderer;

    private GameObject currentLine;
    public RectTransform drawPanel;
    private List<Vector2> fingerPositions;

    public GameObject linePrefab;
    private Camera _camera;


    private void Start()
    {
        _camera = Camera.main;
    }

    private void CreateLine()
    {
        currentLine = Instantiate(linePrefab, Vector3.zero, Quaternion.identity);
        curentLineRenderer = currentLine.GetComponent<LineRenderer>();
        fingerPositions = new List<Vector2>();
        fingerPositions.Add(_camera.ScreenToWorldPoint(Input.mousePosition));
        fingerPositions.Add(_camera.ScreenToWorldPoint(Input.mousePosition));
        curentLineRenderer.SetPosition(0, fingerPositions[0]);
        curentLineRenderer.SetPosition(1, fingerPositions[1]);
    }

    private void Update()
    {
        Vector2 newFingerPos = _camera.ScreenToWorldPoint(Input.mousePosition);

        if (!isInsideDrawPanel(newFingerPos))
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            CreateLine();
            drawnLines.Add(curentLineRenderer);
        }

        if (Input.GetMouseButton(0))
        {
            if (Vector2.Distance(newFingerPos, fingerPositions[fingerPositions.Count - 1]) > 0.1f)
                UpdateLine(newFingerPos);
        }
    }

    private void UpdateLine(Vector2 newFingerPos)
    {
        fingerPositions.Add(newFingerPos);
        curentLineRenderer.positionCount++;
        curentLineRenderer.SetPosition(curentLineRenderer.positionCount - 1, newFingerPos);
    }

    private bool isInsideDrawPanel(Vector2 point)
    {
        var r = drawPanel.rect;
        return point.x > r.x &&
               point.x < r.x + r.width &&
               point.y < r.y + r.height / 2 &&
               point.y > r.y - r.height / 2;
    }
}