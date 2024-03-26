using System;
using System.Threading;
using wShadow.Templates;
using System.Collections.Generic;
using wShadow.Warcraft.Classes;
using wShadow.Warcraft.Defines;
using wShadow.Warcraft.Managers;


public class BoomkinWOTLK : Rotation
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

        if (me.IsDead() || me.IsGhost() || me.IsCasting() || me.IsMoving() || me.IsChanneling() || me.Auras.Contains("Drink") || me.Auras.Contains("Food")) return false;

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
        if (Api.Spellbook.CanCast("Thorns") && !Api.Player.Auras.Contains("Thorns") && !Api.Player.IsMounted())
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Thorns");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Thorns"))
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
        if (target == null || target.IsDead() || me.IsDead() || me.IsGhost() || me.IsCasting() || me.IsMoving() || me.IsChanneling() || me.Auras.Contains("Drink") || me.Auras.Contains("Food")) return false;
        // Healing
        if (Api.Spellbook.CanCast("Rejuvenation") && healthPercentage <= 70 && !me.Auras.Contains("Rejuvenation"))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Rejuvenation");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Rejuvenation"))
                return true;
        }
        if (Api.Spellbook.CanCast("Regrowth") && healthPercentage <= 45 && !me.Auras.Contains("Regrowth"))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Regrowth");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Regrowth"))
                return true;
        }
        if (healthPercentage <= 35 && Api.Spellbook.CanCast("Healing Touch"))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Healing Touch");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Healing Touch"))
                return true;
        }

        // Mana regeneration
        if (!me.Auras.Contains("Innervate") && mana <= 30 && Api.Spellbook.CanCast("Innervate"))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Innervate");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Innervate"))
                return true;
        }

        // Form management
        if (!me.Auras.Contains("Moonkin Form",false) && Api.Spellbook.CanCast("Moonkin Form"))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Moonkin Form");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Moonkin Form"))
                return true;
        }

        // Debuffs
        if (!target.Auras.Contains("Faerie Fire") && me.Auras.Contains("Moonkin Form") && Api.Spellbook.CanCast("Faerie Fire"))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Faerie Fire");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Faerie Fire"))
                return true;
        }

        // DPS rotation
        if (Api.Spellbook.CanCast("Force of Nature") && Api.HasMacro("Treant") && targetDistance <= 30 && !Api.Spellbook.OnCooldown("Force of Nature"))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Force of Nature");
            Console.ResetColor();

            if (Api.UseMacro("Treant"))
                return true;
        }
        if (Api.Spellbook.CanCast("Starfall") && targetDistance <= 20)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Starfall");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Starfall"))
                return true;
        }
        if (!target.Auras.Contains("Insect Swarm") && !me.Auras.Contains(48517) && !me.Auras.Contains(48518) && Api.Spellbook.CanCast("Insect Swarm"))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Insect Swarm");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Insect Swarm"))
                return true;
        }
        if (!target.Auras.Contains("Moonfire") && Api.Spellbook.CanCast("Moonfire"))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Moonfire");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Moonfire"))
                return true;
        }
        if (me.Auras.Contains(48517) && me.Auras.TimeRemaining(48517) > 1 && Api.Spellbook.CanCast("Wrath"))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Wrath");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Wrath"))
                return true;
        }
        if (me.Auras.Contains(48518) && me.Auras.TimeRemaining(48518) > 1 && Api.Spellbook.CanCast("Starfire"))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Starfire");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Starfire"))
                return true;
        }
        if (Api.Spellbook.CanCast("Wrath") && Api.Spellbook.CanCast("Starfire"))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Wrath");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Wrath") && mana > 11)
                return true;
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Casting Starfire");
                Console.ResetColor();

                if (Api.Spellbook.Cast("Starfire") && mana > 16)
                    return true;
            }
        }
        if (!target.Auras.Contains("Moonfire") && Api.Spellbook.CanCast("Moonfire"))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Moonfire");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Moonfire"))
                return true;
        }

        



        return base.CombatPulse();
    }

    private void LogPlayerStats()
    {
        var me = Api.Player;

        var mana = me.ManaPercent;
        var healthPercentage = me.HealthPercent;


        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"{mana}% Mana available");
        Console.WriteLine($"{healthPercentage}% Health available");   // Insert your player stats logging using the new API
    }
}