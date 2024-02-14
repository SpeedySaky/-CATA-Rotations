using System;
using System.Threading;
using wShadow.Templates;
using System.Collections.Generic;
using wShadow.Warcraft.Classes;
using wShadow.Warcraft.Defines;
using wShadow.Warcraft.Managers;


public class BMhunterWOTLK : Rotation
{
	private List<string> npcConditions = new List<string>();
	private bool HasItem(object item)
        => Api.Inventory.HasItem(item);
		public bool IsValid(WowUnit unit)
	{
		if (unit == null || unit.Address == null)
		{
			return false;
		}
		return true;
	}
    private int debugInterval = 30; // Set the debug interval in seconds
    private DateTime lastDebugTime = DateTime.MinValue;
    private DateTime lastCallPetTime = DateTime.MinValue;
		private DateTime lastFeedTime = DateTime.MinValue;
		
	private DateTime lastMarkLogTime = DateTime.MinValue;
	private TimeSpan markCooldown = TimeSpan.FromSeconds(10);

	private TimeSpan callPetCooldown = TimeSpan.FromSeconds(10);

    public override void Initialize()
    {  
	//targets
	npcConditions.Add("Innkeeper");
        npcConditions.Add("Auctioneer");
        npcConditions.Add("Banker");
        npcConditions.Add("FlightMaster");
        npcConditions.Add("GuildBanker");
        npcConditions.Add("PlayerVehicle");
        npcConditions.Add("StableMaster");
        npcConditions.Add("Repair");
        npcConditions.Add("Trainer");
        npcConditions.Add("TrainerClass");
        npcConditions.Add("TrainerProfession");
        npcConditions.Add("Vendor");
        npcConditions.Add("VendorAmmo");
        npcConditions.Add("VendorFood");
        npcConditions.Add("VendorPoison");
        npcConditions.Add("VendorReagent");
        npcConditions.Add("WildBattlePet");
        npcConditions.Add("GarrisonMissionNPC");
        npcConditions.Add("GarrisonTalentNPC");
        npcConditions.Add("QuestGiver");
		lastDebugTime = DateTime.Now;
        LogPlayerStats();
        // Use this method to set your tick speeds.
        // The simplest calculation for optimal ticks (to avoid key spam and false attempts)

		// Assuming wShadow is an instance of some class containing UnitRatings property
        SlowTick = 800;
        FastTick = 300;

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
	var PetHealth  = 0.0f;
	     if(IsValid(pet))
		 {
		   PetHealth = pet.HealthPercent;
		 }  
	var healthPercentage = me.HealthPercent;
	var targethealth = target.HealthPercent;


if ((DateTime.Now - lastDebugTime).TotalSeconds >= debugInterval)
        {
            LogPlayerStats();
            lastDebugTime = DateTime.Now; // Update lastDebugTime
        }
// Health percentage of the player

// Power percentages for different resources

// Target distance from the player

if (me.IsDead() || me.IsGhost() || me.IsCasting() || me.IsMoving() || me.IsChanneling() || me.IsLooting() || me.IsMounted()) return false;
        if (me.Auras.Contains("Drink") || me.Auras.Contains("Food")) return false;
		
		if (Api.Spellbook.CanCast("Aspect of the Cheetah") && !me.Auras.Contains("Aspect of the Cheetah")  )
			
					{
						Console.ForegroundColor = ConsoleColor.Green;
						Console.WriteLine("Casting Aspect of the Cheetah");
						Console.ResetColor();

					if (Api.Spellbook.Cast("Aspect of the Cheetah"))
						
					return true;
						
					}
					
if ((DateTime.Now - lastCallPetTime) >= callPetCooldown && !IsValid(pet) && Api.Spellbook.CanCast("Call Pet"))
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
 if (!IsValid(pet) && Api.Spellbook.CanCast("Revive Pet"))
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("Casting Revive Pet");
        Console.ResetColor();

        if (Api.Spellbook.Cast("Revive Pet"))
        {
            return true;
        }
    }

if (IsValid(pet) && (DateTime.Now - lastFeedTime).TotalMinutes >= 10 && Api.HasMacro("Feed"))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Feeding pet.");
            Console.ResetColor();

            if (Api.UseMacro("Feed"))
            {
                lastFeedTime = DateTime.Now; // Update lastFeedTime

                // Log the estimated time until the next feeding attempt
                var nextFeedTime = lastFeedTime.AddMinutes(10);
                var timeUntilNextFeed = nextFeedTime - DateTime.Now;

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"Next feed pet in: {timeUntilNextFeed.TotalMinutes} minutes.");
                Console.ResetColor();

