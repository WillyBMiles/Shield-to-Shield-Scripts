using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Motion_Spiral : ProjectileMotion
{
    public bool CounterClockwise = true;
    [Header("MUST use Lifetime, target determines ending point")]
    [Tooltip("Number of total revolutions around origin")]
    public Value numberOfRevolutions = new Value();
    [Tooltip("How far offset from forward (same handedness as CounterClockwise option above)")]
    public Value OffsetInDegrees = new Value();
    public bool OriginCharacter = false;
    public bool TargetCharacter = false;
    public Value height = new Value();
    public bool PointInMoveDirection = true;


    public override void UpdateThis(LocalProjectile projectile, CallInfo callInfo, bool dontMove = false)
    {
        float relativeLifetime = 1 - projectile.currentDuration / projectile.maxDuration;
        Vector3 target = TargetCharacter ? projectile.callInfo.target.transform.position : projectile.callInfo.targetPoint ?? projectile.callInfo.target.transform.position;
        target = new Vector3(target.x, 0f, target.z);
        Vector3 origin = OriginCharacter ? projectile.callInfo.origin.transform.position : projectile.callInfo.originPoint ?? projectile.callInfo.origin.transform.position;
        origin = new Vector3(origin.x, 0f, origin.z);
        float dist = Vector3.Distance(target, origin);
        float radius = relativeLifetime * dist;
        float revs = numberOfRevolutions.GetValue(callInfo);
        float radianOffset = OffsetInDegrees.GetValue(callInfo) * Mathf.PI / 180f;
        float angle = revs * 2 * Mathf.PI * relativeLifetime + radianOffset;

        float localX = Mathf.Cos(angle) * radius;
        float localY = Mathf.Sin(angle) * radius * (CounterClockwise ? 1 : -1);

        Vector3 dir = (target - origin).normalized;
        Vector3 dirright = Vector3.Cross(dir, Vector3.up);
        Vector3 final = origin + dir * localX + dirright * localY;
        final = new Vector3(final.x, height.GetValue(callInfo), final.z);
        if (dontMove)
            return;
        projectile.transform.position = final;
        if (PointInMoveDirection)
        {
            if (projectile.lastPositionMotion != projectile.transform.position && !float.IsInfinity(projectile.lastPositionMotion.x))
                projectile.transform.rotation = Quaternion.LookRotation(projectile.lastPositionMotion - projectile.transform.position);
            projectile.lastPositionMotion = projectile.transform.position;
        }
       
    }
}
