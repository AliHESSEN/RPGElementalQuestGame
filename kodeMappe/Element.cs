using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable] // to show classes in the inspector
public class Element
{

    // making serializeField for element 

    [SerializeField] ElementCore _base; // the base for element
    [SerializeField] int level; // the level


    public ElementCore Base 
    { 
        get { return _base; }
    
    } 

    public int Level 
    { 
        get { return  level; }
    } 

    public int Exp { get; set; }

    public int HP { get; set; }

    public List<Ability> Ability { get; set; }
    public Dictionary<Stat, int> Stats { get; private set; }

    public Dictionary<Stat, int> StatBoosts { get; private set; }

    public Condition Status { get; private set; }
    public int StatusTime { get; set; }

    public Condition HarmfulEffect { get; private set; }
    public int HarmfulEffectTime { get; set; }

    public Ability AbilityiCurrent { get; set; }

    public Queue<string> ChangeInStatus { get; private set; } = new Queue<string>();
    public bool ChangeInHealth { get; set; }
    public event System.Action OnStatusChanged;

    public void FunctionToInitializElementalMoves() // function to inti moves / abilities for elementeals
    {

        //Generate moves
        Ability = new List<Ability>();
        foreach (var move in Base.LearnableMoves)
        {
            if (move.Level <= Level)
                Ability.Add(new Ability(move.Base));

            if (Ability.Count >= 4)
                break;
        }

        Exp = Base.FunctionTogetExp(level);

        functionForStatsCalculation();
        HP = MaxHp;

        StatResetFunction();
        Status = null;
        HarmfulEffect = null;

    }

    void functionForStatsCalculation() // calculate stats
    {
        Stats = new Dictionary<Stat, int>();
        Stats.Add(Stat.Attack, Mathf.FloorToInt((Base.Attack * Level) / 100f) + 5 );
        Stats.Add(Stat.Defense, Mathf.FloorToInt((Base.Attack * Level) / 100f) + 5 );
        Stats.Add(Stat.Magic, Mathf.FloorToInt((Base.Attack * Level) / 100f) + 5 );
        Stats.Add(Stat.MagicDefence, Mathf.FloorToInt((Base.Attack * Level) / 100f) + 5 );
        Stats.Add(Stat.Speed, Mathf.FloorToInt((Base.Attack * Level) / 100f) + 5 );
    
        MaxHp = Mathf.FloorToInt((Base.Speed * Level) / 100f) + 10 + Level; 
    }

    void StatResetFunction() // function to reset stats boosts
    {

        StatBoosts = new Dictionary<Stat, int>()
        {
            {Stat.Attack, 0 },
            {Stat.Defense, 0 },
            {Stat.Magic, 0 },
            {Stat.MagicDefence, 0 },
            {Stat.Speed, 0 },
        };
    }

    int FunctionToGetElementalStats(Stat stat)
    {
        int statVal = Stats[stat];

        int boost = StatBoosts[stat];
        var boostValues = new float[] { 1f, 1.5f, 2f, 2.5f, 3f, 3.5f, 4f };

        if (boost >= 0)
            statVal = Mathf.FloorToInt(statVal + boostValues[boost]);
        else
            statVal = Mathf.FloorToInt(statVal / boostValues[-boost]);


        return statVal;
    }

    public void FunctionToAddBoosts(List<StatBoost> elementalStatBoost)
    {
        foreach (var statBoost in elementalStatBoost)
        {
            var statNumbers = statBoost.stat;
            var boostNumbers = statBoost.boost;

            StatBoosts[statNumbers] = Mathf.Clamp(StatBoosts[statNumbers] + boostNumbers, -6, 6);

            if (boostNumbers > 0)
                ChangeInStatus.Enqueue($"{Base.Name}'s {statNumbers} rose!");
            else
                ChangeInStatus.Enqueue($"{Base.Name}'s {statNumbers} fell!");

            Debug.Log($"{statNumbers} has been boosted to {StatBoosts[statNumbers]}");
        }
    }

    //calcute stats for element in current level

