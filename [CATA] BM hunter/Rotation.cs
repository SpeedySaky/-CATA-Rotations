using System;
using System.Threading;
using wShadow.Templates;
using System.Collections.Generic;
using wShadow.Warcraft.Classes;
using wShadow.Warcraft.Defines;
using wShadow.Warcraft.Managers;


public class BMhunterWOTLK : Rotation
{
    private List<string> npcConditions = new List<string>
    {
        "Innkeeper", "Auctioneer", "Banker", "FlightMaster", "GuildBanker",
        "PlayerVehicle", "StableMaster", "Repair", "Trainer", "TrainerClass",
        "TrainerProfession", "Vendor", "VendorAmmo", "VendorFood", "VendorPoison",
        "VendorReagent", "WildBattlePet", "GarrisonMissionNPC", "GarrisonTalentNPC",
        "QuestGiver"
    };
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
    private TimeSpan callPetCooldown = TimeSpan.FromSeconds(10);

    private DateTime lastMarkLogTime = DateTime.MinValue;
    private TimeSpan markCooldown = TimeSpan.FromSeconds(10);


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
        SlowTick = 1500;
        FastTick = 1000;

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
        var focus = me.FocusPercent;
        var pet = me.Pet();
        var PetHealth = 0.0f;
        if (IsValid(pet))
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

        if (me.IsDead() || me.IsGhost() || me.IsCasting() || me.IsChanneling() || me.IsMounted() || me.Auras.Contains("Drink") || me.Auras.Contains("Food")) return false;
        if ((DateTime.Now - lastCallPetTime) >= callPetCooldown && !IsValid(pet) && Api.HasMacro("CallPet"))


        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Call Pet.");
            Console.ResetColor();

            if (Api.UseMacro("CallPet"))
            {
                lastCallPetTime = DateTime.Now; // Update the lastCallPetTime after successful casting
                return true;
            }
        }
        if (!IsValid(pet) && Api.Spellbook.CanCast("Revive Pet"))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Ressing Pet");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Revive Pet"))
            {
                return true;
            }
        }
        if (Api.Spellbook.CanCast("Aspect of the Hawk") && !me.Auras.Contains("Aspect of the Hawk", false))

        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Aspect of the Hawk");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Aspect of the Hawk"))

                return true;

        }


        // Additional actions for when the pet is dead
        if (IsValid(pet) && PetHealth < 70 && Api.Spellbook.CanCast("Mend Pet") && !pet.Auras.Contains("Mend Pet"))
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Pet health is low healing him");
            Console.ResetColor();
            if (Api.Spellbook.Cast("Mend Pet"))

                return true;
        }
        var reaction = me.GetReaction(target);

        if (!target.IsDead() && (reaction != UnitReaction.Friendly && reaction != UnitReaction.Honored && reaction != UnitReaction.Revered && reaction != UnitReaction.Exalted) && !IsNPC(target))
        {
            if (Api.Spellbook.CanCast("Serpent Sting"))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Casting Serpent Sting");
                Console.ResetColor();

                if (Api.Spellbook.Cast("Serpent Sting"))
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
        var focus = me.FocusPercent;
        var targethealth = target.HealthPercent;
        var healthPercentage = me.HealthPercent;
        var pet = me.Pet();
        var PetHealth = 0.0f;
        if (IsValid(pet))
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
        if (me.IsDead() || me.IsGhost() || me.IsCasting() || me.IsMoving() || me.IsChanneling() || me.IsMounted() || me.Auras.Contains("Drink") || me.Auras.Contains("Food")) return false;

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
        if (!IsValid(pet) && Api.Spellbook.CanCast("Call Pet"))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Ressing Pet");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Revive Pet"))
            {
                return true;
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
        if (Api.Spellbook.CanCast("Aspect of the Hawk") && !me.Auras.Contains("Aspect of the Hawk", false))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Aspect of the Hawk");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Aspect of the Hawk"))
            {
                // Insert other ranged spells or abilities here
            }
        }
        if (Api.Spellbook.CanCast("Kill Command") && !Api.Spellbook.OnCooldown("Kill Command") && IsValid(pet) && focus > 40)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Kill Command");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Kill Command"))
            {
                return true;
            }
        }
        if (Api.Spellbook.CanCast("Intimidation") && Api.Spellbook.HasSpell("Intimidation") && !Api.Spellbook.OnCooldown("Intimidation") && IsValid(pet) && (target.IsCasting() || target.IsChanneling()))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Intimidation");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Intimidation"))
            {
                return true;
            }
        }
        if (Api.Spellbook.CanCast("Kill Shot") && !Api.Spellbook.OnCooldown("Kill Shot") && targethealth <= 20)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Kill Shot");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Kill Shot"))
            {
                return true;
            }
        }
        if (Api.Spellbook.CanCast("Bestial Wrath") && Api.Spellbook.HasSpell("Bestial Wrath") && !Api.Spellbook.OnCooldown("Bestial Wrath") && IsValid(pet))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Bestial Wrath");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Bestial Wrath"))
            {
                return true;
            }
        }
        if (Api.Spellbook.CanCast("Rapid Fire") && Api.Spellbook.HasSpell("Rapid Fire") && !Api.Spellbook.OnCooldown("Rapid Fire") && IsValid(pet))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Rapid Fire");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Rapid Fire"))
            {
                return true;
            }
        }
        if (Api.Spellbook.CanCast("Fervor") && Api.Spellbook.HasSpell("Fervor") && !Api.Spellbook.OnCooldown("Fervor") && IsValid(pet) && focus < 50)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Fervor");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Fervor"))
            {
                return true;
            }
        }

        // Insert other melee spells or abilities here
        if (IsValid(pet) && PetHealth < 40 && Api.Spellbook.CanCast("Mend Pet") && !pet.Auras.Contains("Mend Pet"))
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Pet health is low healing him");
            Console.ResetColor();
            if (Api.Spellbook.Cast("Mend Pet"))

                return true;
        }





        if (Api.Spellbook.CanCast("Serpent Sting") && !target.Auras.Contains("Serpent Sting") && healthPercentage > 30 && focus > 20)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Serpent Sting");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Serpent Sting"))
                return true;
        }

        if (Api.Spellbook.CanCast("Arcane Shot") && focus > 25)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Arcane Shot");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Arcane Shot"))
                return true;
        }

        if (Api.Spellbook.CanCast("Cobra Shot") && target.Auras.Contains("Serpent Sting"))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Cobra Shot");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Cobra Shot"))
            {
                return true;
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




        return base.CombatPulse();
    }
    private void LogPlayerStats()
    {
        // Variables for player and target instances
        var me = Api.Player;
        var target = Api.Target;
        var mana = me.ManaPercent;
        var pet = me.Pet();
        var PetHealth = 0.0f;
        if (IsValid(pet))
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

        if (Api.HasMacro("CallPet"))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Pet macro 'CallPet' is present.");
            Console.ResetColor();
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("INGAME ..... Create Macro ");
            Console.WriteLine("Macro name : CallPet");
            Console.WriteLine("Macro code : /cast Call Pet 1");

            Console.WriteLine("Save macro, exit options and when ingame RELOAD UI");
            Console.ResetColor();
        }

        if (!IsValid(pet))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Pet is not summoned.");
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