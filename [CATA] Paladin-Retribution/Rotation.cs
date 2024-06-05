using System;
using System.Threading;
using wShadow.Templates;
using System.Collections.Generic;
using wShadow.Warcraft.Classes;
using wShadow.Warcraft.Defines;
using wShadow.Warcraft.Managers;


public class RetPalaWOTLK : Rotation
{
    private int debugInterval = 5;
    private DateTime lastDebugTime = DateTime.MinValue;
    public bool IsValid(WowUnit unit)
    {
        if (unit == null || unit.Address == null)
        {
            return false;
        }
        return true;
    }
    private List<string> npcConditions = new List<string>
    {
        "Innkeeper", "Auctioneer", "Banker", "FlightMaster", "GuildBanker",
        "PlayerVehicle", "StableMaster", "Repair", "Trainer", "TrainerClass",
        "TrainerProfession", "Vendor", "VendorAmmo", "VendorFood", "VendorPoison",
        "VendorReagent", "WildBattlePet", "GarrisonMissionNPC", "GarrisonTalentNPC",
        "QuestGiver"
    };

    private CreatureType GetCreatureType(WowUnit unit)
    {
        return unit.Info.GetCreatureType();
    }
    public override void Initialize()
    {

        lastDebugTime = DateTime.Now;
        LogPlayerStats();
        SlowTick = 1000;
        FastTick = 400;

    }

