using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class ElementTeam : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI nameText; // used for name text
    [SerializeField] TextMeshProUGUI levelText; // used for level text
    [SerializeField] HPBar hpBar; // used for hp
    [SerializeField] Color highlightedColor; // used to highlight when hover over 


    Element _element;

    //Public functions to set values for these three objects
    public void FunctionToSetDataValues(Element element)
    {

        _element = element;

        nameText.text = element.Base.Name;
        levelText.text = "Lvl" + element.Level; //get the level
        hpBar.FunctionToSetElementsHealth((float)element.HP / element.MaxHp);

    }

    public void funtionToSelect(bool picked)
    {
        if (picked)
            nameText.color = highlightedColor;
        else
            nameText.color = Color.black;
    }
}
