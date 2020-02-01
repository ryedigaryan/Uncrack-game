using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnTestUser : MonoBehaviour
{

    public GameObject drawPanelObject;
    public GameObject doneButton;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        DestroyImageOnCavas(drawPanelObject);
        DestroyImageOnCavas(doneButton);
    }

    private void DestroyImageOnCavas(GameObject go)
    {
        Destroy(go.GetComponent<CanvasRenderer>());
        Destroy(go.GetComponent<Image>());
    }
}
