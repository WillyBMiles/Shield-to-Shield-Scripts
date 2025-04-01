using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class CallInfoSync
{
    public static void WriteCallInfo(this NetworkWriter writer, CallInfo callInfo)
    {
        writer.Write(callInfo.InstanceID);

        writer.Write(callInfo.CallbackID);
        writer.Write(callInfo.control);

        writer.Write(callInfo.activation);
        writer.Write(callInfo.ability);
        writer.Write(callInfo.effect);
        writer.Write(callInfo.caster);
        writer.Write(callInfo.target);
        writer.Write(callInfo.targetPoint);
        writer.Write(callInfo.origin);
        writer.Write(callInfo.originPoint);

        writer.Write(callInfo.itemID);

    }
    public static CallInfo ReadCallInfo(this NetworkReader reader)
    {

        return new CallInfo()
        {
            InstanceID = reader.ReadInt(),

            CallbackID = reader.ReadString(),
            control = reader.ReadControl(),

            activation = reader.ReadActivation(),
            ability = reader.ReadAbility(),
            effect = reader.ReadEffect(),
            caster = reader.ReadCharacter(),
            target = reader.ReadCharacter(),
            targetPoint = reader.ReadVector3Nullable(),
            origin = reader.ReadCharacter(),
            originPoint = reader.ReadVector3Nullable(),

            itemID = reader.ReadString(),

        };
    }
} 

public struct CallInfo 
{
    public int InstanceID;

    public string CallbackID;
    public Control control;

    public Activation activation;
    public Ability ability;
    public Effect effect;
    public Character caster;
    public Character target;
    public Vector3? targetPoint;
    public Character origin;
    public Vector3? originPoint;

    public Vector3 actualOriginPoint { get
        {
            return originPoint ?? (origin != null ? origin.transform.position : new Vector3());
        } }
    public Vector3 actualTargetPoint{ get 
        {
            return targetPoint ?? (target != null ? target.transform.position : new Vector3());
        } }

    public string itemID;
    public Item item { get { return Item.GetItem(itemID); } set { itemID = value == null ? Item.GetID(value) : value.uniqueIdentifier; }  }


    public CallInfo Duplicate() //TODO: Delete because callinfos copy themselves on mutation?
    {
        return (CallInfo) this.MemberwiseClone();
    }

    /*
    public CallInfo()
    {
        InstanceID = -1;
        CallbackID = "";
        control = null;
        activation = null;
        ability = null;
        effect = null;
        caster = null;
        target = null;
        targetPoint = null;
        origin = null;
        originPoint = null;
        itemID = Item.GetID(null);
    }*/
}
