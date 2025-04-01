using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalShield : MonoBehaviour
{
    [Header("Attach to a Projectile!!")]
    Character caster;
    [Tooltip("Who do we stop?")]
    public TargetMask mask = TargetMask.MyEnemies; // enemies from this
    public bool followCharacter;

    LocalProjectile proj;
    Character c;

    static List<LocalShield> shields = new List<LocalShield>();
    
    /// <summary>
    /// If obj contains a LocalShield, then return it. Otherwise return null.
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static LocalShield GetShield(GameObject obj)
    {
        foreach (LocalShield ls in shields)
        {
            if (ls.gameObject == obj)
                return ls;
        }
        return null;
    }

    // Start is called before the first frame update
    void Start()
    {
        proj = GetComponentInParent<LocalProjectile>();
        if (proj == null)
        {
            c = GetComponentInParent<Character>();
            caster = c;
        }
        else
        {
            caster = proj.callInfo.caster;
        }
           
        shields.Add(this);
        
    }
    private void Update()
    {
        
    }

    public bool ShouldProjectileStop(Character character)
    {
        if (caster == null)
            return false;
        return caster.FitsMask(character, mask);

    }

    private void OnDestroy()
    {
        shields.Remove(this);
    }

}
