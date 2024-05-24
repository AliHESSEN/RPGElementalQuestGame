using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script defines an AbilityCore class, which is a ScriptableObject. 
// ScriptableObjects in Unity are data containers that can be used to save large amounts of data, independent of class instances.

[CreateAssetMenu(fileName = "Move", menuName = "Element/Create new move")]
public class AbilityCore : ScriptableObject
{
    // Serialized fields to be set in the Unity Editor
    [SerializeField] string name; // Name of the ability

    [TextArea]
    [SerializeField] string abilityInfo; // Description of the ability

    [SerializeField] ElementType type; // The elemental type of the ability (e.g., Fire, Water)
    [SerializeField] int damage; // Damage value of the ability
    [SerializeField] int hitRating; // Chance of the ability hitting the target
    [SerializeField] int ap; // Number of times the move can be performed
    [SerializeField] AbilityCategory abilityCategory; // Category of the ability (Physical, Magic, Status)
    [SerializeField] AbilityEffects abilityEffects; // Effects associated with the ability
    [SerializeField] AbilityTarget abilityTarget; // Target of the ability (Foe, Self)

    // Properties to access the private fields
    public string Name
    {
        get { return name; }
    }

    public string AbilityInfo
    {
        get { return abilityInfo; }
    }

    public ElementType Type
    {
        get { return type; }
    }

    public int Damage
    {
        get { return damage; }
    }

    public int HitRating
    {
        get { return hitRating; }
    }

    public int Ap
    {
        get { return ap; }
    }

    public AbilityCategory AbilityCategory
    {
        get { return abilityCategory; }
    }

    public AbilityEffects AbilityEffects
    {
        get { return abilityEffects; }
    }

    public AbilityTarget AbilityTarget
    {
        get { return abilityTarget; }
    }
}

// A class to define the effects of an ability
[System.Serializable]
public class AbilityEffects
{
    [SerializeField] List<StatBoost> buffs; // List of stat boosts provided by the ability
    [SerializeField] HarmfulEffectID status; // Status effect caused by the ability
    [SerializeField] HarmfulEffectID harmfulEffect; // Harmful effect caused by the ability

    // Properties to access the private fields
    public List<StatBoost> Buffs
    {
        get { return buffs; }
    }

    public HarmfulEffectID Status
    {
        get { return status; }
    }

    public HarmfulEffectID HarmfulEffect
    {
        get { return harmfulEffect; }
    }
}

// A class to define stat boosts
[System.Serializable]
public class StatBoost
{
    public Stat stat; // The stat that is boosted
    public int boost; // The amount by which the stat is boosted
}

// Enum to define the category of an ability
public enum AbilityCategory
{
    Physical, Magic, Status
}

// Enum to define the target of an ability
public enum AbilityTarget
{
    Foe, Self
}

