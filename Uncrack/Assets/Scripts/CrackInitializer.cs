using System.IO;
using UnityEditor;
using UnityEngine;

public class CrackInitializer : MonoBehaviour
{
    public Transform tr;
    public GameObject earthPrefab;

    public LineRenderer lr;

    // Start is called before the first frame update
    void Start()
    {
        Vector3[] linePoints = new Vector3[lr.positionCount];
        lr.GetPositions(linePoints);
        string s = "";
        for (var i = 0; i < linePoints.Length; i++)
        {
            var p = linePoints[i];
            p.z = 1;
            p += lr.gameObject.transform.position;
            s += $"{p} ";

            // var go = Instantiate(earthPrefab, Vector3.zero, Quaternion.identity);
            // go.transform.position = p;
        }
    }

    // Update is called once per frame
    void Update()
    {


    }
}