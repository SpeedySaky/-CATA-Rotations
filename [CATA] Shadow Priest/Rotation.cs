using System;
using System.Threading;
using wShadow.Templates;
using System.Collections.Generic;
using wShadow.Warcraft.Classes;
using wShadow.Warcraft.Defines;
using wShadow.Warcraft.Managers;


public class PriestShadowWOTLK : Rotation
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
    private CreatureType GetCreatureType(WowUnit unit)
    {
        return unit.Info.GetCreatureType();
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
        var mana = me.ManaPercent;
        var healthPercentage = me.HealthPercent;
        var target = Api.Target;

        var targetDistance = target.Position.Distance2D(me.Position);
        var reaction = me.GetReaction(target);

        var targethealth = target.HealthPercent;

        if (me.IsDead() || me.IsGhost() || me.IsCasting() || me.IsChanneling() || me.IsMounted() || me.Auras.Contains("Drink") || me.Auras.Contains("Food")) return false;

        if ((DateTime.Now - lastDebugTime).TotalSeconds >= debugInterval)
        {
            LogPlayerStats();
            lastDebugTime = DateTime.Now; // Update lastDebugTime
        }


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
            if (Api.Spellbook.CanCast("Vampiric Embrace") && !me.Auras.Contains("Vampiric Embrace",false))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Casting Vampiric Embrace");
                Console.ResetColor();
                if (Api.Spellbook.Cast("Vampiric Embrace"))
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
            if (Api.Spellbook.CanCast("Inner Fire") && !me.Auras.Contains("Inner Fire",false))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Casting Inner Fire");
                Console.ResetColor();
                if (Api.Spellbook.Cast("Inner Fire"))
                {
                    return true;
                }
            }
            if (Api.Spellbook.CanCast("Shadowform") && !me.Auras.Contains("Shadowform",false))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Casting Shadowform");
                Console.ResetColor();
                if (Api.Spellbook.Cast("Shadowform"))
                {
                    return true;
                }
            }

        if (!target.IsDead() && (reaction != UnitReaction.Friendly && reaction != UnitReaction.Honored && reaction != UnitReaction.Revered && reaction != UnitReaction.Exalted) && !IsNPC(target))
        {
            if (Api.Spellbook.CanCast("Shadow Word: Pain") && targetDistance < 40 )
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Casting Shadow Word: Pain");
                Console.ResetColor();

                if (Api.Spellbook.Cast("Shadow Word: Pain"))
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
        var healthPercentage = me.HealthPercent;
        var targethealth = target.HealthPercent;
        var targetDistance = target.Position.Distance2D(me.Position);
        if (me.IsDead() || me.IsGhost() || me.IsCasting() || me.IsMoving() || me.IsChanneling() || me.IsMounted() || me.Auras.Contains("Drink") || me.Auras.Contains("Food")) return false;

        if ((DateTime.Now - lastDebugTime).TotalSeconds >= debugInterval)
        {
            LogPlayerStats();
            lastDebugTime = DateTime.Now; // Update lastDebugTime
        }



        if (Api.Spellbook.CanCast("Shadowform") && !me.Auras.Contains("Shadowform",false))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Shadowform");
            Console.ResetColor();
            if (Api.Spellbook.Cast("Shadowform"))
            {
                return true;
            }
        }

        if (Api.Spellbook.CanCast("Power Word: Shield") && !me.Auras.Contains("Power Word: Shield") && !me.Auras.Contains("Weakened Soul"))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Power Word: Shield");
            Console.ResetColor();
            if (Api.Spellbook.Cast("Power Word: Shield"))
            {
                return true;
            }
        }
        if (Api.Spellbook.CanCast("Shadowfiend") && !Api.Spellbook.OnCooldown("Shadowfiend"))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Shadowfiend");
            Console.ResetColor();
            if (Api.Spellbook.Cast("Shadowfiend"))
            {
                return true;
            }
        }
        if (Api.Spellbook.CanCast("Shadow Word: Pain") && !target.Auras.Contains("Shadow Word: Pain") && targethealth >= 30 && mana >= 22)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Shadow Word: Pain");
            Console.ResetColor();
            if (Api.Spellbook.Cast("Shadow Word: Pain"))
            {
                return true;
            }
        }
        if (Api.Spellbook.CanCast("Shadow Word: Death") && !target.Auras.Contains("Shadow Word: Death") && targethealth <= 10 && mana >= 12)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Shadow Word: Death");
            Console.ResetColor();
            if (Api.Spellbook.Cast("Shadow Word: Death"))
            {
                return true;
            }
        }
        if (Api.Spellbook.CanCast("Devouring Plague") && !target.Auras.Contains("Devouring Plague") && targethealth >= 30 && mana>=25)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Devouring Plague");
            Console.ResetColor();
            if (Api.Spellbook.Cast("Devouring Plague"))
            {
                return true;
            }
        }
        if (Api.Spellbook.CanCast("Mind Blast") && targethealth >= 30 && mana >= 17)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Mind Blast");
            Console.ResetColor();
            if (Api.Spellbook.Cast("Mind Blast"))
            {
                return true;
            }
        }
        if (Api.Spellbook.CanCast("Shoot"))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Shoot");
            Console.ResetColor();
            if (Api.Spellbook.Cast("Shoot"))
            {
                return true;
            }
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





        Console.ResetColor();
    }
}