using System;
using System.Threading;
using wShadow.Templates;
using System.Collections.Generic;
using wShadow.Warcraft.Classes;
using wShadow.Warcraft.Defines;
using wShadow.Warcraft.Managers;
using wShadow.Warcraft.Usefuls;

public class BloodDK : Rotation
{
    private DateTime lastLogTime = DateTime.MinValue;
    private int logInterval = 10; // Set the log interval in seconds

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
        SlowTick = 1200;
        FastTick = 400;

        PassiveActions.Add((true, () => false));
        CombatActions.Add((true, () => false));

        lastLogTime = DateTime.Now;
    }

    public override bool PassivePulse()
    {
        var me = Api.Player;
        var pet = me.Pet();
        var target = Api.Target;
        var PetHealth = 0.0f;

        var targetDistance = target.Position.Distance2D(me.Position);
        if (!target.IsValid() || me.IsDead() || me.IsGhost() || me.IsCasting() || me.IsMoving() || me.IsChanneling() || me.IsMounted() || me.Auras.Contains("Drink") || me.Auras.Contains("Food")) return false;
        if (IsValid(pet))
        {
            PetHealth = pet.HealthPercent;
        }
        LogPlayerStatsPeriodically();

        if (!IsValid(pet) && me.Auras.Contains("Glyph of Raise Dead", false) && Api.Spellbook.CanCast("Raise Dead"))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Raise Dead because we have the glyph.");
            // Console.ResetColor();

            if (Api.Spellbook.Cast("Raise Dead"))
                return true;
        }
        else if (HasItem("Corpse Dust") && !IsValid(pet) && Api.Spellbook.CanCast("Raise Dead"))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Raise Dead with Corpse Dust as we don't have the glyph.");
            // Console.ResetColor();

            if (Api.Spellbook.Cast("Raise Dead"))
                return true;
        }
        if (Api.Spellbook.CanCast("Horn of Winter") && !me.Auras.Contains("Horn of Winter"))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Horn of Winter");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Horn of Winter"))
                return true;
        }

        if (Api.Spellbook.CanCast("Blood Presence") && !me.Auras.Contains("Blood Presence", false))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Blood Presence");

            if (Api.Spellbook.CanCast("Blood Presence"))
            {
                if (Api.Spellbook.Cast("Blood Presence"))
                    return true;
            }
        }
        var reaction = me.GetReaction(target);

        if (!target.IsDead() && (reaction != UnitReaction.Friendly && reaction != UnitReaction.Honored && reaction != UnitReaction.Revered && reaction != UnitReaction.Exalted) && !IsNPC(target))
        {
            if (Api.Spellbook.CanCast("Death Grip") && targetDistance > 5 && targetDistance < 30 && !Api.Spellbook.OnCooldown("Death Grip"))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Casting Death Grip");
                Console.ResetColor();

                if (Api.Spellbook.Cast("Death Grip"))
                    return true;
            }
        }

        return base.PassivePulse();
    }

    public override bool CombatPulse()
    {
        var me = Api.Player;
        var target = Api.Target;
        var health = me.HealthPercent;
        var runicPower = me.GetUnitPower("RUNIC_POWER") / 10f;
        var bloodRunes = Api.BloodRunesReady();
        var frostRunes = Api.FrostRunesReady();
        var unholyRunes = Api.UnholyRunesReady();
        var deathRunes = Api.DeathRunesReady();
        var now = DateTime.Now;
        var timeSinceLastLog = now - lastLogTime;

        if (timeSinceLastLog.TotalSeconds >= logInterval)
        {
            LogPlayerStatsPeriodically();
        }
        if (me.IsDead() || me.IsGhost() || me.IsCasting() || me.IsMoving() || me.IsChanneling() || me.IsMounted() || me.Auras.Contains("Drink") || me.Auras.Contains("Food") || !target.IsValid()) return false;

        // Determine if we're in an AoE scenario
        bool isAoE = Api.UnfriendlyUnitsNearby(10, true) >= 3;

        // Use cooldowns
        if (Api.Spellbook.CanCast("Empower Rune Weapon") && bloodRunes == 0 && frostRunes == 0 && unholyRunes == 0 && !Api.Spellbook.OnCooldown("Empower Rune Weapon"))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Empower Rune Weapon");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Empower Rune Weapon"))
                return true;
        }

        // Use Rune Tap if you're below 30% health
        if (Api.Spellbook.CanCast("Rune Tap") && health <= 30)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Rune Tap");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Rune Tap"))
                return true;
        }

        // Use Vampiric Blood when you're below 50% health
        if (Api.Spellbook.CanCast("Vampiric Blood") && health <= 50)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Vampiric Blood");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Vampiric Blood"))
                return true;
        }

        // Use Icebound Fortitude when you're below 20% health
        if (Api.Spellbook.CanCast("Icebound Fortitude") && health <= 20)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Icebound Fortitude");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Icebound Fortitude"))
                return true;
        }

        // Use Dancing Rune Weapon when you're surrounded by 3 or more enemies
        if (Api.Spellbook.CanCast("Dancing Rune Weapon") && Api.UnfriendlyUnitsNearby(10, true) >= 3 && runicPower >= 60 && !Api.Spellbook.OnCooldown("Dancing Rune Weapon"))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Dancing Rune Weapon");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Dancing Rune Weapon"))
                return true;
        }
        if (Api.Spellbook.CanCast("Bone Shield") && !me.Auras.Contains("Bone Shield") && !Api.Spellbook.OnCooldown("Bone Shield"))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Bone Shield");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Bone Shield"))
                return true;
        }
        if (isAoE)
        {
            // AoE rotation
            // Use Death and Decay when fighting multiple enemies or when Crimson Scourge procs
            if (Api.HasMacro("Death and Decay") && Api.Spellbook.CanCast("Death and Decay") && unholyRunes >= 1 && !Api.Spellbook.OnCooldown("Death and Decay"))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Casting Death and Decay");
                Console.ResetColor();

                if (Api.UseMacro("Death and Decay"))
                    return true;
            }

            // Use Blood Boil to dump Blood Runes if multiple targets are present or when Crimson Scourge procs
            if (Api.Spellbook.CanCast("Blood Boil") && me.Auras.Contains("Crimson Scourge"))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Casting Blood Boil");
                Console.ResetColor();

                if (Api.Spellbook.Cast("Blood Boil"))
                    return true;
            }
            if (Api.Spellbook.CanCast("Outbreak") && !target.Auras.Contains("Blood Plague") && !target.Auras.Contains("Frost Fever"))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Casting Outbreak");
                Console.ResetColor();

                if (Api.Spellbook.Cast("Outbreak"))
                    return true;
            }
            if (Api.Spellbook.CanCast("Icy Touch") && !target.Auras.Contains("Frost Fever") && (frostRunes >= 1 || deathRunes >= 1))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Casting Icy Touch  ");
                Console.ResetColor();

                if (Api.Spellbook.Cast("Icy Touch"))
                {
                    return true;
                }
            }

            if ((Api.Spellbook.CanCast("Plague Strike") && (!target.Auras.Contains("Blood Plague") || !target.Auras.Contains("Ebon Plague")) && (unholyRunes >= 1)))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Casting Plague Strike ");
                Console.ResetColor();

                if (Api.Spellbook.Cast("Plague Strike"))
                {
                    return true;
                }
            }
            if (Api.Spellbook.CanCast("Death Strike") && !Api.Spellbook.OnCooldown("Death Strike") && frostRunes >= 1 && unholyRunes >= 1)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Casting Death Strike");
                Console.ResetColor();

                if (Api.Spellbook.Cast("Death Strike"))
                    return true;
            }

            // Use Heart Strike to dump Blood Runes
            if (Api.Spellbook.CanCast("Heart Strike") && (bloodRunes >= 1 || deathRunes >= 1))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Casting Heart Strike");
                Console.ResetColor();

                if (Api.Spellbook.Cast("Heart Strike"))
                    return true;
            }
            if (Api.Spellbook.CanCast("Blood Boil") && ((bloodRunes >= 1 || deathRunes >= 1)))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Casting Blood Boil");
                Console.ResetColor();

                if (Api.Spellbook.Cast("Blood Boil"))
                    return true;
            }
            if (Api.Spellbook.CanCast("Death Coil") && runicPower >= 40)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Casting Death Coil");
                Console.ResetColor();

                if (Api.Spellbook.Cast("Death Coil"))
                    return true;
            }
        }

        else
        {
            if (Api.Spellbook.CanCast("Outbreak") && !target.Auras.Contains("Blood Plague") && !target.Auras.Contains("Frost Fever"))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Casting Outbreak");
                Console.ResetColor();

                if (Api.Spellbook.Cast("Outbreak"))
                    return true;
            }
            if (Api.HasMacro("Death and Decay") && Api.Spellbook.CanCast("Death and Decay") && unholyRunes >= 1 && !Api.Spellbook.OnCooldown("Death and Decay"))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Casting Death and Decay");
                Console.ResetColor();

                if (Api.UseMacro("Death and Decay"))
                    return true;
            }
            if (Api.Spellbook.CanCast("Icy Touch") && !target.Auras.Contains("Frost Fever") && (frostRunes >= 1 || deathRunes >= 1))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Casting Icy Touch  ");
                Console.ResetColor();

                if (Api.Spellbook.Cast("Icy Touch"))
                {
                    return true;
                }
            }

            if ((Api.Spellbook.CanCast("Plague Strike") && (!target.Auras.Contains("Blood Plague") || !target.Auras.Contains("Ebon Plague")) && (unholyRunes >= 1)))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Casting Plague Strike ");
                Console.ResetColor();

                if (Api.Spellbook.Cast("Plague Strike"))
                {
                    return true;
                }
            }
            // Use Death Strike to heal
            if (Api.Spellbook.CanCast("Death Strike") && !Api.Spellbook.OnCooldown("Death Strike") && frostRunes >= 1 && unholyRunes >= 1)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Casting Death Strike");
                Console.ResetColor();

                if (Api.Spellbook.Cast("Death Strike"))
                    return true;
            }

            // Use Heart Strike to dump Blood Runes
            if (Api.Spellbook.CanCast("Heart Strike") && (bloodRunes >= 1 || deathRunes >= 1))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Casting Heart Strike");
                Console.ResetColor();

                if (Api.Spellbook.Cast("Heart Strike"))
                    return true;
            }
            if (Api.Spellbook.CanCast("Death Coil") && runicPower >= 40)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Casting Death Coil");
                Console.ResetColor();

                if (Api.Spellbook.Cast("Death Coil"))
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
    private void LogPlayerStatsPeriodically()
    {
        var now = DateTime.Now;
        var timeSinceLastLog = now - lastLogTime;

        if (timeSinceLastLog.TotalSeconds >= logInterval)
        {
            var runicPower = Api.Player.GetUnitPower("RUNIC_POWER") / 10f;
            var bloodRunes = Api.BloodRunesReady();
            var frostRunes = Api.FrostRunesReady();
            var unholyRunes = Api.UnholyRunesReady();
            var deathRunes = Api.DeathRunesReady();
            Console.ForegroundColor = ConsoleColor.Red;

            Console.WriteLine($"{bloodRunes} Blood runes available");
            Console.WriteLine($"{unholyRunes} Unholy runes available");
            Console.WriteLine($"{frostRunes} Frost runes available");
            Console.WriteLine($"{deathRunes} Death runes available");
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine($"{runicPower} Runic Power available");
            Console.ResetColor();

            // Update the last log time to now
            lastLogTime = now;
        }
    }

}
