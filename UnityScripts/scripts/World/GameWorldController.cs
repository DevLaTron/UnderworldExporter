﻿using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using UnityEngine.UI;

/// <summary>
/// Game world controller for controlling references and various global activities
/// </summary>

public class GameWorldController : UWEBase {

		public bool bGenNavMeshes=true;
		public float testUVadjust=0f;

		/// <summary>
		/// Enables texture animation effects
		/// </summary>
	public bool EnableTextureAnimation;


		/// <summary>
		/// The level model parent object
		/// </summary>
	public GameObject LevelModel;
	
		/// <summary>
		/// The instance of this class
		/// </summary>
	public static GameWorldController instance;

		/// <summary>
		/// What level number we are currently on.
		/// </summary>	
	public int LevelNo;

		/// <summary>
		/// What level the player starts on in a quick start
		/// </summary>
		public int startLevel=0;
		/// <summary>
		/// What start position for the player.
		/// </summary>
		public Vector3 StartPos=new Vector3(38f, 4f, 2.7f);


	/// <summary>
	/// Array of cycled game palettes for animation effects.
	/// </summary>
	public Texture2D[] paletteArray= new Texture2D[8];

	/// <summary>
	/// The index of the palette currently in use
	/// </summary>
	public int paletteIndex=0;

	/// <summary>
	/// The palette index when going in reverse.
	/// </summary>
	public int paletteIndexReverse=0;

	/// <summary>
	/// The Variables for the check/set variable traps
	/// </summary>
	public int[] variables = new int[127];
	
	/// <summary>
	/// The tilemap class for the game
	/// </summary>
	public TileMap[] Tilemaps = new TileMap[9];

	/// <summary>
	/// The player character.
	/// </summary>
	[SerializeField]
	private UWCharacter _playerUW;
	public UWCharacter playerUW {
		get { return _playerUW; }
		set { _playerUW=value; }
		}


	/// <summary>
	/// The music controller for the game
	/// </summary>
	private MusicController mus;


	/// <summary>
	/// The game object that picked up items are parented to.
	/// </summary>
	public GameObject InventoryMarker;

	/// <summary>
	/// The game name.
	/// </summary>
	/// Value is passed to UWEBase and used in all resource file loads
	public string game;

	//public string UI_Name;
	/// <summary>
	/// The object master class for storing and reading object properties in an external file
	/// </summary>
	public ObjectMasters objectMaster;

	/// <summary>
	/// The critter properties from objects.dat
	/// </summary>
	public Critters critterData;

	/// <summary>
	/// Common Object Properties
	/// </summary>
	//public CommonObjProps commobj;

	// <summary>
	// Weapon properties.
	// </summary>
//	public WeaponProps weaponprops;

	/// <summary>
	/// The grey scale shader. Reference to allow loading of a hidden shader.
	/// </summary>
	public Shader greyScale;

	/// <summary>
	/// The vortex effect shader.  Reference to allow loading of a hidden shader.
	/// </summary>
	public Shader vortex;

	/// <summary>
	/// Is the game at the main menu or should it start at the mainmenu.
	/// </summary>
	public bool AtMainMenu;

	/// <summary>
	/// Path to lev.ark file to load
	/// </summary>
	public string Lev_Ark_File;

		public string Lev_Ark_File_Selected = "Data\\Lev.ark";

	/// <summary>
	/// The graves file for associating grave textures with grave objects
	/// </summary>
	public string Graves_File;	
	
	/// <summary>
	/// The material master list for matching the texture list to materials.
	/// </summary>
	public Material[] MaterialMasterList=new Material[260];

	public Material[] SpecialMaterials=new Material[1];

		/// <summary>
		/// The material for doors
		/// </summary>
		public Material[] MaterialDoors=new Material[13];

	/// <summary>
	/// Gameobject to load the objects at
	/// </summary>
	public GameObject ObjectMarker;

		/// <summary>
		/// The object lists for each level.
		/// </summary>
	public ObjectLoader[] objectList= new ObjectLoader[9];

	public RAIN.Navigation.NavMesh.NavMeshRig NavRigLand;
	public RAIN.Navigation.NavMesh.NavMeshRig NavRigWater;//To implement for create npc

	/// <summary>
	/// Shared palettes for artwork
	/// </summary>
	public PaletteLoader palLoader;

	/// <summary>
	/// The bytloader for bty files
	/// </summary>
	public BytLoader bytloader;
		/// <summary>
		/// The tex loader for textures
		/// </summary>
	public TextureLoader texLoader;
		/// <summary>
		/// The spell icons gr loader
		/// </summary>
	public GRLoader SpellIcons;
		/// <summary>
		/// The object art gr loader
		/// </summary>
	public GRLoader ObjectArt;

		/// <summary>
		/// The door art.
		/// </summary>
	public GRLoader DoorArt;

	/// <summary>
	/// The tm object art.
	/// </summary>
	public GRLoader TmObjArt;

