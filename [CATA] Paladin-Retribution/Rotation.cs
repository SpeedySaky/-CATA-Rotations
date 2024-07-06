using System;
using System.Threading;
using wShadow.Templates;
using System.Collections.Generic;
using wShadow.Warcraft.Classes;
using wShadow.Warcraft.Defines;
using wShadow.Warcraft.Managers;
using wShadow.WowBots;
using wShadow.WowBots.PartyInfo;

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

    public override bool PassivePulse()
    {
        var me = Api.Player;
        var healthPercentage = me.HealthPercent;
        var mana = me.ManaPercent;
        var target = Api.Target;
        var targetDistance = target.Position.Distance2D(me.Position);


        if ( me.IsDead() || me.IsGhost() || me.IsCasting() ||  me.IsChanneling()  || me.Auras.Contains("Drink") || me.Auras.Contains("Food")) return false;

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

        if (Api.Spellbook.CanCast("Blessing of Kings") && mana > 5 && !Api.Player.Auras.Contains("Blessing of Kings") && !Api.Player.Auras.Contains("Hand of Protection") && !Api.Player.Auras.Contains("Divine Protection") && !Api.Player.IsMounted())
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Blessing of Kings");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Blessing of Kings"))
            {
                return true;
            }
        }
        else if (Api.Spellbook.CanCast("Blessing of Might") && mana > 5 && !Api.Player.Auras.Contains("Blessing of Kings") && !Api.Player.Auras.Contains("Blessing of Might") && !Api.Player.IsMounted())
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Blessing of Might");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Blessing of Might"))
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

        if (Api.Spellbook.CanCast("Flash of Light")  && healthPercentage < 60 && mana >31)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Flash of Light");
            Console.ResetColor();
            if (Api.Spellbook.Cast("Flash of Light"))
            {
                return true;
            }
        }
        if (Api.Spellbook.CanCast("Holy Wrath") && targetHealth <= 20 && mana >20)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Holy Wrath");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Holy Wrath"))
            {
                return true;
            }
        }
        if (Api.Spellbook.CanCast("Rebuke") && mana > 10 && !Api.Spellbook.OnCooldown("Rebuke") && (target.IsCasting() || target.IsChanneling()) )
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Rebuke");
            Console.ResetColor();
            if (Api.Spellbook.Cast("Rebuke"))
            {
                return true;
            }
        }
        if (Api.Spellbook.CanCast("Divine Protection") && healthPercentage < 45 && !me.IsCasting() && !Api.Player.Auras.Contains("Forbearance") && mana > 3)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Divine Protection");
            Console.ResetColor();
            if (Api.Spellbook.Cast("Divine Protection"))
            {
                return true;
            }
        }

        if (Api.Player.Auras.Contains("Divine Protection") && healthPercentage <= 50 && Api.Spellbook.CanCast("Holy Light") && mana > 12) 
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
       
        if (Api.Spellbook.CanCast("Avenging Wrath") && !Api.Spellbook.OnCooldown("Avenging Wrath") && mana > 8)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Avenging Wrath");
            Console.ResetColor();
            if (Api.Spellbook.Cast("Avenging Wrath"))
            {
                return true;
            }
        }


        if (Api.Spellbook.CanCast("Seal of Truth") && !Api.Player.Auras.Contains("Seal of Truth") && mana > 14)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Seal of Truth");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Seal of Truth"))
            {
                return true;
            }
        }
        else
        if (Api.Spellbook.CanCast("Seal of Righteousness") && !Api.Player.Auras.Contains("Seal of Truth") && mana > 14 && !Api.Player.Auras.Contains("Seal of Righteousness"))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Seal of Righteousness");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Seal of Righteousness"))
            {
                return true;
            }
        }

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

        if (Api.UnfriendlyUnitsNearby(8, true) >= 2 && Api.Spellbook.CanCast("Divine Storm") && !!Api.Spellbook.OnCooldown("Divine Storm") && mana > 5)
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
        else if (Api.Spellbook.CanCast("Hammer of Wrath") && Api.Player.Auras.Contains("Avenging Wrath") && !Api.Spellbook.OnCooldown("Avenging Wrath") && mana > 12)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Hammer of Wrath");
            Console.ResetColor();
            if (Api.Spellbook.Cast("Hammer of Wrath"))
            {
                return true;
            }
        }


        if (Api.Spellbook.CanCast("Crusader Strike") && !Api.Spellbook.OnCooldown("Crusader Strike") && mana > 10)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Crusader Strike");
            Console.ResetColor();
            if (Api.Spellbook.Cast("Crusader Strike"))
            {
                return true;
            }
        }
        if (Api.Spellbook.CanCast("Judgement") && mana > 15 && !Api.Spellbook.OnCooldown("Judgement") && !Api.Spellbook.OnCooldown("Judgement") && (me.Auras.Contains("Seal of Truth") || me.Auras.Contains("Seal of Righteousness") || me.Auras.Contains("Seal of Command")) && mana > 5)
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


    public void BuffPartyMembers(PartyBot partyBotInstance)
    {
        if (Api.Spellbook.CanCast("Blessing of Kings") && Api.Player.ManaPercent > 5 && !Api.Player.IsMounted())
        {
            // Target and buff the leader
            PartyBot.TargetLeader();
            CastBuffOnTarget("Blessing of Kings");

            // Target and buff the tank(s)
            for (int i = 0; i < partyBotInstance.TankCount; i++)
            {
                PartyBot.TargetTank(i);
                CastBuffOnTarget("Blessing of Kings");
            }

            // Target and buff the healer(s)
            for (int i = 0; i < partyBotInstance.HealerCount; i++)
            {
                PartyBot.TargetHealer(i);
                CastBuffOnTarget("Blessing of Kings");
            }

            // Target and buff the DPS members
            for (int i = 0; i < partyBotInstance.DpsCount; i++)
            {
                PartyBot.TargetDps(i);
                CastBuffOnTarget("Blessing of Kings");
            }
        }
    }

    private void CastBuffOnTarget(string spellName)
    {
        // Assuming there's a method to check if the current target already has the buff
        // This is pseudocode and needs to be replaced with actual logic
        if (!Api.Target.Auras.Contains(spellName))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Casting {spellName} on current target.");
            Console.ResetColor();
            Api.Spellbook.Cast(spellName);

            // Add a delay to ensure the game registers the cast before moving on
            Thread.Sleep(1000); // Adjust based on actual game behavior
        }
    }



    private void LogPlayerStats()
    {
        var me = Api.Player;

        var mana = me.ManaPercent;
        var healthPercentage = me.HealthPercent;
        var HolyPower = me.HolyPower;

        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"{mana}% Mana available");
        Console.WriteLine($"{healthPercentage}% Health available");
        Console.WriteLine($"{HolyPower} HolyPower available");
        if (PartyBot.IsInParty)
        {
            Console.WriteLine($"Player is in a party. Party size: {PartyBot.MemberCount}");
        }
        else
        {
            Console.WriteLine("Player is not in a party.");
        }

        // Get the party leader
        WowUnit leaderUnit = PartyBot.GetLeader();
        if (leaderUnit != null)
        {
            Console.WriteLine($"Leader Name: {leaderUnit.Name}"); // Assuming WowUnit has a Name property
        }

        // Example for getting the first DPS, Tank, and Healer names
        WowUnit dpsUnit = PartyBot.GetDpsUnit(0);
        if (dpsUnit != null)
        {
            Console.WriteLine($"DPS Name: {dpsUnit.Name}"); // Assuming WowUnit has a Name property
        }

        WowUnit tankUnit = PartyBot.GetTankUnit(0);
        if (tankUnit != null)
        {
            Console.WriteLine($"Tank Name: {tankUnit.Name}"); // Assuming WowUnit has a Name property
        }

        WowUnit healerUnit = PartyBot.GetHealerUnit(0);
        if (healerUnit != null)
        {
            Console.WriteLine($"Healer Name: {healerUnit.Name}"); // Assuming WowUnit has a Name property
        }

        // If you need to list all party members
        WowUnit[] memberUnits = PartyBot.GetMemberUnits();
        if (memberUnits != null)
        {
            foreach (WowUnit member in memberUnits)
            {
                Console.WriteLine($"Member Name: {member.Name}"); // Assuming WowUnit has a Name property
            }
        }

    }

}