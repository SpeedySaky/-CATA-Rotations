using System;
using System.Threading;
using wShadow.Templates;
using System.Collections.Generic;
using wShadow.Warcraft.Classes;
using wShadow.Warcraft.Defines;
using wShadow.Warcraft.Managers;
using wShadow.Warcraft.Usefuls;

public class BoomkinCATA : Rotation
{
    private int debugInterval = 5;
    private DateTime lastDebugTime = DateTime.MinValue;
    private List<string> npcConditions = new List<string>
    {
        "Innkeeper", "Auctioneer", "Banker", "FlightMaster", "GuildBanker",
        "PlayerVehicle", "StableMaster", "Repair", "Trainer", "TrainerClass",
        "TrainerProfession", "Vendor", "VendorAmmo", "VendorFood", "VendorPoison",
        "VendorReagent", "WildBattlePet", "GarrisonMissionNPC", "GarrisonTalentNPC",
        "QuestGiver"
    };
    private DateTime Swarm = DateTime.MinValue;
    private TimeSpan lastInsectSwarmCast = TimeSpan.FromSeconds(10);

    private DateTime Sunfire = DateTime.MinValue;
    private TimeSpan lastSunfire = TimeSpan.FromSeconds(10);

    public bool IsValid(WowUnit unit)
    {
        if (unit == null || unit.Address == null)
        {
            return false;
        }
        return true;
    }
    public bool FlightForm()
    {
        var me = Api.Player;

        if (me.Auras.Contains("Flight Form", false) || me.Auras.Contains("Swift Flight Form", false) || me.Auras.Contains("Ghost Wolf", false))
            return true;
        else
            return false;
    }
    private CreatureType GetCreatureType(WowUnit unit)
    {
        return unit.Info.GetCreatureType();
    }
    public override void Initialize()
    {

        lastDebugTime = DateTime.Now;
        LogPlayerStats();
        SlowTick = 800;
        FastTick = 400;

    }

    public override bool MountedPulse()
    {
        var me = Api.Player;
        var healthPercentage = me.HealthPercent;
        var mana = me.ManaPercent;

        if (me.IsDead() || me.IsGhost()) return false;




        return base.MountedPulse();
    }




