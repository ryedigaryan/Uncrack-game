using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoToNextLevel : MonoBehaviour
{
    public Canvas currentLvl;
    public Canvas nextLvl;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void switchLevel()
    {
        currentLvl.gameObject.SetActive(false);
        currentLvl.enabled = false;
        
        
        DrawLine.drawnLines = new List<LineRenderer>();
        
        foreach (var c in TestUserDraw.cements)
        {
            Destroy(c);
        }
        TestUserDraw.cements = new List<GameObject>();
        
        nextLvl.gameObject.SetActive(true);
        nextLvl.enabled = true;
    }
}
