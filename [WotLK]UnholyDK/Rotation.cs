using System;
using System.Threading;
using wShadow.Templates;
using wShadow.Warcraft.Classes;
using wShadow.Warcraft.Defines;
using wShadow.Warcraft.Managers;
using wShadow.Warcraft.Structures.Wow_Player;
using wShadow.Warcraft.Defines.Wow_Player;
using wShadow.Warcraft.Defines.Wow_Spell;

public class UnholyDK : Rotation
{
    private DateTime lastLogTime = DateTime.MinValue;
    private int logInterval = 5; // Set the log interval in seconds

    public override void Initialize()
    {
        SlowTick = 600;
        FastTick = 200;

        PassiveActions.Add((true, () => false));
        CombatActions.Add((true, () => false));

        lastLogTime = DateTime.Now;
    }

    public override bool PassivePulse()
{
    var me = Api.Player;
    var pet = me.Pet();
	var target = Api.Target;

	var targetDistance = target.Position.Distance2D(me.Position);
    if (me.IsDead() || me.IsGhost() || me.IsCasting()) return false;
    if (me.HasAura("Drink") || me.HasAura("Food")) return false;

    LogPlayerStatsPeriodically();

    if (me.HasPassive("Glyph of Raise Dead") && pet == null && Api.Spellbook.CanCast("Raise Dead"))
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Casting Raise Dead because we have the glyph.");
        // Console.ResetColor();

        if (Api.Spellbook.Cast("Raise Dead"))
            return true;
    }
    else if (me.HasItem("Corpse Dust") &&  pet == null && Api.Spellbook.CanCast("Raise Dead"))
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Casting Raise Dead with Corpse Dust as we don't have the glyph.");
        // Console.ResetColor();

        if (Api.Spellbook.Cast("Raise Dead"))
            return true;
    }
	if (Api.Spellbook.CanCast("Horn of Winter") && !me.HasAura("Horn of Winter"))
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Casting Horn of Winter");
        Console.ResetColor();

        if (Api.Spellbook.Cast("Horn of Winter"))
            return true;
    }
	
	 if Api.Spellbook.CanCast("Unholy Presence") && me.HasPermanent("Blood Presence") || me.HasPermanent("Frost Presence"))
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Casting Unholy Presence");

        if (Api.Spellbook.CanCast("Unholy Presence"))
        {
            if (Api.Spellbook.Cast("Unholy Presence"))
                return true;
        }
		 }
		 
		 if (!target.IsDead())
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
		var runicPower = me.RunicPower;
			var bloodRunes = Api.BloodRunesReady();
            var unholyRunes = Api.UnholyRunesReady();
            var frostRunes = Api.FrostRunesReady();
            var deathRunes = Api.DeathRunesReady();
        if (!me.IsValid() || me.IsDeadOrGhost()) return false;
    

        
        if (!target.IsValid() || target.IsDeadOrGhost()) return false;

        if (me.HasPassive("Glyph of Raise Dead") && pet == null && Api.Spellbook.CanCast("Raise Dead"))
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Casting Raise Dead because we have the glyph.");
        // Console.ResetColor();

        if (Api.Spellbook.Cast("Raise Dead"))
            return true;
    }
    else if (me.HasItem("Corpse Dust") &&  pet == null && Api.Spellbook.CanCast("Raise Dead"))
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Casting Raise Dead with Corpse Dust as we don't have the glyph.");
        // Console.ResetColor();

        if (Api.Spellbook.Cast("Raise Dead"))
            return true;
    }
	
	 if (!target.IsDead())
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
	if (Api.Spellbook.CanCast("Horn of Winter") && !me.HasAura("Horn of Winter"))
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

	if (Api.Spellbook.CanCast("Death Pact") && pet != null && pet.IsValid() && health <= 10 && !Api.Spellbook.OnCooldown("Death Pact"))
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


if (Api.UnitsNearby(10, true) >= 2 && Api.Spellbook.CanCast("Pestilence") && bloodRunes >= 1 || deathRunes>= 1 && target.HasAura("Frost Fever") && target.HasAura("Blood Plague"))
{
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine($"Casting Pestilence ");
    Console.ResetColor();

    if (Api.Spellbook.Cast("Pestilence"))
        return true;
}

if (Api.Spellbook.CanCast("Death Strike") && health <= 60 && bloodRunes >= 1 && target.HasAura("Frost Fever") && target.HasAura("Blood Plague") && runicPower >= 15)
{
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine($"Casting Death Strike ");
    Console.ResetColor();

    if (Api.Spellbook.Cast("Death Strike")) 
        return true;
}       
if (Api.Spellbook.CanCast("Army of the Dead") && !me.IsCasting() && !Api.Spellbook.OnCooldown("Army of the Dead") && Api.UnitsNearby(10, true) >= 2)
{
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine($"Casting Army of the Dead");
    Console.ResetColor();

    if (Api.Spellbook.Cast("Army of the Dead"))
        return true;
}

if (Api.UnitsNearby(10, true) >= 2 && Api.Spellbook.CanCast("Blood Boil") && (bloodRunes >= 1 || deathRunes >= 1))
{
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine($"Casting Blood Boil");
    Console.ResetColor();

    if (Api.Spellbook.Cast("Blood Boil"))
        return true;
}

