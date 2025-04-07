using System;
using Unity.Behavior;

[BlackboardEnum]
public enum GunType
{
    Handguns,            // Small, lightweight firearms such as pistols and revolvers.
    Submachine_Guns,     // Compact automatic firearms with high fire rate, effective at close range.
    Assault_Rifles,      // Versatile rifles with selective fire modes, used for mid-range combat.
    Shotguns,            // Powerful close-range weapons that fire pellets or slugs.
    Sniper_Rifles,       // Long-range precision rifles designed for high accuracy.
    Light_Machine_Guns,  // Fully automatic support weapons with high magazine capacity.
    Heavy_Weapons,       // Large, powerful weapons such as miniguns or grenade launchers.
    Explosive_Weapons    // Weapons that deal area damage, such as RPGs and grenades.
}

[BlackboardEnum]
public enum FireType
{
	Semi_Automatic, //One bullet per trigger pull
	Fully_Automatic, // Fires continuously while holding the trigger
	Burst_Fire, // Fires a set number of bullets per trigger pull
	Bolt_Action, // Requires cycling the bolt between shots
	Lever_Action, // minecraft lever to chamber rounds
	Pump_Action, // rack the pump back
	Charge_Based, // charge beam
	Beam, // big beam
	Projectile_Based, // bow n arrow type shit
}