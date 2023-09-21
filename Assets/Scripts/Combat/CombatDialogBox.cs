using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CombatDialogBox : MonoBehaviour
{
    [SerializeField] int lettersPerSec;
    [SerializeField] Text dialogText;

    public void SetDialog(string dialog)
    {
        dialogText.text = dialog;
    }

    //×ÖÄ¸ÒÀ´Î³öÏÖ
    public IEnumerator TypeDialog(string dialog)
    {
        dialogText.text = "";
        foreach (var letter in dialog .ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(1f / lettersPerSec);
        }
    }

}