    public bool FunctionForLevelUpCheck()
    {
        if (Exp > Base.FunctionTogetExp(level + 1))
        {
            ++level;
            return true;
        }

        return false;
    }
    public int Attack
    {
        get { return FunctionToGetElementalStats(Stat.Attack);  } 
    }
    public int Defense
    {
        get { return FunctionToGetElementalStats(Stat.Defense); }
    }
    public int Magic
    {
        get { return FunctionToGetElementalStats(Stat.Magic); }
    }
    public int MagicDefence
    {
        get { return FunctionToGetElementalStats(Stat.MagicDefence); }
    }
    public int Speed {
        get { 
            return FunctionToGetElementalStats(Stat.Speed); 
        }
    }


    public int MaxHp { get; private set; }
    

    //Function for taking damage
    public DamageInformation FunctionForTakingDamage(Ability ability, Element attacker)
    {


        // adding chance to get crits
        float critChance = 1f;
        bool crit = false;
        if (Random.value * 100f <= 7.23f)
            critChance = 2f;
        

        // using the effectivness function from the table
        //skill1 and 2 is the type of elements (need to change name later) and using two because an element can have two types
        float typeEffectivness = TypeTable.GetElementalEffectiveness(ability.Core.Type, this.Base.type1) * TypeTable.GetElementalEffectiveness(ability.Core.Type, this.Base.type2);

        var damageInformation = new DamageInformation()
        {
            Type = typeEffectivness,
            Critical = crit,
            Fainted = false
        };

        float attack = (ability.Core.AbilityCategory == AbilityCategory.Magic) ? attacker.Magic : attacker.Attack;
        float defense = (ability.Core.AbilityCategory == AbilityCategory.Magic) ? MagicDefence: Defense;
     

        //This is a formula for taking damage @ bulbapedia with type effectivness and crit chance implemented
        float modifiers = Random.Range(0.85f, 1f) * typeEffectivness * critChance;
        float atk = (2 * attacker.Level + 10) / 250f;
        float def = atk * ability.Core.Damage * ((float)attacker.Attack / Defense) + 2;
        int damage = Mathf.FloorToInt(def * modifiers);

        FunctionToUpdateHealth(damage);

        // return false;
        return damageInformation;

    }

    public void FunctionToUpdateHealth(int damageTaken)
    {
        HP = Mathf.Clamp(HP - damageTaken, 0, MaxHp);
        ChangeInHealth = true;

    }

    public void FunctionForStatus(HarmfulEffectID statusVar)
    {
        if (Status != null) return;

        Status = ConditionsDB.Conditions[statusVar];
        Status?.OnStart?.Invoke(this);
        ChangeInStatus.Enqueue($"{Base.Name} {Status.ShowConditionMessageInBattle}");
        OnStatusChanged?.Invoke();
    }

    public void FunctionToHealStatus()
    {
        Status = null;
        OnStatusChanged?.Invoke();
    }

    public void functionToSetHarmfulEffects(HarmfulEffectID harmfulEffects)
    {
        if (HarmfulEffect != null) return;

        HarmfulEffect = ConditionsDB.Conditions[harmfulEffects];
        HarmfulEffect?.OnStart?.Invoke(this);
        ChangeInStatus.Enqueue($"{Base.Name} {HarmfulEffect.ShowConditionMessageInBattle}");
        OnStatusChanged?.Invoke();
    }

    public void FunctionToRemoveHarmFulEffects()
    {
        HarmfulEffect = null;
        
    }

    public Ability RandomEnemyMoveFunction() //This is for enemy player to perform a random move
    {
        int randomAbility = Random.Range(0, Ability.Count);
        return Ability[randomAbility];
    }

    public bool DetermineIfMoveIsAllowed() //check if mve is allowed
    {
        bool performAbilityCheck = true; //  ability check
        if (Status?.OnBeforeMove != null) // Check if there is a status effect with a defined before move function
        {
            if (!Status.OnBeforeMove(this))
                performAbilityCheck = false;
        }

        if (HarmfulEffect?.OnBeforeMove != null) // check if there is a harmful effect
        {
            if (!HarmfulEffect.OnBeforeMove(this))
                performAbilityCheck = false;
        }

        return performAbilityCheck;
    }
    public void ChoiceAfterTurn()
    {
        Status?.WhenTurnIsDone?.Invoke(this);
        HarmfulEffect?.WhenTurnIsDone?.Invoke(this);
    }

    public void BattleEndingFunction()
    {
        HarmfulEffect = null;
        StatResetFunction();
    }
}

public class DamageInformation
{

    public bool Critical { get; set; }
    public float Type { get; set; }
    public bool Fainted { get; set; }


}


