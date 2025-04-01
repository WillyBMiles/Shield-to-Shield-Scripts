using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Text.RegularExpressions;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

[CreateAssetMenu(fileName = "New Ability", menuName = "Ability/Ability")]
public class Ability : SerializedScriptableObject
{
    [Header("ID must be UNIQUE")]
    public string ID;

    [HideInInspector]
    public Skill mySkill;

    public enum Type
    {
        Default = 0,
        ShortAttack,
        LongAttack,
        ProjectileAttack,
        ShortStrike,
        LongStrike,
        ProjectileStrike,
        Maneuver,
        UnarmedStrike
    }

    [System.Flags]
    public enum Tag
    {
        None = 0,
        Boomerang = 1,
    }

    public Type type = Type.Default;
    public Tag tags = Tag.None;

    [Tooltip("Need not be unique.")]
    public string BeautyName;
    [Tooltip("Can be silenced. Use for magical abilities. Something can both be silencable and disarmable.")]
    public bool CanBeSilenced = false;

    [Tooltip("Can be disarmed. Use for active attack like abilities. Something can both be silencable and disarmable.")]
    public bool CanBeDisarmed = true;

    [Tooltip("Can't walk while winding up or down")]
    public bool CantMoveWhileCasting = false;

    [HideIf(nameof(CantMoveWhileCasting))]
    public Value CastingSpeedMult = new Value();

    [HideIf(nameof(CantMoveWhileCasting))]
    public Value CastFreezeTime = new Value() {  baseMult = 0f,};

    [HideIf(nameof(CantMoveWhileCasting))]
    public bool CastFreezeTimeDontScaleWithAbilityBonus = false;

    [Tooltip("Lock cursor for all activations, if any activation is casting")]
    public bool LockCursorForAll = false;

    [InlineEditor]
    [Tooltip("To copy an object: Update IDs, then copy the entire folder, change the Ability ID then Recalculate Subobjects and Update IDs")]
    public List<Activation> activations = new List<Activation>();

    [Tooltip("Only counts if this is a passive.")]
    public bool HideInPassives = false;

    [Tooltip("If true won't make the 'Cant Cast' sound when casting fails")]
    public bool DontMakeInvalidNoise = false;


    [Tooltip("Can't have more than 10 ability values")]
    public List<Value> Values = new List<Value>();

    [PreviewField(Alignment = ObjectFieldAlignment.Left, Height = 50)]
    [HideLabel]
    [HorizontalGroup("Icon and Description", 50)]
    public Sprite icon;

    [VerticalGroup("Icon and Description/Descrption")]
    [Tooltip("Use $1, $2 etc to mean 'show value X as a string here', ~1 means show value X as a NONEXPANDABLE string")]
    [TextArea(15, 20)]
    public string description;

    #region Editor
    List<string> targets = new List<string>();

    List<Property> properties = new List<Property>();
#if UNITY_EDITOR

    /*NO LONGER NECESSARY
    [Button]
    void AddActivation()
    {
        //string name = CreateScriptableObject<Activation, Activation>(this, ID,
        //    "Activation" + activations.Count, out Activation activation, activations);
        Activation activation = new Activation();
        activation.name = ID + "Activation" + activations.Count;
        activation.ID = name;
        activations.Add(activation);
        activation._ability = this;
        //Save();
    }
    */

    
    /* no longer needed
    [MyBox.ButtonMethod]
    void RecalculateSubobjects()
	{
        //attatches all Activations in immediate sub-folder
        //and recursively attatches subobjects from there
        //If the activations are stored elsewhere, this effectively clears the activations list
        activations.Clear();
        string grossPath = AssetDatabase.GetAssetPath(this);
        string path = grossPath.Substring(0, grossPath.LastIndexOf("/"));
        string[] subFolders = AssetDatabase.GetSubFolders(path);
        
        string[] guids = AssetDatabase.FindAssets("t:Activation", subFolders);
        foreach (string guid in guids) {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            Activation activation = AssetDatabase.LoadAssetAtPath<Activation>(assetPath);
            activations.Add(activation);
            activation.RecalculateSubobjects();
        }
        ReorderActivations();
        UpdateTargets();

	}
    */

