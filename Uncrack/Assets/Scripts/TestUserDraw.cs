using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestUserDraw : MonoBehaviour
{
    public Canvas canvas;
    private RectTransform drawPanelTransform;
    public GameObject cementPrefab;
    public GameObject drawPanelObject;
    public GameObject doneButton;
    public Rect cementRect;

    public List<LineRenderer> crackLines;
    private List<LineRenderer> cementLines;
    private LinkedList<Vector3> crackPoints;
    private LinkedList<Vector3> cementPoints;
    private LinkedListNode<Vector3> currentCementPointNode;

    private void Start()
    {
        _camera = Camera.main;
        drawPanelTransform = (RectTransform) drawPanelObject.transform;
    }

    public float checksPerSecond = 2;
    public float maxCheckTimeSeconds;

    private int currentChecksCount;
    private bool runCheck;
    private float elapsed; // since check start

    private int total;
    public float winPercentThreshold;
    private Camera _camera;

    void FixedUpdate()
    {
        if (!runCheck)
        {
            return;
        }

        elapsed += Time.deltaTime;
        if (elapsed > maxCheckTimeSeconds)
        {
            LevelLost(LostCause.NOT_ENOUGH_CEMENT);
            return;
        }

        int nextChecksCount = (int) (elapsed / checksPerSecond);

        for (int i = currentChecksCount;
            currentCementPointNode != null && i < nextChecksCount;
            currentCementPointNode = currentCementPointNode.Next, i++)
        {
            var cementPoint = currentCementPointNode.Value;
            cementPoint.z = 1;
            // var r = drawPanelTransform.rect;
            // var leftBot = _camera.ScreenToWorldPoint(new Vector3(-r.width / 2, -r.height / 2));
            // var rightTop = _camera.ScreenToWorldPoint(new Vector3(0, 0));
            // var offset = rightTop - leftBot;
            // CreateCementAt(cementPoint + 2 * offset);
            CreateCementAt(cementPoint);
            RemoveCrackPoint(cementPoint);
        }

        currentChecksCount = nextChecksCount;

        if (currentCementPointNode == null) // all user drawn points are processed
        {
            runCheck = false;
            if (crackPoints.Count == 0)
            {
                LevelWon();
            }
            else
            {
                CheckCementCoverage();
            }
        }
    }

    private void CheckCementCoverage()
    {
        float coveragePercent = 100 - (crackPoints.Count / (float) total * 100);
        if (coveragePercent >= winPercentThreshold)
        {
            LevelWon();
        }
        else
        {
            LevelLost(LostCause.NOT_ENOUGH_COVERAGE);
        }
    }

    private void LevelWon()
    {
        Debug.Log("LEVEL WON");
    }

    private void RemoveCrackPoint(Vector3 cementPoint)
    {
        for (var n = crackPoints.First; n != null; n = n.Next)
        {
            if (cementIsOk(cementPoint, n.Value))
            {
                crackPoints.Remove(n);
            }
        }
    }

    private bool cementIsOk(Vector3 cementPosition, Vector3 crackPosition)
    {
        cementRect.x = cementPosition.x;
        cementRect.y = cementPosition.y;
        return cementRect.Contains(crackPosition);
    }

    private void CreateCementAt(Vector3 cementPoint)
    {
        Instantiate(cementPrefab, cementPoint, Quaternion.identity);
    }


    enum LostCause
    {
        NOT_ENOUGH_CEMENT,
        NOT_ENOUGH_COVERAGE
    }

    private void LevelLost(LostCause cause)
    {
        Debug.Log($"LEVEL LOST {cause}");
    }

    private void PrepareCanvasForUnckracking()
    {
        DestroyImageOnCanvas(drawPanelObject);
        DestroyCementLinesOnCanvas();
        DestroyImageOnCanvas(doneButton);
    }

    private void DestroyCementLinesOnCanvas()
    {
        foreach (var l in cementLines)
        {
            Destroy(l);
        }
    }

    private void DestroyImageOnCanvas(GameObject go)
    {
        Destroy(go.GetComponent<Image>());
        Destroy(go.GetComponent<CanvasRenderer>());
    }

    private void ConvertToWorldPoint(LinkedList<Vector3> list)
    {
        for (var n = list.First; n != null; n = n.Next)
        {
            n.Value = _camera.ScreenToWorldPoint(n.Value);
        }
    }

    private void InterpolateCementPoints()
    {
        for (var n = cementPoints.First; n != null; n = n.Next)
        {
            n.Value *= 2;
        }
    }

    private void printCementAndCrackPoints()
    {
        Debug.Log("drawPanelRect size: " + drawPanelTransform.rect.size);

        Debug.Log("cement");
        foreach (var p in cementPoints)
        {
            Debug.Log(p.ToString());
        }

        Debug.Log("crack");
        foreach (var p in crackPoints)
        {
            Debug.Log(p.ToString());
        }
    }

    LinkedList<Vector3> extractPoints(List<LineRenderer> lines)
    {
        var points = new LinkedList<Vector3>();
        foreach (var line in lines)
        {
            var pointsOnLine = extractPoints(line);
            foreach (var pol in pointsOnLine)
            {
                points.AddLast(pol);
            }
        }

        return points;
    }

    Vector3[] extractPoints(LineRenderer lr)
    {
        Vector3[] linePoints = new Vector3[lr.positionCount];
        lr.GetPositions(linePoints);
        for (var i = 0; i < linePoints.Length; i++)
        {
            var p = linePoints[i];
            p.z = 1;
            p += lr.gameObject.transform.position;
            linePoints[i] = p;
        }


        return linePoints;
    }

    public void onUserDoneClick()
    {
        cementLines = DrawLine.drawnLines;

        crackPoints = extractPoints(crackLines);
        cementPoints = extractPoints(cementLines);
        currentChecksCount = 0;
        runCheck = true;
        elapsed = 0F;
        total = crackPoints.Count;

        ConvertToWorldPoint(crackPoints);
        // printCementAndCrackPoints();
        InterpolateCementPoints();
        // printCementAndCrackPoints();
        currentCementPointNode = cementPoints.First;

        PrepareCanvasForUnckracking();
    }
}