	/// <summary>
	/// The tm flat art.
	/// </summary>
	public GRLoader TmFlatArt;

	/// <summary>
	/// Small animations art.
	/// </summary>
	public GRLoader TmAnimo;

	/// <summary>
	/// The lev ark file data.
	/// </summary>
	private char[] lev_ark_file_data;

	/// <summary>
	/// The female armor
	/// </summary>
	public GRLoader armor_f;

	/// <summary>
	/// The male armor.
	/// </summary>
	public GRLoader armor_m;

	/// <summary>
	/// The cursors art
	/// </summary>
	public GRLoader grCursors;

	/// <summary>
	/// The health & mana flasks.
	/// </summary>
	public GRLoader grFlasks;

		/// <summary>
		/// Cutscene data
		/// </summary>
	public CutsLoader cutsLoader;

	public CritLoader[] critsLoader= new CritLoader[64];

	/// <summary>
	/// The object dat file
	/// </summary>
	public ObjectDatLoader objDat;

		/// <summary>
		/// The common object properties for uw
		/// </summary>
	public CommonObjectDatLoader commonObject;

		public ObjectPropLoader ShockObjProp;

		/// <summary>
		/// The terrain data from terrain.dat
		/// </summary>
	public TerrainDatLoader terrainData;


	/// <summary>
	/// The weapon animation frames.
	/// </summary>
	public WeaponAnimation weaps;
	public WeaponAnimationPlayer WeaponAnim;
	public WeaponsLoader weapongr;



	public struct bablGlobal
	{
		public int ConversationNo;
		public int Size;
		public int[] Globals;
	};

	public bablGlobal[] bGlobals;
	public ConversationVM convVM;

	void  LoadPath()
	{
		string fileName = Application.dataPath + "//..//" + game + "_path.txt";
		StreamReader fileReader = new StreamReader(fileName, Encoding.Default);
		Loader.BasePath=fileReader.ReadLine().TrimEnd();
	}

	/// <summary>
	/// Awake this instance.
	/// </summary>
	/// Should be the very first script to run 
	void Awake()
	{
		instance=this;
		LoadPath();

		UWEBase._RES = game;
		Loader._RES=game;
				switch(game)
				{
				case GAME_SHOCK:
						palLoader = new PaletteLoader();
						palLoader.Path=Loader.BasePath + "res\\data\\gamepal.res";
						palLoader.PaletteNo=700;
						palLoader.LoadPalettes();
						texLoader=new TextureLoader();
						objectMaster=new ObjectMasters();
						ObjectArt=new GRLoader("res\\data\\objart.res",1350);
						ShockObjProp= new ObjectPropLoader();
						break;
				default:
					objectMaster=new ObjectMasters();
					objDat = new ObjectDatLoader();
					commonObject= new CommonObjectDatLoader();


					palLoader = new PaletteLoader();
					palLoader.Path=Loader.BasePath + "data\\pals.dat";
					palLoader.LoadPalettes();
					bytloader=new BytLoader();

					texLoader=new TextureLoader();
					ObjectArt=new GRLoader(GRLoader.OBJECTS_GR);
					SpellIcons = new GRLoader(GRLoader.SPELLS_GR);
					DoorArt=new GRLoader(GRLoader.DOORS_GR);
					TmObjArt=new GRLoader(GRLoader.TMOBJ_GR);
					TmFlatArt=new GRLoader(GRLoader.TMFLAT_GR);
					TmAnimo=new GRLoader(GRLoader.ANIMO_GR);
					armor_f=new GRLoader(GRLoader.ARMOR_F_GR);
					armor_m=new GRLoader(GRLoader.ARMOR_M_GR);
					grCursors = new GRLoader(GRLoader.CURSORS_GR);
					grFlasks=new GRLoader(GRLoader.FLASKS_GR);

					terrainData= new TerrainDatLoader();
					weaps=new WeaponAnimation();
					break;
				}
	}