    /*
    void ReorderActivations()
	{
        foreach (Activation activation in activations)
		{
            activation.effects.Sort(delegate (Effect x, Effect y)
            {
                return x.EffectID.Substring(x.EffectID.Length - 1).CompareTo(y.EffectID.Substring(y.EffectID.Length - 1));
            });
        }
        
	}*/

    [Button]
    void UpdateIDs()
	{
        //Updates IDs so that they match this Ability's IDs
        int i = 0;
        foreach (Activation activation in activations)
		{
            activation.ID = ID + "Activation" + i;
            activation.name = activation.ID;
            activation.UpdateIDs();
            i++;
		}
	}

    [Button]
    public virtual void UpdateTargets()
	{
        InitializeTargetIDs(targets);

        foreach (Activation activation in activations)
		{
            GatherTargetsFromActivation(activation);
		}
        
        foreach (Activation activation in activations)
		{
            UpdateTargetsForActivation(activation);
		}
	}

    public static void InitializeTargetIDs(List<string> targetIDs)
    {
        targetIDs.Clear();
        targetIDs.Add("CURSOR");
        targetIDs.Add("SELF");
        targetIDs.Add("HIT");
        targetIDs.Add("ORIGIN HIT"); 
        //IF YOU ADD ONE ALSO ADD TO ACTIVATION SIMPLE CALCULATION
    }

    protected void GatherTargetsFromActivation(Activation activation)
    {
        if (activation == null)
            return;
        activation._ability = this;
        foreach (Target target in activation.targets)
        {
            AddTargetID(targets, target);
        }
        foreach (Effect effect in activation.effects)
        {
            AddTargetID(targets, effect);
        }
    }

    public static void AddTargetID(List<string> targetIds, Property property)
    {
        if (property is Target target)
        {
            if (!targetIds.Contains(target.TargetID))
                targetIds.Add(target.TargetID);
        }

        if (property is Effect effect)
        {
            if (effect is Projectile p)
            {
                if (!targetIds.Contains(p.TargetOverride))
                {
                    targetIds.Add(p.TargetOverride);
                }
            }
            if (effect is Effect_Spawn spawn)
            {
                if (spawn.StoreCharacterTargetID != "" && !targetIds.Contains(spawn.StoreCharacterTargetID))
                    targetIds.Add(spawn.StoreCharacterTargetID);
            }
            if (effect is Effect_IndividualCallback call)
            {
                if (call.TriggeringTargetID != "" && !targetIds.Contains(call.TriggeringTargetID))
                    targetIds.Add(call.TriggeringTargetID);
            }
        }
    }

    protected void UpdateTargetsForActivation(Activation activation)
    {
        if (activation == null)
            return;
        properties.Clear();
        properties.AddRange(activation.effects);
        properties.AddRange(activation.limitations);
        properties.AddRange(activation.targets);
        foreach (Property p in properties)
        {
            p.UpdateTargets(targets);
        }
    }


    public static string CreateScriptableObject<T, U>(ScriptableObject source, string ID, string FolderName, out T t, List<T> ts = null) where T : ScriptableObject where U : T
    {

        T asset = ScriptableObject.CreateInstance<U>();

        string typeName = asset.GetType().Name;
        string name = ID + typeName;
        if (ts != null)
            name += ts.Count;
        string pathToAsset = AssetDatabase.GetAssetPath(source);
        pathToAsset = pathToAsset.Substring(0, pathToAsset.LastIndexOf("/"));
        string folderName = FolderName;
        if (!AssetDatabase.IsValidFolder(pathToAsset + "/" + folderName))
            AssetDatabase.CreateFolder(pathToAsset, folderName);
        AssetDatabase.CreateAsset(asset, pathToAsset + "/" + folderName + "/" + name + ".asset");
        AssetDatabase.SaveAssets();

        //EditorUtility.FocusProjectWindow();

        //Selection.activeObject = asset;

        t = asset;
        return name;
    }



