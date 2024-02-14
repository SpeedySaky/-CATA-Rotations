using System;
using System.Threading;
using wShadow.Templates;
using System.Collections.Generic;
using wShadow.Warcraft.Classes;
using wShadow.Warcraft.Defines;
using wShadow.Warcraft.Managers;


public class PriestShadowWOTLK : Rotation
{

    private int debugInterval = 5; // Set the debug interval in seconds
    private DateTime lastDebugTime = DateTime.MinValue;

    public override void Initialize()
    {
        // Can set min/max levels required for this rotation.

        lastDebugTime = DateTime.Now;
        LogPlayerStats();
        // Use this method to set your tick speeds.
        // The simplest calculation for optimal ticks (to avoid key spam and false attempts)

        // Assuming wShadow is an instance of some class containing UnitRatings property
        SlowTick = 600;
        FastTick = 200;

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
        var mana = me.ManaPercent;
        var healthPercentage = me.HealthPercent;

        if ((DateTime.Now - lastDebugTime).TotalSeconds >= debugInterval)
        {
            LogPlayerStats();
            lastDebugTime = DateTime.Now; // Update lastDebugTime
       

        if (me.IsDead() || me.IsGhost() || me.IsCasting() || me.IsMoving() || me.IsChanneling() || me.IsLooting() || me.Auras.Contains("Drink") || me.Auras.Contains("Food") || !me.IsMounted()) return false;

        if (Api.Spellbook.CanCast("Renew") && !me.Auras.Contains("Renew") && healthPercentage < 80)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Renew");
            Console.ResetColor();
            if (Api.Spellbook.Cast("Renew"))
            {
                return true;
            }
        }
        if (Api.Spellbook.CanCast("Power Word: Fortitude") && !me.Auras.Contains("Power Word: Fortitude"))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Power Word: Fortitude");
            Console.ResetColor();
            if (Api.Spellbook.Cast("Power Word: Fortitude"))
            {
                return true;
            }
        }
        if (Api.Spellbook.CanCast("Divine Spirit") && !me.Auras.Contains("Divine Spirit"))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Divine Spirit");
            Console.ResetColor();
            if (Api.Spellbook.Cast("Divine Spirit"))
            {
                return true;
            }
        }
        if (Api.Spellbook.CanCast("Vampiric Embrace") && !me.Auras.Contains("Vampiric Embrace"))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Vampiric Embrace");
            Console.ResetColor();
            if (Api.Spellbook.Cast("Vampiric Embrace"))
            {
                return true;
            }
        }
        if (Api.Spellbook.CanCast("Shadow Protection") && !me.Auras.Contains("Shadow Protection"))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Shadow Protection");
            Console.ResetColor();
            if (Api.Spellbook.Cast("Shadow Protection"))
            {
                return true;
            }
        }
        if (Api.Spellbook.CanCast("Inner Fire") && !me.Auras.Contains("Inner Fire"))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Inner Fire");
            Console.ResetColor();
            if (Api.Spellbook.Cast("Inner Fire"))
            {
                return true;
            }
        }
        if (Api.Spellbook.CanCast("Shadowform") && !me.Auras.Contains("Shadowform"))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Shadowform");
            Console.ResetColor();
            if (Api.Spellbook.Cast("Shadowform"))
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
        var healthPercentage = me.HealthPercent;
        var targethealth = target.HealthPercent;
        var targetDistance = target.Position.Distance2D(me.Position);

        if ((DateTime.Now - lastDebugTime).TotalSeconds >= debugInterval)
        {
            LogPlayerStats();
            lastDebugTime = DateTime.Now; // Update lastDebugTime
        }



        if (Api.Spellbook.CanCast("Shadowform") && !me.Auras.Contains("Shadowform"))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Shadowform");
            Console.ResetColor();
            if (Api.Spellbook.Cast("Shadowform"))
            {
                return true;
            }
        }

        if (Api.Spellbook.CanCast("Power Word: Shield") && !me.Auras.Contains("Power Word: Shield")&& && !me.Auras.Contains("Weakened Soul"))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Power Word: Shield");
            Console.ResetColor();
            if (Api.Spellbook.Cast("Power Word: Shield"))
            {
                return true;
            }
        }
        if (Api.Spellbook.CanCast("Shadowfiend") && !Api.Spellbook.OnCooldown("Shadowfiend"))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Shadowfiend");
            Console.ResetColor();
            if (Api.Spellbook.Cast("Shadowfiend"))
            {
                return true;
            }
        }
        if (Api.Spellbook.CanCast("Shadow Word: Pain") && !target.Auras.Contains("Shadow Word: Pain") && targethealth >= 30)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Shadow Word: Pain");
            Console.ResetColor();
            if (Api.Spellbook.Cast("Shadow Word: Pain"))
            {
                return true;
            }
        }
        if (Api.Spellbook.CanCast("Shadow Word: Death") && !target.Auras.Contains("Shadow Word: Death") && targethealth <= 10)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Shadow Word: Death");
            Console.ResetColor();
            if (Api.Spellbook.Cast("Shadow Word: Death"))
            {
                return true;
            }
        }
        if (Api.Spellbook.CanCast("Vampiric Touch") && !target.Auras.Contains("Vampiric Touch") && targethealth >= 30)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Vampiric Touch");
            Console.ResetColor();
            if (Api.Spellbook.Cast("Vampiric Touch"))
            {
                return true;
            }
        }
        if (Api.Spellbook.CanCast("Devouring Plague") && !target.Auras.Contains("Devouring Plague") && targethealth >= 30)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Devouring Plague");
            Console.ResetColor();
            if (Api.Spellbook.Cast("Devouring Plague"))
            {
                return true;
            }
        }
        if (Api.Spellbook.CanCast("Mind Blast") && targethealth >= 30)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Mind Blast");
            Console.ResetColor();
            if (Api.Spellbook.Cast("Mind Blast"))
            {
                return true;
            }
        }
        if (Api.Spellbook.CanCast("Shoot"))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Shoot");
            Console.ResetColor();
            if (Api.Spellbook.Cast("Shoot"))
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

        // Health percentage of the player
        var healthPercentage = me.HealthPercent;


        // Target distance from the player
        var targetDistance = target.Position.Distance2D(me.Position);


        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"{mana}% Mana available");
        Console.WriteLine($"{healthPercentage}% Health available");

        Console.ResetColor();





        Console.ResetColor();
    }
}