using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;


public class BattleHud : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI nameText; // for names
    [SerializeField] TextMeshProUGUI levelText; // for level
    [SerializeField] TextMeshProUGUI statusText; // for status conditions
    [SerializeField] HPBar hpBar;
    [SerializeField] GameObject expBar;

    [SerializeField] Color disColor; // disease
    [SerializeField] Color ignColor; // ignite
    [SerializeField] Color ptrColor; // petrified
    [SerializeField] Color slwColor; // slow
    [SerializeField] Color colColor; // cold



    Element _element;
    Dictionary<HarmfulEffectID, Color> conditionEffectColors;


    //defines data
    public void DefineData(Element element)
    {

        _element = element;

        nameText.text = element.Base.Name;
        FunctionToSetElementLevel();
        hpBar.FunctionToSetElementsHealth((float)element.HP / element.MaxHp);
        FunctionForExp();

        conditionEffectColors = new Dictionary<HarmfulEffectID, Color>()
        {
            {HarmfulEffectID.dis, disColor },
            {HarmfulEffectID.ign, ignColor },
            {HarmfulEffectID.ptr, ptrColor },
            {HarmfulEffectID.slw, slwColor },
            {HarmfulEffectID.col, colColor },
        };

        ThisFunctionSetsConditionText();
        _element.OnStatusChanged += ThisFunctionSetsConditionText;

    }

    // sets conditions
    void ThisFunctionSetsConditionText()
    {
        if (_element.Status == null)
        {
            statusText.text = "";
        }
        else
        {
            statusText.text = _element.Status.Id.ToString().ToUpper();
            statusText.color = conditionEffectColors[_element.Status.Id];
        }

        
    }

    public void FunctionToSetElementLevel()
    {
        levelText.text = "Lvl" + _element.Level; //get the level

    }

    public void FunctionForExp() // scale the experience based on the scaled experience value
    {
        if (expBar == null) return;

        float expVal = FunctionToGetScaledEXp(); // Get the scaled experience value.
        expBar.transform.localScale = new Vector3(expVal, 1, 1); // Update the scale of the experience bar using the calculated value

    }

    public IEnumerator FunctionForExpAnimation(bool notFull=false) // animation for experience bar 
    {
        if (expBar == null) yield break;

        if (notFull) // If notFull is true then we set the initial scale to zero
            expBar.transform.localScale = new Vector3(0, 1, 1);

        float expValue = FunctionToGetScaledEXp(); // get the scaled experience value
        yield return expBar.transform.DOScaleX(expValue, 1.5f).WaitForCompletion(); // start the animation for exp bar

    }

    float FunctionToGetScaledEXp()
    {
        int elementalLevelExp = _element.Base.FunctionTogetExp(_element.Level);
        int elementalNextLevelUpExp = _element.Base.FunctionTogetExp(_element.Level + 1);

        float elemExp = (float) (_element.Exp - elementalLevelExp) / (elementalNextLevelUpExp - elementalLevelExp);
        return Mathf.Clamp01(elemExp);
    }
    //This will update the HP of the Element in battle
    public IEnumerator FunctionToUpdateElementalsHP()
    {
        if (_element.ChangeInHealth)
        {
            yield return hpBar.FunctionForHPBarAnimation((float)_element.HP / _element.MaxHp);
            _element.ChangeInHealth = false;
        }
    }
}
