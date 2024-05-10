using System;
using System.Threading;
using wShadow.Templates;
using System.Linq;
using System.Collections.Generic;
using wShadow.Warcraft.Classes;
using wShadow.Warcraft.Defines;
using wShadow.Warcraft.Managers;




public class EnhaShamanWOTLK : Rotation
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
        SlowTick = 800;
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
        // Variables for player and target instances
        var me = Api.Player;
        var target = Api.Target;
        var mana = me.ManaPercent;
        var targethealth = target.HealthPercent;
        var healthPercentage = me.HealthPercent;

        var meTarget = me.Target;
        if ((DateTime.Now - lastDebugTime).TotalSeconds >= debugInterval)
        {
            LogPlayerStats();
            lastDebugTime = DateTime.Now; // Update lastDebugTime
        }

        var targetDistance = target.Position.Distance2D(me.Position);
        if (me.IsDead() || me.IsGhost() || me.IsCasting() || me.IsMoving() || me.IsChanneling() || me.IsMounted() || me.Auras.Contains("Drink") || me.Auras.Contains("Food")) return false;

        if (Api.Spellbook.CanCast("Strength of Earth Totem") && !me.Auras.Contains("Strength of Earth", false) && mana > 50)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Strength of Earth Totem");
            Console.ResetColor();
            if (Api.Spellbook.Cast("Strength of Earth Totem"))
            {
                return true;
            }
        }
        if (Api.Spellbook.CanCast("Earth Elemental Totem") && !Api.Spellbook.OnCooldown("Earth Elemental Totem"))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Earth Elemental Totem");
            Console.ResetColor();
            if (Api.Spellbook.Cast("Earth Elemental Totem"))
            {
                return true;
            }
        }
        if (Api.Spellbook.CanCast("Windfury Totem") && !me.Auras.Contains("Windfury Totem", false) && mana > 50)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Windfury Totem");
            Console.ResetColor();
            if (Api.Spellbook.Cast("Windfury Totem"))
            {
                return true;
            }
        }

        var searingTotem = Api.Units.FirstOrDefault(unit => unit.Name == "Searing Totem");

        if (searingTotem == null || !searingTotem.IsValid())
        {
            if (Api.Spellbook.CanCast("Searing Totem"))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Casting Searing Totem");
                Console.ResetColor();
                if (Api.Spellbook.Cast("Searing Totem"))
                {
                    return true;
                }
            }
        }

        if (Api.Spellbook.CanCast("Lightning Shield") && !me.Auras.Contains("Lightning Shield") && mana > 50)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Lighting Shield");
            Console.ResetColor();
            if (Api.Spellbook.Cast("Lightning Shield"))
            {
                return true;
            }
        }
        else
        if (Api.Spellbook.CanCast("Water Shield") && !me.Auras.Contains("Water Shield") && mana < 50)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Water Shield");
            Console.ResetColor();
            if (Api.Spellbook.Cast("Water Shield"))
            {
                return true;
            }
        }
        if(Api.Spellbook.CanCast("Healing Surge") && healthPercentage <= 50)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Healing Surge");
            Console.ResetColor();
            if (Api.Spellbook.Cast("Healing Surge"))
            {
                return true;
            }
        }
        if (Api.Spellbook.CanCast("Healing Wave") && healthPercentage <= 80 )
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Healing Wave");
            Console.ResetColor();
            if (Api.Spellbook.Cast("Healing Wave"))
            {
                return true;
            }
        }
        if (Api.Spellbook.CanCast("Healing Wave") && healthPercentage <= 50 && mana > 20)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Healing Wave");
            Console.ResetColor();
            if (Api.Spellbook.Cast("Healing Wave"))
            {
                return true;
            }
        }


        if (Api.Spellbook.CanCast("Wind Shear") && !Api.Spellbook.OnCooldown("Wind Shear") && targetDistance <= 25 && (target.IsCasting() || target.IsChanneling()))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Wind Shear");
            Console.ResetColor();
            if (Api.Spellbook.Cast("Wind Shear"))
            {
                return true;
            }
        }
        else
        if (Api.Spellbook.CanCast("Earth Shock") && !Api.Spellbook.OnCooldown("Earth Shock") && targetDistance <= 25 && (target.IsCasting() || target.IsChanneling()))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Earth Shock");
            Console.ResetColor();
            if (Api.Spellbook.Cast("Earth Shock"))
            {
                return true;
            }
        }


        if (Api.Spellbook.CanCast("Lightning Bolt") && me.Auras.Contains("Maelstrom Weapon") && me.Auras.GetStacks("Maelstrom Weapon") == 5)

        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Lightning Bolt with 5 Maelstrom Weapon");
            Console.ResetColor();
            if (Api.Spellbook.Cast("Lightning Bolt"))
            {
                return true;
            }
        }
        if (Api.Spellbook.CanCast("Lava Lash") && !Api.Spellbook.OnCooldown("Lava Lash"))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Lava Lash");
            Console.ResetColor();
            if (Api.Spellbook.Cast("Lava Lash"))
            {
                return true;
            }
        }

        if (Api.Spellbook.CanCast("Flame Shock") && !Api.Spellbook.OnCooldown("Flame Shock") && !target.Auras.Contains("Flame Shock"))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Flame Shock");
            Console.ResetColor();
            if (Api.Spellbook.Cast("Flame Shock"))
            {
                return true;
            }
        }
        if (Api.Spellbook.CanCast("Stormstrike") && !Api.Spellbook.OnCooldown("Stormstrike"))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Stormstrike");
            Console.ResetColor();
            if (Api.Spellbook.Cast("Stormstrike"))
            {
                return true;
            }
        }
        if (Api.Spellbook.CanCast("Auto Attack"))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Attack");
            Console.ResetColor();
            if (Api.Spellbook.Cast("Auto Attack"))
            {
                return true;
            }
        }


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