using System;
using System.Threading;
using wShadow.Templates;
using System.Collections.Generic;
using wShadow.Warcraft.Classes;
using wShadow.Warcraft.Defines;
using wShadow.Warcraft.Managers;




public class AfflictionLock : Rotation
{
    private List<string> npcConditions = new List<string>
    {
        "Innkeeper", "Auctioneer", "Banker", "FlightMaster", "GuildBanker",
        "PlayerVehicle", "StableMaster", "Repair", "Trainer", "TrainerClass",
        "TrainerProfession", "Vendor", "VendorAmmo", "VendorFood", "VendorPoison",
        "VendorReagent", "WildBattlePet", "GarrisonMissionNPC", "GarrisonTalentNPC",
        "QuestGiver"
    };
    private int debugInterval = 5; // Set the debug interval in seconds
    private DateTime lastDebugTime = DateTime.MinValue;
    public bool IsValid(WowUnit unit)
    {
        if (unit == null || unit.Address == null)
        {
            return false;
        }
        return true;
    }
    private bool HasItem(object item) => Api.Inventory.HasItem(item);
    public override void Initialize()
    {
        //targets 
        npcConditions.Add("Innkeeper");
        npcConditions.Add("Auctioneer");
        npcConditions.Add("Banker");
        npcConditions.Add("FlightMaster");
        npcConditions.Add("GuildBanker");
        npcConditions.Add("PlayerVehicle");
        npcConditions.Add("StableMaster");
        npcConditions.Add("Repair");
        npcConditions.Add("Trainer");
        npcConditions.Add("TrainerClass");
        npcConditions.Add("TrainerProfession");
        npcConditions.Add("Vendor");
        npcConditions.Add("VendorAmmo");
        npcConditions.Add("VendorFood");
        npcConditions.Add("VendorPoison");
        npcConditions.Add("VendorReagent");
        npcConditions.Add("WildBattlePet");
        npcConditions.Add("GarrisonMissionNPC");
        npcConditions.Add("GarrisonTalentNPC");
        npcConditions.Add("QuestGiver");
        // Can set min/max levels required for this rotation.

        lastDebugTime = DateTime.Now;
        LogPlayerStats();
        // Use this method to set your tick speeds.
        // The simplest calculation for optimal ticks (to avoid key spam and false attempts)

        // Assuming wShadow is an instance of some class containing UnitRatings property
        SlowTick = 1200;
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
        // Variables for player and target instances
        var me = Api.Player;
        var target = Api.Target;
        var mana = me.ManaPercent;
        var healthPercentage = me.HealthPercent;
        var targethealth = target.HealthPercent;
        var pet = me.Pet();
        var PetHealth = 0.0f;
        if (IsValid(pet))
        {
            PetHealth = pet.HealthPercent;
        }
        var TargetHealth = 0.0f;
        if (IsValid(target))
        {
            TargetHealth = target.HealthPercent;
        }

        if ((DateTime.Now - lastDebugTime).TotalSeconds >= debugInterval)
        {
            LogPlayerStats();
            lastDebugTime = DateTime.Now; // Update lastDebugTime
        }
        // Health percentage of the player

        // Power percentages for different resources

        // Target distance from the player

        if (me.IsDead() || me.IsGhost() || me.IsCasting() || me.IsMoving() || me.IsChanneling() || me.IsMounted() || me.Auras.Contains("Drink") || me.Auras.Contains("Food")) return false;



        if (!HasItem("Healthstone") && Api.Spellbook.CanCast("Create Healthstone"))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Create Healthstone");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Create Healthstone"))
            {
                return false;
            }
        }


        if (Api.Spellbook.CanCast("Life Tap") && healthPercentage > 80 && mana < 30)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Life Tap");
            Console.ResetColor();
            if (Api.Spellbook.Cast("Life Tap"))
            {
                return true;
            }
        }

        if (Api.Spellbook.CanCast("Fel Armor") && !me.Auras.Contains("Fel Armor", false))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Fel Armor");
            Console.ResetColor();
            if (Api.Spellbook.Cast("Fel Armor"))
            {
                return true;
            }
        }
        else if (Api.Spellbook.CanCast("Demon Armor") && !me.Auras.Contains("Fel Armor", false) && !me.Auras.Contains("Demon Armor", false))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Demon Armor");
            Console.ResetColor();
            if (Api.Spellbook.Cast("Demon Armor"))
            {
                return true;
            }
        }
        else if (Api.Spellbook.CanCast("Demon Skin") && !me.Auras.Contains("Fel Armor", false) && !me.Auras.Contains("Demon Armor", false) && !me.Auras.Contains("Demon Skin", false))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Demon Skin");
            Console.ResetColor();
            if (Api.Spellbook.Cast("Demon Skin"))
            {
                return true;
            }
        }


        if (IsValid(pet) && PetHealth < 50 && healthPercentage > 50 && Api.Spellbook.CanCast("Health Funnel"))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Healing Pet ");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Health Funnel"))
                return true;
        }

        if (!IsValid(pet))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Summon VoidWalker.");
            Console.ResetColor();

            if (Api.UseMacro("Imp"))
            {
                return true;
            }
        }
        //if (!IsValid(pet) && Api.Spellbook.CanCast(688))
        //{
        //  Console.ForegroundColor = ConsoleColor.Green;
        // Console.WriteLine("Casting Summon Imp.");
        // Console.ResetColor();

        //if (Api.Spellbook.Cast(688))
        //{
        //   return true;
        //}
        //}
        //if (!IsValid(pet))
        //{
        //  Console.ForegroundColor = ConsoleColor.Green;
        //  Console.WriteLine("Casting Summon Imp.");
        // Console.ResetColor();

        //if (Api.UseMacro("Imp"))
        //{
        //  return true;
        //}
        //}

        if (Api.Spellbook.CanCast("Soul Link") && !me.Auras.Contains("Soul Link", false) && IsValid(pet))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Soul Link");
            Console.ResetColor();
            if (Api.Spellbook.Cast("Soul Link"))
            {
                return true;
            }
        }
        if (Api.HasMacro("Combat") && !target.IsDead() && !IsNPC(target))

        //macro needed
        //Macro name : Combat
        //Macro code:
        // /cast Curse of Agony
        // /petattack	
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Starting Combat");
            Console.ResetColor();

            if (Api.UseMacro("Combat"))
            {
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
        var targethealth = target.HealthPercent;
        var healthPercentage = me.HealthPercent;
        var targetDistance = target.Position.Distance2D(me.Position);


        if (me.IsDead() || me.IsGhost() || me.IsCasting() || me.IsMoving() || me.IsChanneling() || me.IsMounted() || me.Auras.Contains("Drink") || me.Auras.Contains("Food")) return false;

        if ((DateTime.Now - lastDebugTime).TotalSeconds >= debugInterval)
        {
            LogPlayerStats();
            lastDebugTime = DateTime.Now; // Update lastDebugTime
        }
        // Health percentage of the player


        var pet = me.Pet();
        var PetHealth = 0.0f;
        if (IsValid(pet))
        {
            PetHealth = pet.HealthPercent;
        }
        var meTarget = me.Target;
        if (!IsValid(pet))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Summon Imp.");
            Console.ResetColor();

            if (Api.Spellbook.Cast(688))
            {
                return true;
            }
        }
        if (!IsValid(pet) && mana>50)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Summon Imp.");
            Console.ResetColor();

            if (Api.UseMacro("Imp"))
            {
                return true;
            }
        }
        if (meTarget == null || target.IsDead())
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Assist Pet");
            Console.ResetColor();

            // Use the Target property to set the player's target to the pet's target
            if (Api.UseMacro("AssistPet"))
            {
                // Successfully assisted the pet, continue rotation
                // Don't return true here, continue with the rest of the combat logic
                // without triggering a premature exit
            }
        }
        if (Api.Spellbook.CanCast("Drain Soul") && Api.Inventory.ItemCount("Soul Shard") <= 1 && targethealth <= 20)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Drain Soul");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Drain Soul"))
            {
                return true;
            }
        }


        {
            if (Api.Inventory.HasItem("Healthstone") && healthPercentage <= 40 && !Api.Inventory.OnCooldown("Healthstone"))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Using Healthstone");
                Console.ResetColor();

                if (Api.Inventory.Use("Healthstone"))
                {
                    return true;
                }
            }
        }
        if (meTarget == null || target.IsDead())
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Assist Pet");
            Console.ResetColor();

            // Use the Target property to set the player's target to the pet's target
            if (Api.UseMacro("AssistPet"))
            {
                // Successfully assisted the pet, continue rotation
                // Don't return true here, continue with the rest of the combat logic
                // without triggering a premature exit
            }
        }
        if (PetHealth < 50 && healthPercentage > 50 && Api.Spellbook.CanCast("Health Funnel"))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Healing Pet ");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Health Funnel"))
                return true;
        }
        

        if (Api.Spellbook.CanCast("Drain Life") && healthPercentage <= 50 && mana >= 10)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Drain Life");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Drain Life"))
                return true;
        }
        // Affliction Warlock rotation
        if (Api.Spellbook.CanCast("Haunt") && !target.Auras.Contains("Haunt") && !Api.Spellbook.OnCooldown("Haunt") && mana>10)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Haunt");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Haunt"))
                return true;
        }

        if (Api.Spellbook.CanCast("Unstable Affliction") && !target.Auras.Contains("Unstable Affliction") && mana > 10  )
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Unstable Affliction");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Unstable Affliction"))
                return true;
        }

        if (Api.Spellbook.CanCast("Corruption") && !target.Auras.Contains("Corruption") && mana > 5)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Corruption");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Corruption"))
                return true;
        }
        if (Api.Spellbook.CanCast(980) && !target.Auras.Contains("Bane of Agony") && targethealth >= 30 && mana >= 10)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Bane of Doom");
            Console.ResetColor();

            if (Api.Spellbook.Cast(980))
                return true;
        }
        if (Api.Spellbook.CanCast("Curse of Agony") && !target.Auras.Contains("Curse of Agony") && mana > 10)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Curse of Agony");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Curse of Agony"))
                return true;
        }

        


        if (Api.Spellbook.CanCast("Shadow Bolt"))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Shadow Bolt");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Shadow Bolt"))
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
    private void LogPlayerStats()
    {
        // Variables for player and target instances
        var me = Api.Player;
        var target = Api.Target;
        var mana = me.Mana;

        // Health percentage of the player
        var healthPercentage = me.HealthPercent;
        // Target distance from the player
        var targetDistance = target.Position.Distance2D(me.Position);

        var pet = me.Pet();
        var PetHealth = 0.0f;
        if (IsValid(pet))
        {
            PetHealth = pet.HealthPercent;
        }
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"{mana} Mana available");
        Console.WriteLine($"{healthPercentage}% Health available");

        Console.ResetColor();

        // Get the current count of Soul Shards in the inventory
        int soulShardCount = Api.Inventory.ItemCount("Soul Shard");

        if (soulShardCount >= 0)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Current Soul Shard Count: " + soulShardCount); // Debugging line
            Console.ResetColor();


        }

        if (!IsValid(pet))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Pet is not summoned.");
            Console.ResetColor();
            // Additional actions for when the pet is dead
        }
        else
    if (IsValid(pet))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Pet is summoned.");
            Console.ResetColor();
            // Additional actions for when the pet is dead
        }
        if (Api.Spellbook.CanCast("Summon Imp"))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Can Cast Summon Imp.");
            Console.ResetColor();
            // Additional actions for when the pet is dead
        }
        else
        if (!Api.Spellbook.CanCast("Summon Imp"))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Can't Cast Summon Imp.");
            Console.ResetColor();
            // Additional actions for when the pet is dead
        }
        if (Api.Spellbook.CanCast(688))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Can Cast 688");
            Console.ResetColor();
            // Additional actions for when the pet is dead
        }
        else
        if (!Api.Spellbook.CanCast(688))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Can't Cast 688");
            Console.ResetColor();
            // Additional actions for when the pet is dead
        }
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