    [Button]
    void Save()
    {
        EditorUtility.SetDirty(this);

        ClearActivations();
        foreach (Activation activation in activations)
        {
            EditorUtility.SetDirty(activation);
            /*
            foreach (Effect effect in activation.effects)
            {
                EditorUtility.SetDirty(effect);
            }
            foreach (Limitation limitation in activation.limitations)
            {
                EditorUtility.SetDirty(limitation);
            }
            foreach (Target target in activation.targets)
            {
                EditorUtility.SetDirty(target);
            }
            */

        }
        
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    [Button("Add Activation")]
    void AddActivation()
    {
        Activation activation = (Activation)ScriptableObject.CreateInstance("Activation");
        activation.ID = name + activations.Count;
        activation.name = activation.ID;
        activations.Add(activation);
        activation._ability = this;
        AssetDatabase.AddObjectToAsset(activation, this);
        ClearActivations();
    }

    [Button("Add Passive Activation")]
    void AddPassiveActivation()
    {
        Activation activation = (Activation)ScriptableObject.CreateInstance("Activation");
        activation.ID = name + activations.Count;
        activation.name = activation.ID;
        activations.Add(activation);
        activation._ability = this;
        activation.CastAnyTime = true;
        activation.CooldownID = "_";
        activation.SetCastingAbility = false;
        activation.SetPreparingAbility = false;
        activation.CooldownTiming = 0;
        activation.StaminaActivation = 0;
        activation.StaminaCost = new Value() { baseMult = 0f };
        activation.CastAutomatically = true;
        activation.CantBeInterrupted = true;
        activation.PointTowardsCursor = Timing.None;
        AssetDatabase.AddObjectToAsset(activation, this);
        ClearActivations();
    }

    [Button("Duplicate Last Activation")]
    void DuplicateLastActivation()
    {
        if (activations.Count == 0)
            return;

        Activation activation = DuplicateActivation(activations[activations.Count - 1], name + activations.Count);
        
        activations.Add(activation);
        ClearActivations();
    }

    protected Activation DuplicateActivation(Activation a, string newName)
    {
        Activation activation = SerializedScriptableObject.Instantiate(a);
        activation.ID = newName;
        activation.name = activation.ID;
        activation._ability = this;
        AssetDatabase.AddObjectToAsset(activation, this);
        return activation;
    }

    public Activation duplicateThis;

    [Button]
    void DuplicateGivenActivation()
    {
        if (duplicateThis == null)
            return;
        Activation activation = DuplicateActivation(duplicateThis, duplicateThis.ID + Universal.RandomString(5));

        activations.Add(activation);
        ClearActivations();
        duplicateThis = null;
    }


    [Button("Force Clean Up Activations")]
    protected void ClearActivations()
    {
        Object[] objects = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(this));
        foreach (Object o in objects)
        {
            if (o is Activation a)
            {
                if (ShouldDisposeOfActivation(a))
                {
                    AssetDatabase.RemoveObjectFromAsset(a);
                }
            }
        }

    }

    protected virtual bool ShouldDisposeOfActivation(Activation a)
    {
        return !activations.Contains(a);
    }

#endif
    #endregion

    static protected Dictionary<string, Ability> abilities = new Dictionary<string, Ability>();

    public virtual string Description(Character character, Item item, bool beauty)
	{
        return AddAbilityText(character, this, item, description, beauty);
	}

    public static string AddAbilityText(Character character, Ability ability, Item item, 
        string original, 
        bool beauty)
    {
        CallInfo callInfo = new CallInfo()
        {
            caster = character,
            ability = ability,
            item = item,
        };

        string[] strings = original.Split('$');
        string final = "";
        int i = 0;
        foreach (string s in strings)
        {
            if (i == 0)
            {
                final += s;
                i++;
                continue;
            }
            if (s.Length == 0)
                continue;
            if (int.TryParse(s.Substring(0, 1), out int index))
            {
                if (ability.Values.Count > index)
                {
                    if (beauty)
                        final += ability.Values[index].BeautyString(callInfo);
                    else
                        final += ability.Values[index].ValueString(callInfo);

                    final += s.Substring(1);
                }
                else
                    final += "$" + s;
            }
            else
                final += "$" + s;
        }

        strings = final.Split('~');
        string final2 = "";
        i = 0;
        foreach (string s2 in strings)
        {
            if (i == 0)
            {
                final2 = s2;
                i++;
                continue;
            }
            if (s2.Length == 0)
                continue;
            if (int.TryParse(s2.Substring(0, 1), out int index))
            {
                if (ability.Values.Count > index)
                {
                    final2 += ability.Values[index].BeautyString(callInfo);
                    final2 += s2.Substring(1);
                }
                else
                    final2 += "~" + s2;
            }
            else
                final2 += "~" + s2;
        }


        strings = Regex.Split(final2, @"###");
        if (strings.Length != 2)
            return final2;
        return beauty ? strings[0] : strings[0] + strings[1];
    }

