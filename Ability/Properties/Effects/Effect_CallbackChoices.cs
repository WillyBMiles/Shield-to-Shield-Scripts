using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect_CallbackChoices : Effect
{
    public class OptionWrapper
    {
        public string Text;
        public string CallbackID;
    }
    [Header("Shows continuously on activation.")]
    [Tooltip("Can use $0 etc. if you want")]
    [TextArea(15, 20)]
    public string TopText;
    public bool KeepTop;
    public List<OptionWrapper> options = new List<OptionWrapper>();

    static List<(string, Popup.PopupCallInfo, Popup.PopupInfo)> popupOptions = new List<(string, Popup.PopupCallInfo, Popup.PopupInfo)>();

    public override void LocalOnlyEffect(CallInfo callInfo)
    {
        base.LocalOnlyEffect(callInfo);
        if (!callInfo.caster.IsAuthoritative())
            return;
        if (popupOptions == null)
            popupOptions = new List<(string, Popup.PopupCallInfo, Popup.PopupInfo)>();
        popupOptions.Clear();

        foreach (var option in options)
        {
            Popup.PopupInfo info = new Popup.PopupInfo() {
                activation = callInfo.activation,
                ability = callInfo.ability,
                CallbackID = option.CallbackID,
                character = callInfo.caster,
                item = callInfo.item,
                InstanceID = callInfo.InstanceID,
            };
            popupOptions.Add((Ability.AddAbilityText(callInfo.caster, callInfo.ability, callInfo.item, option.Text, true), 
                CallbackWrapper, info));
        }

        Popup.ShowPopupInfo(Ability.AddAbilityText(callInfo.caster, callInfo.ability, callInfo.item, TopText, true), KeepTop,
            popupOptions.ToArray()); //generates a lot of garbage?
    }

    
    void CallbackWrapper(Popup.PopupInfo info)
    {
        CallInfo callInfo = new CallInfo
        {
            item = info.item,
            activation = info.activation,
            caster = info.character,
            ability = info.ability,
            CallbackID = info.CallbackID,
            InstanceID = info.InstanceID, //???
        };

        info.activation.OnCallback?.Invoke(callInfo);
    }

    public override bool HasLocalOnlyEffect()
    {
        return true;
    }

    public override bool HasServerEffect()
    {
        return false;
    }

    protected override bool AbstractHasLocalEffect()
    {
        return false;
    }

    public override bool CanHitCharacters()
    {
        return true;
    }

    public override bool CanHitPoints()
    {
        return true;
    }


}
