using System;
using System.Threading;
using wShadow.Templates;
using System.Collections.Generic;
using wShadow.Warcraft.Classes;
using wShadow.Warcraft.Defines;
using wShadow.Warcraft.Managers;



public class ProtWarr : Rotation
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
        SlowTick = 400;
        FastTick = 150;

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

        if (me.IsDead() || me.IsGhost() || me.IsCasting() || me.IsChanneling() || !me.IsMounted() || me.Auras.Contains("Food")) return false;

        if ((DateTime.Now - lastDebugTime).TotalSeconds >= debugInterval)
        {
            LogPlayerStats();
            lastDebugTime = DateTime.Now; // Update lastDebugTime
        }
        if (Api.Spellbook.CanCast("Battle Stance") && !me.Auras.Contains("Battle Stance",false) && !me.Auras.Contains("Warbringer",false))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Battle Stance");
            Console.ResetColor();
            if (Api.Spellbook.Cast("Battle Stance"))

                return true;
        }
        else if (Api.Spellbook.CanCast("Defensive Stance") && !me.Auras.Contains("Defensive Stance",false) && me.Auras.Contains("Warbringer",false))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Defensive Stance");
            Console.ResetColor();
            if (Api.Spellbook.Cast("Defensive Stance"))

                return true;
        }

        var reaction = me.GetReaction(target);

        if (!target.IsDead() && (reaction != UnitReaction.Friendly && reaction != UnitReaction.Honored && reaction != UnitReaction.Revered && reaction != UnitReaction.Exalted) && !IsNPC(target) && Api.Spellbook.CanCast("Charge") && targetDistance <= 25)
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

        var meTarget = me.Target;


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

        if (healthPercentage <= 30) //Last Stand and Shield wall
        {
            if (Api.Spellbook.CanCast("Shield Wall") && !me.Auras.Contains("Shield Wall"))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Casting Shield Wall (Emergency)");
                Console.ResetColor();

                if (Api.Spellbook.Cast("Shield Wall"))
                    return true;
            }

            if (Api.Spellbook.CanCast("Last Stand") && !me.Auras.Contains("Last Stand"))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Casting Last Stand (Emergency)");
                Console.ResetColor();

                if (Api.Spellbook.Cast("Last Stand"))
                    return true;
            }
        }
        if (Api.Spellbook.CanCast("Defensive Stance") && !me.Auras.Contains("Defensive Stance"))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Defensive Stance");
            Console.ResetColor();
            if (Api.Spellbook.Cast("Defensive Stance"))

                return true;
        }

        if (healthPercentage >= 80 && Api.Spellbook.CanCast("Bloodrage") && !Api.Spellbook.OnCooldown("Bloodrage")) //Last Stand and Shield wall
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Bloodrage");
            Console.ResetColor();
            if (Api.Spellbook.Cast("Bloodrage"))
            {
                return true;
            }
        }
        if (Api.Spellbook.CanCast("Shield Block") && !Api.Spellbook.OnCooldown("Shield Block"))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Shield Block");
            Console.ResetColor();
            if (Api.Spellbook.Cast("Shield Block"))
            {
                return true;
            }
        }
        if (Api.Spellbook.CanCast("Shield Bash") && !Api.Spellbook.OnCooldown("Shield Bash") && rage >= 10 && (target.IsCasting() || target.IsChanneling()))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Shield Bash");
            Console.ResetColor();
            if (Api.Spellbook.Cast("Shield Bash"))
            {
                return true;
            }
        }
        if (Api.Spellbook.CanCast("Demoralizing Shout") && !target.Auras.Contains("Demoralizing Shout") && rage >= 10 && targethealth >= 30  )
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Demoralizing Shout");
            Console.ResetColor();
            if (Api.Spellbook.Cast("Demoralizing Shout"))

                return true;
        }
        if (Api.Spellbook.CanCast("Thunder Clap") && !target.Auras.Contains("Thunder Clap") && rage >= 20 && targethealth >= 30)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Thunder Clap");
            Console.ResetColor();
            if (Api.Spellbook.Cast("Thunder Clap"))

                return true;
        }
        if (Api.Spellbook.CanCast("Revenge") && !Api.Spellbook.OnCooldown("Revenge") && rage >= 5)
        {
            if ((DateTime.Now - lastRevengeTime) >= RevengeCooldown)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Casting Revenge");
                Console.ResetColor();

                if (Api.Spellbook.Cast("Revenge"))
                {
                    lastRevengeTime = DateTime.Now;
                    return true;
                }
            }

        }
        if (Api.Spellbook.CanCast("Shield Slam") && !Api.Spellbook.OnCooldown("Shield Slam") && rage >= 20)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Shield Slam");
            Console.ResetColor();
            if (Api.Spellbook.Cast("Shield Slam"))

                return true;
        }
        if (Api.Spellbook.CanCast("Devastate") && target.AuraStacks("Sunder Armor") < 5 && rage >= 15)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Devastate");
            Console.ResetColor();
            if (Api.Spellbook.Cast("Devastate"))

                return true;
        }
        if (Api.Spellbook.CanCast("Cleave") && Api.UnitsTargetingMe(8, true) >= 2 && rage >= 30)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Cleave");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Cleave"))
                return true;
        }
        if (Api.Spellbook.CanCast("Attack"))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Attack");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Attack"))
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