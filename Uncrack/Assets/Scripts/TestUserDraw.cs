using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using UnityEngine;

public class TestUserDraw : MonoBehaviour
{
    public RectTransform cementPrefab;

    
    public List<LineRenderer> crackLines;
    private List<LineRenderer> userLines;
    private LinkedList<Vector3> crackPoints;
    private LinkedList<Vector3> userPoints;

    private void Start()
    {
        
    }
    
    public float checksPerSecond = 2;
    public float maxCheckTime;

    private int currentChecksCount;
    private bool runCheck;
    private float elapsed; // since check start

    private int success;
    private int total;
    public float winPercentThreshold;
    void Update()
    {
        if (!runCheck)
        {
            return;
        }

        elapsed += Time.deltaTime;
        if (elapsed > maxCheckTime)
        {
            LevelLost();
            return;
        }
        
        int nextChecksCount = (int) (elapsed / checksPerSecond);

        for (int i = currentChecksCount; i < nextChecksCount; i++)
        {
            var userPoint = userPoints.First.Value;
            CreateCementAt(userPoint);
            if (IsOnCrack(userPoint))
            {
                success++;
            }
            total++;
        }

        currentChecksCount = nextChecksCount;

        if (currentChecksCount > userPoints.Count)
        {
            
        }
    }

    private bool IsOnCrack(Vector3 userPoint)
    {
        throw new NotImplementedException();
    }

    private void CreateCementAt(Vector3 userPoint)
    {
        throw new NotImplementedException();
    }

    private void LevelLost()
    {
        Debug.Log("LEVEL LOST");
    }


    private void OnMouseDown()
    {
        userLines = DrawLine.drawnLines;

        crackPoints = extractPoints(crackLines);
        userPoints = extractPoints(userLines);
        currentChecksCount = 0;
        runCheck = true;
        elapsed = 0F;
        success = 0;
        total = 0;
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
}
