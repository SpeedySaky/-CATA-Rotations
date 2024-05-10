using System;
using System.Threading;
using wShadow.Templates;
using System.Linq;
using wShadow;
using System.Collections.Generic;
using wShadow.Warcraft.Classes;
using wShadow.Warcraft.Defines;
using wShadow.Warcraft.Managers;




public class RestoShamanWOTLK : Rotation
{
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
        if (unit == null || unit.Address == null || unit.Name != "Searing Totem")
        {
            return false;
        }
        return true;
    }

    private bool HasItem(object item)
        => Api.Inventory.HasItem(item);

    private int debugInterval = 30; // Set the debug interval in seconds
    private DateTime lastDebugTime = DateTime.MinValue;
    private bool HasEnchantment(EquipmentSlot slot, string enchantmentName)
    {
        return Api.Equipment.HasEnchantment(slot, enchantmentName);
    }
    private TimeSpan Searing = TimeSpan.FromSeconds(20);
    private DateTime LastSearing = DateTime.MinValue;



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
        var targetDistance = target.Position.Distance2D(me.Position);


        if ((DateTime.Now - lastDebugTime).TotalSeconds >= debugInterval)
        {
            LogPlayerStats();
            lastDebugTime = DateTime.Now; // Update lastDebugTime
        }

        if (me.IsDead() || me.IsGhost() || me.IsCasting() || me.IsMoving() || me.IsChanneling() || me.IsMounted() || me.Auras.Contains("Drink") || me.Auras.Contains("Food")) return false;
        bool hasOffhandEnchantment = HasEnchantment(EquipmentSlot.OffHand, "Flametongue");
        bool hasMainHandEnchantment = HasEnchantment(EquipmentSlot.MainHand, "Windfury");


        //off hand

        if (Api.Spellbook.CanCast("Windfury Weapon") && !hasMainHandEnchantment)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Windfury Weapon");
            Console.ResetColor();
            if (Api.Spellbook.Cast("Windfury Weapon"))
            {
                return true;
            }
        }
        if (Api.Spellbook.CanCast("Flametongue Weapon") && !hasOffhandEnchantment)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Flametongue Weapon");
            Console.ResetColor();
            if (Api.Spellbook.Cast("Flametongue Weapon"))
            {
                return true;
            }
        }


        if (Api.Spellbook.CanCast("Lightning Shield") && !me.Auras.Contains("Lightning Shield") && mana > 30)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Lighting Shield");
            Console.ResetColor();
            if (Api.Spellbook.Cast("Lightning Shield"))
            {
                return true;
            }
        }

        if (Api.Spellbook.CanCast("Totemic Recall") && (me.Auras.Contains("Stoneskin", false) || me.Auras.Contains("Windfury Totem", false)))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Totemic Recall");
            Console.ResetColor();
            if (Api.Spellbook.Cast("Totemic Recall"))
            {
                return true;
            }
        }

        return base.PassivePulse();

    }

    public override bool CombatPulse()
    {
        var me = Api.Player;
        var mana = me.ManaPercent;
        var healthPercentage = me.HealthPercent;

        if (me.IsDead() || me.IsGhost() || me.IsCasting() || me.IsMoving() || me.IsChanneling() || me.IsMounted() || me.Auras.Contains("Drink") || me.Auras.Contains("Food")) return false;

        // ... existing code ...

        // Get the party members
        WowUnit[] partyMembers = wShadow.C_Party.GetMembers();

        // Iterate over each party member
        for (int i = 0; i < partyMembers.Length; i++)
        {
            var member = partyMembers[i];

            // Check if the party member's health is below a certain threshold
            if (member.HealthPercent < 50)
            {
                // If so, cast a healing spell on them
                if (Api.Spellbook.CanCast("Healing Wave"))
                {
                    // Target the party member
                    member.TryTarget();

                    // Cast the spell
                    if (Api.Spellbook.Cast("HealingWave"))
                    {
                        return true;
                    }
                }
            }
        }

        // ... existing code ...

        return base.CombatPulse();
    }


    private void LogPlayerStats()
    {
        // Variables for player and target instances
        var me = Api.Player;
        var target = Api.Target;
        var mana = me.ManaPercent;
        var healthPercentage = me.HealthPercent;


        // Target distance from the player


        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"{mana}% Mana available");
        Console.WriteLine($"{healthPercentage}% Health available");

        Console.ResetColor();

        if (me.Auras.Contains("Stoneskin"))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Have Stoneskin");
            Console.ResetColor();

        }
        if (me.Auras.Contains("Stoneskin"))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Have Stoneskin perma");
            Console.ResetColor();

        }


        var searingTotem = Api.Units.FirstOrDefault(unit => unit.Name == "Searing Totem");

        if (searingTotem != null && searingTotem.IsValid())
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("We have Searing Totem");
            Console.ResetColor();
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("We don't have Searing Totem");
            Console.ResetColor();
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