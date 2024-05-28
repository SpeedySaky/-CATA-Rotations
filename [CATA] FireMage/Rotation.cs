using System;
using System.Threading;
using wShadow.Templates;
using System.Collections.Generic;
using wShadow.Warcraft.Classes;
using wShadow.Warcraft.Defines;
using wShadow.Warcraft.Managers;



public class FireMage : Rotation
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
	private bool HasItem(EquipmentSlot slot)
    {
        return Api.Equipment.HasItem(slot);
    }
    private bool HasItem(object item)
        => Api.Inventory.HasItem(item);
    private int debugInterval = 5; // Set the debug interval in seconds
    private DateTime lastDebugTime = DateTime.MinValue;

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
        // Variables for player and target instances
        var me = Api.Player;
        var target = Api.Target;
        var pet = me.Pet();

        if ((DateTime.Now - lastDebugTime).TotalSeconds >= debugInterval)
        {
            LogPlayerStats();
            lastDebugTime = DateTime.Now; // Update lastDebugTime
        }
        // Health percentage of the player
        var healthPercentage = me.HealthPercent;
        var mana = me.ManaPercent;
        var targetDistance = target.Position.Distance2D(me.Position);
        var reaction = me.GetReaction(target);


        if (me.IsDead() || me.IsGhost() || me.IsCasting() || me.IsMoving() || me.IsChanneling() || me.IsMounted() || me.Auras.Contains("Drink") || me.Auras.Contains("Food")) return false;


        if (Api.Spellbook.CanCast("Conjure Refreshment") && !HasConjuredItem())
        {
            if (Api.Spellbook.Cast("Conjure Refreshment"))
            {
                Console.WriteLine("Conjured Refreshment.");
                // Add further actions if needed after conjuring water
            }
        }
        if (Api.Spellbook.CanCast("Conjure Mana Gem") && !HasItem("Mana Gem"))
        {
            if (Api.Spellbook.Cast("Conjure Mana Gem"))
            {
                Console.WriteLine("Conjured Mana Gem.");
                // Add further actions if needed after conjuring Mana Gem
            }
        }
        if (Api.Spellbook.CanCast("Molten Armor") && !me.Auras.Contains("Molten Armor"))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Molten Armor");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Molten Armor"))
                return true;
        }

        if (Api.Spellbook.CanCast("Amplify Magic") && !me.Auras.Contains("Amplify Magic"))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Amplify Magic");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Amplify Magic"))
                return true;
        }
        if (Api.Spellbook.CanCast("Arcane Brilliance") && !me.Auras.Contains("Arcane Brilliance"))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Arcane Brilliance");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Arcane Brilliance"))
                return true;
        }





        if (target.IsValid())
        { 
        if (!target.IsDead() && (reaction != UnitReaction.Friendly && reaction != UnitReaction.Honored && reaction != UnitReaction.Revered && reaction != UnitReaction.Exalted) &&
            mana > 20 && !IsNPC(target))
        {
            if (Api.Spellbook.CanCast("Pyroblast") && !target.Auras.Contains("Pyroblast"))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Casting Pyroblast");
                Console.ResetColor();

                if (Api.Spellbook.Cast("Pyroblast"))
                    return true;
            }

            else
            {
                if (Api.Spellbook.CanCast("Frostbolt") && !target.Auras.Contains("Frostbolt") && !Api.Spellbook.OnCooldown("Frostbolt"))
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Casting Frostbolt");
                    Console.ResetColor();

                    if (Api.Spellbook.Cast("Frostbolt"))
                        return true;
                }
            }
        }
    }
        return base.PassivePulse();

    }

    public override bool CombatPulse()
    {
        var me = Api.Player;
        var target = Api.Target;
        var healthPercentage = me.HealthPercent;
        var targethealth = target.HealthPercent;
        var mana = me.ManaPercent;
        var targetDistance = target.Position.Distance2D(me.Position);
        if (me.IsDead() || me.IsGhost() || me.IsCasting() || me.IsMoving() || me.IsChanneling() || me.IsMounted() || me.Auras.Contains("Drink") || me.Auras.Contains("Food")) return false;

        if ((DateTime.Now - lastDebugTime).TotalSeconds >= debugInterval)
        {
            LogPlayerStats();
            lastDebugTime = DateTime.Now; // Update lastDebugTime
        }

        if (Api.Spellbook.CanCast("Conjure Mana Gem") && !HasItem("Mana Gem"))
        {
            if (Api.Spellbook.Cast("Conjure Mana Gem"))
            {
                Console.WriteLine("Conjured Mana Gem.");
                // Add further actions if needed after conjuring Mana Gem
            }
        }





        if (Api.Spellbook.CanCast("Mana Shield") && !Api.Spellbook.OnCooldown("Mana Shield") && healthPercentage <= 30)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Mana Shield");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Mana Shield"))
                return true;
        }
        if (Api.Spellbook.CanCast("Evocation") && !Api.Spellbook.OnCooldown("Evocation") && mana <= 30)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Evocation");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Evocation"))
                return true;
        }
        if (Api.Spellbook.CanCast("Combustion") && !Api.Spellbook.OnCooldown("Combustion") && ( target.Auras.Contains("Living Bomb") || target.Auras.Contains("Pyroblast") || target.Auras.Contains("Ignite")))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Combustion");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Combustion"))
                return true;
        }
        if (Api.Spellbook.CanCast("Counterspell") && !Api.Spellbook.OnCooldown("Counterspell") && (target.IsCasting() || target.IsChanneling()))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Counterspell");
            Console.ResetColor();
            if (Api.Spellbook.Cast("Counterspell"))
            {
                return true;
            }
        }
        if (Api.Spellbook.CanCast("Flame Orb") && !Api.Spellbook.OnCooldown("Flame Orb") && !Api.Spellbook.OnCooldown("Flame Orb"))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Flame Orb");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Flame Orb"))
                return true;
        }
        if (Api.Spellbook.CanCast("Living Bomb") && !target.Auras.Contains("Living Bomb") && !Api.Spellbook.OnCooldown("Living Bomb"))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Living Bomb");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Living Bomb"))
                return true;
        }

        if (Api.Spellbook.CanCast("Pyroblast") && !target.Auras.Contains("Pyroblast") && !Api.Spellbook.OnCooldown("Pyroblast"))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Pyroblast");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Pyroblast"))
                return true;
        }
               
        if (Api.Spellbook.CanCast("Fire Blast") && !Api.Spellbook.OnCooldown("Fire Blast") )
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Fire Blast");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Fire Blast"))
                return true;
        }
        if (Api.Spellbook.CanCast("Scorch")  && !Api.Spellbook.OnCooldown("Scorch"))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Scorch");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Scorch"))
                return true;
        }
        if (Api.Spellbook.CanCast("Fireball") && mana >= 19 )
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Fireball");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Fireball"))
                return true;
        }

        if (Api.Spellbook.CanCast("Frostbolt") && targethealth < 20 && mana > 20)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Frostbolt");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Frostbolt"))
                return true;
        }
        bool hasRanged= HasItem(EquipmentSlot.Extra);

        if (Api.Spellbook.CanCast("Shoot") && hasRanged )
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Shoot");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Shoot"))
                return true;
        }
		else
		if (Api.Spellbook.CanCast("Auto Attack") && !hasRanged)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("No ranged weapon--going auto attack");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Auto Attack"))
                return true;
        }



        return base.CombatPulse();
    }
    private bool HasConjuredItem()
    {
        var conjuredItems = new List<string>
    {
        "Conjured Mana Cookie",
        "Conjured Mana Brownie",
        "Conjured Mana Cupcake",
        "Conjured Mana Lollipop",
        "Conjured Mana Pie",
        "Conjured Mana Strudel",
        "Conjured Mana Cake"
    };

        foreach (var item in conjuredItems)
        {
            if (HasItem(item))
            {
                return true;
            }
        }

        return false;
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
        var mana = me.ManaPercent;

        // Health percentage of the player
        var healthPercentage = me.HealthPercent;


        // Target distance from the player
        var targetDistance = target.Position.Distance2D(me.Position);


        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"{mana}% Mana available");
        Console.WriteLine($"{healthPercentage}% Health available");
        Console.ResetColor();


        if (me.Auras.Contains("Frost Armor")) // Replace "Thorns" with the actual aura name
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.ResetColor();
            var remainingTimeSeconds = me.Auras.TimeRemaining("Frost Armor");
            var remainingTimeMinutes = remainingTimeSeconds / 60; // Convert seconds to minutes
            var roundedMinutes = Math.Round(remainingTimeMinutes / 1000, 1); // Round to one decimal place

            Console.WriteLine($"Remaining time for Frost Armor: {roundedMinutes} minutes");
            Console.ResetColor();
        }
        if (me.Auras.Contains(28682)) // Replace "Thorns" with the actual aura name
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.ResetColor();


            Console.WriteLine($"Have Combustion Passive");
            Console.ResetColor();
        }
        if (me.Auras.Contains(28682)) // Replace "Thorns" with the actual aura name
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.ResetColor();


            Console.WriteLine($"Have Combustion Aura");
            Console.ResetColor();
        }
        if (me.Auras.Contains(28682)) // Replace "Thorns" with the actual aura name
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.ResetColor();


            Console.WriteLine($"Have Combustion Auras.Contains");
            Console.ResetColor();
        }


        Console.ResetColor();
    }
}