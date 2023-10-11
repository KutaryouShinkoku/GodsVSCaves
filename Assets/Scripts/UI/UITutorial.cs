using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITutorial : MonoBehaviour
{
    public Text tutorialConfirm;
    [SerializeField] GameObject uiTutorial;
    [SerializeField] GameObject uiStep1;
    [SerializeField] GameObject uiStep2;
    [SerializeField] GameObject uiStep3;

    private void Start()
    {
        //uiTutorial.SetActive(false);
        tutorialConfirm.text = ($"{Localize.GetInstance().GetTextByKey("TutorialConfirm_disc")}").Replace("\\n", "\n"); 
    }

    public void OpenTutorial()
    {
        uiTutorial.SetActive(true);
        uiStep1.SetActive(true);
        uiStep2.SetActive(false);
        uiStep3.SetActive(false);
    }

    public void CloseTutorial()
    {
        uiTutorial.SetActive(false);
    }

    public void ContinueTutorial()
    {
        uiStep1.SetActive(false);
        uiStep2.SetActive(true);
        uiStep3.SetActive(false);
    }
    public void ShowAdds()
    {
        uiStep1.SetActive(false);
        uiStep2.SetActive(false);
        uiStep3.SetActive(true);
    }
}
