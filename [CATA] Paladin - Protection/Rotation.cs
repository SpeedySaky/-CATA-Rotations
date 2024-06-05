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

        // Maintain your assigned Aura
        // Assuming "Devotion Aura" as assigned Aura
        if (Api.Spellbook.CanCast("Devotion Aura") && !Api.Player.Auras.Contains("Devotion Aura", false))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Devotion Aura");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Devotion Aura"))
            {
                return true;
            }
        }
        if (Api.Spellbook.CanCast("Holy Shield") && !Api.Spellbook.OnCooldown("Holy Shield") && mana >3)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Holy Shield");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Holy Shield"))
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
        // Cast Divine Protection
        if (Api.Spellbook.CanCast("Divine Protection") && !Api.Spellbook.OnCooldown("Divine Protection"))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Divine Protection");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Divine Protection"))
            {
                return true;
            }
        }

        // Cast Divine Guardian
        if (Api.Spellbook.CanCast("Divine Guardian") && !Api.Spellbook.OnCooldown("Divine Guardian"))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Divine Guardian");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Divine Guardian"))
            {
                return true;
            }
        }

        // Cast Ardent Defender
        if (Api.Spellbook.CanCast("Ardent Defender") && !Api.Spellbook.OnCooldown("Ardent Defender"))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Ardent Defender");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Ardent Defender"))
            {
                return true;
            }
        }

        // Cast Guardian of Ancient Kings
        if (Api.Spellbook.CanCast("Guardian of Ancient Kings") && !Api.Spellbook.OnCooldown("Guardian of Ancient Kings"))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Guardian of Ancient Kings");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Guardian of Ancient Kings"))
            {
                return true;
            }
        }

        // Cast Lay on Hands
        if (Api.Spellbook.CanCast("Lay on Hands") && !Api.Spellbook.OnCooldown("Lay on Hands") && healthPercentage<10)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Lay on Hands");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Lay on Hands"))
            {
                return true;
            }
        }

        // Use Seal of Truth
        if (Api.Spellbook.CanCast("Seal of Truth") && !Api.Player.Auras.Contains("Seal of Truth") && mana >14)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Seal of Truth");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Seal of Truth"))
            {
                return true;
            }
        }
        CreatureType targetCreatureType = GetCreatureType(target);

        if ((Api.Player.Auras.Contains(59578) || Api.Player.Auras.Contains(53489)) && Api.Spellbook.CanCast("Exorcism") && !Api.Spellbook.OnCooldown("Exorcism"))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Exorcism with Art of truth");
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
        // Cast Avenging Wrath
        if (Api.Spellbook.CanCast("Avenging Wrath") && !Api.Spellbook.OnCooldown("Avenging Wrath") & mana > 8)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Avenging Wrath");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Avenging Wrath"))
            {
                return true;
            }
        }

        // Cast Divine Plea if you need Mana, or to generate 3 Holy Power thanks to Shield of the Templar
        if (Api.Spellbook.CanCast("Divine Plea") && mana < 50)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Divine Plea");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Divine Plea"))
            {
                return true;
            }
        }

        // Cast Shield of the Righteous with 3 Holy Power
        if (Api.Spellbook.CanCast("Shield of the Righteous") && HolyPower >= 3)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Shield of the Righteous");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Shield of the Righteous"))
            {
                return true;
            }
        }



        // Cast Avenger's Shield
        if (Api.Spellbook.CanCast("Avenger's Shield") && mana> 6)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Avenger's Shield");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Avenger's Shield"))
            {
                return true;
            }
        }

        // Cast Hammer of Wrath when target is below 20% HP
        if (Api.Spellbook.CanCast("Hammer of Wrath") && targetHealth <= 20 && mana > 12)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Hammer of Wrath");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Hammer of Wrath"))
            {
                return true;
            }
        }

        // Cast Judgement
        if (Api.Spellbook.CanCast("Judgement") && mana > 5 && !Api.Spellbook.OnCooldown("Judgement") && !Api.Spellbook.OnCooldown("Judgement") && (me.Auras.Contains("Seal of Truth") || me.Auras.Contains("Seal of Righteousness") || me.Auras.Contains("Seal of Command")))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Judgement");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Judgement"))
            {
                return true;
            }
        }

        // Cast Consecration
        if (Api.UnfriendlyUnitsNearby(8, true) >= 2 && Api.Spellbook.CanCast("Consecration") && !!Api.Spellbook.OnCooldown("Consecration") && mana > 55)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Consecration");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Consecration"))
            {
                return true;
            }
        }

        // Cast Holy Wrath

        if (Api.Spellbook.CanCast("Holy Wrath") && mana > 20 && (targetCreatureType == CreatureType.Undead || targetCreatureType == CreatureType.Demon))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Holy Wrath");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Holy Wrath"))
            {
                return true;
            }
        }
        if (Api.Spellbook.CanCast("Crusader Strike") && mana > 10)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Crusader Strike");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Crusader Strike"))
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


        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine($"{mana}% Mana available");
        Console.WriteLine($"{healthPercentage}% Health available");   // Insert your player stats logging using the new API
        Console.WriteLine($"{HolyPower} HolyPower available");   // Insert your player stats logging using the new API

        if (Api.Player.Auras.Contains("Seal of Truth"))
        {
            double timeRemaining = Api.Player.Auras.TimeRemaining("Seal of Truth");
            int minutes = (int)(timeRemaining / 1000 / 60);
            int seconds = (int)(timeRemaining / 1000 % 60);
            Console.WriteLine($"Time remaining for Seal of Truth: {minutes} minutes and {seconds} seconds");
        }
        else
        {
            Console.WriteLine("Seal of Truth is not active.");
        }

        Console.ResetColor();


    }
}