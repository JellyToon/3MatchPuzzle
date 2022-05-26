using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Title : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public CanvasGroup canvasGroup1;


    private void Start()
    {
        Screen.SetResolution(1600, 900, false);
    }
    public void StageLoad(int i)
    {
        string stage = "Stage" + i.ToString();

        SceneManager.LoadScene(stage);
    }

    public void ClickStartButton()
    {
        canvasGroup.alpha = 1.0f;
        canvasGroup.blocksRaycasts = true;
    }

    public void ClickManualButton()
    {
        canvasGroup1.alpha = 1.0f;
        canvasGroup1.blocksRaycasts = true;
    }

    public void ClickManualExitButton()
    {
        canvasGroup1.alpha = 0.0f;
        canvasGroup1.blocksRaycasts = false;
    }


    public void ClickExitButton()
    {
        Application.Quit();
    }

    public void ClickReStart()
    {
        SceneManager.LoadScene("Title");
    }
}
