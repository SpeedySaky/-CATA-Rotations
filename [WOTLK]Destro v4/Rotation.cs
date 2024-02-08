using System;
using System.Threading;
using wShadow.Templates;
using wShadow.Warcraft.Classes;
using wShadow.Warcraft.Defines;
using wShadow.Warcraft.Managers;



public class DestroLockWOTLK : Rotation
{
	
    private int debugInterval = 5; // Set the debug interval in seconds
    private DateTime lastDebugTime = DateTime.MinValue;
    public bool IsValid(WowUnit unit)
	{
		if (unit == null || unit.Address == null)
		{
			return false;
		}
		return true;
	}
    public override void Initialize()
    {  
	// Can set min/max levels required for this rotation.
        
		 lastDebugTime = DateTime.Now;
        LogPlayerStats();
        // Use this method to set your tick speeds.
        // The simplest calculation for optimal ticks (to avoid key spam and false attempts)

		// Assuming wShadow is an instance of some class containing UnitRatings property
        SlowTick = 600;
        FastTick = 200;

        // You can also use this method to add to various action lists.

        // This will add an action to the internal passive tick.
        // bool: needTarget -> If true action will not fire if player does not have a target
        // Func<bool>: function -> Action to attempt, must return true or false.
        PassiveActions.Add((true, () => false));

        // This will add an action to the internal combat tick.
        // bool: needTarget -> If true action will not fire if player does not have a target
        // Func<bool>: function -> Action to attempt, must return true or false.
        CombatActions.Add((true, () => false));
		
		
		
    }
	public override bool PassivePulse()
	{
	 // Variables for player and target instances
	 var me = Api.Player;
var target = Api.Target;
		var mana = me.ManaPercent;
var pet = me.Pet();
var healthPercentage = me.HealthPercent;
var targethealth = target.HealthPercent;
	  var PetHealth  = 0.0f;
	     if(IsValid(pet))
		 {
		   PetHealth = pet.HealthPercent;
		 }  
		 var TargetHealth  = 0.0f;
	     if(IsValid(target))
		 {
		   TargetHealth = target.HealthPercent;
		 }
ShadowApi shadowApi = new ShadowApi();

if ((DateTime.Now - lastDebugTime).TotalSeconds >= debugInterval)
        {
            LogPlayerStats();
            lastDebugTime = DateTime.Now; // Update lastDebugTime
        }
// Health percentage of the player

// Power percentages for different resources

// Target distance from the player

if (me.IsDead() || me.IsGhost() || me.IsCasting() || me.IsChanneling() || me.IsLooting() ) return false;
        if (me.HasAura("Drink") || me.HasAura("Food")) return false;
		
string[] healthstoneTypes = { "Minor Healthstone", "Lesser Healthstone", "Healthstone", "Greater Healthstone", "Major Healthstone", "Master Healthstone", "Demonic Healthstone", "Fel Healthstone" };

bool needsHealthstone = true;

foreach (string healthstoneType in healthstoneTypes)
{
    if (shadowApi.Inventory.HasItem(healthstoneType))
    {
        needsHealthstone = false;
        break;
    }
}

if (needsHealthstone && shadowApi.Inventory.HasItem("Soul Shard") && shadowApi.Spellbook.HasLearned("Create Healthstone") && shadowApi.Spellbook.CanCast("Create Healthstone"))
{
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("Create Healthstone");
    Console.ResetColor();

    if (shadowApi.Spellbook.Cast("Create Healthstone"))
    {
        return true;
    }
}

	
		if (Api.Spellbook.CanCast("Life Tap") && healthPercentage>80 && mana<30) 
			{
              Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("Casting Life Tap");
    Console.ResetColor();
    if (Api.Spellbook.Cast("Life Tap"))
    {
        return true;
    } 
	}
	if (Api.Spellbook.CanCast("Fel Armor") && !me.HasAura("Fel Armor") ) 
			{
              Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("Casting Fel Armor");
    Console.ResetColor();
    if (Api.Spellbook.Cast("Fel Armor"))
    {
        return true;
    } 
	}
	
	if (Api.Spellbook.CanCast("Demon Armor") && !me.HasAura("Fel Armor") && !me.HasAura("Demon Armor") ) 
			{
              Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("Casting Demon Armor");
    Console.ResetColor();
    if (Api.Spellbook.Cast("Demon Armor"))
    {
        return true;
    } 
	}
	
	if (Api.Spellbook.CanCast("Demon Skin") && !me.HasAura("Fel Armor") && !me.HasAura("Demon Armor") ) 
			{
              Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("Casting Demon Skin");
    Console.ResetColor();
    if (Api.Spellbook.Cast("Demon Skin"))
    {
        return true;
    } 
	}
	
	
     if (PetHealth<50 && healthPercentage>50 && Api.Spellbook.CanCast("Health Funnel"))
		{
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Healing Pet ");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Health Funnel"))
                return true;
        }
		
 if (!IsValid(pet) && Api.Spellbook.CanCast("Summon Voidwalker") && shadowApi.Inventory.HasItem("Soul Shard") && mana>30)
{
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("Casting Summon VoidWalker.");
    Console.ResetColor();

    if (Api.Spellbook.Cast("Summon Voidwalker"))
    {
        return true;
    }
}
else if (!IsValid(pet) && Api.Spellbook.CanCast("Summon Imp") && !Api.Spellbook.CanCast("Summon Voidwalker") && mana >30)
{
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("Casting Summon Imp.");
    Console.ResetColor();

    if (Api.Spellbook.Cast("Summon Imp"))
    {
        return true;
    }
} 	

