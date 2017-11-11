﻿using UnityEngine;
using System.Collections;


/// <summary>
/// A pressure trigger that is activated when a weight is placed or removed from a tile.
/// </summary>
/// There are multiple Item ids that have this trigger type.
/// A pressure trigger only triggers on weights put on.
/// A release trigger only triggers on weights removed. Both triggers track if the weight is on them.
/// Currently this implementation is tested for IDs 436 & 437 which change the texture of the tile.
/// 
/// The y paramater seems to control if the texture changes when weight is released. Depending on value will either toggle texture, do nothing or use texture 6 (?)
/// x might do the same for putting weight on.
/// Hard coding of texture values does not seem right but i've seen no exceptions yet.
public class a_pressure_trigger : trigger_base {

		/// <summary>
		/// The tile X that contains this trigger.
		/// </summary>	
	public int TileXToWatch;
		/// <summary>
		/// The tile Y that contains this trigger.
		/// </summary>
	public int TileYToWatch;

		/// <summary>
		/// The texture index when weight is on the trigger.
		/// </summary>
	//public int TextureOn;

		/// <summary>
		/// The texture index when no weight is on the trigger.
		/// </summary>
	//public int TextureOff;

		/// <summary>
		/// The colliders that are in contact with the trigger.
		/// </summary>
	public Collider[] colliders;

		/// <summary>
		/// If true the trigger will fire when weight is taken off.
		/// </summary>
	public bool IsReleaseTrigger;

		/// <summary>
		/// Any door trap that is linked to this trigger. For instant opening.
		/// </summary>
	public a_door_trap door; //Any door that this trigger might use


		/// <summary>
		/// The weight on trigger in this frame
		/// </summary>
	public float WeightOnTrigger;

		/// <summary>
		/// The  weight on trigger in the last frame
		/// </summary>
	public float PreviousWeightOnTrigger;

		/// <summary>
		/// The position of the center of the tile.
		/// </summary>
	private Vector3 TileVector;

		/// <summary>
		/// The contact area that detects the presence of objects.
		/// </summary>
	public Vector3 ContactArea= new Vector3(0.4f,0.1f,0.4f);


	protected override void Start ()
	{
		base.Start ();
		IsReleaseTrigger = ((objInt().item_id==437) || (objInt().item_id==421));
						
		TileXToWatch=objInt().tileX;
		TileYToWatch=objInt().tileY;
		TileVector=GameWorldController.instance.currentTileMap().getTileVector(TileXToWatch,TileYToWatch);

		int currentFloorTexture=GameWorldController.instance.currentTileMap().Tiles[TileXToWatch,TileYToWatch].floorTexture;
		GameWorldController.instance.currentTileMap().Tiles[TileXToWatch,TileYToWatch].PressureTriggerIndex=objInt().objectloaderinfo.index;

		colliders= Physics.OverlapBox(TileVector, new Vector3(0.4f,0.1f,0.4f));
		WeightOnTrigger=0f;		
		for (int i=0; i<=colliders.GetUpperBound(0);i++)
		{
			if (colliders[i].gameObject.GetComponent<ObjectInteraction>()!=null)
			{
				WeightOnTrigger+= colliders[i].gameObject.GetComponent<ObjectInteraction>().GetWeight();
			}
			else if( (colliders[i].gameObject.GetComponent<UWCharacter>()!=null) ||  (colliders[i].gameObject.GetComponent<Feet>()!=null) )
			{
				WeightOnTrigger+=5000;
			}
		}
		PreviousWeightOnTrigger=WeightOnTrigger;
		//Debug.Log("starting weight is " + PreviousWeightOnTrigger);

		if ( GameWorldController.instance.objectMaster.type[GameWorldController.instance.CurrentObjectList().objInfo[objInt().link].item_id]== ObjectInteraction.A_DOOR_TRAP)
		{					
			ObjectInteraction objDoorTrap=	ObjectLoader.getObjectIntAt(objInt().link);

			if (objDoorTrap!=null)
			{
				door = objDoorTrap.GetComponent<a_door_trap>();
			}
		}
	}

		/// <summary>
		/// Update this instance.
		/// </summary>
		/// Continually checks what objects are in the contact area.
	public override void Update ()
	{
		base.Update();
		
		colliders= Physics.OverlapBox(TileVector, ContactArea);
		WeightOnTrigger=0f;
		for (int i=0; i<=colliders.GetUpperBound(0);i++)
		{
			if (colliders[i].gameObject.GetComponent<ObjectInteraction>()!=null)
			{
				WeightOnTrigger+= colliders[i].gameObject.GetComponent<ObjectInteraction>().GetWeight();
			}
			else if( (colliders[i].gameObject.GetComponent<UWCharacter>()!=null) ||  (colliders[i].gameObject.GetComponent<Feet>()!=null) )
			{
				WeightOnTrigger+=5000;
			}
		}

		if (IsReleaseTrigger)
		{
			if ((WeightOnTrigger<1.0f) && (PreviousWeightOnTrigger>=1.0f))								
			{
				ReleaseWeightFrom();	
			}
			else if ((WeightOnTrigger>=1.0f) && (PreviousWeightOnTrigger<1.0f) && (!GameWorldController.WorldReRenderPending))
			{//Trigger has gained weight but is a release trigger.
				UpdateTileTexture(8);	
			}
		}
		else
		{
			if ((WeightOnTrigger>=1.0f) && (PreviousWeightOnTrigger<1.0f))								
			{
				PutWeightOn();
			}
			else if ((WeightOnTrigger<=1.0f) && (PreviousWeightOnTrigger>1.0f) && (!GameWorldController.WorldReRenderPending))
			{//Trigger has lost weight but is not a release trigger.
				UpdateTileTexture(7);	
			}
		}

		PreviousWeightOnTrigger=WeightOnTrigger;
	}
	
		/// <summary>
		/// Puts the weight on the trigger and activates
		/// </summary>
	public void PutWeightOn()
	{
		//UpdateTileTexture(GameWorldController.instance.currentTileMap().Tiles[TileXToWatch,TileYToWatch].floorTexture+1);
		UpdateTileTexture(8);
		if (door!=null)
		{
			door.TriggerInstantly=true;
		}
		Activate ();	
		if (door!=null)
		{
			door.TriggerInstantly=false;
		}
	}
		/// <summary>
		/// Releases the weight from the trigger and activates
		/// </summary>
	public void ReleaseWeightFrom()
	{
		//UpdateTileTexture(GameWorldController.instance.currentTileMap().Tiles[TileXToWatch,TileYToWatch].floorTexture-1);
		UpdateTileTexture(7);
		if (door!=null)
		{
			door.TriggerInstantly=true;
		}
		Activate ();	
		if (door!=null)
		{
			door.TriggerInstantly=false;
		}	
	}

		/// <summary>
		/// Updates the tile texture of the floor.
		/// </summary>
		/// <param name="newTexture">New texture.</param>
	public void UpdateTileTexture(int newTexture)
	{//Question. Is the texture map always 7 & 8??
				if (objInt().x==3)
				{//TODO:confirm this behaviour is consistent
						return;
				}
		GameWorldController.instance.currentTileMap().Tiles[TileXToWatch,TileYToWatch].floorTexture = (short)newTexture;	
		GameWorldController.instance.currentTileMap().Tiles[TileXToWatch,TileYToWatch].TileNeedsUpdate();
		GameObject tileToDestroy= GameWorldController.FindTile(TileXToWatch,TileYToWatch,TileMap.SURFACE_FLOOR);
		if (tileToDestroy!=null)
		{
			Destroy(tileToDestroy);				
		}		
	}
}