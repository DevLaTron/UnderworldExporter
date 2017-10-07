﻿using UnityEngine;
using System.Collections;

public class a_hack_trap_gemrotate : a_hack_trap {

		//Possibly used to cycle through the black rock gem states and target destinations.
		//Linked from a timer trigger at index 1000

		//This has a quality of 54d

		//Probably changes variable no 6 to set what is reachable. There is probably another value that sets what levels are available..
	int prevWorld=-1;

	protected override void Start ()
	{
		base.Start ();
		//GameWorldController.instance.playerUW.quest().variables[6]=0;
	}

		/// <summary>
		/// Executes the trap.
		/// </summary>
		/// <param name="src">Source.</param>
		/// <param name="triggerX">Trigger x.</param>
		/// <param name="triggerY">Trigger y.</param>
		/// <param name="State">State.</param>
		/// Toggles the variable controlling which world is available from this facet of the gem.
		/// In the future change the lighting on the gem.
	public override void ExecuteTrap (object_base src, int triggerX, int triggerY, int State)
	{
	//		base.ExecuteTrap (src, triggerX, triggerY, State);
				/*
		if (GameWorldController.instance.variables[6] == 0)
		{
			GameWorldController.instance.variables[6] = 8;	
		}
		else
		{
			GameWorldController.instance.variables[6] = 0;		
		}
		*/

		GameWorldController.instance.playerUW.quest().variables[6]++;
		if (GameWorldController.instance.playerUW.quest().variables[6] > a_hack_trap_teleport.NoOfWorlds)
		{
			GameWorldController.instance.playerUW.quest().variables[6]=0;	
		}
		if (prevWorld!=GameWorldController.instance.playerUW.quest().variables[6])
		{
			Debug.Log("Now serving world " +  GameWorldController.instance.playerUW.quest().variables[6]);				
		}		
		prevWorld=GameWorldController.instance.playerUW.quest().variables[6];
	}

	public override void PostActivate ()
	{

	}
}