var reaction = me.GetReaction(target);
	if (Api.HasMacro("Combat") && reaction != UnitReaction.Friendly && targethealth>=1)
  
			//macro needed
			//Macro name : Combat
			//Macro code:
			// /cast Curse of Agony
			// /petattack	
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Starting Combat");
            Console.ResetColor();
            
            if (Api.UseMacro("Combat"))
            {
                return true;
            }
        }
        
				return base.PassivePulse();

		

				}
		
public override bool CombatPulse()
    {
	// Variables for player and target instances
var me = Api.Player;
var target = Api.Target;
		var mana = me.ManaPercent;
var targethealth = target.HealthPercent;
var healthPercentage = me.HealthPercent;
	var targetDistance = target.Position.Distance2D(me.Position);

ShadowApi shadowApi = new ShadowApi();

if (me.IsDead() || me.IsGhost() || me.IsCasting() || me.IsMoving() || me.IsChanneling() || me.IsLooting() ) return false;
        if (me.HasAura("Drink") || me.HasAura("Food")) return false;

 if ((DateTime.Now - lastDebugTime).TotalSeconds >= debugInterval)
        {
            LogPlayerStats();
            lastDebugTime = DateTime.Now; // Update lastDebugTime
        }
// Health percentage of the player


var pet = me.Pet();
	  var PetHealth  = 0.0f;
	     if(IsValid(pet))
		 {
		   PetHealth = pet.HealthPercent;
		 }        	
		var meTarget = me.Target;
		  
        if (meTarget == null || target.IsDead())
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Assist Pet");
            Console.ResetColor();

            // Use the Target property to set the player's target to the pet's target
            if (Api.UseMacro("AssistPet"))
            {
                // Successfully assisted the pet, continue rotation
                // Don't return true here, continue with the rest of the combat logic
                // without triggering a premature exit
            }
        }
if (Api.Spellbook.CanCast("Drain Soul") && shadowApi.Inventory.ItemCount("Soul Shard") <= 2 && targethealth <= 20)
{
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("Casting Drain Soul");
    Console.ResetColor();

    if (Api.Spellbook.Cast("Drain Soul"))
    {
        return true;
    }
}
		string[] healthstoneTypes = { "Minor Healthstone", "Lesser Healthstone", "Healthstone", "Greater Healthstone", "Major Healthstone", "Master Healthstone", "Demonic Healthstone", "Fel Healthstone" };


