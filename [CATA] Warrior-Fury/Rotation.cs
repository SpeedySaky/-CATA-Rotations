using System;
using System.Threading;
using wShadow.Templates;
using System.Collections.Generic;
using wShadow.Warcraft.Classes;
using wShadow.Warcraft.Defines;
using wShadow.Warcraft.Managers;





public class ZerkWarr : Rotation
{
    private List<string> npcConditions = new List<string>
    {
        "Innkeeper", "Auctioneer", "Banker", "FlightMaster", "GuildBanker",
        "PlayerVehicle", "StableMaster", "Repair", "Trainer", "TrainerClass",
        "TrainerProfession", "Vendor", "VendorAmmo", "VendorFood", "VendorPoison",
        "VendorReagent", "WildBattlePet", "GarrisonMissionNPC", "GarrisonTalentNPC",
        "QuestGiver"
    };
    public bool IsValid(WowUnit unit)
    {
        if (unit == null || unit.Address == null)
        {
            return false;
        }
        return true;
    }
    private bool HasItem(object item) => Api.Inventory.HasItem(item);
    private int debugInterval = 5; // Set the debug interval in seconds
    private DateTime lastDebugTime = DateTime.MinValue;
    private DateTime lastRevengeTime = DateTime.MinValue;
    private TimeSpan RevengeCooldown = TimeSpan.FromSeconds(5.5);

    public override void Initialize()
    {
        // Can set min/max levels required for this rotation.

        lastDebugTime = DateTime.Now;
        LogPlayerStats();
        // Use this method to set your tick speeds.
        // The simplest calculation for optimal ticks (to avoid key spam and false attempts)

        // Assuming wShadow is an instance of some class containing UnitRatings property
        SlowTick = 1200;
        FastTick = 400;

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
        var me = Api.Player;
        var healthPercentage = me.HealthPercent;
        var rage = me.Rage;
        var target = Api.Target;
        var targetDistance = target.Position.Distance2D(me.Position);

        if (me.IsDead() || me.IsGhost() || me.IsCasting() || me.IsMoving() || me.IsChanneling() || me.IsMounted() || me.Auras.Contains("Drink") || me.Auras.Contains("Food")) return false;

        if ((DateTime.Now - lastDebugTime).TotalSeconds >= debugInterval)
        {
            LogPlayerStats();
            lastDebugTime = DateTime.Now;
        }
        if (Api.Spellbook.CanCast("Battle Stance") && !me.Auras.Contains("Battle Stance", false))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Battle Stance");
            Console.ResetColor();
            if (Api.Spellbook.Cast("Battle Stance"))

                return true;
        }



        var reaction = me.GetReaction(target);

