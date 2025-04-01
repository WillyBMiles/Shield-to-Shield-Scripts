using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;


public class Motion_Seek : ProjectileMotion
{
	public Value speed = new Value();

	[ShowIf("@type == Type.TargetPoint")]
	[Tooltip("Only works on nonmoving targets")]
	public bool InterpretSpeedAsTimeToReachTargetInstead = false;
	public Type type;
	[Tooltip("Stay at the same y height all the time")]
	public bool stayAtHeight = true;

	[ShowIf("@stayAtHeight && type == Type.TargetPoint")]
	public bool chooseEndHeight = false;

	[ShowIf(nameof(chooseEndHeight))]
	public float EndHeight = 1f;

	[ShowIf(nameof(stayAtHeight))]
	[Tooltip("Only used if stayAtHeight is true")]
	public float Height = 1f;

	[ShowIf("@stayAtHeight && !chooseEndHeight")]
	[Tooltip("Arc height per distance, to be at different heights, Height is startHeight, only used if MaxArcHeight = 0f")]
	public float ArcPerDistance = 0f;

	[ShowIf("@stayAtHeight && !chooseEndHeight")]
	[Tooltip("Max Arc Height, Height is startHeight, overwrites ArcPerDistance")]
	public float MaxArcHeight = 0f;

	[Tooltip("Rotate speed, in degrees per second")]
	public float RotateSpeed = 0f;

	[HideIf("@RotateSpeed == 0f")]
	public Axis RotateAxis;

	
	public bool PointInMoveDirection = false;
	
	public enum Axis
	{
		Right,
		Forward,
		Up
	}

	public enum Type
	{
		TargetDirection, TargetCharacter, TargetPoint
	}



	public override void UpdateThis(LocalProjectile projectile, CallInfo callInfo, bool dontMove = false)
	{
		if (projectile == null || projectile.transform == null)
			return;

        Vector3 nextPos = NextPosition(projectile, callInfo);

        if (PointInMoveDirection)
        {
            if (float.IsInfinity(projectile.lastPositionMotion.x) && nextPos != projectile.transform.position)
            {
                projectile.transform.rotation = Quaternion.LookRotation(nextPos - projectile.transform.position);
            }
        }
        if (dontMove && stayAtHeight)
        {
            projectile.transform.position = new Vector3(projectile.transform.position.x, nextPos.y, projectile.transform.position.z);
        }else if (!dontMove)
        {
            projectile.transform.position = nextPos;
        }

		if (PointInMoveDirection)
        {
			if (!float.IsInfinity(projectile.lastPositionMotion.x) && projectile.transform.position != projectile.lastPositionMotion)
			{
                projectile.transform.rotation = Quaternion.LookRotation(projectile.transform.position - projectile.lastPositionMotion);
            }
			
		}
			
		if (RotateSpeed != 0f)
		{
			Vector3 axis = Vector3.zero;
			switch (RotateAxis)
			{
				case Axis.Forward:
					axis = Vector3.forward;
					break;
				case Axis.Up:
					axis = Vector3.up;
					break;
				case Axis.Right:
					axis = Vector3.right;
					break;
			}
			
			projectile.transform.rotation = projectile.transform.rotation * Quaternion.AngleAxis(RotateSpeed * Time.deltaTime, axis);
		}
		projectile.lastPositionMotion = projectile.transform.position;

	}

    public Vector3 NextPosition(LocalProjectile projectile, CallInfo callInfo)
    {
        Vector3 target = projectile.transform.position;
        Vector3 output = projectile.transform.position;

        Vector3 targetPoint = projectile.callInfo.targetPoint ?? projectile.callInfo.target.transform.position;
        Vector3 originPoint = projectile.callInfo.originPoint ?? projectile.callInfo.origin.transform.position;
        if (projectile.speedMotion == null && type == Type.TargetPoint)
        {
            float s = speed.GetValue(callInfo);
            projectile.speedMotion = InterpretSpeedAsTimeToReachTargetInstead ? (targetPoint - originPoint).magnitude / s : s;
        }
        if (stayAtHeight)
        {
            output  = new Vector3(projectile.transform.position.x, 0f, projectile.transform.position.z);
        }

        switch (type)
        {
            case Type.TargetCharacter:
                if (projectile.callInfo.target != null)
                {
                    target = projectile.callInfo.target.transform.position;
                }
                output =
                    Vector3.MoveTowards(projectile.transform.position, target, 
                    speed.GetValue(callInfo) * Time.deltaTime);
                break;
            case Type.TargetDirection:
                if (targetPoint != originPoint)
                {
                    float distanceFromOrigin = projectile.distanceFromPoint ?
                        Vector3.Distance(originPoint, projectile.transform.position) :
                        Vector3.Distance(projectile.callInfo.origin.transform.position, projectile.transform.position);
                    float maxDistanceMove = projectile.maxDistance - distanceFromOrigin;
                    if (maxDistanceMove < 0f)
                        maxDistanceMove = 0f;
                    if (projectile.maxDistance < 0f)
                    {
                        maxDistanceMove = Mathf.Infinity;
                    }

                    Vector3 direction = (targetPoint - originPoint).normalized;
                    float finalSpeed = Time.deltaTime * speed.GetValue(callInfo);
                    finalSpeed = Mathf.Min(maxDistanceMove, finalSpeed);
                    output += direction * finalSpeed;
                    target = targetPoint;
                }

                break;
            case Type.TargetPoint:
                target = targetPoint;
                output = Vector3.MoveTowards(output, targetPoint,
                    projectile.speedMotion.Value * Time.deltaTime);
                break;
        }
        if (stayAtHeight)
        {
            Vector2 origin2 = new Vector2(originPoint.x, originPoint.z);
            Vector2 target2 = new Vector2(target.x, target.z);
            Vector2 current2 = new Vector2(output.x, output.z);
            float maxDistance = Vector2.Distance(origin2, target2);
            float currentDistance = Vector2.Distance(origin2, current2);
            if (chooseEndHeight)
            {
                float distProp = currentDistance / maxDistance;
                float h = Height + distProp * (EndHeight - Height);
                output = new Vector3(output.x, h, output.z);
            }
            else
            {

                float proportion = maxDistance != 0 ? currentDistance / maxDistance : 0f;
                float heightMult = Mathf.Max(0f, 4 * proportion - 4 * proportion * proportion);
                float ArcHeight;
                if (MaxArcHeight != 0f)
                    ArcHeight = MaxArcHeight;
                else
                    ArcHeight = ArcPerDistance * maxDistance;

                output = new Vector3(output.x, Height + ArcHeight * heightMult, output.z);
            }


        }

        return output;
    }



}