	void Start () {

		instance=this;

		switch(_RES)
		{
		case GAME_SHOCK:
			AtMainMenu=false;
			UWHUD.instance.gameObject.SetActive(false);
			SwitchLevel(startLevel);
			return;

		case GAME_UWDEMO:
				//case GAME_UW2:
				//UW Demo does not go to the menu. It will load automatically into the gameworld
				AtMainMenu=false;	
				StringController.instance.LoadStringsPak(Loader.BasePath+"data\\strings.pak");
				convVM.LoadCnvArk(Loader.BasePath+"data\\cnv.ark");
				break;
		case GAME_UW2:
				StringController.instance.LoadStringsPak(Loader.BasePath+"data\\strings.pak");
				convVM.LoadCnvArkUW2(Loader.BasePath+"data\\cnv.ark");
				break;		
		default:
				StringController.instance.LoadStringsPak(Loader.BasePath+"data\\strings.pak");
				convVM.LoadCnvArk(Loader.BasePath+"data\\cnv.ark");
				break;
		}

		if (EnableTextureAnimation==true)
		{
			UWHUD.instance.CutsceneFullPanel.SetActive(false);
			InvokeRepeating("UpdateAnimation",0.2f,0.2f);
		}

		if (AtMainMenu)
		{
			SwitchLevel(-1);//Turn off all level maps
			UWHUD.instance.CutsceneFullPanel.SetActive(true);
			UWHUD.instance.mainmenu.gameObject.SetActive(true);
			//Freeze player movement and put them at a set location
			playerUW.playerController.enabled=false;
			playerUW.playerMotor.enabled=false;
			playerUW.transform.position=Vector3.zero;

			getMus().InIntro=true;
		}
		else
		{			
			UWHUD.instance.CutsceneFullPanel.SetActive(false);	
			UWHUD.instance.mainmenu.gameObject.SetActive(false);
			UWHUD.instance.RefreshPanels(UWHUD.HUD_MODE_INVENTORY);
			SwitchLevel(startLevel);
		}
		InvokeRepeating("PositionDetect",0.0f,0.02f);
		return;
	}

		/// <summary>
		/// Gets the current level model.
		/// </summary>
		/// <returns>The current level model gameobject</returns>
	public GameObject getCurrentLevelModel()
	{
		//return GameWorldController.instance.WorldModel[LevelNo].transform.FindChild("Level" + LevelNo + "_model").gameObject;
		return LevelModel;
	}

	/// <summary>
	/// Updates the global shader parameter for the colorpalette shaders at set intervals. To enable texture animation
	/// </summary>
	void UpdateAnimation()
	{
		Shader.SetGlobalTexture ("_ColorPaletteIn",paletteArray[paletteIndex]);

		if (paletteIndex<paletteArray.GetUpperBound(0))
		{
			paletteIndex++;
		}
		else
		{
			paletteIndex=0;
		}

		//In Reverse

		Shader.SetGlobalTexture ("_ColorPaletteInReverse",paletteArray[paletteIndexReverse]);
		
		if (paletteIndexReverse>0)
		{
			paletteIndexReverse--;
		}
		else
		{
			paletteIndexReverse=paletteArray.GetUpperBound(0);
		}
		return;
	}

	/// <summary>
	/// inds a door in the tile pointed to by the two coordinates.
	/// </summary>
	/// <returns>The door.</returns>
	/// <param name="x">The x coordinate.</param>
	/// <param name="y">The y coordinate.</param>
	public static GameObject findDoor(int x, int y)
	{
		return GameObject.Find ("door_" +x .ToString ("D3") + "_" + y.ToString ("D3"));
	}

	/// <summary>
	/// Finds the tile or wall at the specified coordinates.
	/// </summary>
	/// <returns>The tile.</returns>
	/// <param name="x">The x coordinate.</param>
	/// <param name="y">The y coordinate.</param>
	/// <param name="surface">Surface.</param>
	public static GameObject FindTile(int x, int y, int surface)
	{
		string tileName = GetTileName (x,y,surface);
		Transform found = instance.getCurrentLevelModel().transform.FindChild (tileName);
		if (found!=null)
		{
			return found.gameObject;
		}
		Debug.Log("Cannot find " + tileName);
		return null;
	}
	
		/// <summary>
		/// Gets the gameobject name for the specified tile x,y and surface. Eg Wall_02_03, Tile_22_23
		/// </summary>
		/// <returns>The tile name.</returns>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		/// <param name="surface">Surface.</param>
		/// Surfaces are 
	public static string GetTileName(int x, int y, int surface)
	{//Assumes we'll only ever need to deal with open/solid tiles with floors and ceilings.
		string tileName;
		string X; string Y;
		X=x.ToString ("D2");
		Y=y.ToString ("D2");
		switch (surface)
		{
		case TileMap.SURFACE_WALL:  //SURFACE_WALL:
		{
			tileName= "Wall_" + X + "_" + Y;
			break;
		}
		case TileMap.SURFACE_CEIL: //SURFACE_CEIL:
		{
			tileName="Ceiling_" + X + "_" + Y;
			break;
		}
		case TileMap.SURFACE_FLOOR:
		case TileMap.SURFACE_SLOPE:
		default:
		{
			tileName="Tile_" + X  + "_" + Y;
			break;
		}
		}
		return tileName;
	}
	
	/// <summary>
	/// Finds a tile in the current level by name
	/// </summary>
	/// <returns>The tile by name.</returns>
	/// <param name="tileName">Tile name.</param>
	public static GameObject FindTileByName(string tileName)
	{
		return instance.getCurrentLevelModel().transform.FindChild (tileName).gameObject;
	}
	
	/// <summary>
	/// Returns the transform of the levels object marker. So objects will remain on that level
	/// </summary>
	/// <returns>The marker.</returns>
	public Transform LevelMarker()
	{
		return ObjectMarker.transform;
		//return LevelObjects[LevelNo].transform;
	}

