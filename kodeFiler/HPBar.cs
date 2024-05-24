using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPBar : MonoBehaviour
{
    [SerializeField] GameObject health;



    public void FunctionToSetElementsHealth(float scaledHealth)
    {
        health.transform.localScale = new Vector3(scaledHealth, 1f);  //This will sett the HP 
    }

    //This will show animation on the hp bar
    public IEnumerator FunctionForHPBarAnimation(float elemHealth)
    {
        float startingElemHealth = health.transform.localScale.x;
        float changedElementalHealth = startingElemHealth - elemHealth;

        while (startingElemHealth - elemHealth > Mathf.Epsilon)
        {
            startingElemHealth -= changedElementalHealth * Time.deltaTime;
            health.transform.localScale = new Vector3(startingElemHealth, 1f);
            yield return null;
        }
        health.transform.localScale = new Vector3(elemHealth, 1f);


    }
}
