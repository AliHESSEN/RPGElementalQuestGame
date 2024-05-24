using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TeamDisplayer : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI infoText;

    ElementTeam[] elementTeamList;
    List<Element> elements;

    public void Init()
    {

        elementTeamList = GetComponentsInChildren<ElementTeam>();
    }
   
    public void functionToSetUpelementalTeamData(List<Element> elements)
    {
        this.elements = elements;

        for (int i = 0; i < elementTeamList.Length; i++)
        {
            if (i < elements.Count)
                elementTeamList[i].FunctionToSetDataValues(elements[i]);
            else
                elementTeamList[i].gameObject.SetActive(false); // turning off team list
        }

        infoText.text = "Pick your Element!"; // text that will be shown in team list screen
    }

    public void ElementPickFunction(int pickedElement) // highlights the selected element
    {
        for (int i = 0; i < elements.Count; i++) // loop through each element
        {
            if (i == pickedElement)
                elementTeamList[i].funtionToSelect(true);
            else
                elementTeamList[i].funtionToSelect(false);
        }
        
    }

    public void FunctionToShowInfoText(string infText)
    {
        infoText.text = infText;
    }

}