	/// <summary>
	/// Switches the level to another one. Disables the map and level objects of the old one.
	/// </summary>
	/// <param name="newLevelNo">New level no.</param>
		/// 
		public void SwitchLevel(int newLevelNo)
		{
			if (newLevelNo!=-1)
			{
				if(LevelNo==-1)
				{//I'm at the main menu. Load up the file data now.
					InitLevelData();
				}

				//Check loading
				if (Tilemaps[newLevelNo]==null)
				{//Data has not been loaded for this level
					Tilemaps[newLevelNo]=new TileMap();
					Tilemaps[newLevelNo].thisLevelNo=newLevelNo;
					
					if (UWEBase._RES!=UWEBase.GAME_SHOCK)
					{
						Tilemaps[newLevelNo].BuildTileMapUW(lev_ark_file_data, newLevelNo);
						objectList[newLevelNo]=new ObjectLoader();
						objectList[newLevelNo].LoadObjectList( Tilemaps[newLevelNo],lev_ark_file_data);	
					}
					else
					{
						Tilemaps[newLevelNo].BuildTileMapShock(lev_ark_file_data, newLevelNo);
						objectList[newLevelNo]=new ObjectLoader();
						objectList[newLevelNo].LoadObjectListShock(Tilemaps[newLevelNo],lev_ark_file_data);
					}
					if (UWEBase.EditorMode==false)
					{
						Tilemaps[newLevelNo].CleanUp(_RES);//I can reduce the tile map complexity after I know about what tiles change due to objects									
					}

				}
				if (UWEBase._RES!=UWEBase.GAME_SHOCK)
				{
					//Call events for inventory objects on level transition.
					foreach (Transform t in GameWorldController.instance.InventoryMarker.transform) 
					{
						if (t.gameObject.GetComponent<object_base>()!=null)
						{
							t.gameObject.GetComponent<object_base>().InventoryEventOnLevelExit();
						}
					}

				}

				if(LevelNo!=-1)
				{//Changing from a level that has already loaded
					//Update the positions of all object interactions in the level
					//UpdatePositions();

					if (UWEBase.EditorMode==false)
					{
						ObjectLoader.UpdateObjectList(GameWorldController.instance.currentTileMap(), GameWorldController.instance.CurrentObjectList());		
					}
					//Store the state of the object list with just the objects in objects transform for when I re
					
				}


				//Get my object info into the tile map.
				LevelNo=newLevelNo;
				switch(UWEBase._RES)
				{
				case GAME_SHOCK:
						break;
				default:
						critsLoader= new CritLoader[64];//Clear out animations
						if (UWEBase.EditorMode==false)
						{
							//Call events for inventory objects on level transition.
							foreach (Transform t in GameWorldController.instance.InventoryMarker.transform) 
							{
								if (t.gameObject.GetComponent<object_base>()!=null)
								{
									t.gameObject.GetComponent<object_base>().InventoryEventOnLevelEnter();
								}
							}
						}
						break;
				}

				TileMapRenderer.GenerateLevelFromTileMap(LevelModel,_RES,Tilemaps[newLevelNo],objectList[newLevelNo]);

				switch(UWEBase._RES)
				{
					case GAME_SHOCK:
						//break;
					default:
						ObjectLoader.RenderObjectList(objectList[newLevelNo],Tilemaps[newLevelNo],LevelMarker().gameObject);
						break;
				}
				

				if (bGenNavMeshes)
				{
					GenerateNavmesh(NavRigLand);
					GenerateNavmesh(NavRigWater);								
				}

				if ((LevelNo==7) && (UWEBase._RES==UWEBase.GAME_UW1))
				{//Create shrine lava.
					GameObject shrineLava = new GameObject();
					shrineLava.transform.parent=LevelMarker();
					shrineLava.transform.localPosition=new Vector3(39f,0.402f,39.61f);
					shrineLava.transform.localScale=new Vector3(6f,0.2f,4.8f);
					shrineLava.AddComponent<ShrineLava>();
					shrineLava.AddComponent<BoxCollider>();
					shrineLava.GetComponent<BoxCollider>().isTrigger=true;
				}
			}
		}

		/// <summary>
		/// Switchs the level and puts the player at the floor level of the new level
		/// </summary>
		/// <param name="newLevelNo">New level no.</param>
		/// <param name="newTileX">New tile x.</param>
		/// <param name="newTileY">New tile y.</param>
		public void SwitchLevel(int newLevelNo, int newTileX, int newTileY)
		{
				SwitchLevel(newLevelNo);
				float targetX=(float)newTileX*1.2f + 0.6f;
				float targetY= (float)newTileY*1.2f + 0.6f;
				float Height = ((float)(GameWorldController.instance.Tilemaps[newLevelNo].GetFloorHeight(newTileX,newTileY)))*0.15f;
				GameWorldController.instance.playerUW.transform.position=new Vector3(targetX,Height+0.1f,targetY);
		}

