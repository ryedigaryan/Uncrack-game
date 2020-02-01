using System.Collections;
using System.Collections.Generic;
using System.Timers;
using TMPro;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UI;

public class ControlTimer : MonoBehaviour
{
    // to use for timer
    public TextMeshProUGUI timerText;
    public float timerInitialValue;

    // to enable
    public Image paper;
    public Image runButton;
    public Image nextLevelButton;
    public Image paperBackground;
    
    // to disable
    public SpriteRenderer crackUI;

    private float timerValue;
    
    // Start is called before the first frame update
    void Start()
    {
        timerValue = timerInitialValue;
        timerText.text = timerValue.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        timerValue -= Time.deltaTime;
        timerText.text = ((int)timerValue).ToString();

        if (timerValue <= 0)
        {
            ActivateWhatIsNecessary();
            DeactivateWhatIsNecessary();
            Destroy(timerText.gameObject);
        }
    }

    private void DeactivateWhatIsNecessary()
    {
        crackUI.gameObject.SetActive(true);
        crackUI.enabled = false;
    }

    private void ActivateWhatIsNecessary()
    {
        paper.gameObject.SetActive(true);
        paper.enabled = true;
        runButton.gameObject.SetActive(true);
        runButton.enabled = true;
        nextLevelButton.gameObject.SetActive(true);
        nextLevelButton.enabled = (true);
        paperBackground.gameObject.SetActive(true);
        paperBackground.enabled = true;
    }
}
