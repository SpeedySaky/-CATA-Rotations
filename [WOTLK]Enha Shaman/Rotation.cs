using System;
using System.Threading;
using wShadow.Templates;
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
        if (unit == null || unit.Address == null)
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
        FastTick = 300;

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

if (me.IsDead() || me.IsGhost() || me.IsCasting() || me.IsMoving() || me.IsChanneling() || me.IsLooting() || !me.IsMounted()) return false;
        if (me.Auras.Contains("Drink") || me.Auras.Contains("Food")) return false;
        bool hasFlametongueEnchantment = HasEnchantment(EquipmentSlot.MainHand, "Flametongue 1");
        bool hasFlametongueEnchantment2 = HasEnchantment(EquipmentSlot.MainHand, "Flametongue 2");
        bool hasFlametongueEnchantment3 = HasEnchantment(EquipmentSlot.MainHand, "Flametongue 3");
        bool hasFlametongueEnchantment4 = HasEnchantment(EquipmentSlot.MainHand, "Flametongue 4");
        bool hasFlametongueEnchantment5 = HasEnchantment(EquipmentSlot.MainHand, "Flametongue 5");
        bool hasFlametongueEnchantment6 = HasEnchantment(EquipmentSlot.MainHand, "Flametongue 6");
        bool hasFlametongueEnchantment7 = HasEnchantment(EquipmentSlot.MainHand, "Flametongue 7");
        bool hasFlametongueEnchantment8 = HasEnchantment(EquipmentSlot.MainHand, "Flametongue 8");
        bool hasFlametongueEnchantment9 = HasEnchantment(EquipmentSlot.MainHand, "Flametongue 9");
        bool hasRockbiterEnchantment1 = HasEnchantment(EquipmentSlot.MainHand, "Rockbiter 1");
        bool hasRockbiterEnchantment2 = HasEnchantment(EquipmentSlot.MainHand, "Rockbiter 2");
        bool hasRockbiterEnchantment3 = HasEnchantment(EquipmentSlot.MainHand, "Rockbiter 3");
        bool hasWindfuryEnchantment1 = HasEnchantment(EquipmentSlot.MainHand, "Windfury");
        bool hasWindfuryEnchantment2 = HasEnchantment(EquipmentSlot.MainHand, "Windfury 2");
        bool hasWindfuryEnchantment3 = HasEnchantment(EquipmentSlot.MainHand, "Windfury 3");
        bool hasWindfuryEnchantment4 = HasEnchantment(EquipmentSlot.MainHand, "Windfury 4");
        bool hasWindfuryEnchantment5 = HasEnchantment(EquipmentSlot.MainHand, "Windfury 5");
        bool hasWindfuryEnchantment6 = HasEnchantment(EquipmentSlot.MainHand, "Windfury 6");
        bool hasWindfuryEnchantment7 = HasEnchantment(EquipmentSlot.MainHand, "Windfury 7");
        bool hasWindfuryEnchantment8 = HasEnchantment(EquipmentSlot.MainHand, "Windfury 8");
        bool hasWindfuryEnchantment9 = HasEnchantment(EquipmentSlot.MainHand, "Windfury 9");


        //off hand
        bool hasOffhandEnchantment1 = HasEnchantment(EquipmentSlot.OffHand, "Flametongue 1");
        bool hasOffhandEnchantment2= HasEnchantment(EquipmentSlot.OffHand, "Flametongue 2");
        bool hasOffhandEnchantment3= HasEnchantment(EquipmentSlot.OffHand, "Flametongue 3");
        bool hasOffhandEnchantment4= HasEnchantment(EquipmentSlot.OffHand, "Flametongue 4");
        bool hasOffhandEnchantment5= HasEnchantment(EquipmentSlot.OffHand, "Flametongue 5");
        bool hasOffhandEnchantment6= HasEnchantment(EquipmentSlot.OffHand, "Flametongue 6");
        bool hasOffhandEnchantment7= HasEnchantment(EquipmentSlot.OffHand, "Flametongue 7");
        bool hasOffhandEnchantment8= HasEnchantment(EquipmentSlot.OffHand, "Flametongue 8");
        bool hasOffhandEnchantment9= HasEnchantment(EquipmentSlot.OffHand, "Flametongue 9");

        bool hasAnyRockbiterEnchantment = hasRockbiterEnchantment1 || hasRockbiterEnchantment2 || hasRockbiterEnchantment3;
        bool hasAnyWindfuryEnchantment = hasWindfuryEnchantment1 || hasWindfuryEnchantment2 || hasWindfuryEnchantment3 || hasWindfuryEnchantment4 || hasWindfuryEnchantment5 ;
        bool hasAnyFlametongueEnchantment = hasFlametongueEnchantment || hasFlametongueEnchantment2 || hasFlametongueEnchantment3 || hasFlametongueEnchantment4 || hasFlametongueEnchantment5 || hasFlametongueEnchantment6 || hasFlametongueEnchantment7 || hasFlametongueEnchantment8 || hasFlametongueEnchantment9;

        if ( Api.Spellbook.CanCast("Windfury Weapon") && me.Level >40 && !hasAnyWindfuryEnchantment)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Rockbiter Weapon");
            Console.ResetColor();
            if (Api.Spellbook.Cast("Windfury Weapon"))
            {
                return true;
            }
        }
        if ( Api.Spellbook.CanCast("Flametongue Weapon") && !hasAnyFlametongueEnchantment && me.Level>10 && me.Level<40)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Flametongue Weapon");
            Console.ResetColor();
            if (Api.Spellbook.Cast("Flametongue Weapon"))
            {
                return true;
            }
        }
        else if (!hasAnyRockbiterEnchantment && Api.Spellbook.CanCast("Rockbiter Weapon") && me.Level <10)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Rockbiter Weapon");
            Console.ResetColor();
            if (Api.Spellbook.Cast("Rockbiter Weapon"))
            {
                return true;
            }
        }
        if (Api.Spellbook.CanCast("Flametongue Weapon") && me.Level > 40 && (!hasOffhandEnchantment1 || !hasOffhandEnchantment2 || !hasOffhandEnchantment3 || !hasOffhandEnchantment4 || !hasOffhandEnchantment5 || !hasOffhandEnchantment6 || !hasOffhandEnchantment7 || !hasOffhandEnchantment8 || !hasOffhandEnchantment9 ))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Flametongue Weapon on Offhand");
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
        if ( !target.IsDead() && !IsNPC(target) && Api.Spellbook.CanCast("Lightning Bolt") && healthPercentage > 50 && mana > 20 && targetDistance>25)  
    
        
      
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Lightning Bolt");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Lightning Bolt"))
            {
                return true;
            }
        }
        if (Api.Spellbook.CanCast("Totemic Recall") && (me.Auras.Contains("Stoneskin") || me.Auras.Contains("Windfury Totem")))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Totemic Recall");
            Console.ResetColor();
            if (Api.Spellbook.Cast("Totemic Recall"))
            {
                return true;
            }
        }
        if (Api.Spellbook.CanCast("Stoneskin Totem") && me.Auras.Contains("Stoneskin"))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Removing Stoneskin Totem");
            Console.ResetColor();
            if (Api.Spellbook.Cast("Stoneskin Totem"))
            {
                return true;
            }
        }
        if (Api.Spellbook.CanCast("Windfury Totem") && me.Auras.Contains("Windfury Totem"))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Removing Windfury Totem");
            Console.ResetColor();
            if (Api.Spellbook.Cast("Windfury Totem"))
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

        if (Api.Spellbook.CanCast("Stoneskin Totem") && !me.Auras.Contains("Stoneskin") && mana > 50)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Stoneskin Totem");
            Console.ResetColor();
            if (Api.Spellbook.Cast("Stoneskin Totem"))
            {
                return true;
            }
        }
        if (Api.Spellbook.CanCast("Windfury Totem") && !me.Auras.Contains("Windfury Totem") && mana > 50)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Windfury Totem");
            Console.ResetColor();
            if (Api.Spellbook.Cast("Windfury Totem"))
            {
                return true;
            }
        }
        if ((DateTime.Now - LastSearing) >= Searing)
        {
            // Check if you can cast Searing Totem and have enough mana
            if (Api.Spellbook.CanCast("Searing Totem") && mana > 50)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Casting Searing Totem");
                Console.ResetColor();

                // Cast Searing Totem
                if (Api.Spellbook.Cast("Searing Totem"))
                {
                    // Update the timestamp for the last time Searing Totem was cast
                    LastSearing = DateTime.Now;
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
        if (Api.Spellbook.CanCast("Mana Shield") && !me.Auras.Contains("Lightning Shield") && mana < 50)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Lighting Shield");
            Console.ResetColor();
            if (Api.Spellbook.Cast("Mana Shield"))
            {
                return true;
            }
        }
        if (Api.Spellbook.CanCast("Healing Wave") && healthPercentage <= 50 && me.AuraStacks("Maelstrom Weapon") == 5)
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
        if (Api.Spellbook.CanCast("Shamanistic Rage") && !Api.Spellbook.OnCooldown("Shamanistic Rage"))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Shamanistic Rage");
            Console.ResetColor();
            if (Api.Spellbook.Cast("Shamanistic Rage"))
            {
                return true;
            }
        }
        if (Api.Spellbook.CanCast("Feral Spirit") && !Api.Spellbook.OnCooldown("Feral Spirit"))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Feral Spirit");
            Console.ResetColor();
            if (Api.Spellbook.Cast("Feral Spirit"))
            {
                return true;
            }
        }
        if (Api.Spellbook.CanCast("Wind Shear") && !Api.Spellbook.OnCooldown("Wind Shear") && (target.IsCasting() || target.IsChanneling()))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Wind Shear");
            Console.ResetColor();
            if (Api.Spellbook.Cast("Wind Shear"))
            {
                return true;
            }
        }
        if (Api.Spellbook.CanCast("Earth Shock") && !Api.Spellbook.OnCooldown("Earth Shock") && (target.IsCasting() || target.IsChanneling()))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Earth Shock");
            Console.ResetColor();
            if (Api.Spellbook.Cast("Earth Shock"))
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
            GetStacks
        if (Api.Spellbook.CanCast("Lightning Bolt") && me.AuraStacks("Maelstrom Weapon") == 5)

        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Lightning Bolt");
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

        Console.ResetColor();
    }
	private bool IsNPC(WowUnit unit)
{
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