		// This will regenerate the navigation mesh when called
		void GenerateNavmesh(RAIN.Navigation.NavMesh.NavMeshRig NavRig)
		{//From Legacy.rivaltheory.com/forums/topics/runtime-navmesh-generation-and-path-finding-tutorial
				int _threadcount=4;
				// Unregister any navigation mesh we may already have (probably none if you are using this)
				NavRig.NavMesh.UnregisterNavigationGraph();
				NavRig.NavMesh.Size = 20;
				//float startTime = Time.time;
				NavRig.NavMesh.StartCreatingContours(_threadcount);
				NavRig.NavMesh.CreateAllContours();
				//float endTime = Time.time;
				//Debug.Log("NavMesh generated in " + (endTime - startTime) + "s");
				NavRig.NavMesh.RegisterNavigationGraph();
				NavRig.Awake();

		}


	/*
	public void SwitchLevel(int newLevelNo)
	{
		for (int i=0; i <=WorldModel.GetUpperBound(0);i++)
		{
			if(WorldModel[i]==null)
			{
				WorldModel[i]=GameObject.Find("_Level" + i);
			}
			WorldModel[i].SetActive(i==newLevelNo);
			LevelObjects[i].SetActive(i==newLevelNo);
		}	
		LevelNo=newLevelNo;
	}*/

	/// <summary>
	/// Freezes the movement of the specified object if it has a rigid body attached.
	/// </summary>
	/// <param name="myObj">My object.</param>
	public static void FreezeMovement(GameObject myObj)
	{//Stop objects which can move in the 3d world from moving when they are in the inventory or containers.
			Rigidbody rg = myObj.GetComponent<Rigidbody>();
			if (rg!=null)
			{
					rg.useGravity=false;
					rg.constraints = 
							RigidbodyConstraints.FreezeRotationX 
							| RigidbodyConstraints.FreezeRotationY 
							| RigidbodyConstraints.FreezeRotationZ 
							| RigidbodyConstraints.FreezePositionX 
							| RigidbodyConstraints.FreezePositionY 
							| RigidbodyConstraints.FreezePositionZ;
			}
	}

		/// <summary>
		/// Unfreeze the movement of the specified object if it has a rigid body attached.
		/// </summary>
		/// <param name="myObj">My object.</param>
		public static void UnFreezeMovement(GameObject myObj)
		{//Allow objects which can move in the 3d world to moving when they are released.
				Rigidbody rg = myObj.GetComponent<Rigidbody>();
				if (rg!=null)
				{
						rg.useGravity=true;
						rg.constraints = 
								RigidbodyConstraints.FreezeRotationX 
								| RigidbodyConstraints.FreezeRotationY 
								| RigidbodyConstraints.FreezeRotationZ;

				}
		}

		public MusicController getMus()
		{
			if (mus==null)	
			{
				mus=GameObject.Find("_MusicController").GetComponent<MusicController>();
			}
			return mus;
		}


		public TileMap currentTileMap()
		{
				if (LevelNo==-1)
				{
						return null;
				}
				else
				{
						return Tilemaps[LevelNo];				
				}
			
		}

		/// <summary>
		/// Detects where the player currently is an updates their swimming state and auto map as needed.
		/// </summary>
		public void PositionDetect()
		{
				if ((AtMainMenu==true) || (WindowDetect.InMap))
				{
						return;
				}
				TileMap.visitTileX =(int)(playerUW.transform.position.x/1.2f);
				TileMap.visitTileY =(int)(playerUW.transform.position.z/1.2f);
				currentTileMap().SetTileVisited(TileMap.visitTileX,TileMap.visitTileY);
				GameWorldController.instance.playerUW.isSwimming=((TileMap.OnWater) && (!GameWorldController.instance.playerUW.isWaterWalking)) ;

		}

		public ObjectLoader CurrentObjectList()
		{
				if (LevelNo==-1)
				{
						return null;
				}
				else
				{
						return objectList[LevelNo];
				}
		}

		/// <summary>
		/// Moves the object to the game world where it will be managed by the objectloader list
		/// </summary>
		/// <param name="obj">Object.</param>
		public static void MoveToWorld(GameObject obj)
		{
				//Debug.Log(obj.name + "is moved to world");
				MoveToWorld(obj.GetComponent<ObjectInteraction>());
		}

		public static void MoveToWorld(ObjectInteraction obj)
		{
			//Add item to a free slot on the item list and point the instance back to this.
				ObjectLoader.AssignObjectToList(ref obj);

				//Not needed???
		}

		/// <summary>
		/// Moves to inventory where it will no longer be managed by the objectloader list.
		/// </summary>
		/// <param name="obj">Object.</param>
		public static void MoveToInventory(GameObject obj)
		{
			MoveToInventory(obj.GetComponent<ObjectInteraction>());
		}

