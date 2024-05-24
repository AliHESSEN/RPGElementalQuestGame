using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Condition
{
    public HarmfulEffectID Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string ShowConditionMessageInBattle { get; set; }

    public Action<Element> OnStart { get; set; }

    public Func<Element, bool> OnBeforeMove { get; set; }

    public Action<Element> WhenTurnIsDone { get; set; }


}
