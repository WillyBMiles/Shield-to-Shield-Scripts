using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;


public static class HitInfoSync
{
    public static void WriteHitInfo(this NetworkWriter writer, HitInfo hitInfo)
    {
        writer.Write(hitInfo.unalteredAmount);
        writer.Write(hitInfo.finalAmount);
        writer.Write(hitInfo.staggerAmount);

        writer.Write((int)hitInfo.type);
        writer.Write((int)hitInfo.subtypes);
        writer.Write(hitInfo.hitBlock);
        writer.Write(hitInfo.hitArmor);
        writer.Write(hitInfo.brokeArmor);

        writer.Write(hitInfo.deflected);
        writer.Write(hitInfo.originPoint);

        writer.Write(hitInfo.canStagger);

        writer.Write(hitInfo.triggersOnHit);

        writer.Write(hitInfo.casterID);
        writer.Write(hitInfo.hitID);
        writer.Write(hitInfo.triggeredThisID);

        writer.Write(hitInfo.InstanceID);
    }

    public static HitInfo ReadHitInfo(this NetworkReader reader)
    {
        HitInfo hi = new HitInfo
        {
            unalteredAmount = reader.ReadFloat(),
            finalAmount = reader.ReadFloat(),
            staggerAmount = reader.ReadFloat(),

            type = (Damage.Type)reader.ReadInt(),
            subtypes = (Damage.SubType)reader.ReadInt(),
            hitBlock = reader.ReadBool(),
            hitArmor = reader.ReadBool(),
            brokeArmor = reader.ReadBool(),

            deflected = reader.ReadBool(),
            originPoint = reader.ReadVector3(),

            canStagger = reader.ReadBool(),

            triggersOnHit = reader.ReadBool(),

            casterID = reader.ReadInt(),
            hitID = reader.ReadInt(),
            triggeredThisID = reader.ReadInt(),

            InstanceID = reader.ReadInt(),
        };
        return hi;
    }
}

public class HitInfo
{
    public float unalteredAmount;
    public float finalAmount;
    public float staggerAmount;

    public Damage.Type type;
    public Damage.SubType subtypes;

    public bool hitBlock;
    public bool hitArmor;
    public bool brokeArmor;
    public bool deflected;

    public bool triggersOnHit;

    public bool canStagger;

    public Vector3 originPoint;
    public int casterID;
    public int hitID;
    public int triggeredThisID;

    public bool DontHitStop;

    public int InstanceID;


    public HitInfo Duplicate()
    {
        HitInfo newHitInfo = (HitInfo) this.MemberwiseClone();
        return newHitInfo;
    }
}