		public static void MoveToInventory(ObjectInteraction obj)
		{//Break the instance back to the object list
			obj.objectloaderinfo.InUseFlag=0;//This frees up the slot to be replaced with another item.
		}

		public void UpdatePositions()
		{
			foreach (Transform t in GameWorldController.instance.ObjectMarker.transform) 
			{
				if (t.gameObject.GetComponent<ObjectInteraction>()!=null)
				{
					t.gameObject.GetComponent<ObjectInteraction>().UpdatePosition();	
				}
			}
		}


		/// <summary>
		/// Writes a lev ark file based on the stored file
		/// </summary>
		public void WriteBackLevArk(int slotNo)
		{
				//ObjectLoader.UpdateObjectList();
				ObjectLoader.UpdateObjectList(GameWorldController.instance.currentTileMap(), GameWorldController.instance.CurrentObjectList());	
				//Write back tile states
				for (int l=0; l<=Tilemaps.GetUpperBound(0); l++)
				{
						if (GameWorldController.instance.Tilemaps[l] !=null)
						{
								for (int x=0; x<=TileMap.TileMapSizeX;x++)
								{
										for (int y=0; y<=TileMap.TileMapSizeY;y++)
										{		

											//	Debug.Log(l +  " " + x + " " + y);
												TileInfo t = Tilemaps[l].Tiles[x,y];

												long addptr = t.address;

												//Shift the bits to construct my data
												int tileType = t.tileType;
												int floorHeight = (t.floorHeight/2) << 4;


												int ByteToWrite = tileType | floorHeight ;//| floorTexture | noMagic;//This will be set in the original data
												lev_ark_file_data[addptr]= (char) ( lev_ark_file_data[addptr] | (char)(ByteToWrite) );

												int floorTexture = t.floorTexture<<2;
												int noMagic = t.noMagic << 6;

												ByteToWrite= floorTexture | noMagic;
												lev_ark_file_data[addptr+1]= (char) ( lev_ark_file_data[addptr+1] | (char)(ByteToWrite) );


												ByteToWrite = ((t.indexObjectList & 0x3FF) <<6) | (t.wallTexture & 0x3F);
												lev_ark_file_data[addptr+2]=(char)(ByteToWrite & 0xFF);
												lev_ark_file_data[addptr+3]=(char)((ByteToWrite>>8) & 0xFF);

												//int WallTexture = t.wallTexture;
												//int ObjectIndex = (t.indexObjectList & 0x3)<<6;//First write the first 2 bits at 6
												//ByteToWrite = WallTexture |  ObjectIndex;
												//lev_ark_file_data[addptr+2]= (char) ( lev_ark_file_data[addptr+2] | (char)(ByteToWrite) );

												//Now write the rest of the object index
												//ObjectIndex = t.indexObjectList >>2;
												//ByteToWrite= ObjectIndex;
												//lev_ark_file_data[addptr+3]= (char) ( lev_ark_file_data[addptr+3] | (char)(ByteToWrite) );


												//lev_ark_file_data[addptr+2]= lev_ark_file_data[addptr+2] | (char)secondByte;
												//ALl i think i will change is the floor texture, type, height and first object index
												/*0000 tile properties / flags:

										bits     len  description
										0- 3    4    tile type (0-9, see below)
										4- 7    4    floor height
										8       1    unknown (?? special light feature ??) always 0 in uw1
										9       1    0, never used in uw1
										10-13    4    floor texture index (into texture mapping)
										14       1    when set, no magic is allowed to cast/to be casted upon
										15       1    door bit (when 1, a door is present)

										0002 tile properties 2 / object list link

										bits     len  description
										0- 5    6    wall texture index (into texture mapping)
										6-15    10   first object in tile (index into master object list)*/
												
										}	
								}		
						}
					
				}

				//Write back object data
				for (int l=0; l<=objectList.GetUpperBound(0); l++)
				{
						if (objectList[l]!=null)
						{
								long addptr = objectList[l].objectsAddress;
								for (int o=0; o<=objectList[l].objInfo.GetUpperBound(0);o++)
								{
										ObjectLoaderInfo currobj= objectList[l].objInfo[o];
										if (currobj!=null)
										{
												int ByteToWrite= (currobj.is_quant << 15) |
																(currobj.invis << 14) |
																(currobj.doordir << 13) |
																(currobj.enchantment << 12) |
																((currobj.flags & 0x0F) << 9) |
																(currobj.item_id & 0x1FF) ;

												lev_ark_file_data[addptr]=(char)(ByteToWrite & 0xFF);
												lev_ark_file_data[addptr+1]=(char)((ByteToWrite>>8) & 0xFF);

												ByteToWrite = ((currobj.x & 0x7) << 13) |
																((currobj.y & 0x7) << 10) |
																((currobj.heading & 0x7) << 7) |
																((currobj.zpos & 0x7F));
												lev_ark_file_data[addptr+2]=(char)(ByteToWrite & 0xFF);
												lev_ark_file_data[addptr+3]=(char)((ByteToWrite>>8) & 0xFF);

												ByteToWrite = (((int)currobj.next & 0x3FF)<<6) |
																(currobj.quality & 0x3F); 
												lev_ark_file_data[addptr+4]=(char)(ByteToWrite & 0xFF);
												lev_ark_file_data[addptr+5]=(char)((ByteToWrite>>8) & 0xFF);		
												 
												//objList[x].owner = (int)(DataLoader.getValAtAddress(lev_ark,objectsAddress+address_pointer+6,16) & 0x3F) ;//bits 0-5
												//objList[x].link = (int)(DataLoader.getValAtAddress(lev_ark, objectsAddress + address_pointer + 6, 16) >> 6 & 0x3FF); //bits 6-15
												ByteToWrite = ((currobj.link & 0x3FF)<<6) |
														(currobj.owner & 0x3F); 
												lev_ark_file_data[addptr+6]=(char)(ByteToWrite & 0xFF);
												lev_ark_file_data[addptr+7]=(char)((ByteToWrite>>8) & 0xFF);	



												if (o<256)			
												{//Additional npc mobile data.
														
														lev_ark_file_data[addptr+0x8] = (char)(currobj.npc_hp & 0x8);
														lev_ark_file_data[addptr+0xb] = (char)( (currobj.npc_gtarg & 0xFF) <<4  |
																(currobj.npc_goal & 0xF));

														lev_ark_file_data[addptr+0xd]= (char)
																(
																((currobj.npc_attitude & 0x3)<<14) |
																((currobj.npc_talkedto & 0x1)<<13) |
																		currobj.npc_level & 0xF
																);

														lev_ark_file_data[addptr+0x16]= (char)(
																((currobj.npc_xhome & 0x3F)<<10) |
																((currobj.npc_yhome & 0x3F)<<4)
														);

														lev_ark_file_data[addptr+0x18]= (char)(
																((currobj.npc_heading & 0xF)<<4) 
														);

														lev_ark_file_data[addptr+0x19]= (char)(
																((currobj.npc_hunger & 0x3F)) 
														);

														lev_ark_file_data[addptr+0x1a]= (char)(
																((currobj.npc_whoami & 0xFF)) 
														);

													addptr=addptr+8+19;	
												}	
												else
												{													
													addptr=addptr+8;
												}
										}
								}
						}
				}

				//Write the array to file

				byte[] dataToWrite = new byte[lev_ark_file_data.GetUpperBound(0)+1];
				for (long i=0; i<=lev_ark_file_data.GetUpperBound(0);i++)
				{
						dataToWrite[i] = (byte)lev_ark_file_data[i];
				}
				File.WriteAllBytes(Loader.BasePath +  "save" + slotNo + "\\lev.ark" , dataToWrite);			
		}


