using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;



public class BattleUnit : MonoBehaviour
{
    
    [SerializeField] bool playerElement; // to check if its player element or not
    [SerializeField] BattleHud bothHuds; // huds used for player and enemy units

    public bool CheckForPlayerUnit {
        get { return playerElement;  }

    }

    public BattleHud BothHuds {
        get { return bothHuds; }

    }

    public Element Element { get; set; }

    Image image;
    Vector3 startingLocation;
    Color imageOrgColor;

    private void Awake()
    {
        image = GetComponent<Image>();
        startingLocation = image.transform.localPosition;
        imageOrgColor = image.color;
    }

    public void FunctionToDefineElements(Element element)
    {
        Element = element; // Assign the created Element to the Element property
        if (playerElement)
            GetComponent<Image>().sprite = Element.Base.ElemImageBack; // this will show the backside of the spirte
        else
            GetComponent<Image>().sprite = Element.Base.ElemImageFront; // front

        bothHuds.gameObject.SetActive(true); // enabeling the hud
        bothHuds.DefineData(element);

        image.color = imageOrgColor;
        FunctionToShowElementStartAnimation();

    }

    public void FunctionToDisableHudInBattle() // used to diable the hud in battle

    {

        bothHuds.gameObject.SetActive(false); // disabeling the hud

    }

    public void FunctionToShowElementStartAnimation()
    {
        if (playerElement)
            image.transform.localPosition = new Vector3(-500f, startingLocation.y);
        else
            image.transform.localPosition = new Vector3(-500f, startingLocation.y);

        image.transform.DOLocalMoveX(startingLocation.x, 1f);
    
    }

    public void FunctionToShowAttackAnimation() // attack animations
    {
        var series = DOTween.Sequence();
        if (playerElement)
            series.Append(image.transform.DOLocalMoveX(startingLocation.x + 50f, 0.25f));
        else
            series.Append(image.transform.DOLocalMoveX(startingLocation.x - 50f, 0.25f));

        series.Append(image.transform.DOLocalMoveX(startingLocation.x, 0.25f));

    }

    public void FunctionToShowHitAnimation() // hit animations
    {
        var series = DOTween.Sequence();
        series.Append(image.DOColor(Color.gray, 0.1f));
        series.Append(image.DOColor(imageOrgColor, 0.1f));
    }

    public void FunctionToShowDeathAnimation() // death animation for elements
    {
        var series = DOTween.Sequence();
        series.Append(image.transform.DOLocalMoveY(startingLocation.y - 150f, 0.5f));
        series.Join(image.DOFade(0f, 0.5f));
    }

}