if (Api.Spellbook.CanCast("Summon Gargoyle") && runicPower > 60 && !me.IsCasting() && !Api.Spellbook.OnCooldown("Summon Gargoyle") && Api.UnitsNearby(10, true) >= 2)
{
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("Casting Summon Gargoyle");
    Console.ResetColor();

    if (Api.Spellbook.Cast("Summon Gargoyle"))
        return true;
}
if (Api.UnitsNearby(10, true) >= 2 && Api.Spellbook.CanCast("Bone Shield") && !me.HasAura("Bone Shield") &&!Api.Spellbook.OnCooldown("Bone Shield") && (unholyRunes>= 1 || deathRunes >= 1) && runicPower >= 10)
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
if (Api.Spellbook.CanCast("Unbreakable Armor") && me.HasAura("Unbreakable Armor") && frostRunes >= 1 && runicPower >= 10 && Api.UnitsNearby(10, true) >= 2)
{
    
  
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"Casting Unbreakable Armor ");
        Console.ResetColor();

        if (Api.Spellbook.Cast("Unbreakable Armor"))
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
if (Api.Spellbook.CanCast("Empower Rune Weapon") && !Api.Player.IsCasting() && !Api.Spellbook.OnCooldown("Empower Rune Weapon") && (bloodRunes < 1 || unholyRunes < 1 || frostRunes < 1) && runicPower>= 25)
{
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine($"Casting Empower Rune Weapon ");
    Console.ResetColor();

    if (Api.Spellbook.Cast("Empower Rune Weapon"))
        return true;
}
if ((Api.Spellbook.CanCast("Obliterate") && target.HasAura("Frost Fever") && target.HasAura("Blood Plague") && unholyRunes >= 1 && deathRunes >= 1 && runicPower >= 15) || (Api.Spellbook.CanCast("Obliterate") && target.HasAura("Frost Fever") && target.HasAura("Blood Plague") && deathRunes >= 1 && frostRunes >= 1 && runicPower >= 15) || (Api.Spellbook.CanCast("Obliterate") && target.HasAura("Frost Fever") && target.HasAura("Blood Plague") && deathRunes >= 1 && unholyRunes >= 1 && runicPower >= 15))
{
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine($"Casting Obliterate ");
    Console.ResetColor();

    if (Api.Spellbook.Cast("Obliterate"))
    {
        return true;
    }
}

if (Api.Spellbook.CanCast("Death Coil") && runicPower > 60)
{
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine($"Casting Death Coil with {runicPower} Runic Power");
    Console.ResetColor();

    if (Api.Spellbook.Cast("Death Coil"))
    {
        return true;
    }
}

if (Api.Spellbook.CanCast("Icy Touch") && !target.HasAura("Frost Fever") && (frostRunes >= 1 || deathRunes >= 1) && runicPower >= 10)
{
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine($"Casting Icy Touch  ");
    Console.ResetColor();

    if (Api.Spellbook.Cast("Icy Touch"))
    {
        return true;
    }
}

if ((Api.Spellbook.CanCast("Plague Strike") && !target.HasAura("Blood Plague") && (unholyRunes >= 1 || deathRunes >= 1) && runicPower >= 10) || (Api.Spellbook.CanCast("Plague Strike") && !target.HasAura("Ebon Plague") && (unholyRunes >= 1 || deathRunes >= 1) && runicPower >= 10))
{
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine($"Casting Plague Strike ");
    Console.ResetColor();

    if (Api.Spellbook.Cast("Plague Strike"))
    {
        return true;
    }
}

if ((Api.Spellbook.CanCast("Scourge Strike") && !target.HasAura("Blood Plague") && (unholyRunes >= 1 || deathRunes >= 1) && runicPower >= 10) || (Api.Spellbook.CanCast("Scourge Strike") && !target.HasAura("Blood Plague") && (frostRunes >= 1 || deathRunes >= 1) && runicPower >= 10))
{
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine($"Casting Scourge Strike ");
    Console.ResetColor();

    if (Api.Spellbook.Cast("Scourge Strike"))
    {
        return true;
    }
}
 
 if (Api.Spellbook.CanCast("Blood Strike") && (deathRunes >= 1 || bloodRunes >= 1) && runicPower >= 10)
{
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine($"Casting Blood Strike ");
    Console.ResetColor();

    if (Api.Spellbook.Cast("Blood Strike"))
    {
        return true;
    }
}
 
 if (Api.Spellbook.CanCast("Death Strike") && unholyRunes >= 1 && frostRunes >= 1 && target.HasAura("Frost Fever") && target.HasAura("Blood Plague") && runicPower >= 15)
{
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine($"Casting Death Strike  ");
    Console.ResetColor();

    if (Api.Spellbook.Cast("Death Strike"))
    {
        return true;
    }
}

if (Api.Spellbook.CanCast("Icy Touch") && (frostRunes >= 1 || deathRunes >= 1) && runicPower >= 10)
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
			
			if (me.HasPassive("Glyph of Raise Dead")) // Replace "Thorns" with the actual aura name
{
		 Console.ForegroundColor = ConsoleColor.Blue;

       Console.WriteLine($"Glyph of Raise Dead");
}
		if (me.HasPassive("Unholy Presence") || me.HasPassive("Improved Unholy Presence"))
    {
        Console.ForegroundColor = ConsoleColor.Blue;
        if (me.HasPassive(48265))
            Console.WriteLine("Unholy Presence");
        else if (me.HasPassive(50392))
            Console.WriteLine("Improved Unholy Presence");

        Console.ResetColor();
    }
	if (me.HasPermanent("Blood Presence"))
    {
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine("Blood Presence");
        Console.ResetColor();
    }
    else if (me.HasPermanent("Frost Presence"))
    {
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine("Frost Presence");
        Console.ResetColor();
    }
    else if (me.HasPermanent("Unholy Presence"))
    {
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine("Unholy Presence");
        Console.ResetColor();
    }
	
        }
    }
}
