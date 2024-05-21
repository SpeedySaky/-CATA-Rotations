using System;
using System.Threading;
using wShadow.Templates;
using System.Collections.Generic;
using wShadow.Warcraft.Classes;
using wShadow.Warcraft.Defines;
using wShadow.Warcraft.Managers;




public class ProtWarr : Rotation
{

    private int debugInterval = 5; // Set the debug interval in seconds
    private DateTime lastDebugTime = DateTime.MinValue;
    private DateTime lastRevengeTime = DateTime.MinValue;
    private TimeSpan RevengeCooldown = TimeSpan.FromSeconds(5.5);
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
        SlowTick = 1000;
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
        var me = Api.Player;
        var healthPercentage = me.HealthPercent;
        var rage = me.Rage;
        var target = Api.Target;
        var targetDistance = target.Position.Distance2D(me.Position);

        if (me.IsDead() || me.IsGhost() || me.IsCasting() || me.IsMoving() || me.IsChanneling() || me.IsMounted() || me.Auras.Contains("Drink") || me.Auras.Contains("Food")) return false;

        if ((DateTime.Now - lastDebugTime).TotalSeconds >= debugInterval)
        {
            LogPlayerStats();
            lastDebugTime = DateTime.Now; // Update lastDebugTime
        }
        if (Api.Spellbook.CanCast("Battle Stance") && !me.Auras.Contains("Battle Stance",false) && !me.Auras.Contains("Warbringer",false))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Battle Stance");
            Console.ResetColor();
            if (Api.Spellbook.Cast("Battle Stance"))

