using System;
using System.Threading;
using wShadow.Templates;
using wShadow.Warcraft.Classes;
using wShadow.Warcraft.Defines;
using wShadow.Warcraft.Managers;



public class BMhunterWOTLK : Rotation
{
	
    private int debugInterval = 5; // Set the debug interval in seconds
    private DateTime lastDebugTime = DateTime.MinValue;
    private DateTime lastCallPetTime = DateTime.MinValue;
	private TimeSpan callPetCooldown = TimeSpan.FromSeconds(10);
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
var PetHealth = pet.HealthPercent;

ShadowApi shadowApi = new ShadowApi();

if ((DateTime.Now - lastDebugTime).TotalSeconds >= debugInterval)
        {
            LogPlayerStats();
            lastDebugTime = DateTime.Now; // Update lastDebugTime
        }
// Health percentage of the player

// Power percentages for different resources

// Target distance from the player

if (me.IsDead() || me.IsGhost() || me.IsCasting() || me.IsMoving() || me.IsChanneling() || me.IsLooting() || !me.IsMounted()) return false;
        if (me.HasAura("Drink") || me.HasAura("Food")) return false;
		
		if (Api.Spellbook.CanCast("Aspect of the Cheetah") && !me.HasPermanent("Aspect of the Cheetah") )
			
					{
						Console.ForegroundColor = ConsoleColor.Green;
						Console.WriteLine("Casting Aspect of the Cheetah");
						Console.ResetColor();

					if (Api.Spellbook.Cast("Aspect of the Cheetah"))
						
					return true;
						
					}
	if (Api.Spellbook.CanCast("Call Pet")  && (DateTime.Now - lastCallPetTime) >= callPetCooldown)
{
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("Casting Call Pet.");
    Console.ResetColor();

    if (Api.Spellbook.Cast("Call Pet"))
    {
        lastCallPetTime = DateTime.Now; // Update the lastCallPetTime after successful casting
        return true;
    }
}

// Additional actions for when the pet is dead
else if (pet.IsDead() && Api.Spellbook.CanCast("Revive Pet"))
{
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine("Casting Revive Pet");
    Console.ResetColor();

    if (Api.Spellbook.Cast("Revive Pet"))
    {
        return true;
    }
 }
 if (!pet.IsDead() && PetHealth < 40  &&  Api.Spellbook.CanCast("Mend Pet") && !pet.HasAura("Mend Pet") && mana >10 )
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("Pet health is low healing him");
        Console.ResetColor();
		if (Api.Spellbook.Cast("Mend Pet"))
            
                return true;
}
if (Api.Spellbook.CanCast("Aspect of the Hawk") && !me.HasPermanent("Aspect of the Hawk") && !me.IsMounted() )
			
					{
						Console.ForegroundColor = ConsoleColor.Green;
						Console.WriteLine("Casting Aspect of the Hawk");
						Console.ResetColor();

					if (Api.Spellbook.Cast("Aspect of the Hawk"))
						
					return true;
					}
   if (Api.Spellbook.CanCast("Hunter's Mark") &&!target.HasAura("Hunter's Mark") && healthPercentage > 50 &&  mana > 20 && PetHealth>50)
  
    {
        var reaction = me.GetReaction(target);
        
        if (reaction != UnitReaction.Friendly)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Mark");
            Console.ResetColor();
            
           if (Api.UseMacro("Mark"))
            {
                return true;
            }
        }
        else
        {
            // Handle if the target is friendly
			 Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Target is friendly. Skipping marking cast.");
			            Console.ResetColor();

        }
    }
    else
    {
        // Handle if the target is not valid
        Console.WriteLine("Invalid target. Skipping marking cast.");
		            Console.ResetColor();

    }     
	if (Api.Spellbook.CanCast("Serpent Sting") && target.HasAura("Hunter's Mark") && healthPercentage > 50 &&  mana > 20)
{
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("Casting Serpent Sting");
    Console.ResetColor();

    if (Api.Spellbook.Cast("Serpent Sting"))
        return true;
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
ShadowApi shadowApi = new ShadowApi();
var pet = me.Pet();
	  var PetHealth  = 0.0f;
	     if(IsValid(pet))
		 {
		   PetHealth = pet.HealthPercent;
		 }        	
		var meTarget = me.Target;
 if ((DateTime.Now - lastDebugTime).TotalSeconds >= debugInterval)
        {
            LogPlayerStats();
            lastDebugTime = DateTime.Now; // Update lastDebugTime
        }
// Health percentage of the player


// Target distance from the player
        var TargetDistance = Target.Position.Distance2D(Player.Position);

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
		if (TargetDistance <= 5)
        {
            if (Api.Spellbook.CanCast("Aspect of the Monkey") && !me.HasPermanent("Aspect of the Monkey"))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Casting Aspect of the Monkey");
                Console.ResetColor();

                if (Api.Spellbook.Cast("Aspect of the Monkey"))
                {
                    return true;
                }
            }
            // Insert other melee spells or abilities here
        }
        else
        {
            if (Api.Spellbook.CanCast("Aspect of the Hawk") && !me.HasPermanent("Aspect of the Hawk") && (TargetDistance >= 10))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Casting Aspect of the Hawk");
                Console.ResetColor();

                if (Api.Spellbook.Cast("Aspect of the Hawk"))
                {
                    // Insert other ranged spells or abilities here
                }
            }
        }
		if (TargetDistance <= 8)
        {
            if (Api.Spellbook.CanCast("Raptor Strike"))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Casting Raptor Strike");
                Console.ResetColor();

                if (Api.Spellbook.Cast("Raptor Strike"))
                {
                    return true;
                }
            }
        }
		if (Api.UnfriendlyUnitsNearby(10, true) >= 2)
        {
            if (Api.Spellbook.CanCast("Explosive Shot"))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Casting Explosive Shot");
                Console.ResetColor();

                if (Api.Spellbook.Cast("Explosive Shot"))
                {
                    return true;
                }
            }
        }
        else if (TargetDistance <= 8)
        {
            if (Api.Spellbook.CanCast("Immolation Trap") && !target.HasAura("Immolation Trap"))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Casting Immolation Trap");
                Console.ResetColor();

                if (Api.Spellbook.Cast("Immolation Trap"))
                {
                    return true;
                }
            }
        }
		
		 if (Api.Spellbook.CanCast("Kill Command") && !pet.IsDead())
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Kill Command");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Kill Command"))
            {
                return true;
            }
        }
		 if (Api.Spellbook.CanCast("Cobra Shot"))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Cobra Shot");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Cobra Shot"))
            {
                return true;
            }
        }
		 if (targetDistance > 10)
        {
            if (Api.Spellbook.CanCast("Steady Shot"))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Casting Steady Shot");
                Console.ResetColor();

                if (Api.Spellbook.Cast("Steady Shot"))
                {
                    return true;
                }
            }
        }
		if (Api.Spellbook.CanCast("Serpent Sting") && target.HasAura("Hunter's Mark") && healthPercentage > 50 &&  mana > 20)
{
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("Casting Serpent Sting");
    Console.ResetColor();

    if (Api.Spellbook.Cast("Serpent Sting"))
        return true;
	}
	
	
return base.CombatPulse();
}
private void LogPlayerStats()
    {
        // Variables for player and target instances
var me = Api.Player;
var target = Api.Target;
var mana = me.ManaPercent;

// Health percentage of the player
var healthPercentage = me.HealthPercent;


// Target distance from the player


        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"{mana}% Mana available");
        Console.WriteLine($"{healthPercentage}% Health available");

		Console.ResetColor();


	
	

Console.ResetColor();
    }
	}