		/// <summary>
		/// Inits the level data maps and textures.
		/// </summary>
		void InitLevelData()
		{
				switch (_RES)
				{
				case GAME_SHOCK:
						Tilemaps=new TileMap[15];
						objectList=new ObjectLoader[15];
						break;
				case GAME_UWDEMO:
						Tilemaps = new TileMap[1];
						objectList=new ObjectLoader[1];
						break;
				case GAME_UW2:
						Tilemaps = new TileMap[72];//Not all are in use.
						objectList=new ObjectLoader[72];
						break;
				case GAME_UW1:
				default:
						Tilemaps = new TileMap[9];
						objectList=new ObjectLoader[9];
						break;
				}



				switch (UWEBase._RES)
				{
				case UWEBase.GAME_SHOCK:
						MaterialMasterList= new Material[273];
						break;
				case UWEBase.GAME_UWDEMO:
						MaterialMasterList= new Material[58];
						break;
				case UWEBase.GAME_UW2:
						MaterialMasterList= new Material[256];//For each texture in UW2
						break;
				case UWEBase.GAME_UW1:						
				default:
						MaterialMasterList= new Material[260];//For each texture in UW1
						break;
				}

				//Load up my map materials
				for (int i =0; i<=MaterialMasterList.GetUpperBound(0);i++)
				{
					MaterialMasterList[i]=(Material)Resources.Load(_RES+"/Materials/textures/" + _RES + "_" + i.ToString("d3"));
					MaterialMasterList[i].mainTexture= texLoader.LoadImageAt(i);
				}
				if (_RES==GAME_UW1)
				{
					SpecialMaterials[0]=(Material)Resources.Load(_RES+"/Materials/textures/" + _RES + "_224_maze");
					SpecialMaterials[0].mainTexture=texLoader.LoadImageAt(224);
				}
				switch (_RES)
				{
					case GAME_SHOCK:
						break;

					default:
					//Load up my door texture
					for (int i =0; i<=MaterialDoors.GetUpperBound(0);i++)
						{

						MaterialDoors[i]= (Material)Resources.Load(_RES + "/Materials/doors/doors_" +i.ToString("d2") +"_material");	
						MaterialDoors[i].mainTexture = DoorArt.LoadImageAt(i);
						}
					break;

				}
	
				//Load up my tile maps
				//First read in my lev_ark file
				switch(UWEBase._RES)
				{
				case GAME_SHOCK:
						Lev_Ark_File = "res\\data\\archive.dat";
						break;	
				case UWEBase.GAME_UWDEMO:
						Lev_Ark_File = "Data\\level13.st";
						break;
				case UWEBase.GAME_UW2:
				case UWEBase.GAME_UW1:						
				default:
						Lev_Ark_File =  Lev_Ark_File_Selected; //"Data\\lev.ark";//Eventually this will be a save game.
						break;
				}

				if (!DataLoader.ReadStreamFile(Loader.BasePath + Lev_Ark_File, out lev_ark_file_data))
				{
						Debug.Log(Loader.BasePath + Lev_Ark_File + "File not loaded");
						Application.Quit();
				}		
		}