                return true;
        }
        else if (Api.Spellbook.CanCast("Defensive Stance") && !me.Auras.Contains("Defensive Stance",false) && me.Auras.Contains("Warbringer",false))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Defensive Stance");
            Console.ResetColor();
            if (Api.Spellbook.Cast("Defensive Stance"))

                return true;
        }
        var reaction = me.GetReaction(target);

        if (!target.IsDead() && (reaction != UnitReaction.Friendly && reaction != UnitReaction.Honored && reaction != UnitReaction.Revered && reaction != UnitReaction.Exalted) && !IsNPC(target))

            if (Api.Spellbook.CanCast("Charge") && (me.Auras.Contains("Battle Stance",false) || me.Auras.Contains("Warbringer",false)) && targetDistance <= 25)


            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Casting Charge");
                Console.ResetColor();

                if (Api.Spellbook.Cast("Charge"))
                {
                    return true;
                }
            }


        return base.PassivePulse();
    }



    public override bool CombatPulse()
    {
        var me = Api.Player;
        var healthPercentage = me.HealthPercent;
        var rage = me.Rage;
        var target = Api.Target;
        var targethealth = target.HealthPercent;
        var enemiesNearby = Api.UnfriendlyUnitsNearby(5, true);

        // Check if player is dead, a ghost, casting, moving, channeling, mounted, or has certain auras
        if (me.IsDead() || me.IsGhost() || me.IsCasting() || me.IsMoving() || me.IsChanneling() || me.IsMounted() || me.Auras.Contains("Drink") || me.Auras.Contains("Food")) return false;

        if (Api.Spellbook.CanCast("Defensive Stance") && !me.Auras.Contains("Defensive Stance", false))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Defensive Stance");
            Console.ResetColor();
            if (Api.Spellbook.Cast("Defensive Stance"))

                return true;
        }
        if ((Api.Spellbook.CanCast("Pummel") || Api.Spellbook.CanCast("Concussion Blow")) && (target.IsCasting() || target.IsChanneling()) )
        {
            if (Api.Spellbook.CanCast("Pummel") && !Api.Spellbook.OnCooldown("Pummel"))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Casting Pummel");
                Console.ResetColor();

                if (Api.Spellbook.Cast("Pummel"))
                {
                    return true;
                }
            }
            else if (Api.Spellbook.CanCast("Concussion Blow") && !Api.Spellbook.OnCooldown("Concussion Blow"))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Casting Concussion Blow");
                Console.ResetColor();

                if (Api.Spellbook.Cast("Concussion Blow"))
                {
                    return true;
                }
            }
        }
        // Use Demoralizing Shout if you need to maintain the debuff
        if (Api.Spellbook.CanCast("Demoralizing Shout") && !target.Auras.Contains("Demoralizing Shout") && rage >= 10)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Demoralizing Shout");
            Console.ResetColor();
            if (Api.Spellbook.Cast("Demoralizing Shout"))
                return true;
        }
        if (Api.Spellbook.CanCast("Battle Shout") && !me.Auras.Contains("Battle Shout"))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Battle Shout");
            Console.ResetColor();
            if (Api.Spellbook.Cast("Battle Shout"))
                return true;
        }

        // Use Commanding Shout for Rage as needed
        if (Api.Spellbook.CanCast("Commanding Shout") && !me.Auras.Contains("Commanding Shout") && rage < 60)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Commanding Shout");
            Console.ResetColor();
            if (Api.Spellbook.Cast("Commanding Shout"))
                return true;
        }
        if (Api.Spellbook.CanCast("Shield Block") && !Api.Spellbook.OnCooldown("Shield Block") && rage >= 10)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Shield Block");
            Console.ResetColor();
            if (Api.Spellbook.Cast("Shield Block"))
                return true;
        }

        // Use Shield Wall on 2-minute cooldown to reduce damage taken during high damage raid mechanics
        if (Api.Spellbook.CanCast("Shield Wall") && !Api.Spellbook.OnCooldown("Shield Wall") && rage >= 10)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Shield Wall");
            Console.ResetColor();
            if (Api.Spellbook.Cast("Shield Wall"))
                return true;
        }

        // Use Last Stand on 3-minute cooldown whenever your health drops dangerously low
        if (healthPercentage <= 30 && Api.Spellbook.CanCast("Last Stand") && !Api.Spellbook.OnCooldown("Last Stand"))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Last Stand");
            Console.ResetColor();
            if (Api.Spellbook.Cast("Last Stand"))
                return true;
        }

        // Use Enraged Regeneration on 3-minute cooldown together with Last Stand
        if (healthPercentage <= 30 && Api.Spellbook.CanCast("Enraged Regeneration") && !Api.Spellbook.OnCooldown("Enraged Regeneration") && me.Auras.Contains("Last Stand"))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Enraged Regeneration");
            Console.ResetColor();
            if (Api.Spellbook.Cast("Enraged Regeneration"))
                return true;
        }

        // Use Rallying Cry on 3-minute cooldown to help the group survive raid mechanics
        if (Api.Spellbook.CanCast("Rallying Cry") && !Api.Spellbook.OnCooldown("Rallying Cry") && rage >= 10)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Rallying Cry");
            Console.ResetColor();
            if (Api.Spellbook.Cast("Rallying Cry"))
                return true;
        }

        // Use Challenging Shout on 3-minute cooldown when enemies get loose, usually on trash
        if (Api.Spellbook.CanCast("Challenging Shout") && !Api.Spellbook.OnCooldown("Challenging Shout") && rage >= 10)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Challenging Shout");
            Console.ResetColor();
            if (Api.Spellbook.Cast("Challenging Shout"))
                return true;
        }

        // Use Inner Rage on 30-second cooldown when you have high Rage that you can't spend otherwise
        if (Api.Spellbook.CanCast("Inner Rage") && !Api.Spellbook.OnCooldown("Inner Rage") && rage >= 60)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Inner Rage");
            Console.ResetColor();
            if (Api.Spellbook.Cast("Inner Rage"))
                return true;
        }

        
        // AoE rotation
        if (enemiesNearby >= 2)
        {

            // Apply Rend to 1 target
            if (Api.Spellbook.CanCast("Rend") && !target.Auras.Contains("Rend"))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Casting Rend");
                Console.ResetColor();
                if (Api.Spellbook.Cast("Rend"))
                    return true;
            }
            if (Api.Spellbook.CanCast("Sunder Armor") && (!target.Auras.Contains("Sunder Armor") || target.Auras.GetStacks("Sunder Armor") < 3))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Casting Sunder Armor");
                Console.ResetColor();

                if (Api.Spellbook.Cast("Sunder Armor"))
                {
                    return true;
                }
            }

            // Use Thunder Clap on cooldown; spreads Rend via Blood and Thunder
            if (Api.Spellbook.CanCast("Thunder Clap") && !Api.Spellbook.OnCooldown("Thunder Clap") && rage >= 20)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Casting Thunder Clap");
                Console.ResetColor();
                if (Api.Spellbook.Cast("Thunder Clap"))
                    return true;
            }

            // Use Shockwave on cooldown
            if (Api.Spellbook.CanCast("Shockwave") && !Api.Spellbook.OnCooldown("Shockwave") && rage >= 15)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Casting Shockwave");
                Console.ResetColor();
                if (Api.Spellbook.Cast("Shockwave"))
                    return true;
            }

            // Use Revenge on cooldown
           

            // Use Shield Slam on any open globals
            if (Api.Spellbook.CanCast("Shield Slam") && !Api.Spellbook.OnCooldown("Shield Slam") && rage >= 20)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Casting Shield Slam");
                Console.ResetColor();
                if (Api.Spellbook.Cast("Shield Slam"))
                    return true;
            }

            // Use Cleave for any additional Rage
            if (Api.Spellbook.CanCast("Cleave") && rage >= 30)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Casting Cleave");
                Console.ResetColor();
                if (Api.Spellbook.Cast("Cleave"))
                    return true;
            }
            if (Api.Spellbook.CanCast("Heroic Strike") && rage >= 60)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Casting Heroic Strike");
                Console.ResetColor();
                if (Api.Spellbook.Cast("Heroic Strike"))
                    return true;
            }
            if (Api.Spellbook.CanCast("Revenge") && !Api.Spellbook.OnCooldown("Revenge") && rage >= 5)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Casting Revenge");
                Console.ResetColor();
                if (Api.Spellbook.Cast("Revenge"))
                    return true;
            }
        }
        // Single target rotation
        else
        {
            // Use Shield Slam on cooldown
            if (Api.Spellbook.CanCast("Shield Slam") && !Api.Spellbook.OnCooldown("Shield Slam") && rage >= 20 && !Api.Spellbook.OnCooldown("Shield Slam"))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Casting Shield Slam");
                Console.ResetColor();
                if (Api.Spellbook.Cast("Shield Slam"))
                    return true;
            }

            // Use Revenge on cooldown
            
            if (Api.Spellbook.CanCast("Sunder Armor") && (!target.Auras.Contains("Sunder Armor") || target.Auras.GetStacks("Sunder Armor") < 3) && rage >= 15)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Casting Sunder Armor");
                Console.ResetColor();

                if (Api.Spellbook.Cast("Sunder Armor"))
                {
                    return true;
                }
            }

            // Use Devastate as a filler during the gaps in your rotation
            if (Api.Spellbook.CanCast("Devastate") && rage >= 15)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Casting Devastate");
                Console.ResetColor();
                if (Api.Spellbook.Cast("Devastate"))
                    return true;
            }
           
            if (Api.Spellbook.CanCast("Heroic Strike") && rage >= 60)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Casting Heroic Strike");
                Console.ResetColor();
                if (Api.Spellbook.Cast("Heroic Strike"))
                    return true;
            }
            if (Api.Spellbook.CanCast("Revenge") && !Api.Spellbook.OnCooldown("Revenge") && rage >= 5)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Casting Revenge");
                Console.ResetColor();
                if (Api.Spellbook.Cast("Revenge"))
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

        var rage = me.Rage;
        var healthPercentage = me.HealthPercent;


        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"{rage}% Rage available");
        Console.WriteLine($"{healthPercentage}% Health available");


        if (me.Auras.Contains("Defensive Stance")) // Replace "Thorns" with the actual aura name
        {
            Console.ForegroundColor = ConsoleColor.Yellow;


            Console.WriteLine($"Auras.Contains  Defensive Stance");
        }

        if (me.Auras.Contains("Defensive Stance")) // Replace "Thorns" with the actual aura name
        {
            Console.ForegroundColor = ConsoleColor.Yellow;


            Console.WriteLine($"Auras.Contains  Defensive Stance");
        }
        if (me.Auras.Contains("Defensive Stance")) // Replace "Thorns" with the actual aura name
        {
            Console.ForegroundColor = ConsoleColor.Yellow;


            Console.WriteLine($"Auras.Contains  Defensive Stance");
        }

        if (me.Auras.Contains("Warbringer")) // Replace "Thorns" with the actual aura name
        {
            Console.ForegroundColor = ConsoleColor.Yellow;


            Console.WriteLine($"Auras.Contains Warbringer");
        }

        if (me.Auras.Contains("Warbringer")) // Replace "Thorns" with the actual aura name
        {
            Console.ForegroundColor = ConsoleColor.Yellow;


            Console.WriteLine($"Auras.Contains  Warbringer");
        }
        if (me.Auras.Contains("Warbringer")) // Replace "Thorns" with the actual aura name
        {
            Console.ForegroundColor = ConsoleColor.Yellow;


            Console.WriteLine($"Auras.Contains  Warbringer");
        }
    }
}