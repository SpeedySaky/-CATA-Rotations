using System;
using System.Threading;
using wShadow.Templates;
using System.Collections.Generic;
using wShadow.Warcraft.Classes;
using wShadow.Warcraft.Defines;
using wShadow.Warcraft.Managers;


public class WOTLKRogueNoStealth : Rotation
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
    private CreatureType GetCreatureType(WowUnit unit)
    {
        return unit.Info.GetCreatureType();
    }
    private int debugInterval = 5; // Set the debug interval in seconds
    private DateTime lastDebugTime = DateTime.MinValue;

    private bool HasItem(object item) => Api.Inventory.HasItem(item);
    private DateTime lastQuickdraw = DateTime.MinValue;
    private TimeSpan QuickdrawCooldown = TimeSpan.FromSeconds(11);
    private DateTime lastBetween = DateTime.MinValue;
    private TimeSpan BetweenCooldown = TimeSpan.FromSeconds(10);

    public override void Initialize()
    {
        // Can set min/max levels required for this rotation.

        lastDebugTime = DateTime.Now;
        LogPlayerStats();
        // Use this method to set your tick speeds.
        // The simplest calculation for optimal ticks (to avoid key spam and false attempts)

        // Assuming wShadow is an instance of some class containing UnitRatings property
        SlowTick = 1000;
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
        var healthPercentage = me.HealthPercent;
        var energy = me.Energy; // Energy
        var points = me.ComboPoints;
        var targetDistance = target.Position.Distance2D(me.Position);
        if (me.IsDead() || me.IsGhost() || me.IsCasting() || me.IsMoving() || me.IsChanneling() || me.IsMounted() || me.Auras.Contains("Drink") || me.Auras.Contains("Food")) return false;

        if ((DateTime.Now - lastDebugTime).TotalSeconds >= debugInterval)
        {
            LogPlayerStats();
            lastDebugTime = DateTime.Now; // Update lastDebugTime
        }
        // Health percentage of the player

        // Power percentages for different resources

        // Target distance from the player

        if (me.IsDead() || me.IsGhost() || me.IsCasting() || me.IsMoving() || me.IsChanneling() || me.IsMounted()  || me.Auras.Contains("Food")) return false;



        return base.PassivePulse();

    }

    public override bool CombatPulse()
    {
        // Variables for player and target instances
        var me = Api.Player;
        var target = Api.Target;
        if ((DateTime.Now - lastDebugTime).TotalSeconds >= debugInterval)
        {
            LogPlayerStats();
            lastDebugTime = DateTime.Now; // Update lastDebugTime
        }
        // Health percentage of the player
        var healthPercentage = me.HealthPercent;
        var targethealth = target.HealthPercent;
        var energy = me.Energy; // Energy
        var points = me.ComboPoints;
        if (me.IsDead() || me.IsGhost() || me.IsCasting() || me.IsMoving() || me.IsChanneling() || me.IsMounted() || me.Auras.Contains("Drink") || me.Auras.Contains("Food")) return false;

        string[] HP = { "Major Healing Potion", "Superior Healing Potion", "Greater Healing Potion", "Healing Potion", "Lesser Healing Potion", "Minor Healing Potion" };

        if (me.HealthPercent <= 70 && !Api.Inventory.OnCooldown(HP))
        {
            foreach (string hpot in HP)
            {
                if (HasItem(hpot))
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Using Healing potion");
                    Console.ResetColor();
                    if (Api.Inventory.Use(hpot))
                    {
                        return true;
                    }
                }
            }
        }
        // Target distance from the player
        var targetDistance = target.Position.Distance2D(me.Position);

        if (me.IsDead() || me.IsGhost() || me.IsCasting()) return false;
        if (me.Auras.Contains("Drink") || me.Auras.Contains("Food")) return false;

        if (Api.Spellbook.CanCast("Kick") && !Api.Spellbook.OnCooldown("Kick") && (target.IsCasting() || target.IsChanneling()))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Kick");
            Console.ResetColor();
            if (Api.Spellbook.Cast("Kick"))
            {
                return true;
            }
        }
        else if (Api.Spellbook.CanCast("Kidney Shot") && energy >= 25 && points>=1 && !Api.Spellbook.OnCooldown("Kidney Shot") && (target.IsCasting() || target.IsChanneling()))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Kidney Shot");
            Console.ResetColor();
            if (Api.Spellbook.Cast("Kidney Shot"))
            {
                return true;
            }
        }
		if (Api.Spellbook.CanCast("Evasion") && !me.Auras.Contains("Evasion") && Api.UnfriendlyUnitsNearby(15, true) >= 2 && !Api.Spellbook.OnCooldown("Evasion"))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Casting Evasion");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Evasion"))
                return true;
        }
        if (Api.Spellbook.CanCast("Adrenaline Rush") && !Api.Spellbook.OnCooldown("Adrenaline Rush") && Api.UnfriendlyUnitsNearby(5, true)>=2)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Adrenaline Rush");
            Console.ResetColor();
            if (Api.Spellbook.Cast("Adrenaline Rush"))
            {
                return true;
            }
        }
        if (Api.Spellbook.CanCast("Killing Spree") && !Api.Spellbook.OnCooldown("Killing Spree"))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Killing Spree");
            Console.ResetColor();
            if (Api.Spellbook.Cast("Killing Spree"))
            {
                return true;
            }
        }
        if (Api.Spellbook.CanCast("Blade Flurry") && !Api.Spellbook.OnCooldown("Blade Flurry") && energy >= 25 && Api.UnfriendlyUnitsNearby(5, true) >= 2)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Blade Flurry");
            Console.ResetColor();
            if (Api.Spellbook.Cast("Blade Flurry"))
            {
                return true;
            }
        }
        CreatureType targetCreatureType = GetCreatureType(target);

        if (Api.Spellbook.CanCast("Rupture") && energy >= 25 && !target.Auras.Contains("Rupture") && points >= 3 && targethealth > 30 && (targetCreatureType == CreatureType.Elemental || targetCreatureType == CreatureType.Mechanical))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Rupture");
            Console.ResetColor();
            if (Api.Spellbook.Cast("Rupture"))
            {
                return true;
            }
        }
		if (Api.Spellbook.CanCast("Recuperate") && !me.Auras.Contains("Recuperate") && points >= 3)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Casting Recuperate");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Recuperate"))
                return true;
        }
        if (Api.Spellbook.CanCast("Slice and Dice") && points >= 3 && !me.Auras.Contains("Slice and Dice") && energy >= 25)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Casting Slice and Dice ");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Slice and Dice"))
                return true;
        }
        if (Api.Spellbook.CanCast("Revealing Strike") && points == 4 && energy >= 40)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Casting Revealing Strike ");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Revealing Strike"))
                return true;
        }
        if (Api.Spellbook.CanCast("Eviscerate") && points == 5 && energy >= 35)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Casting Eviscerate ");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Eviscerate"))
                return true;
        }
        if (Api.Spellbook.CanCast("Sinister Strike") && energy >= 45)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Casting Sinister Strike ");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Sinister Strike"))
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
        // Variables for player and target instances
        var me = Api.Player;
        var target = Api.Target;

        // Health percentage of the player
        var healthPercentage = me.HealthPercent;

        var energy = me.Energy; // Energy
        var points = me.ComboPoints;

        // Target distance from the player
        var targetDistance = target.Position.Distance2D(me.Position);


        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"{energy}% Energy available");
        Console.WriteLine($"{healthPercentage}% Health available");
        Console.WriteLine($"{points} points available");

        Console.ResetColor();


    }

}



