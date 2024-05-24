using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionsDB : MonoBehaviour
{
    public static void Init()
    {
        foreach (var harmEffect in Conditions)
        {
            var conditionId = harmEffect.Key;
            var condition = harmEffect.Value;

            condition.Id = conditionId;
        }
    }

    public static Dictionary<HarmfulEffectID, Condition> Conditions { get; set; } = new Dictionary<HarmfulEffectID, Condition>()
    {
        {
            HarmfulEffectID.dis,
            new Condition()
            {
                Name = "Disease",
                ShowConditionMessageInBattle = "has been affected with Disease",
                WhenTurnIsDone = (Element element) =>
                {
                    element.FunctionToUpdateHealth(element.MaxHp / 8);
                    element.ChangeInStatus.Enqueue($"{element.Base.Name} Is suffering damage from the disease");
                }

            }
        },
        {
            HarmfulEffectID.ign,
            new Condition()
            {
                Name = "Ignite",
                ShowConditionMessageInBattle = "has been set a blaze and is now Ignited",
                WhenTurnIsDone = (Element element) =>
                {
                    element.FunctionToUpdateHealth(element.MaxHp / 16);
                    element.ChangeInStatus.Enqueue($"{element.Base.Name} Is taking damage from the Ignite burn");
                }

            }
        },
        {
            HarmfulEffectID.slw,
            new Condition()
            {
                Name = "Slowed",
                ShowConditionMessageInBattle = "has been Slowed",
                OnBeforeMove = (Element element) =>
                {
                    if (Random.Range(1, 5 ) == 1)
                    {
                        element.ChangeInStatus.Enqueue($"{element.Base.Name} Is too Slow");
                        return false;
                    }

                    return true;
                }

            }
        },
        {
            HarmfulEffectID.col,
            new Condition()
            {
                Name = "Cold",
                ShowConditionMessageInBattle = "Has the body filled with ice",
                OnBeforeMove = (Element element) =>
                {
                    if (Random.Range(1, 5 ) == 1)
                    {
                        element.FunctionToHealStatus();
                        element.ChangeInStatus.Enqueue($"{element.Base.Name} Has removed the ice from the body");
                        return true;
                    }

                    return false;
                }

            }
        },
        {
            HarmfulEffectID.ptr,
            new Condition()
            {
                Name = "Petrified",
                ShowConditionMessageInBattle = "has been turned petrified and turned to stone",
                OnStart = (Element element) =>
                {
                    //sleep for 1-3 turns
                    element.StatusTime = Random.Range(1, 4);
                    Debug.Log($"will be petrfified for {element.StatusTime} moves");
                },
                OnBeforeMove = (Element element) =>
                {
                    if (element.StatusTime <= 0)
                    {
                        element.FunctionToHealStatus();
                        element.ChangeInStatus.Enqueue($"{element.Base.Name} woke up");
                        return true;

                    }

                    element.StatusTime--;
                    element.ChangeInStatus.Enqueue($"{element.Base.Name} is sleeping");
                    return false;
                }

            }
        },

        //volatile sattus conditions
        {
            HarmfulEffectID.confusion,
            new Condition()
            {
                Name = "Madness",
                ShowConditionMessageInBattle = "is affected with madness and is losing their mind",
                OnStart = (Element element) =>
                {
                    //sleep for 1-3 turns
                    element.StatusTime = Random.Range(1, 4);
                    Debug.Log($"will be mad for {element.HarmfulEffectTime} moves");
                },
                OnBeforeMove = (Element element) =>
                {
                    if (element.HarmfulEffectTime <= 0)
                    {
                        element.FunctionToRemoveHarmFulEffects();
                        element.ChangeInStatus.Enqueue($"{element.Base.Name} is not insane");
                        return true;

                    }

                    element.HarmfulEffectTime--;
                    if (Random.Range(1, 3) == 1)
                        return true;

                    element.ChangeInStatus.Enqueue($"{element.Base.Name} is insane");
                    element.FunctionToUpdateHealth(element.MaxHp / 8);
                    element.ChangeInStatus.Enqueue($"It taking damage from the madness");
                    return false;
                }

            }
        }
    };
}

public enum HarmfulEffectID
{
    none, dis, ign, ptr, slw, col, 
    confusion
}