    public virtual void Initialize()
	{
        if (!abilities.ContainsKey(ID))
		{
            abilities.Add(ID, this);

            foreach (var activation in activations)
                activation.Initialize(this);
        }
    }

	public virtual void UpdateThis(CallInfo callInfo, bool isAuthoritative)
	{
        if (callInfo.caster == null)
            return;
        callInfo.ability = this;
        foreach (Activation activation in activations)
		{
            CallInfo newInfo = callInfo.Duplicate();
            newInfo.activation = activation;

            activation.UpdateThis(newInfo, isAuthoritative);
		}

	}

    public static Ability GetAbility(string ID)
	{
        if (ID == null)
            return null;
        if (!abilities.ContainsKey(ID))
            return null;
        return abilities[ID];
	}

    public virtual float GetLowestCooldown(CallInfo callInfo, bool relative)
	{
        float cooldown = -1f;
        foreach (Activation activation in activations)
		{
            CallInfo newInfo = callInfo.Duplicate();
            newInfo.activation = activation;

            float cd = activation.CheckCooldown(newInfo, true, relative);
            if (cd == -1f)
                continue;

            if (cooldown == -1f)
                cooldown = cd;
            else
                cooldown = Mathf.Max(cd, cooldown);

		}
        if (cooldown == -1f)
            return 0f;
        return cooldown;
	}


    public const string OUTLINETIMER = "_OutlineTimer";
    public const string MAXOUTLINETIMER = "_MaxOutlineTimer";

    public const string SHOWCORNERNUMBER = "_ShowNumber"; //anything but 0 is show
    public const string CORNERNUMBER = "_CornerNumber";
    public float CheckOutlineTimer(Character character, string itemID)
    {
        if (character == null)
            return 0f;

        return character.CheckStoredFloat(this, itemID, OUTLINETIMER, 0f, false, false) / 
            character.CheckStoredFloat(this, itemID, MAXOUTLINETIMER, 1f, false, false);
    }

    public bool OverrideCanPrepare(CallInfo callInfo, out Sprite sprite)
    {
        sprite = null;
        foreach (Activation activation in activations)
        {
            CallInfo newInfo = callInfo.Duplicate();
            newInfo.activation = activation;

            if (activation.ShowCanCastIfCanPrepare && activation.CanPrepare(newInfo, false))
            {
                sprite = activation.ReplaceIcon;
                return true;
            }
        }
        return false;
    }
    
   
    public static bool IsPassive(Control control)
    {
        if (control is KeyboardControl)
            return false;
        if (control is MouseControl)
            return false;
        return true;
    }
    public bool CanPrepareAnotherActivation(Character character, Item item)
    {
        foreach (Activation activation in activations)
        {
            CallInfo callInfo = new CallInfo()
            {
                caster = character,
                item = item,
                ability = this,
                activation = activation,
            };
            if (activation.SetPreparingAbility && activation.CanPrepare(callInfo))
                return true;
        }
        return false;
    }

    public virtual void TargetIni(Dictionary<(Ability, Activation, string), Vector3> CursorPoints, string itemID)
    {
        foreach (Activation activation in activations)
        {
            AddActivationToCursorPoints(CursorPoints, activation, itemID);
        }
    }
    protected void AddActivationToCursorPoints(Dictionary<(Ability, Activation, string), Vector3> CursorPoints, 
        Activation activation, string itemID)
    {
        if (!CursorPoints.ContainsKey((this, activation, itemID)))
            CursorPoints.Add((this, activation, itemID), new Vector3());
    }

    public Color GetMyColor()
    {
        if (mySkill == null)
            return Color.white;
        return mySkill.color;
    }

    public bool UsesStamina()
    {
        foreach (Activation a in activations)
        {
            if (a.ShouldUseStamina())
                return true;
        }
        return false;
    }
   
}