		public char[] tilemapfiledata()
		{
				return lev_ark_file_data;
		}


		/// <summary>
		/// Inits the B globals.
		/// </summary>
		/// <param name="SlotNo">Slot no.</param>
		public void InitBGlobals(int SlotNo)
		{
			char[] bglob_data;
			if (SlotNo==0)	
			{//Init from BABGLOBS.DAT. Initialise the data.
				if (DataLoader.ReadStreamFile(Loader.BasePath + "data\\BABGLOBS.DAT", out bglob_data))
				{
					int NoOfSlots = bglob_data.GetUpperBound(0)/4;
					int add_ptr=0;
					bGlobals = new bablGlobal[NoOfSlots+1];
					for (int i=0; i<=NoOfSlots;i++)
					{
						bGlobals[i].ConversationNo =(int)DataLoader.getValAtAddress(bglob_data,add_ptr,16);
						bGlobals[i].Size =(int)DataLoader.getValAtAddress(bglob_data,add_ptr+2,16);
						bGlobals[i].Globals = new int[bGlobals[i].Size];
						add_ptr = add_ptr+4;
					}
				}	
			}
			else
			{
				int NoOfSlots=0;//Assumes the same no of slots that is in the babglobs is in bglobals.
				if (DataLoader.ReadStreamFile(Loader.BasePath + "data\\BABGLOBS.DAT", out bglob_data))
				{
					NoOfSlots = bglob_data.GetUpperBound(0)/4;
					NoOfSlots++;
				}
				if (DataLoader.ReadStreamFile(Loader.BasePath + "Save" + SlotNo + "\\BGLOBALS.DAT", out bglob_data))
				{
					//int NoOfSlots = bglob_data.GetUpperBound(0)/4;
					int add_ptr=0;
					bGlobals = new bablGlobal[NoOfSlots];
					for (int i=0; i<NoOfSlots;i++)
					{
										
						bGlobals[i].ConversationNo =(int)DataLoader.getValAtAddress(bglob_data,add_ptr,16);
						bGlobals[i].Size =(int)DataLoader.getValAtAddress(bglob_data,add_ptr+2,16);
						bGlobals[i].Globals = new int[bGlobals[i].Size];
						add_ptr+=4;
						for (int g=0; g<bGlobals[i].Size; g++)
						{
							bGlobals[i].Globals[g]=	(int)DataLoader.getValAtAddress(bglob_data,add_ptr,16);							
							add_ptr = add_ptr+2;
						}						
					}
				}		
			}
		}

		public void WriteBGlobals(int SlotNo)
		{
			int fileSize=0;
			for (int c=0; c<=bGlobals.GetUpperBound(0);c++)
			{
				fileSize+=4;	//No and size
				fileSize+=bGlobals[c].Size * 2;				
			}
			//Create an output byte array
			Byte[] output = new byte[fileSize];
			int add_ptr=0;
			for (int c=0; c<=bGlobals.GetUpperBound(0);c++)
			{
				//Write Slot No
				output[add_ptr]=(byte) (bGlobals[c].ConversationNo & 0xff);
				output[add_ptr+1]=(byte)( (bGlobals[c].ConversationNo >>8) & 0xff);
				//Write Size
				output[add_ptr+2]=(byte)( bGlobals[c].Size & 0xff);
				output[add_ptr+3]=(byte) ((bGlobals[c].Size >>8) & 0xff);
				add_ptr=add_ptr+4;
				for (int g = 0; g<=bGlobals[c].Globals.GetUpperBound(0); g++)
				{
					output[add_ptr]=(byte)( bGlobals[c].Globals[g] & 0xff);
					output[add_ptr+1]=(byte) ((bGlobals[c].Globals[g] >>8) & 0xff);
					add_ptr+=2;
				}
			}
			File.WriteAllBytes(Loader.BasePath +  "save" + SlotNo + "\\BGLOBALS.DAT" , output);

		}

}