                return true;
            }
        }
	if (IsValid(pet) && PetHealth < 40  &&  Api.Spellbook.CanCast("Mend Pet") && !pet.Auras.Contains("Mend Pet") && mana >10 )
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("Pet health is low healing him");
        Console.ResetColor();
		if (Api.Spellbook.Cast("Mend Pet"))
            
                return true;
}
 if (IsValid(pet) && PetHealth < 40  &&  Api.Spellbook.CanCast("Mend Pet") && !pet.Auras.Contains("Mend Pet") && mana >10 )
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("Pet health is low healing him");
        Console.ResetColor();
		if (Api.Spellbook.Cast("Mend Pet"))
            
                return true;
}
if (Api.Spellbook.CanCast("Aspect of the Hawk") && !me.Auras.Contains("Aspect of the Hawk") && !me.IsMounted()  && !me.Auras.Contains("Aspect of the Cheetah"))
			
					{
						Console.ForegroundColor = ConsoleColor.Green;
						Console.WriteLine("Casting Aspect of the Hawk");
						Console.ResetColor();

					if (Api.Spellbook.Cast("Aspect of the Hawk"))
						
					return true;
					}
if ( !target.IsDead() && !IsNPC(target)  && Api.Spellbook.CanCast("Hunter's Mark") && !target.Auras.Contains("Hunter's Mark") && healthPercentage > 50 && mana > 20 && PetHealth > 50)  
    
        
      
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Mark");
            Console.ResetColor();
            
           if (Api.UseMacro("Mark"))
            {
				lastMarkLogTime = DateTime.Now; // Update the lastMarkTime after successful casting
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
        var targetDistance = target.Position.Distance2D(me.Position);

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
		if (Api.Spellbook.CanCast("Kill Command") && IsValid(pet))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Kill Command");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Kill Command"))
            {
                return true;
            }
        }
		 if (Api.Spellbook.CanCast("Intimidation") && Api.Spellbook.HasSpell("Intimidation") && IsValid(pet) && (target.IsCasting() || target.IsChanneling()))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Intimidation");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Intimidation"))
            {
                return true;
            }
        }
		if (targetDistance <= 5 )
        {
            if (Api.Spellbook.CanCast("Aspect of the Monkey") && !me.Auras.Contains("Aspect of the Monkey"))
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
        
       
        
            if (Api.Spellbook.CanCast("Mongoose Strike"))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Casting Mongoose Strike");
                Console.ResetColor();

                if (Api.Spellbook.Cast("Mongoose Strike"))
                {
                    return true;
                }
            }
                
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
		
        
            if (Api.Spellbook.CanCast("Immolation Trap") && !target.Auras.Contains("Immolation Trap"))
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
		
		 if (targetDistance > 10 )
		 {
		       
			if (Api.Spellbook.CanCast("Aspect of the Hawk") && !me.Auras.Contains("Aspect of the Hawk") )
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Casting Aspect of the Hawk");
                Console.ResetColor();

                if (Api.Spellbook.Cast("Aspect of the Hawk"))
                {
                    // Insert other ranged spells or abilities here
                }
            }
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
        
		if (Api.Spellbook.CanCast("Serpent Sting") && !target.Auras.Contains("Serpent Sting") && healthPercentage > 30 &&  mana > 20)
		{
		Console.ForegroundColor = ConsoleColor.Green;
		Console.WriteLine("Casting Serpent Sting");
		Console.ResetColor();

			if (Api.Spellbook.Cast("Serpent Sting"))
			return true;
		}
		
		if (Api.Spellbook.CanCast("Auto Shot"))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Casting Auto Shot");
                Console.ResetColor();

                if (Api.Spellbook.Cast("Auto Shot"))
                {
                    return true;
                }
            }
		
		
		}
	
return base.CombatPulse();
}
private void LogPlayerStats()
    {
        // Variables for player and target instances
var me = Api.Player;
var target = Api.Target;
var mana = me.ManaPercent;
var pet = me.Pet();
var PetHealth  = 0.0f;
	     if(IsValid(pet))
		 {
		   PetHealth = pet.HealthPercent;
		 }  
// Health percentage of the player
var healthPercentage = me.HealthPercent;


// Target distance from the player


        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"{mana}% Mana available");
        Console.WriteLine($"{healthPercentage}% Health available");

		Console.ResetColor();


	
	if (!IsValid(pet))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Pet is nnot summoned.");
            Console.ResetColor();
            // Additional actions for when the pet is dead
        }
		else 
if (IsValid(pet))			
{
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Pet is summoned.");
            Console.ResetColor();
            // Additional actions for when the pet is dead
        }

Console.ResetColor();
    }
    private bool IsNPC(WowUnit unit)
    {
        if (!IsValid(unit))
        {
            // If the unit is not valid, consider it not an NPC
            return false;
        }

        foreach (var condition in npcConditions)
        {
            switch (condition)
            {
                case "Innkeeper" when unit.IsInnkeeper():
                case "Auctioneer" when unit.IsAuctioneer():
                case "Banker" when unit.IsBanker():
                case "FlightMaster" when unit.IsFlightMaster():
                case "GuildBanker" when unit.IsGuildBanker():
                case "StableMaster" when unit.IsStableMaster():
                case "Trainer" when unit.IsTrainer():
                case "Vendor" when unit.IsVendor():
                case "QuestGiver" when unit.IsQuestGiver():
                    return true;
            }
        }

        return false;
    }

}