using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Motion_Stationary : ProjectileMotion
{
	[Tooltip("Stay at the same y height all the time")]
	public bool stayAtHeight = true;
	[ShowIf(nameof(stayAtHeight))]
	[Tooltip("Only used if stayAtHeight is true")]
	public float Height = 1f;

	[Tooltip("Must use in conjunction with duration in localprojectile")]
	[HideIf(nameof(stayAtHeight))]
	public bool ConstantHeightSpeed = false;

	[ShowIf("@stayAtHeight == false && ConstantHeightSpeed == true")]
	public float StartHeight;

	[ShowIf("@stayAtHeight == false && ConstantHeightSpeed == true")]
	public float EndHeight;

	public bool PointFromOriginToTarget = false;
	[ShowIf(nameof(PointFromOriginToTarget))]
	[Tooltip("Only works if target is a character.")]
	public bool TrackTarget = false;

	public bool SpawnAtTarget = false;

	[HideIf(nameof(PointFromOriginToTarget))]
	public bool HaveRandomRotation = false;

	public enum Type
	{
		TargetDirection, TargetCharacter, TargetPoint
	}

	public override void UpdateThis(LocalProjectile projectile, CallInfo callInfo, bool dontMove = false)
	{
		Vector3 point = SpawnAtTarget ? projectile.callInfo.targetPoint ?? projectile.transform.position : projectile.transform.position;


		if (stayAtHeight)
		{
			point = new Vector3(point.x, Height, point.z);
			
			
		}else if (ConstantHeightSpeed)
        {
			float height = Mathf.Lerp(EndHeight, StartHeight, projectile.currentDuration / projectile.maxDuration);
			point = new Vector3(projectile.transform.position.x, height, point.z);

		}
		projectile.transform.position = point;

		Vector3 targetPoint = TrackTarget ? projectile.callInfo.target.transform.position : projectile.callInfo.targetPoint ?? projectile.callInfo.target.transform.position;
		Vector3 originPoint = projectile.callInfo.originPoint.Value;

		if (PointFromOriginToTarget && Vector3.Distance(targetPoint, originPoint) > 0)
		{
			Vector3 point2 = new Vector3(targetPoint.x, 0f, targetPoint.z);
            if (!TrackTarget)
                point2 = new Vector3(projectile.targetStartPoint.x, 0f, projectile.targetStartPoint.z);

            Vector3 origin2 = new Vector3(originPoint.x, 0f, originPoint.z);

			projectile.transform.rotation = Quaternion.LookRotation(point2 - origin2, Vector3.up);
		}

		if (HaveRandomRotation && !PointFromOriginToTarget)
        {
			Quaternion rotation = Quaternion.Euler(0f, projectile.TimeCreated * 10000f, 0f);
			projectile.transform.rotation = rotation;
        }
	}
}
