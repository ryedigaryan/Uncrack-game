using UnityEngine;
using UnityEngine.UI;

public class PlayPressProcessor : MonoBehaviour
{
    public GameObject ptObject;
    public Image paintingTool;
    public Image playDark;
    public Canvas mainMenu;
    public Canvas lvl1;

    private Vector2 ptStartPos;
    
    public void onPlayPress()
    {
        Debug.Log("enabled");
        paintingTool.enabled = true;
        ptStartPos = paintingTool.transform.position;
    }

    private float progress = 0;
    private void Update()
    {
        if (progress >= 100)
        {
            mainMenu.gameObject.SetActive(false);
            lvl1.gameObject.SetActive(true);
            return;
        }
        if (paintingTool.enabled)
        {
            progress += 200 * Time.deltaTime;
            paintingTool.transform.position = (ptStartPos + (Vector2.right * (float) (5.5 * progress / 100)));
            playDark.fillAmount = progress / 100;
        }
    }
}
