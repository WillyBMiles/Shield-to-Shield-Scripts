using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Mirror;

#region CharacterClass
public static class CharacterClassWriter
{
    public static void WriteCharacterClass(this NetworkWriter writer, CharacterClass cc)
    {
        if (cc == null)
        {
            writer.Write(false);
            return;
        }
        writer.Write(true);

        writer.Write(cc.Name);

    }

    public static CharacterClass ReadCharacterClass(this NetworkReader reader)
    {
        if (!reader.ReadBool())
            return null;

        return CharacterClass.GetCharacterClassByName(reader.ReadString());
    }

}

#endregion


[CreateAssetMenu(fileName = "new Character Class", menuName = "Character Class")]
public class CharacterClass : SerializedScriptableObject
{
    public string Name = "";

    public Attribute attribute = new Attribute();
    public List<ClassAbilityWrapper> activeAbilities = new List<ClassAbilityWrapper>();

    public List<ClassAbilityWrapper> passiveAbilities = new List<ClassAbilityWrapper>();

    [TextArea(15, 20)]
    public string Description = "";

    public Sprite sprite;
    public Color color;

    public bool ShowTertiaryBar = false;
    [ShowIf(nameof(ShowTertiaryBar))]
    public Value barValue = new Value();

    [ShowIf(nameof(ShowTertiaryBar))]
    public Value maxBarValue = new Value();

    [ShowIf(nameof(ShowTertiaryBar))]
    public Color barFillColor = Color.white;

    static List<CharacterClass> classes;

    static List<Object> objs = new List<Object>();

    public bool UseForms = false;

    [ShowIf(nameof(UseForms))]
    [Header("Form 1")]
    public Dictionary<ClassAbilityWrapper, Control> Form1Actives = new Dictionary<ClassAbilityWrapper, Control>();
    [ShowIf(nameof(UseForms))]
    public List<ClassAbilityWrapper> Form1Passives = new List<ClassAbilityWrapper>();
    [ShowIf(nameof(UseForms))]
    [Header("Form2")]
    public Dictionary<ClassAbilityWrapper, Control> Form2Actives = new Dictionary<ClassAbilityWrapper, Control>();
    [ShowIf(nameof(UseForms))]
    public List<ClassAbilityWrapper> Form2Passives = new List<ClassAbilityWrapper>();


    private void OnEnable()
    {
        if (classes == null)
        {
            classes = new List<CharacterClass>();
        }
        classes.Add(this);
        InitializeClass();

    }

    public void InitializeClass()
    {
        if (activeAbilities != null)
        {
            foreach (var a in activeAbilities)
            {
                a.ability.Initialize();
            }
        }
        
        if (passiveAbilities != null)
        {
            foreach (var a in passiveAbilities)
            {
                a.ability.Initialize();
            }
        }     

        if (Form1Actives != null)
        {
            foreach (var pair in Form1Actives)
            {
                pair.Key.ability.Initialize();
            }
        }

        if (Form1Passives != null)
        {
            foreach (var a in Form1Passives)
            {
                a.ability.Initialize();
            }
        }
        
        
        if (Form2Actives != null)
        {
            foreach (var pair in Form2Actives)
            {
                pair.Key.ability.Initialize();
            }
        }
        
        if (Form2Passives != null)
        {
            foreach (var a in Form2Passives)
            {
                a.ability.Initialize();
            }
        }
        
    }

#if false
#if UNITY_EDITOR
    private void OnValidate()
    {
        try
        {
            objs.Clear();
            objs.AddRange(UnityEditor.PlayerSettings.GetPreloadedAssets());
            if (!objs.Contains(this))
            {
                objs.Add(this);
            }
            objs.Remove(null);

            UnityEditor.PlayerSettings.SetPreloadedAssets(objs.ToArray());
        }
        catch
        {
            //PASS
        }

    }

#endif
#endif

    public static CharacterClass GetCharacterClassByName(string Name)
    {
        CharacterClass c = null;
        foreach (CharacterClass cc in classes)
        {
            if (cc.Name == Name)
                c = cc;
        }
        return c;
    }
}

public class ClassAbilityWrapper {
    public int LevelEarned;
    public Ability ability;
}
