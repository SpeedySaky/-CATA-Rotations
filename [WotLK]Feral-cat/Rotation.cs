using System;
using System.Threading;
using wShadow.Templates;
using System.Collections.Generic;
using wShadow.Warcraft.Classes;
using wShadow.Warcraft.Defines;
using wShadow.Warcraft.Managers;


public class CatDruid : Rotation
{
    private bool HasItem(object item)
        => Api.Inventory.HasItem(item);


    private List<string> npcConditions = new List<string>
    {
        "Innkeeper", "Auctioneer", "Banker", "FlightMaster", "GuildBanker",
        "PlayerVehicle", "StableMaster", "Repair", "Trainer", "TrainerClass",
        "TrainerProfession", "Vendor", "VendorAmmo", "VendorFood", "VendorPoison",
        "VendorReagent", "WildBattlePet", "GarrisonMissionNPC", "GarrisonTalentNPC",
        "QuestGiver"
    };
    private int debugInterval = 30; // Set the debug interval in seconds
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
        SlowTick = 800;
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
        var me = Api.Player;
        var health = me.HealthPercent;
        var target = Api.Target;
        var targetDistance = target.Position.Distance2D(me.Position);
        var mana = me.ManaPercent;
        var reaction = me.GetReaction(target);

        var targethealth = target.HealthPercent;
        var meTarget = me.Target;


        if (me.IsDead() || me.IsGhost() || me.IsCasting() || me.IsMounted() || me.Auras.Contains("Swift Flight Form",false) || me.Auras.Contains("Flight Form",false) || me.Auras.Contains("Travel Form",false) || me.Auras.Contains("Drink") || me.Auras.Contains("Food")) return false;

        if ((DateTime.Now - lastDebugTime).TotalSeconds >= debugInterval)
        {
            LogPlayerStats();
            lastDebugTime = DateTime.Now; // Update lastDebugTime
        }

        if (Api.Spellbook.CanCast("Thorns") && !me.Auras.Contains("Thorns"))
        {

            Print($"Casting Thorns", ConsoleColor.Green);
            if (Api.Spellbook.Cast("Thorns"))
                return true;

        }

        if (Api.Spellbook.CanCast("Mark of the Wild") && !me.Auras.Contains("Mark of the Wild"))
        {

            Print($"Casting Mark of the Wild", ConsoleColor.Green);
            if (Api.Spellbook.Cast("Mark of the Wild"))
                return true;

        }

        if (Api.Spellbook.CanCast("Rejuvenation") && health <= 60 && !me.Auras.Contains("Rejuvenation"))
        {
            Print($"Casting Rejuvenation", ConsoleColor.Green);
            if (Api.Spellbook.Cast("Rejuvenation"))
                return true;
        }

        if (Api.Spellbook.CanCast("Regrowth") && health <= 40 && !me.Auras.Contains("Regrowth"))
        {
            Print($"Casting Regrowth", ConsoleColor.Green);
            if (Api.Spellbook.Cast("Regrowth"))
                return true;
        }
        if (Api.Spellbook.CanCast("Healing Touch") && health <= 30)
        {
            Print($"Casting Healing Touch", ConsoleColor.Green);
            if (Api.Spellbook.Cast("Healing Touch"))
                return true;
        }


    
             
           
        



