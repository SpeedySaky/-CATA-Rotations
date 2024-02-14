using System;
using System.Threading;
using wShadow.Templates;
using System.Collections.Generic;
using wShadow.Warcraft.Classes;
using wShadow.Warcraft.Defines;
using wShadow.Warcraft.Managers;



public class FireMageWotlk : Rotation
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

        if (me.IsDead() || me.IsGhost() || me.IsCasting() || me.IsMoving() || me.IsChanneling() || !me.IsMounted()) return false;
        if (me.Auras.Contains("Drink") || me.Auras.Contains("Food")) return false;


        string[] waterTypes = { "Conjured Mana Strudel", "Conjured Mountain Spring Water", "Conjured Crystal Water", "Conjured Sparkling Water", "Conjured Mineral Water", "Conjured Spring Water", "Conjured Purified Water", "Conjured Fresh Water", "Conjured Water" };
        string[] foodTypes = { "Conjured Mana Strudel", "Conjured Cinnamon Roll", "Conjured Sweet Roll", "Conjured Sourdough", "Conjured Pumpernickel", "Conjured Rye", "Conjured Bread", "Conjured Muffin" };

        bool needsWater = true;
        bool needsFood = true;
        foreach (string waterType in waterTypes)
        {
            if (HasItem(waterType))
            {
                needsWater = false;
                break;
            }
        }

        foreach (string foodType in foodTypes)
        {
            if (HasItem(foodType))
            {
                needsFood = false;
                break;
            }
        }
        string[] GemTypes = { "Mana Agate", "Mana Sapphire", "Mana Emerald", "Mana Ruby", "Mana Citrine", "Mana Jade" };
        bool needsgem = true;

        foreach (string gemType in GemTypes)
        {
            if (HasItem(gemType))
            {
                needsgem = false;
                break;
            }
        }
        if (Api.Spellbook.CanCast("Conjure Mana Gem") && needsgem)
        {
            if (Api.Spellbook.Cast("Conjure Mana Gem"))
            {
                Console.WriteLine("Conjure Mana Gem.");
                // Add further actions if needed after conjuring water
            }
        }
        if (Api.Spellbook.CanCast("Conjure Refreshment") && (needsWater || needsFood))
        {
            if (Api.Spellbook.Cast("Conjure Refreshment"))
            {
                Console.WriteLine("Conjured Refreshment.");
                // Add further actions if needed after conjuring water
            }
        }
        if (Api.Spellbook.CanCast("Mage Armor") && !me.Auras.Contains("Mage Armor"))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Mage Armor");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Mage Armor"))
                return true;
        }
        if (Api.Spellbook.CanCast("Frost Armor") && !me.Auras.Contains("Frost Armor") && !me.Auras.Contains("Mage Armor"))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Frost Armor");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Frost Armor"))
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
        if (Api.Spellbook.CanCast("Arcane Intellect") && !me.Auras.Contains("Arcane Intellect"))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Arcane Intellect");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Arcane Intellect"))
                return true;
        }





        // Now needsWater variable will indicate if the character needs water
        if (needsWater)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Character needs water!");
            Console.ResetColor();

            // Add logic here to conjure water or perform any action needed to acquire water
            // Example: Cast "Conjure Water" spell
            // Assuming the API allows for conjuring water in a similar way to casting spells
            if (Api.Spellbook.CanCast("Conjure Water"))
            {
                if (Api.Spellbook.Cast("Conjure Water"))
                {
                    Console.WriteLine("Conjured water.");
                    // Add further actions if needed after conjuring water
                }
            }
        }

        // Now needsWater variable will indicate if the character needs food
        if (needsFood)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Character needs food!");
            Console.ResetColor();

            // Add logic here to conjure water or perform any action needed to acquire food
            // Example: Cast "Conjure food" spell
            // Assuming the API allows for conjuring food in a similar way to casting spells
            if (Api.Spellbook.CanCast("Conjure Food"))
            {
                if (Api.Spellbook.Cast("Conjure Food"))
                {
                    Console.WriteLine("Conjured Food.");
                    // Add further actions if needed after conjuring water
                }

            }
        }
        if (!target.IsDead() && (reaction != UnitReaction.Friendly && reaction != UnitReaction.Honored && reaction != UnitReaction.Revered && reaction != UnitReaction.Exalted) &&
            mana > 20 && !IsNPC(target))
        {
            var reaction = me.GetReaction(target);

            if (reaction != UnitReaction.Friendly)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Casting Fire Blast");
                Console.ResetColor();

                if (Api.Spellbook.Cast("Fire Blast"))
                {
                    return true;
                }
            }
            else
            {
                // Handle if the target is friendly
                Console.WriteLine("Target is friendly. Skipping Fire Blast cast.");
            }
        }
        else
        {
            // Handle if the target is not valid
            Console.WriteLine("Invalid target. Skipping Fire Blast cast.");
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

        if ((DateTime.Now - lastDebugTime).TotalSeconds >= debugInterval)
        {
            LogPlayerStats();
            lastDebugTime = DateTime.Now; // Update lastDebugTime
        }

        string[] GemTypes = { "Mana Jade", "Mana Citrine", "Mana Ruby", "Mana Emerald", "Mana Sapphire", "Mana Agate" };

        foreach (string gem in GemTypes)
        {
            if (mana <= 50 && !Api.Inventory.OnCooldown(gem) && HasItem(gem))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Using {gem} for mana");
                Console.ResetColor();

                if (Api.Inventory.Use(gem))
                {
                    return true;
                }
            }
        }

        if (me.IsDead() || me.IsGhost() || me.IsCasting() || me.IsChanneling()) return false;
        if (me.Auras.Contains("Drink") || me.Auras.Contains("Food")) return false;




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


        if (Api.Spellbook.CanCast("Combustion") && !me.Auras.Contains(28682) && !Api.Spellbook.OnCooldown("Combustion"))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Combustion");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Combustion"))
                return true;
        }
        if (Api.Spellbook.CanCast("Living Bomb") && mana >= 22 && !target.Auras.Contains("Living Bomb") && targethealth >= 30 && !Api.Spellbook.OnCooldown("Living Bomb"))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Living Bomb");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Living Bomb"))
                return true;
        }

        if (me.Auras.Contains("Hot Streak") && mana >= 22 && !target.Auras.Contains("Pyroblast") && targethealth >= 20)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Pyroblast");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Pyroblast"))
                return true;
        }
        if (Api.Spellbook.CanCast("Fire Blast") && !Api.Spellbook.OnCooldown("Fire Blast"))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Fire Blast");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Fire Blast"))
                return true;
        }
        if (Api.Spellbook.CanCast("Fireball") && mana >= 19 && targethealth > 20)
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

        if (Api.Spellbook.CanCast("Shoot"))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Shoot");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Shoot"))
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
            var remainingTimeSeconds = me.AuraRemains("Frost Armor");
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
        ShadowApi shadowApi = new ShadowApi();

        // Define food and water types
        string[] waterTypes = { "Conjured Mana Strudel", "Conjured Mountain Spring Water", "Conjured Crystal Water", "Conjured Sparkling Water", "Conjured Mineral Water", "Conjured Spring Water", "Conjured Purified Water", "Conjured Fresh Water", "Conjured Water" };
        string[] foodTypes = { "Conjured Mana Strudel", "Conjured Cinnamon Roll", "Conjured Sweet Roll", "Conjured Sourdough", "Conjured Pumpernickel", "Conjured Rye", "Conjured Bread", "Conjured Muffin" };

        // Count food items in the inventory
        int foodCount = 0;
        foreach (string foodType in foodTypes)
        {
            int count = shadowApi.Inventory.ItemCount(foodType);
            foodCount += count;
        }

        // Count water items in the inventory
        int waterCount = 0;
        foreach (string waterType in waterTypes)
        {
            int count = shadowApi.Inventory.ItemCount(waterType);
            waterCount += count;
        }

        // Display the counts of food and water items
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Current Food Count: " + foodCount);
        Console.WriteLine("Current Water Count: " + waterCount);
        Console.ResetColor();



        Console.ResetColor();
    }
}