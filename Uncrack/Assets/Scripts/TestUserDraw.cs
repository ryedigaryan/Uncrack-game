using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TestUserDraw : MonoBehaviour
{
    
    private RectTransform drawPanelTransform;
    public GameObject linePrefab;
    public GameObject cementPrefab;
    public GameObject drawPanelObject;
    public GameObject paperBackground;
    public GameObject doneButton;
    public float cementRadius = 8;
    public Image nextButton;
    public Image shpaklee;
    public TextMeshProUGUI coveragePercTxt;

    public SpriteRenderer crackUI;

    public List<LineRenderer> crackLines;
    private List<LineRenderer> cementLines;
    private LinkedList<Vector3> crackPoints;
    private LinkedList<Vector3> cementPoints;
    private LinkedListNode<Vector3> currentCementPointNode;

    private void Start()
    {
        _camera = Camera.main;
        drawPanelTransform = (RectTransform) drawPanelObject.transform;
        coveragePercTxt.text = $"0% / {winPercentThreshold}%";
    }

    public float checksPerSecond = 2;
    public float maxCheckTimeSeconds;

    private int currentChecksCount;
    private bool runCheck;
    private float elapsed; // since check start

    private int total;
    public float winPercentThreshold;
    private Camera _camera;
    public Image restartLevel;

    void Update()
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
            var shpakleePoint = cementPoint + new Vector3(0.5F, -0.5F);
            if (i == 0)
            {
                // Debug.Log("shpaklee id: " + System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(shpaklee));
                // Debug.Log("this id: " + System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(this));
                ShowShpakleeAt(shpakleePoint);
            }
            else
            {
                MoveSpakleeTo(shpakleePoint);
            }
            CreateCementAt(cementPoint);
            RemoveCrackPoint(cementPoint);
        }

        coveragePercTxt.text = $"{((int) getCoveragePercent())}% / {winPercentThreshold}%";
        Debug.Log("coverage: " + coveragePercTxt.text);

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

    private void HideShpaklee()
    {
        // Debug.Log("Hide Shpaklee");
        shpaklee.gameObject.SetActive(false);
        shpaklee.enabled = false;
    }

    private void ShowShpakleeAt(Vector3 point)
    {
        // Debug.Log("Show Shpaklee");
        shpaklee.gameObject.SetActive(true);
        shpaklee.enabled = true;
        shpaklee.transform.position = (Vector2) point;
    }

    private void MoveSpakleeTo(Vector3 point)
    {
        // Debug.Log("Move Shpaklee");
        shpaklee.transform.position = (Vector2) point;
    }

    private void CheckCementCoverage()
    {
        float coveragePercent = getCoveragePercent();
        Debug.Log("prc: " + coveragePercent);
        
        if (coveragePercent >= winPercentThreshold)
        {
            LevelWon();
        }
        else
        {
            LevelLost(LostCause.NOT_ENOUGH_COVERAGE);
        }
        
        Destroy(gameObject);
    }

    private float getCoveragePercent()
    {
        return 100 - (crackPoints.Count / (float) total * 100);
    }

    private void LevelWon()
    {
        OnLevelEnd();
        nextButton.gameObject.SetActive(true);
        nextButton.enabled = true;
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
        return Vector2.Distance(cementPosition, crackPosition) <= cementRadius;
        // cementRect.x = cementPosition.x;
        // cementRect.y = cementPosition.y;
        // return cementRect.Contains((Vector2)crackPosition);
    }


    public static List<GameObject> cements = new List<GameObject>();
    private void CreateCementAt(Vector3 cementPoint)
    {
        var go = Instantiate(cementPrefab, cementPoint, Quaternion.identity);
        cements.Add(go);
    }


    enum LostCause
    {
        NOT_ENOUGH_CEMENT,
        NOT_ENOUGH_COVERAGE
    }

    private void OnLevelEnd()
    {
        HideShpaklee();
    }
    
    private void LevelLost(LostCause cause)
    {
        OnLevelEnd();
        Debug.Log("Lost: " + cause);
        restartLevel.gameObject.SetActive(true);
        restartLevel.enabled = true;
    }

    private void HideWhatIsNeeded()
    {
        DestroyImageOnCanvas(paperBackground);
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

    LinkedList<Vector3> extractPoints(List<LineRenderer> lines, bool useWorldPosition)
    {
        var points = new LinkedList<Vector3>();
        foreach (var line in lines)
        {
            var pointsOnLine = extractPoints(line, useWorldPosition);
            foreach (var pol in pointsOnLine)
            {
                points.AddLast(pol);
            }
        }

        return points;
    }

    Vector3[] extractPoints(LineRenderer lr, bool useWorldPosition)
    {
        Vector3[] linePoints = new Vector3[lr.positionCount];
        lr.GetPositions(linePoints);
        for (var i = 0; i < linePoints.Length; i++)
        {
            var p = linePoints[i];
            p.z = 1;
            if (useWorldPosition)
            {
                p += lr.gameObject.transform.position;
            }
            else
            {
                p += lr.gameObject.transform.localPosition;
            }

            linePoints[i] = p;
        }


        return linePoints;
    }

    public void onUserDoneClick()
    {
        cementLines = DrawLine.drawnLines;

        crackPoints = extractPoints(crackLines, false);
        cementPoints = extractPoints(cementLines, true);
        currentChecksCount = 0;
        elapsed = 0F;
        total = crackPoints.Count;

        ConvertToWorldPoint(crackPoints);
         // printCementAndCrackPoints();
        InterpolateCementPoints();
        // printCementAndCrackPoints();
        currentCementPointNode = cementPoints.First;

        HideWhatIsNeeded();
        EnableObjectsForResults();
        
        
        // bugaga
        foreach (var c in crackPoints)
        {
            var go = Instantiate(linePrefab, c, Quaternion.identity);
            go.name += "_CRACK";
            cements.Add(go);
        }
        
        runCheck = true;
    }

    private void EnableObjectsForResults()
    {
        coveragePercTxt.gameObject.SetActive(true);
        coveragePercTxt.enabled = true;
        crackUI.gameObject.SetActive(true);
        crackUI.enabled = true;
    }

    
    public static void cleanup()
    {
        foreach (var c in cements)
        {
            Destroy(c);
        }

        cements = new List<GameObject>();
    }
}