        return base.PassivePulse();
    }

    public override bool CombatPulse()
    {
        if (me.IsDead() || me.IsGhost() || me.IsCasting() || me.Auras.Contains("Drink") || me.Auras.Contains("Food") || !target.IsValid() || target.IsDeadOrGhost()) return false;
        var me = Api.Player;

        var energy = me.EnergyPercent;
        var comboPoints = me.ComboPoints;

        var manaPercentage = me.ManaPercent;
        var healthPercentage = me.HealthPercent;

        var target = Api.Target;
        var targethealth = target.HealthPercent;


        if (!me.Auras.Contains("Innervate") && Api.Spellbook.CanCast("Innervate") && manaPercentage <= 30)
        {
            Print($"Casting Innervate", ConsoleColor.Green);
            if (Api.Spellbook.Cast("Innervate"))
                return true;
        }

        if (healthPercentage <= 30 && !Api.Spellbook.OnCooldown("Survival Instincts"))
        {
            Print($"Casting Survival Instincts as we geting low HP", ConsoleColor.Green);
            if (Api.Spellbook.Cast("Survival Instincts"))
                return true;
        }

        if (healthPercentage <= 30 && !Api.Spellbook.OnCooldown("Barkskin"))
        {
            Print($"Casting Barkskin as we geting low HP", ConsoleColor.Green);
            if (Api.Spellbook.Cast("Barkskin"))
                return true;
        }

        if (Api.Spellbook.CanCast("Regrowth") && !me.Auras.Contains("Regrowth") && me.Auras.Contains("Barkskin"))
        {
            Print($"Casting Regrowth", ConsoleColor.Green);
            if (Api.Spellbook.Cast("Regrowth"))
                return true;
        }

        if (Api.Spellbook.CanCast("Rejuvenation") && !me.Auras.Contains("Rejuvenation") && me.Auras.Contains("Barkskin"))
        {
            Print($"Casting Rejuvenation", ConsoleColor.Green);
            if (Api.Spellbook.Cast("Rejuvenation"))
                return true;
        }

        if (Api.Spellbook.CanCast(768) && !me.Auras.Contains(768, false))
        {
            Print($"Casting Cat Form", ConsoleColor.Green);
            if (Api.Spellbook.Cast(768))
                return true;
        }

        if (Api.Spellbook.CanCast("Faerie Fire (Feral)") && !target.Auras.Contains("Faerie Fire (Feral)"))
        {
            if (target.Auras.TimeRemaining("Faerie Fire (Feral)") <= 1000)
            {
                if (Api.Spellbook.Cast("Faerie Fire (Feral)"))
                {
                    Console.WriteLine("Casting Faerie Fire (Feral)");  // Corrected the function name
                    return true;
                }
            }
        }



        if (Api.Spellbook.CanCast("Maim") && (target.IsCasting() || target.IsChanneling()) && me.Auras.Contains(768, false))
        {
            Print($"Casting Maim", ConsoleColor.Green);
            if (Api.Spellbook.Cast("Maim"))
                return true;
        }

        if (Api.UnitsNearby(15, false) >= 2 && Api.Spellbook.CanCast("Berserk") && !me.Auras.Contains("Berserk") && me.Auras.Contains(768, false))
        {
            Print($"Casting Berserk", ConsoleColor.Green);
            if (Api.Spellbook.Cast("Berserk"))
                return true;
        }
        else if (me.Auras.Contains("Berserk") && Api.Spellbook.CanCast("Mangle (Cat)") && me.Auras.Contains(768,false))
        {
            Print($"Casting Mangle (Cat)", ConsoleColor.Green);
            if (Api.Spellbook.Cast("Mangle (Cat)"))
                return true;
        }

        if (Api.Spellbook.CanCast("Savage Roar") && !me.Auras.Contains("Savage Roar") && comboPoints >= 2 && target.HealthPercent >= 40 && energy >= 25 && me.Auras.Contains(768,false))
        {
            Print($"Casting Savage Roar with {comboPoints} Points and {energy} Energy", ConsoleColor.Green);
            if (Api.Spellbook.Cast("Savage Roar"))
                return true;
        }

        if (Api.Spellbook.CanCast("Tiger's Fury") && !me.Auras.Contains("Tiger's Fury") && !me.Auras.Contains("Berserk") && target.HealthPercent >= 50 && me.Auras.Contains(768,false))
        {
            Print($"Casting Tiger's Fury", ConsoleColor.Green);
            if (Api.Spellbook.Cast("Tiger's Fury"))
                return true;
        }

        if (Api.Spellbook.CanCast("Rake") && !target.Auras.Contains("Rake") && target.HealthPercent >= 30 && energy > 40 && me.Auras.Contains(768,false))
        {
            Print($"Casting Rake with {energy} Energy", ConsoleColor.Green);
            if (Api.Spellbook.Cast("Rake"))
                return true;
        }

        if (Api.Spellbook.CanCast("Rip") && !target.Auras.Contains("Rip") && target.HealthPercent >= 20 && energy > 30 && comboPoints >= 2 && me.Auras.Contains(768,false))
        {
            Print($"Casting Rip with {comboPoints} Points and {energy} Energy", ConsoleColor.Green);
            if (Api.Spellbook.Cast("Rip"))
                return true;
        }

        if (Api.Spellbook.CanCast("Ferocious Bite") && energy > 35 && comboPoints >= 5 && me.Auras.Contains(768,false))
        {
            Print($"Casting Ferocious Bite with {comboPoints} Points and {energy} Energy", ConsoleColor.Green);
            if (Api.Spellbook.Cast("Ferocious Bite"))
                return true;
        }
        if (Api.Spellbook.CanCast("Mangle (Cat)") && comboPoints < 5 && energy >= 45 && !target.Auras.Contains("Mangle (Cat)") && me.Auras.Contains(768,false))
        {
            Print($"Casting Mangle (Cat) with {energy} Energy", ConsoleColor.Green);
            if (Api.Spellbook.Cast("Mangle (Cat)"))
                return true;
        }

        if (Api.Spellbook.CanCast("Claw") && energy >= 45 && me.Auras.Contains(768,false))
        {
            Print($"Casting Claw with {energy} Energy", ConsoleColor.Green);
            if (Api.Spellbook.Cast("Claw"))
                return true;
        }

        return base.CombatPulse();
    }



    private void LogPlayerStats()
    {
        var me = Api.Player;

        var energy = me.EnergyPercent;
        var manaPercentage = me.ManaPercent;
        var healthPercentage = me.HealthPercent;

        Print(new[]
        {
            $"{energy}% Energy available",
            $"{manaPercentage}% Mana available",
            $"{healthPercentage}% Health available"
        }, ConsoleColor.Red);

        // Check if the player has the Cat Form aura by iterating through the player's auras
        // Retrieve the Strength value for a WowPlayer instance

        if (me.Auras.Contains(768, false))
        {
            Print($"Have 768 Form", ConsoleColor.Green);
            
        }
        if (me.Auras.Contains("Cat Form", false))
        {
            Print($"Have Cat Form", ConsoleColor.Green);

        }


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

    private void Print(string message, ConsoleColor color)
    {
        if (string.IsNullOrEmpty(message)) return;
        var old = Console.ForegroundColor;
        Console.ForegroundColor = color;
        Console.WriteLine(message);
        Console.ForegroundColor = old;
    }

    private void Print(string[] messages, ConsoleColor color)
    {
        if (messages is { Length: <= 0 }) return;

        var old = Console.ForegroundColor;
        Console.ForegroundColor = color;
        for (var i = 0; i < messages.Length; i++)
            Console.WriteLine(messages[i]);
        Console.ForegroundColor = old;
    }

}