foreach (string healthstoneType in healthstoneTypes)
{
    if (shadowApi.Inventory.HasItem(healthstoneType) && healthPercentage <= 40 && !shadowApi.Inventory.OnCooldown(healthstoneType))
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"Using {healthstoneType}");
        Console.ResetColor();

        if (shadowApi.Inventory.Use(healthstoneType))
        {
            return true;
        }
    }
}
if (meTarget == null || target.IsDead())
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Assist Pet");
            Console.ResetColor();

            // Use the Target property to set the player's target to the pet's target
            if (Api.UseMacro("AssistPet"))
            {
                // Successfully assisted the pet, continue rotation
                // Don't return true here, continue with the rest of the combat logic
                // without triggering a premature exit
            }
        }
		 if (PetHealth<50 && healthPercentage>50 && Api.Spellbook.CanCast("Health Funnel"))
		{
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Healing Pet ");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Health Funnel"))
                return true;
        }
		if (Api.Spellbook.CanCast("Haunt") && !target.HasAura("Haunt") )
	{
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("Casting Haunt");
    Console.ResetColor();

    if (Api.Spellbook.Cast("Haunt"))
        return true;
	}
	
	if (Api.Spellbook.CanCast("Drain Life") && healthPercentage<=50 && mana>=10 )
	{
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("Casting Drain Life");
    Console.ResetColor();

    if (Api.Spellbook.Cast("Drain Life"))
        return true;
	}
		
		if (Api.Spellbook.CanCast("Curse of Agony") && !target.HasAura("Curse of Agony") && targethealth>=30 && mana>=10)
	{
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("Casting Curse of Agony");
    Console.ResetColor();

    if (Api.Spellbook.Cast("Curse of Agony"))
        return true;
	}	
			if (Api.Spellbook.CanCast("Corruption") && !target.HasAura("Corruption") && targethealth>=30 && mana>=10)
	{
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("Casting Corruption");
    Console.ResetColor();

    if (Api.Spellbook.Cast("Corruption"))
        return true;
	}
	
	if (Api.Spellbook.CanCast("Unstable Affliction") && !target.HasAura("Unstable Affliction") && Api.Spellbook.CanCast("Immolate") && !target.HasAura("Immolate") && targethealth>=30 && mana>=10 )
	{
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("Casting Unstable Affliction");
    Console.ResetColor();

    if (Api.Spellbook.Cast("Unstable Affliction"))
        return true;
	}
	
	if (Api.Spellbook.CanCast("Immolate") && !target.HasAura("Unstable Affliction") && Api.Spellbook.CanCast("Immolate") && !target.HasAura("Immolate") && targethealth>=30 && mana>=10 )
	{
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("Casting Immolate");
    Console.ResetColor();

    if (Api.Spellbook.Cast("Immolate"))
        return true;
	}
	if (Api.Spellbook.CanCast("Conflagrate") && target.HasAura("Immolate") && targethealth>=40 && mana>=10 )
	{
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("Casting Conflagrate");
    Console.ResetColor();

    if (Api.Spellbook.Cast("Conflagrate"))
        return true;
	}
	if (Api.Spellbook.CanCast("Shadow Bolt") && me.HasAura(17941)  && mana>=30 )
	{
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("Casting Shadow Bolt");
    Console.ResetColor();

    if (Api.Spellbook.Cast("Shadow Bolt"))
        return true;
	}
	
	
	if (Api.Spellbook.CanCast("Soul Fire") && targethealth>=200 && mana>=10 && shadowApi.Inventory.ItemCount("Soul Shard") >= 2 )
	{
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("Casting Soul Fire");
    Console.ResetColor();

    if (Api.Spellbook.Cast("Soul Fire"))
        return true;
	}
	if (Api.Spellbook.CanCast("Shadow Bolt") && targethealth>=30  && mana>=20 )
	{
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("Casting Shadow Bolt");
    Console.ResetColor();

    if (Api.Spellbook.Cast("Shadow Bolt"))
        return true;
	}
	
	if (Api.Spellbook.CanCast("Shoot")   )
	{
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("Casting Shoot");
    Console.ResetColor();

    if (Api.Spellbook.Cast("Shoot"))
        return true;
	}
    
		
return base.CombatPulse();
}
private void LogPlayerStats()
    {
        // Variables for player and target instances
var me = Api.Player;
var target = Api.Target;
var mana = me.Mana;

// Health percentage of the player
var healthPercentage = me.HealthPercent;
ShadowApi shadowApi = new ShadowApi();


// Target distance from the player
		var targetDistance = target.Position.Distance2D(me.Position);


        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"{mana} Mana available");
        Console.WriteLine($"{healthPercentage}% Health available");

		Console.ResetColor();

// Get the current count of Soul Shards in the inventory
int soulShardCount = shadowApi.Inventory.ItemCount("Soul Shard");

if ( soulShardCount >= 0 )
{
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("Current Soul Shard Count: " + soulShardCount); // Debugging line
    Console.ResetColor();

    
}

	
	

Console.ResetColor();
    }
	}