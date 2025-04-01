using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Motion_Oval : ProjectileMotion
{
    public bool CounterClockwise = false;
    public bool AlignToMoveAngle = false;

    [HideIf(nameof(UseDuration))]
    public bool DestroyAtEnd = false;
    public bool UseDuration = false;
    [HideIf(nameof(UseDuration))]
    public Value speed = new Value();

    public Value height = new Value();

    [Tooltip("Relative to origin to target position.")]
    public Value width = new Value() { baseMult = 1f, };

    public bool OriginCharacter = false;
    public bool TargetCharacter = false;

    public bool ReturnToOrigin = false;

    public override void UpdateThis(LocalProjectile projectile, CallInfo callInfo, bool dontMove = false)
    {

        Vector3 origin = projectile.callInfo.originPoint ?? projectile.callInfo.origin.transform.position;
        Vector3 actualOrigin = OriginCharacter ? projectile.callInfo.origin.transform.position : projectile.callInfo.originPoint ?? projectile.callInfo.origin.transform.position;
        origin = new Vector3(origin.x, 0f, origin.z);
        Vector3 target = TargetCharacter ? projectile.callInfo.target.transform.position : projectile.callInfo.targetPoint ?? projectile.callInfo.target.transform.position;
        target = new Vector3(target.x, 0f, target.z);

        float mainAxis = Vector3.Distance(origin, target) / 2f;
        float secondAxis = mainAxis * width.GetValue(callInfo);
        Vector3 forwardDirection = (origin - target).normalized;
        Vector3 rightDirection = Vector3.Cross(forwardDirection, Vector3.up);
        Vector3 center = (origin + target) / 2f;

        if (float.IsInfinity(projectile.lastPositionMotion.x))
        {
            projectile.lastPositionMotion = projectile.transform.position;
        }

        float duration = projectile.maxDuration;

        float spd = speed.GetValue(callInfo);
        if (!UseDuration)
        {
            float h = (mainAxis - secondAxis) * (mainAxis - secondAxis) / ((mainAxis + secondAxis) * (mainAxis + secondAxis));
            float length = Mathf.PI * (mainAxis + secondAxis) * (1 + 3 * h / (10 + Mathf.Sqrt(4 - 3 * h)));
            
            duration = length / spd;
        }
        float currentDuration = projectile.TimeCreated == 0f ? 0f : Time.time - projectile.TimeCreated;
        float relativeDuration = currentDuration / duration;
        float angle = relativeDuration * Mathf.PI * 2f;

        Vector3 location;
        if (ReturnToOrigin && relativeDuration > .75f)
        {
            if (!UseDuration)
            {
                float mult = ((Time.time - projectile.TimeCreated) - spd / 10f);
                location = Vector3.MoveTowards(projectile.transform.position, actualOrigin, spd * Time.deltaTime * (mult > 1f ? mult : 1f));
                if (DestroyAtEnd && Vector2.Distance(
                new Vector2(projectile.transform.position.x, projectile.transform.position.z),
                new Vector2(actualOrigin.x, actualOrigin.z)) < spd / 10f * 3f * Time.deltaTime * (mult > 1f ? mult : 1f))
                {
                    projectile.DestroyThis();
                }
            }
            else
            {

                location = Vector3.Lerp(projectile.transform.position, actualOrigin, (1 - relativeDuration) / .25f);
            }
        }
        else
        {
             location = center
                + (CounterClockwise ? 1f : -1f) * Mathf.Sin(angle) * secondAxis * rightDirection
                + mainAxis * Mathf.Cos(angle) * forwardDirection;
        }
        if (angle > 2 * Mathf.PI && DestroyAtEnd && !UseDuration && !ReturnToOrigin)
        {
            projectile.DestroyThis();
        }
        if (ReturnToOrigin && DestroyAtEnd && relativeDuration > .5f)
        {
            if (Vector2.Distance(
                new Vector2(projectile.transform.position.x, projectile.transform.position.z), 
                new Vector2(actualOrigin.x, actualOrigin.z)) < .1f )
            {
                projectile.DestroyThis();
            }
        }



        location = new Vector3(location.x, 
            height.GetValue(callInfo));

        if (AlignToMoveAngle && location != projectile.transform.position)
        {
            projectile.transform.rotation =
                Quaternion.LookRotation(projectile.transform.position - location, Vector3.up);
        }
        if (dontMove)
            return;
        projectile.transform.position = location;


        projectile.lastPositionMotion = projectile.transform.position;
    }
}