    public override bool PassivePulse()
    {
        var me = Api.Player;
        var healthPercentage = me.HealthPercent;
        var mana = me.ManaPercent;
        var target = Api.Target;
        var targetHealth = Api.Target.HealthPercent;
        var targetDistance = target.Position.Distance2D(me.Position);

        if (me.IsMounted() || FlightForm() || me.IsDead() || me.IsGhost() || me.IsCasting() || me.IsChanneling() || me.Auras.Contains("Drink") || me.Auras.Contains("Food")) return false;

        if ((DateTime.Now - lastDebugTime).TotalSeconds >= debugInterval)
        {
            LogPlayerStats();
            lastDebugTime = DateTime.Now; // Update lastDebugTime
        }


        if (Api.Spellbook.CanCast("Innervate") && mana <= 20 && Api.Spellbook.OnCooldown("Innervate"))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Innervate");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Innervate"))
            {
                return true;
            }
        }





        if (Api.Spellbook.CanCast("Mark of the Wild") && !Api.Player.Auras.Contains("Mark of the Wild") && !Api.Player.IsMounted())
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Mark of the Wild");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Mark of the Wild"))
            {
                return true;
            }
        }

        if (Api.Spellbook.CanCast("Rejuvenation") && !Api.Player.Auras.Contains("Rejuvenation") && healthPercentage < 50 && !Api.Player.IsMounted())
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Rejuvenation");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Rejuvenation"))
            {
                return true;
            }
        }
        if (Api.Spellbook.CanCast("Regrowth") && !Api.Player.Auras.Contains("Regrowth") && healthPercentage < 30 && !Api.Player.IsMounted())
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Regrowth");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Regrowth"))
            {
                return true;
            }
        }
        if (Api.Spellbook.CanCast("Healing Touch") && healthPercentage < 40 && !Api.Player.IsMounted())
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Healing Touch");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Healing Touch"))
            {
                return true;
            }
        }

        if (Api.Spellbook.CanCast("Moonkin Form") && !Api.Player.Auras.Contains("Moonkin Form", false) && !Api.Player.IsMounted())
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Moonkin Form");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Moonkin Form"))
            {
                return true;
            }
        }
        var reaction = me.GetReaction(target);
        if (target.IsValid())
        {
            if (!target.IsDead() && (reaction != UnitReaction.Friendly && reaction != UnitReaction.Honored && reaction != UnitReaction.Revered && reaction != UnitReaction.Exalted) && !IsNPC(target) && Api.Spellbook.CanCast("Moonfire") && targetDistance > 5 && targetDistance < 40 && !Api.Spellbook.OnCooldown("Moonfire"))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Casting Moonfire");
                Console.ResetColor();

                if (Api.Spellbook.Cast("Moonfire"))
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
        var target = Api.Target;
        var targetHealth = Api.Target.HealthPercent;
        var targetDistance = target.Position.Distance2D(me.Position);
        var balancePower = me.GetUnitPower("BALANCE");

        if (!target.IsValid() || me.IsDead() || me.IsGhost() || me.IsCasting() || me.IsMoving() || me.IsChanneling() || me.IsMounted() || me.Auras.Contains("Drink") || me.Auras.Contains("Food")) return false;
        CreatureType targetCreatureType = GetCreatureType(target);

        if ((DateTime.Now - lastDebugTime).TotalSeconds >= debugInterval)
        {
            LogPlayerStats();
            lastDebugTime = DateTime.Now; // Update lastDebugTime
        }
        if (Api.Spellbook.CanCast("Solar Beam") && (target.IsCasting() || target.IsChanneling()) && mana > 18)
        {
            if (Api.Spellbook.CanCast("Solar Beam"))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Casting Solar Beam");
                Console.ResetColor();

                if (Api.Spellbook.Cast("Solar Beam"))
                {
                    return true;
                }
            }
        }
        if (Api.Spellbook.CanCast("Moonkin Form") && mana > 13 && !Api.Player.Auras.Contains("Moonkin Form", false) && !Api.Player.IsMounted())
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Moonkin Form");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Moonkin Form"))
            {
                return true;
            }
        }
        if (Api.Spellbook.CanCast("Rejuvenation") && healthPercentage <= 70 && !me.Auras.Contains("Rejuvenation") && mana > 20)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Rejuvenation");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Rejuvenation"))
                return true;
        }
        if (Api.Spellbook.CanCast("Regrowth") && healthPercentage <= 45 && !me.Auras.Contains("Regrowth") && mana > 35)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Regrowth");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Regrowth"))
                return true;
        }
        if (healthPercentage <= 35 && Api.Spellbook.CanCast("Healing Touch") && mana > 30)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Healing Touch");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Healing Touch"))
                return true;
        }

        // Mana regeneration
        if (!me.Auras.Contains("Innervate") && mana <= 30 && Api.Spellbook.CanCast("Innervate") && !Api.Spellbook.OnCooldown("Innervate"))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Innervate");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Innervate"))
                return true;
        }

        // DPS rotation
        if (Api.Spellbook.CanCast("Force of Nature") && mana > 12 && Api.HasMacro("Treant") && targetDistance <= 15 && !Api.Spellbook.OnCooldown("Force of Nature") && Api.UnfriendlyUnitsNearby(10, true) >= 2)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Force of Nature");
            Console.ResetColor();

            if (Api.UseMacro("Treant"))
                return true;
        }
        if (Api.Spellbook.CanCast("Starfall") && targetDistance <= 20 && Api.UnfriendlyUnitsNearby(10, true) >= 2 && !Api.Spellbook.OnCooldown("Starfall") && mana > 35)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Starfall");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Starfall"))
                return true;
        }
        if (!target.Auras.Contains("Insect Swarm") && mana > 8 && Api.Spellbook.CanCast("Insect Swarm") && targetCreatureType != CreatureType.Totem && (DateTime.Now - Swarm) >= lastInsectSwarmCast )
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Insect Swarm");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Insect Swarm"))
                Swarm = DateTime.Now; // Update the last cast time

            return true;
        }

        if (Api.Spellbook.CanCast("Starsurge") && me.Auras.Contains(93400))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Starsurge instant");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Starsurge"))
                return true;
        }
        if (Api.Spellbook.CanCast("Moonfire") && mana > 9 && (!target.Auras.Contains("Moonfire") || !target.Auras.Contains("Sunfire")) && (DateTime.Now - Sunfire) >= lastSunfire )
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Moonfire for filler");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Moonfire"))
                Sunfire = DateTime.Now; // Update the last cast time

            return true;
        }

        if (balancePower == -100)
        {
            if (Api.Spellbook.CanCast("Starfire") && mana > 11)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Casting Starfire");
                Console.ResetColor();

                if (Api.Spellbook.Cast("Starfire"))
                    return true;
            }

            if (Api.Spellbook.CanCast("Wrath") && mana > 9)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Casting Wrath");
                Console.ResetColor();

                if (Api.Spellbook.Cast("Wrath"))
                    return true;
            }
        }
        if (balancePower == 100)
        {
            if (Api.Spellbook.CanCast("Moonfire") && mana > 9 && (!target.Auras.Contains("Moonfire") || !target.Auras.Contains("Sunfire")) && (DateTime.Now - Sunfire) >= lastSunfire)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Casting Sunfire for Solar eclipse");
                Console.ResetColor();

                if (Api.Spellbook.Cast("Moonfire"))
                    Sunfire = DateTime.Now; // Update the last cast time

                return true;
            }
            if (Api.Spellbook.CanCast("Wrath") && mana > 9)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Casting Wrath");
                Console.ResetColor();

                if (Api.Spellbook.Cast("Wrath"))
                    return true;
            }
        }
        if (balancePower > -100 && balancePower <= 0)
        {
            if (Api.Spellbook.CanCast("Moonfire") && !target.Auras.Contains("Moonfire") && mana > 9)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Casting Moonfire for balancePower > -100 && balancePower <0");
                Console.ResetColor();

                if (Api.Spellbook.Cast("Moonfire"))
                    return true;
            }

            if (Api.Spellbook.CanCast("Starsurge") && !Api.Spellbook.OnCooldown("Starsurge") && mana > 11)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Casting Starsurge");
                Console.ResetColor();

                if (Api.Spellbook.Cast("Starsurge"))
                    return true;
            }
            if (Api.Spellbook.CanCast("Wrath") && mana > 9)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Casting Wrath");
                Console.ResetColor();

                if (Api.Spellbook.Cast("Wrath"))
                    return true;
            }
        }


        if (balancePower > 1 && balancePower < 100)
        {
            if (Api.Spellbook.CanCast("Moonfire") && mana > 8 && !target.Auras.Contains("Moonfire") && !target.Auras.Contains("Sunfire"))

            // Your code here

            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Casting Moonfire balancePower >1 && balancePower < 100");
                Console.ResetColor();

                if (Api.Spellbook.Cast("Moonfire"))
                    return true;
            }

            if (Api.Spellbook.CanCast("Starsurge") && !Api.Spellbook.OnCooldown("Starsurge") && mana > 11)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Casting Starsurge");
                Console.ResetColor();

                if (Api.Spellbook.Cast("Starsurge"))
                    return true;
            }
            if (Api.Spellbook.CanCast("Wrath") && mana > 9)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Casting Wrath");
                Console.ResetColor();

                if (Api.Spellbook.Cast("Wrath"))
                    return true;
            }
        }
        if (Api.Spellbook.CanCast("Wrath") && mana > 9)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Wrath");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Wrath"))
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
        var target = Api.Target;
        // Assuming u is a WowUnit or WowPlayer object
        var balancePower = me.GetUnitPower("BALANCE");

        var mana = me.ManaPercent;
        var healthPercentage = me.HealthPercent;


        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"{mana}% Mana available");
        Console.WriteLine($"Balance Power (cata) is {balancePower}");

        foreach (var aura in target.Auras.ActiveAuras)
        {
            Console.WriteLine($"Active Aura Name: {aura.Name}");
        }
        Console.ResetColor();

    }





}