        if (!target.IsDead() && (reaction != UnitReaction.Friendly && reaction != UnitReaction.Honored && reaction != UnitReaction.Revered && reaction != UnitReaction.Exalted) && !IsNPC(target) && Api.Spellbook.CanCast("Charge") && targetDistance <= 25 && targetDistance >= 8)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Charge");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Charge"))
            {
                return true;
            }
        }


        return base.PassivePulse();
    }



    public override bool CombatPulse()
    {
        var me = Api.Player;
        var healthPercentage = me.HealthPercent;
        var rage = me.Rage;
        var target = Api.Target;
        var targethealth = target.HealthPercent;
        var CreatureType = Api.Target.Info.GetCreatureType();

        var meTarget = me.Target;
        if (me.IsDead() || me.IsGhost() || me.IsCasting() || me.IsMoving() || me.IsChanneling() || me.IsMounted() || me.Auras.Contains("Drink") || me.Auras.Contains("Food")) return false;


        if (meTarget == null && Api.HasMacro("Target"))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Target");
            Console.ResetColor();

            // Use the Target property to set the player's target to the pet's target
            if (Api.UseMacro("Target"))
            {
                // Successfully assisted the pet, continue rotation
                // Don't return true here, continue with the rest of the combat logic
                // without triggering a premature exit
            }
        }


        if (Api.Spellbook.CanCast("Berserker Stance") && !me.Auras.Contains("Berserker Stance", false))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Berserker Stance");
            Console.ResetColor();
            if (Api.Spellbook.Cast("Berserker Stance"))

                return true;
        }
        if (Api.Spellbook.CanCast("Berserker Rage") && !me.Auras.Contains("Enrage") && !Api.Spellbook.OnCooldown("Berserker Rage"))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Berserker Rage");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Berserker Rage"))
                return true;
        }
        if (Api.Spellbook.CanCast("Enraged Regeneration") && !me.Auras.Contains("Enraged Regeneration") && !Api.Spellbook.OnCooldown("Enraged Regeneration") && rage >= 15 && healthPercentage <= 30)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Enraged Regeneration");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Enraged Regeneration"))
                return true;
        }

        if (Api.Spellbook.CanCast("Battle Shout") && !me.Auras.Contains("Battle Shout"))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Battle Shout");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Battle Shout"))
                return true;
        }
        if (Api.Spellbook.CanCast("Pummel") && (target.IsCasting() || target.IsChanneling()) && rage >= 10)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Pummel");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Pummel"))
                return true;
        }

        if (healthPercentage >= 30 && Api.Spellbook.CanCast("Death Wish") && !Api.Spellbook.OnCooldown("Death Wish")) //Last Stand and Shield wall
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Death Wish");
            Console.ResetColor();
            if (Api.Spellbook.Cast("Death Wish"))
            {
                return true;
            }
        }
        if (Api.Spellbook.CanCast("Demoralizing Shout") && !target.Auras.Contains("Demoralizing Shout") && rage >= 10 && targethealth >= 30)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Demoralizing Shout");
            Console.ResetColor();
            if (Api.Spellbook.Cast("Demoralizing Shout"))

                return true;
        }
        if (Api.Spellbook.CanCast("Victory Rush") && me.Auras.Contains("Victorious"))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Victory Rush");
            Console.ResetColor();
            if (Api.Spellbook.Cast("Victory Rush"))

                return true;
        }

        if (Api.Spellbook.CanCast("Recklessness") && !Api.Spellbook.OnCooldown("Recklessness")) //Last Stand and Shield wall
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Recklessness");
            Console.ResetColor();
            if (Api.Spellbook.Cast("Recklessness"))
            {
                return true;
            }
        } 
       
        if (Api.Spellbook.CanCast("Whirlwind") && !Api.Spellbook.OnCooldown("Whirlwind") && rage >= 25 && Api.UnfriendlyUnitsNearby(5,true)>=2)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Whirlwind");
            Console.ResetColor();
            if (Api.Spellbook.Cast("Whirlwind"))

                return true;
        }
        if (Api.Spellbook.CanCast("Cleave") && !Api.Spellbook.OnCooldown("Cleave") && rage >= 30 && Api.UnfriendlyUnitsNearby(5, true) >= 2)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Cleave");
            Console.ResetColor();
            if (Api.Spellbook.Cast("Cleave"))

                return true;
        }
        if (Api.Spellbook.CanCast("Bloodthirst") && !Api.Spellbook.OnCooldown("Bloodthirst") && rage >= 20 && me.Auras.Contains("Bloodthirst"))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Bloodthirst");
            Console.ResetColor();
            if (Api.Spellbook.Cast("Bloodthirst"))

                return true;
        }
        if (Api.Spellbook.CanCast("Raging Blow") && rage >= 20 && !Api.Spellbook.OnCooldown("Raging Blow") && (me.Auras.Contains("Enrage") || me.Auras.Contains("Berserker Rage") || me.Auras.Contains("Death Wish")))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Raging Blow");
            Console.ResetColor();
            if (Api.Spellbook.Cast("Raging Blow"))

                return true;
        }
        if (Api.Spellbook.CanCast("Execute") && !Api.Spellbook.OnCooldown("Execute") && rage >= 15 && targethealth <= 20)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Execute");
            Console.ResetColor();
            if (Api.Spellbook.Cast("Execute"))

                return true;
        }
        if (Api.Spellbook.CanCast("Cleave") && Api.UnitsNearby(5, true) >= 2 && rage >= 30)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Cleave");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Cleave"))
                return true;
        }
        if (Api.Spellbook.CanCast("Heroic Strike") && !Api.Spellbook.OnCooldown("Heroic Strike") && rage >= 15)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Heroic Strike");
            Console.ResetColor();
            if (Api.Spellbook.Cast("Heroic Strike"))

                return true;
        }
     
     

        return base.CombatPulse();
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
    private void LogPlayerStats()
    {
        var me = Api.Player;

        var rage = me.Rage;
        var healthPercentage = me.HealthPercent;


        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"{rage}% Rage available");
        Console.WriteLine($"{healthPercentage}% Health available");



    }
}