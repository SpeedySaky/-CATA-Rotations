using System;
using System.Threading;
using wShadow.Templates;
using System.Collections.Generic;
using wShadow.Warcraft.Classes;
using wShadow.Warcraft.Defines;
using wShadow.Warcraft.Managers;
using wShadow.Warcraft.Usefuls;

public class UnholyDK : Rotation
{
    private DateTime lastLogTime = DateTime.MinValue;
    private int logInterval = 5; // Set the log interval in seconds
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
        SlowTick = 800;
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
        if (me.IsDead() || me.IsGhost() || me.IsCasting() || me.IsMoving() || me.IsChanneling() || me.IsMounted() || me.Auras.Contains("Drink") || me.Auras.Contains("Food")) return false;
        if (IsValid(pet))
        {
            PetHealth = pet.HealthPercent;
        }
        LogPlayerStatsPeriodically();

        if (!IsValid(pet)  && Api.Spellbook.CanCast("Raise Dead"))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Raise Dead because we dont have pet.");
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

        if (Api.Spellbook.CanCast("Unholy Presence") && (me.Auras.Contains("Blood Presence") || me.Auras.Contains("Frost Presence")))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Unholy Presence");

            if (Api.Spellbook.CanCast("Unholy Presence"))
            {
                if (Api.Spellbook.Cast("Unholy Presence"))
                    return true;
            }
        }
        var reaction = me.GetReaction(target);

        if (!target.IsDead() && (reaction != UnitReaction.Friendly && reaction != UnitReaction.Honored && reaction != UnitReaction.Revered && reaction != UnitReaction.Exalted)  && !IsNPC(target))
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
        var pet = me.Pet();
        var target = Api.Target;
        var targetDistance = target.Position.Distance2D(me.Position);
        var health = me.HealthPercent;
        var runicPower = me.GetUnitPower("RUNIC_POWER") / 10f;
        var bloodRunes = Api.BloodRunesReady();
        var unholyRunes = Api.UnholyRunesReady();
        var frostRunes = Api.FrostRunesReady();
        var deathRunes = Api.DeathRunesReady();
        var PetHealth = 0.0f;
        if (me.IsDead() || me.IsGhost() || me.IsCasting() || me.IsMoving() || me.IsChanneling() || me.IsMounted() || me.Auras.Contains("Drink") || me.Auras.Contains("Food")) return false;

        if (IsValid(pet))
        {
            PetHealth = pet.HealthPercent;
    
        }

        if (!IsValid(pet) && Api.Spellbook.CanCast("Raise Dead"))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Raise Dead because we dont have pet.");
            // Console.ResetColor();

            if (Api.Spellbook.Cast("Raise Dead"))
                return true;
        }



        if (Api.Spellbook.CanCast("Death Grip") && targetDistance > 5 && targetDistance < 30 && !Api.Spellbook.OnCooldown("Death Grip"))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Death Grip");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Death Grip"))
                return true;
        }

        if (Api.Spellbook.CanCast("Horn of Winter") && !me.Auras.Contains("Horn of Winter"))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Horn of Winter");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Horn of Winter"))
                return true;


            Console.ResetColor();
        }

        if ((Api.Spellbook.CanCast("Strangulate") || Api.Spellbook.CanCast("Mind Freeze")) && (target.IsCasting() || target.IsChanneling()))
        {
            if (Api.Spellbook.CanCast("Strangulate"))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Casting Strangulate");
                Console.ResetColor();

                if (Api.Spellbook.Cast("Strangulate"))
                {
                    return true;
                }
            }
            else if (Api.Spellbook.CanCast("Mind Freeze"))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Casting Mind Freeze");
                Console.ResetColor();

                if (Api.Spellbook.Cast("Mind Freeze"))
                {
                    return true;
                }
            }
        }

        if (Api.Spellbook.CanCast("Death Pact")  && pet.IsValid() && health <= 10 && !Api.Spellbook.OnCooldown("Death Pact"))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Death Pact");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Death Pact"))
                return true;
        }
        if (Api.Spellbook.CanCast("Icebound Fortitude") && health <= 30 && runicPower >= 20 && !Api.Spellbook.OnCooldown("Icebound Fortitude"))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Casting Icebound Fortitude ");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Icebound Fortitude"))
                return true;
        }


        if (Api.UnfriendlyUnitsNearby(10, true) >= 2 && Api.Spellbook.CanCast("Pestilence") && (bloodRunes >= 1 || deathRunes >= 1) && target.Auras.Contains("Frost Fever") && target.Auras.Contains("Blood Plague"))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Casting Pestilence ");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Pestilence"))
                return true;
        }

        if (Api.Spellbook.CanCast("Death Strike") && health <= 60 && unholyRunes >= 1 && frostRunes>=1 )
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Casting Death Strike ");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Death Strike"))
                return true;
        }
        if (Api.Spellbook.CanCast("Army of the Dead") && !me.IsCasting() && !Api.Spellbook.OnCooldown("Army of the Dead") && Api.UnfriendlyUnitsNearby(10, true) >= 2 && bloodRunes >= 1 && frostRunes>=1 && unholyRunes>=1)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Casting Army of the Dead");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Army of the Dead"))
                return true;
        }

        if (Api.UnfriendlyUnitsNearby(10, true) >= 2 && Api.Spellbook.CanCast("Blood Boil") && (bloodRunes >= 1 || deathRunes >= 1))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Casting Blood Boil");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Blood Boil"))
                return true;
        }

        if (Api.Spellbook.CanCast("Summon Gargoyle") && runicPower > 60 && !me.IsCasting() && !Api.Spellbook.OnCooldown("Summon Gargoyle") && Api.UnfriendlyUnitsNearby(10, true) >= 2)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Summon Gargoyle");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Summon Gargoyle"))
                return true;
        }
        if (Api.UnfriendlyUnitsNearby(10, true) >= 2 && Api.Spellbook.CanCast("Bone Shield") && !me.Auras.Contains("Bone Shield") && !Api.Spellbook.OnCooldown("Bone Shield") && (unholyRunes >= 1 || deathRunes >= 1) && runicPower >= 10)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Casting Bone Shield ");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Bone Shield"))
                return true;
        }
        if (Api.Spellbook.CanCast("Blood Tap") && bloodRunes > 1 && (unholyRunes < 1 || frostRunes < 1))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Casting Blood Tap  to get Death rune");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Blood Tap"))
                return true;
        }
        
        if (Api.Spellbook.CanCast("Ghoul Frenzy") && !Api.Spellbook.OnCooldown("Ghoul Frenzy"))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Ghoul Frenzy");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Ghoul Frenzy"))
                return true;
        }
        if (Api.Spellbook.CanCast("Empower Rune Weapon") && !Api.Player.IsCasting() && !Api.Spellbook.OnCooldown("Empower Rune Weapon")  && runicPower >= 25)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Casting Empower Rune Weapon ");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Empower Rune Weapon"))
                return true;
        }
        if ((Api.Spellbook.CanCast("Obliterate") && target.Auras.Contains("Frost Fever") && target.Auras.Contains("Blood Plague") && unholyRunes >= 1 && frostRunes >= 1 && runicPower >= 15) || (Api.Spellbook.CanCast("Obliterate") && target.Auras.Contains("Frost Fever") && target.Auras.Contains("Blood Plague") && deathRunes >= 1 && frostRunes >= 1 ) || (Api.Spellbook.CanCast("Obliterate") && target.Auras.Contains("Frost Fever") && target.Auras.Contains("Blood Plague") && deathRunes >= 1 && unholyRunes >= 1 ))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Casting Obliterate ");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Obliterate"))
            {
                return true;
            }
        }
        if (Api.Spellbook.CanCast("Dark Transformation") && unholyRunes >= 1 && pet.Auras.Contains("Shadow Infusion") && pet.Auras.GetStacks("Shadow Infusion")==5)
        {
            
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Casting Dark Transformation");
                Console.ResetColor();

                if (Api.Spellbook.Cast("Dark Transformation"))
                {
                    return true;
                }
            
        }
    




        if (Api.Spellbook.CanCast("Death Coil") && runicPower > 35)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Casting Death Coil with {runicPower} Runic Power");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Death Coil"))
            {
                return true;
            }
        }
        else if (Api.Spellbook.CanCast("Death Coil") && me.Auras.Contains(81340))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Casting Death Coil with Sudden Doom");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Death Coil"))
            {
                return true;
            }
        }

        if (Api.Spellbook.CanCast("Icy Touch") && !target.Auras.Contains("Frost Fever") && (frostRunes >= 1 || deathRunes >= 1) )
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Casting Icy Touch  ");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Icy Touch"))
            {
                return true;
            }
        }

        if ((Api.Spellbook.CanCast("Plague Strike") && (!target.Auras.Contains("Blood Plague")|| !target.Auras.Contains("Ebon Plague")) && (unholyRunes >= 1)))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Casting Plague Strike ");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Plague Strike"))
            {
                return true;
            }
        }

        if ((Api.Spellbook.CanCast("Scourge Strike") && !target.Auras.Contains("Blood Plague") && (unholyRunes >= 1 || deathRunes >= 1)) || (Api.Spellbook.CanCast("Scourge Strike") && !target.Auras.Contains("Blood Plague") && (frostRunes >= 1 || deathRunes >= 1)))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Casting Scourge Strike ");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Scourge Strike"))
            {
                return true;
            }
        }

        if (Api.Spellbook.CanCast("Blood Strike") && (deathRunes >= 1 || bloodRunes >= 1) )
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Casting Blood Strike ");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Blood Strike"))
            {
                return true;
            }
        }

        if (Api.Spellbook.CanCast("Death Strike") && unholyRunes >= 1 && frostRunes >= 1)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Casting Death Strike  ");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Death Strike"))
            {
                return true;
            }
        }

        if (Api.Spellbook.CanCast("Icy Touch") && (frostRunes >= 1 || deathRunes >= 1))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Casting Icy Touch  ");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Icy Touch"))
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
    private void LogPlayerStatsPeriodically()
    {
        var now = DateTime.Now;
        var timeSinceLastLog = now - lastLogTime;

        if (timeSinceLastLog.TotalSeconds >= logInterval)
        {
            lastLogTime = now;

            var me = Api.Player;
            var pet = me.Pet();

            if (pet != null && pet.IsValid())
            {
                if (!pet.IsDeadOrGhost())
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Pet is alive.");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Pet is dead.");
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("No pet found.");
            }

            var bloodRunes = Api.BloodRunesReady();
            var unholyRunes = Api.UnholyRunesReady();
            var frostRunes = Api.FrostRunesReady();
            var deathRunes = Api.DeathRunesReady();

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"{bloodRunes} Blood runes available");
            Console.WriteLine($"{unholyRunes} Unholy runes available");
            Console.WriteLine($"{frostRunes} Frost runes available");
            Console.WriteLine($"{deathRunes} Death runes available");
            Console.ResetColor();

            if (me.Auras.Contains("Glyph of Raise Dead")) // Replace "Thorns" with the actual aura name
            {
                Console.ForegroundColor = ConsoleColor.Blue;

                Console.WriteLine($"Glyph of Raise Dead");
            }
            if (me.Auras.Contains("Unholy Presence") || me.Auras.Contains("Improved Unholy Presence"))
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                if (me.Auras.Contains(48265))
                    Console.WriteLine("Unholy Presence");
                else if (me.Auras.Contains(50392))
                    Console.WriteLine("Improved Unholy Presence");

                Console.ResetColor();
            }
            if (me.Auras.Contains("Blood Presence"))
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("Blood Presence");
                Console.ResetColor();
            }
            else if (me.Auras.Contains("Frost Presence"))
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("Frost Presence");
                Console.ResetColor();
            }
            else if (me.Auras.Contains("Unholy Presence"))
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("Unholy Presence");
                Console.ResetColor();
            }

        }
    }
}