    public override bool MountedPulse()
    {
        var me = Api.Player;
        var healthPercentage = me.HealthPercent;
        var mana = me.ManaPercent;

        if (me.IsDead() || me.IsGhost()) return false;




        if (Api.Spellbook.CanCast("Crusader Aura") && !Api.Player.Auras.Contains("Crusader Aura", false) && me.IsMounted())
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Crusader Aura");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Crusader Aura"))
            {
                return true;
            }
        }
        return base.MountedPulse();
    }




    public override bool PassivePulse()
    {
        var me = Api.Player;
        var healthPercentage = me.HealthPercent;
        var mana = me.ManaPercent;
        var target = Api.Target;
        var targetDistance = target.Position.Distance2D(me.Position);


        if (!target.IsValid() || me.IsDead() || me.IsGhost() || me.IsCasting() || me.IsMoving() || me.IsChanneling() || me.Auras.Contains("Drink") || me.Auras.Contains("Food")) return false;

        if ((DateTime.Now - lastDebugTime).TotalSeconds >= debugInterval)
        {
            LogPlayerStats();
            lastDebugTime = DateTime.Now; // Update lastDebugTime
        }


        


        if (Api.Spellbook.CanCast("Retribution Aura") && !Api.Player.Auras.Contains("Retribution Aura", false) && !me.IsMounted())
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Retribution Aura");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Retribution Aura"))
            {
                return true;
            }
        }
        else if (Api.Spellbook.CanCast("Devotion Aura") && !Api.Player.Auras.Contains("Devotion Aura", false) && !Api.Player.Auras.Contains("Retribution Aura", false) && !me.IsMounted())
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Devotion Aura");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Devotion Aura"))
            {
                return true;
            }
        }
        else if (Api.Spellbook.CanCast("Crusader Aura") && !Api.Player.Auras.Contains("Crusader Aura", false) && me.IsMounted())
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Crusader Aura");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Crusader Aura"))
            {
                return true;
            }
        }

        if (Api.Spellbook.CanCast("Blessing of Might") && mana > 5 && !Api.Player.Auras.Contains("Blessing of Might") && !Api.Player.Auras.Contains("Hand of Protection") && !Api.Player.Auras.Contains("Divine Protection") && !Api.Player.IsMounted())
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Blessing of Might");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Blessing of Might"))
            {
                return true;
            }
        }
        else if (Api.Spellbook.CanCast("Blessing of Wisdom") && mana > 5 && !Api.Player.Auras.Contains("Blessing of Wisdom") && !Api.Player.Auras.Contains("Blessing of Might") && !Api.Player.IsMounted())
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Blessing of Wisdom");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Blessing of Wisdom"))
            {
                return true;
            }
        }
        var reaction = me.GetReaction(target);

        if (!target.IsDead() && (reaction != UnitReaction.Friendly && reaction != UnitReaction.Honored && reaction != UnitReaction.Revered && reaction != UnitReaction.Exalted) && !IsNPC(target))
        {
            if (Api.Spellbook.CanCast("Judgement") && targetDistance > 5 && targetDistance < 30 && !Api.Spellbook.OnCooldown("Judgement"))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Casting Judgement");
                Console.ResetColor();

                if (Api.Spellbook.Cast("Judgement"))
                    return true;
            }
        }
        return base.PassivePulse();
    }

    public override bool CombatPulse()
    {
        var me = Api.Player;
        var healthPercentage = me.HealthPercent;
        var mana = me.ManaPercent;
        var HolyPower = me.HolyPower;

        var target = Api.Target;
        var targetHealth = Api.Target.HealthPercent;
        if (!target.IsValid() || me.IsDead() || me.IsGhost() || me.IsCasting() || me.IsMoving() || me.IsChanneling() || me.IsMounted() || me.Auras.Contains("Drink") || me.Auras.Contains("Food")) return false;

        if (Api.Spellbook.CanCast("Flash of Light")  && healthPercentage < 60)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Flash of Light");
            Console.ResetColor();
            if (Api.Spellbook.Cast("Flash of Light"))
            {
                return true;
            }
        }
        if (Api.Spellbook.CanCast("Holy Wrath") && targetHealth <= 20)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Holy Wrath");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Holy Wrath"))
            {
                return true;
            }
        }
        if (Api.Spellbook.CanCast("Rebuke") && mana > 10 && !Api.Spellbook.OnCooldown("Rebuke") && (target.IsCasting() || target.IsChanneling()))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Rebuke");
            Console.ResetColor();
            if (Api.Spellbook.Cast("Rebuke"))
            {
                return true;
            }
        }
        if (Api.Spellbook.CanCast("Divine Protection") && healthPercentage < 45 && !me.IsCasting() && !Api.Player.Auras.Contains("Forbearance"))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Divine Protection");
            Console.ResetColor();
            if (Api.Spellbook.Cast("Divine Protection"))
            {
                return true;
            }
        }

        if (Api.Player.Auras.Contains("Divine Protection") && healthPercentage <= 50 && Api.Spellbook.CanCast("Holy Light"))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Holy Light");
            Console.ResetColor();
            if (Api.Spellbook.Cast("Holy Light"))
            {
                return true;
            }
        }
        CreatureType targetCreatureType = GetCreatureType(target);

        if (Api.Player.Auras.Contains(59578) && Api.Spellbook.CanCast("Exorcism")) 
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Exorcism with Art of War");
            Console.ResetColor();
            if (Api.Spellbook.Cast("Exorcism"))
            {
                return true;
            }
        }
        else if (Api.Spellbook.CanCast("Exorcism") && !Api.Spellbook.OnCooldown("Exorcism") && mana > 70 && (targetCreatureType == CreatureType.Undead || targetCreatureType == CreatureType.Demon))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Exorcism on Demon or Undead");
            Console.ResetColor();
            if (Api.Spellbook.Cast("Exorcism"))
            {
                return true;
            }
        }
       
        if (Api.Spellbook.CanCast("Avenging Wrath") && !Api.Spellbook.OnCooldown("Avenging Wrath"))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Avenging Wrath");
            Console.ResetColor();
            if (Api.Spellbook.Cast("Avenging Wrath"))
            {
                return true;
            }
        }


        if (Api.Spellbook.CanCast("Seal of Truth") && !Api.Player.Auras.Contains("Seal of Truth"))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Seal of Truth");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Seal of Truth"))
            {
                return true;
            }
        }

        if (Api.UnfriendlyUnitsNearby(8, true) >= 2 && Api.Spellbook.CanCast("Consecration") && !!Api.Spellbook.OnCooldown("Consecration"))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Consecration");
            Console.ResetColor();
            if (Api.Spellbook.Cast("Consecration"))
            {
                return true;
            }
        }

        if (Api.UnfriendlyUnitsNearby(8, true) >= 2 && Api.Spellbook.CanCast("Divine Storm") && !!Api.Spellbook.OnCooldown("Divine Storm"))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Divine Storm");
            Console.ResetColor();
            if (Api.Spellbook.Cast("Divine Storm"))
            {
                return true;
            }
        }
        if (Api.Spellbook.CanCast("Templar's Verdict") && Api.Player.Auras.Contains(90174))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Templar's Verdict with Divine Purpose");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Templar's Verdict"))
            {
                return true;
            }
        }
        if (Api.Spellbook.CanCast("Templar's Verdict") && HolyPower >= 3)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Templar's Verdict");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Templar's Verdict"))
            {
                return true;
            }
        }
        

        if (Api.Spellbook.CanCast("Hammer of Wrath") && targetHealth <= 20)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Hammer of Wrath");
            Console.ResetColor();
            if (Api.Spellbook.Cast("Hammer of Wrath"))
            {
                return true;
            }
        }
        else if (Api.Spellbook.CanCast("Hammer of Wrath") && Api.Player.Auras.Contains("Avenging Wrath") && !Api.Spellbook.OnCooldown("Avenging Wrath"))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Hammer of Wrath");
            Console.ResetColor();
            if (Api.Spellbook.Cast("Hammer of Wrath"))
            {
                return true;
            }
        }


        if (Api.Spellbook.CanCast("Crusader Strike") && !Api.Spellbook.OnCooldown("Crusader Strike"))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Crusader Strike");
            Console.ResetColor();
            if (Api.Spellbook.Cast("Crusader Strike"))
            {
                return true;
            }
        }
        if (Api.Spellbook.CanCast("Judgement") && mana > 15 && !Api.Spellbook.OnCooldown("Judgement") && !Api.Spellbook.OnCooldown("Judgement") && (me.Auras.Contains("Seal of Truth") || me.Auras.Contains("Seal of Righteousness") || me.Auras.Contains("Seal of Command")))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Judgement");
            Console.ResetColor();
            if (Api.Spellbook.Cast("Judgement"))
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
        var me = Api.Player;

        var mana = me.ManaPercent;
        var healthPercentage = me.HealthPercent;
        var HolyPower = me.HolyPower;


        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"{mana}% Mana available");
        Console.WriteLine($"{healthPercentage}% Health available");   // Insert your player stats logging using the new API
        Console.WriteLine($"{HolyPower} HolyPower available");   // Insert your player stats logging using the new API

    }
}