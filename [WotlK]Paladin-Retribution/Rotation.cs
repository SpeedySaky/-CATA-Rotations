using System;
using System.Threading;
using wShadow.Templates;
using wShadow.Warcraft.Classes;
using wShadow.Warcraft.Defines;
using wShadow.Warcraft.Managers;
using wShadow.Warcraft.Structures.Wow_Player;
using wShadow.Warcraft.Defines.Wow_Player;
using wShadow.Warcraft.Defines.Wow_Spell;

// Ensure to import the required API and other necessary libraries

public class RetPalaWOTLK : Rotation
{
    private int debugInterval = 5;
	    private DateTime lastDebugTime = DateTime.MinValue;


    public override void Initialize()
    {
		
		lastDebugTime = DateTime.Now;
        LogPlayerStats();
        SlowTick = 600;
        FastTick = 200;

    }

    public override bool PassivePulse()
    {
      var me = Api.Player;
		var healthPercentage = me.HealthPercent;
		var mana = me.Mana;

		if (me.IsDead() || me.IsGhost() || me.IsCasting() ) return false;
        if (me.HasAura("Drink") || me.HasAura("Food")) return false;
		
		if ((DateTime.Now - lastDebugTime).TotalSeconds >= debugInterval)
        {
            LogPlayerStats();
            lastDebugTime = DateTime.Now; // Update lastDebugTime
        }
		
		
		if (Api.Spellbook.CanCast("Holy Light") && Api.Player.HealthPercent <= 60)
{
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("Casting Holy Light");
    Console.ResetColor();
    
    if (Api.Spellbook.Cast("Holy Light"))
    {
        return true;
    }
}

if (Api.Spellbook.CanCast("Retribution Aura") && !Api.Player.HasPermanent("Retribution Aura") && !Api.Player.IsMounted())
{
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("Casting Retribution Aura");
    Console.ResetColor();

    if (Api.Spellbook.Cast("Retribution Aura"))
    {
        return true;
    }
}
else if (Api.Spellbook.CanCast("Devotion Aura") && !Api.Player.HasPermanent("Devotion Aura") && !Api.Player.HasPermanent("Retribution Aura") && !Api.Player.IsMounted())
{
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("Casting Devotion Aura");
    Console.ResetColor();

    if (Api.Spellbook.Cast("Devotion Aura"))
    {
        return true;
    }
}
else if (Api.Spellbook.CanCast("Crusader Aura") && !Api.Player.HasPermanent("Crusader Aura") && Api.Player.IsMounted())
{
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("Casting Crusader Aura");
    Console.ResetColor();

    if (Api.Spellbook.Cast("Crusader Aura"))
    {
        return true;
    }
}

if (Api.Spellbook.CanCast("Blessing of Might") && !Api.Player.HasAura("Blessing of Might") && !Api.Player.HasAura("Hand of Protection") && 
    !Api.Player.HasAura("Divine Protection"))
{
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("Casting Blessing of Might");
    Console.ResetColor();

    if (Api.Spellbook.Cast("Blessing of Might"))
    {
        return true;
    }
}
else if (Api.Spellbook.CanCast("Blessing of Wisdom") && !Api.Player.HasAura("Blessing of Wisdom") && !Api.Player.HasAura("Blessing of Might"))
{
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("Casting Blessing of Wisdom");
    Console.ResetColor();

    if (Api.Spellbook.Cast("Blessing of Wisdom"))
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
		var mana = me.Mana;
		 var target = Api.Target;
				var targetHealth = Api.Target.HealthPercent;

		if (Api.Spellbook.CanCast("Flash of Light") && (Api.Player.HasAura(59578) || Api.Player.HasAura(53489)) && Api.Player.HealthPercent < 75)
{
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("Casting Flash of Light");
    Console.ResetColor();
    if (Api.Spellbook.Cast("Flash of Light"))
    {
        return true;
    }
}

if (Api.Spellbook.CanCast("Divine Protection") && Api.Player.HealthPercent < 45 && !me.IsCasting() && !Api.Player.HasAura("Forbearance"))
{
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("Casting Divine Protection");
    Console.ResetColor();
    if (Api.Spellbook.Cast("Divine Protection"))
    {
        return true;
    }
}

if (Api.Player.HasAura("Divine Protection") && healthPercentage <= 50 && Api.Spellbook.CanCast("Holy Light"))
{
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("Casting Holy Light");
    Console.ResetColor();
    if (Api.Spellbook.Cast("Holy Light"))
    {
        return true;
    }
}
if ((Api.Player.HasAura(59578) || Api.Player.HasAura(53489)) &&Api.Spellbook.CanCast("Exorcism") && !Api.Spellbook.OnCooldown("Exorcism") && healthPercentage > 75)
{
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("Casting Exorcism");
    Console.ResetColor();
    if (Api.Spellbook.Cast("Exorcism"))
    {
        return true;
    }
}

if (Api.Spellbook.CanCast("Seal of Command") && !Api.Player.HasAura("Seal of Command"))
{
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("Casting Seal of Command");
    Console.ResetColor();

    if (Api.Spellbook.Cast("Seal of Command"))
    {
        return true;
    }
}
else if (Api.Spellbook.CanCast("Seal of Righteousness") && !Api.Player.HasAura("Seal of Righteousness") && !Api.Player.HasAura("Seal of Command"))
{
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("Casting Seal of Righteousness");
    Console.ResetColor();

    if (Api.Spellbook.Cast("Seal of Righteousness"))
    {
        return true;
    }
}
if (Api.Spellbook.CanCast("Judgement of Wisdom") && !Api.Target.HasAura("Judgement of Wisdom") && !Api.Spellbook.OnCooldown("Judgement of Wisdom") && Api.Player.HealthPercent >= 50)
{
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("Casting Judgement of Wisdom");
    Console.ResetColor();
    if (Api.Spellbook.Cast("Judgement of Wisdom"))
    {
        return true;
    }
}
else if (!Api.Target.HasAura("Judgement of Wisdom") && !Api.Spellbook.OnCooldown("Judgement of Wisdom"))
{
    if (Api.Spellbook.CanCast("Judgement of Light") && !Api.Spellbook.OnCooldown("Judgement of Light") && !Api.Target.HasAura("Judgement of Light"))
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Casting Judgement of Light");
        Console.ResetColor();
        if (Api.Spellbook.Cast("Judgement of Light"))
        {
            return true;
        }
    }
}

if (Api.UnfriendlyUnitsNearby(8, true)>=2 && Api.Spellbook.CanCast("Consecration") && !!Api.Spellbook.OnCooldown("Consecration"))
{
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("Casting Consecration");
    Console.ResetColor();
    if (Api.Spellbook.Cast("Consecration"))
    {
        return true;
    }
}

if (Api.UnfriendlyUnitsNearby(8, true)>=2 && Api.Spellbook.CanCast("Divine Storm") && !!Api.Spellbook.OnCooldown("Divine Storm"))
{
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("Casting Divine Storm");
    Console.ResetColor();
    if (Api.Spellbook.Cast("Divine Storm"))
    {
        return true;
    }
}
if (Api.Spellbook.CanCast("Holy Wrath") && targetHealth <= 20)
{
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("Casting Holy Wrath");
    Console.ResetColor();

    if (Api.Spellbook.Cast("Holy Wrath"))
    {
        return true;
    }
}

if (Api.Spellbook.CanCast("Hammer of Wrath") && targetHealth <= 20)
{
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("Casting Hammer of Wrath");
    Console.ResetColor();
    if (Api.Spellbook.Cast("Hammer of Wrath"))
    {
        return true;
    }
}

if (Api.Spellbook.CanCast("Crusader Strike") && mana > 50 && !Api.Spellbook.OnCooldown("Crusader Strike"))
{
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("Casting Crusader Strike");
    Console.ResetColor();
    if (Api.Spellbook.Cast("Crusader Strike"))
    {
        return true;
    }
}
		
        return base.CombatPulse();
    }

    private void LogPlayerStats()
    {
       var me = Api.Player;

		var mana = me.Mana;
        var healthPercentage = me.HealthPercent;
		

        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"{mana}% Mana available");
        Console.WriteLine($"{healthPercentage}% Health available");   // Insert your player stats logging using the new API
    }
	    }