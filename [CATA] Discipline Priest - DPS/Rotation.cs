using System;
using System.Threading;
using wShadow.Templates;
using System.Collections.Generic;
using wShadow.Warcraft.Classes;
using wShadow.Warcraft.Defines;
using wShadow.Warcraft.Managers;


public class PriestDiscoCATA : Rotation
{
    private bool HasItem(object item)
      => Api.Inventory.HasItem(item);
    private int debugInterval = 5; // Set the debug interval in seconds
    private DateTime lastDebugTime = DateTime.MinValue;
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
        var healthPercentage = me.HealthPercent;
        var targethealth = target.HealthPercent;
        var targetDistance = target.Position.Distance2D(me.Position);
		
        if ((DateTime.Now - lastDebugTime).TotalSeconds >= debugInterval)
        {
            LogPlayerStats();
            lastDebugTime = DateTime.Now; // Update lastDebugTime
        }

        if (me.IsDead() || me.IsGhost() || me.IsCasting() || me.IsMoving() || me.IsChanneling() || me.IsMounted() || me.Auras.Contains("Drink") || me.Auras.Contains("Food")) return false;

        if (Api.Spellbook.CanCast("Renew") && !me.Auras.Contains("Renew") && healthPercentage < 80)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Casting Renew");
                Console.ResetColor();
                if (Api.Spellbook.Cast("Renew"))
                {
                    return true;
                }
            }
            if (Api.Spellbook.CanCast("Power Word: Fortitude") && !me.Auras.Contains("Power Word: Fortitude"))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Casting Power Word: Fortitude");
                Console.ResetColor();
                if (Api.Spellbook.Cast("Power Word: Fortitude"))
                {
                    return true;
                }
            }
            if (Api.Spellbook.CanCast("Divine Spirit") && !me.Auras.Contains("Divine Spirit"))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Casting Divine Spirit");
                Console.ResetColor();
                if (Api.Spellbook.Cast("Divine Spirit"))
                {
                    return true;
                }
            }
           
            if (Api.Spellbook.CanCast("Shadow Protection") && !me.Auras.Contains("Shadow Protection"))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Casting Shadow Protection");
                Console.ResetColor();
                if (Api.Spellbook.Cast("Shadow Protection"))
                {
                    return true;
                }
            }
           if (Api.Spellbook.CanCast("Inner Fire") && !Api.Player.Auras.Contains("Inner Fire", false))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Casting Inner Fire");
                Console.ResetColor();
                if (Api.Spellbook.Cast("Inner Fire"))
                {
                    return true;
                }
            }
        var reaction = me.GetReaction(target);

        if (!target.IsDead() && (reaction != UnitReaction.Friendly && reaction != UnitReaction.Honored && reaction != UnitReaction.Revered && reaction != UnitReaction.Exalted) && !IsNPC(target))
            if (Api.Spellbook.CanCast("Smite") && targetDistance > 5 && targetDistance < 30 && !Api.Spellbook.OnCooldown("Smite"))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Casting Smite");
                Console.ResetColor();
                if (Api.Spellbook.Cast("Smite"))

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
        var healthPercentage = me.HealthPercent;
        var targethealth = target.HealthPercent;
        var targetDistance = target.Position.Distance2D(me.Position);

        if ((DateTime.Now - lastDebugTime).TotalSeconds >= debugInterval)
        {
            LogPlayerStats();
            lastDebugTime = DateTime.Now; // Update lastDebugTime
        }

        string[] HP = { "Major Healing Potion", "Superior Healing Potion", "Greater Healing Potion", "Healing Potion", "Lesser Healing Potion", "Minor Healing Potion" };
        string[] MP = { "Major Mana Potion", "Superior Mana Potion", "Greater Mana Potion", "Mana Potion", "Lesser Mana Potion", "Minor Mana Potion" };
        if (me.IsDead() || me.IsGhost() || me.IsCasting() || me.IsMoving() || me.IsChanneling() || me.IsMounted() || me.Auras.Contains("Drink") || me.Auras.Contains("Food")) return false;

        if (me.HealthPercent <= 70 && (!Api.Inventory.OnCooldown(MP) && !Api.Inventory.OnCooldown(HP)))
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

        if (me.ManaPercent <= 50 && (!Api.Inventory.OnCooldown(MP) && !Api.Inventory.OnCooldown(HP)))
        {
            foreach (string manapot in MP)
            {
                if (HasItem(manapot))
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Using mana potion");
                    Console.ResetColor();
                    if (Api.Inventory.Use(manapot))
                    {
                        return true;
                    }
                }
            }
        }

        if (Api.Spellbook.CanCast("Flash Heal") && healthPercentage <= 50 && mana > 50)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Flash Heal");
            Console.ResetColor();
            if (Api.Spellbook.Cast("Flash Heal"))
            {
                return true;
            }
        }
        if (Api.Spellbook.CanCast("Renew") && healthPercentage <= 50 && mana > 25 && !me.Auras.Contains("Renew"))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Renew");
            Console.ResetColor();
            if (Api.Spellbook.Cast("Renew"))
            {
                return true;
            }
        }
        if (Api.Spellbook.CanCast("Inner Fire") && !Api.Player.Auras.Contains("Inner Fire", false))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Inner Fire");
            Console.ResetColor();
            if (Api.Spellbook.Cast("Inner Fire"))
            {
                return true;
            }
        }

        if (Api.Spellbook.CanCast("Power Infusion") && !Api.Spellbook.OnCooldown("Power Infusion") && mana > 25)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Power Infusion");
            Console.ResetColor();
            if (Api.Spellbook.Cast("Power Infusion"))
            {
                return true;
            }
        }

        if (Api.Spellbook.CanCast("Power Word: Shield") && !me.Auras.Contains("Power Word: Shield") && !me.Auras.Contains("Weakened Soul") && mana > 25)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Power Word: Shield");
            Console.ResetColor();
            if (Api.Spellbook.Cast("Power Word: Shield"))
            {
                return true;
            }
        }
        if (Api.Spellbook.CanCast("Shadowfiend") && !Api.Spellbook.OnCooldown("Shadowfiend") && mana > 25)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Shadowfiend");
            Console.ResetColor();
            if (Api.Spellbook.Cast("Shadowfiend"))
            {
                return true;
            }
        }
        if (Api.Spellbook.CanCast("Shadow Word: Pain") && !target.Auras.Contains("Shadow Word: Pain") && targethealth >= 30 && mana > 35)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Shadow Word: Pain");
            Console.ResetColor();
            if (Api.Spellbook.Cast("Shadow Word: Pain"))
            {
                return true;
            }
        }
        if (Api.Spellbook.CanCast("Holy Fire") && !target.Auras.Contains("Holy Fire") && targethealth >= 30 && mana>35)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Holy Fire");
            Console.ResetColor();
            if (Api.Spellbook.Cast("Holy Fire"))
            {
                return true;
            }
        }
        if (Api.Spellbook.CanCast("Penance") && mana > 35)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Penance");
            Console.ResetColor();
            if (Api.Spellbook.Cast("Penance"))
            {
                return true;
            }
        }
        if (Api.Spellbook.CanCast("Smite") && mana > 25)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Smite");
            Console.ResetColor();
            if (Api.Spellbook.Cast("Smite"))
            {
                return true;
            }
        }
        if (Api.HasMacro("Shoot"))


        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Shoot");
            Console.ResetColor();

            if (Api.UseMacro("Shoot"))
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

        // Health percentage of the player
        var healthPercentage = me.HealthPercent;


        // Target distance from the player
        var targetDistance = target.Position.Distance2D(me.Position);


        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"{mana}% Mana available");
        Console.WriteLine($"{healthPercentage}% Health available");

        Console.ResetColor();





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