using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class LocalProjectile : MonoBehaviour
{
    public LayerMask layerMask;
    public LayerMask stopLayerMask;
    public TargetMask targetMask;
    public string TargetOverride;

    public CallInfo callInfo;

    public ProjectileMotion motion;
    public GameObject appearance;
    public float width;
    public int numberOfCollisions;
    public float maxDistance;
    public float maxDuration;
    public float noHitTime;

    public float finalWidth;
    public float startingWidth;

    public Vector3 targetStartPoint;

    public float currentDuration;

    public bool distanceFromPoint;
    public bool canMultiHit;
    public float MultiHitTickRate;
    public bool canOnlyHitTarget;
    public string effectID;
    public string callbackID;
    public bool destroyUponReachingTargetPoint;
    public bool destroyUponReachingTargetCharacter;
    public bool StoppedByShields;
    public bool canTargetUntargettable;

    public bool CantHitOriginCharacter;

    public string RepeatTargetID;
    public bool InitializeRepeatTarget;

    public bool CanHitTargetsBehind;

    public bool active;

    public Character lastTarget;


    Dictionary<GameObject, float> hitObjects = new Dictionary<GameObject, float>();
    public Vector3 lastPositionMotion { get; set; } //controlled by motion

    public float TimeCreated;
    public Projectile effect;

    public float? speedMotion { get; set; }

    private void Initialize()
    {
        hitObjects.Clear();
        TimeCreated = Time.time;
        lastPos = transform.position;
        lastPositionMotion = Vector3.negativeInfinity;
        if (gos == null)
            gos = new List<GameObject>();
        gos.Clear();
        if (!canMultiHit && RepeatTargetID != null && RepeatTargetID.Length != 0 && InitializeRepeatTarget)
            callInfo.caster.characterAbilities.ResetRepeatHits(callInfo.ability, callInfo.item, RepeatTargetID);
    }

    List<GameObject> gos;
    ItemModel im;
    float maxSpeed = 0f;
	// Update is called once per frame
	void Update()
    {
        if (!active)
            return;
        if (callInfo.caster == null)
            DestroyThis();
        if (noHitTime > 0f)
            noHitTime -= Time.deltaTime;

        if (effect.ChangeWidthOverLifetime)
        {
            if (maxDuration > 0)
            {
                if (effect.PeakHalfway)
                {
                    if (currentDuration < maxDuration / 2f)
                        width = Mathf.Lerp(finalWidth, startingWidth,1f - 2f* currentDuration / (maxDuration));
                    else if (currentDuration > maxDuration / 2f)
                    {
                        width = Mathf.Lerp(startingWidth, finalWidth, 2f - 2f * currentDuration  / (maxDuration));
                    }
                } 
                else
                    width = Mathf.Lerp(startingWidth, finalWidth, 1 - currentDuration / maxDuration);
                transform.localScale = new Vector3(width, width, width);
            }
                
            
        }
        if (canOnlyHitTarget && callInfo.target == null)
		{
            DestroyThis();
            return;
        }

        if (effect.InheritItemColor && appearance != null && im == null)
        {
            im = appearance.GetComponent<ItemModel>();
            im.myItem = callInfo.item;
        }

        if (canMultiHit)
        {
            gos.Clear();
            gos.AddRange(hitObjects.Keys);
            foreach (GameObject go in gos)
            {
                if (Time.time - hitObjects[go] >= MultiHitTickRate)
                {
                    hitObjects.Remove(go);
                }
            }

        }

        Vector3 moveDirection = (transform.position - lastPos).normalized;
        if (Vector3.zero != moveDirection)
            transform.rotation = Quaternion.LookRotation(moveDirection, Vector3.up);

        motion.UpdateThis(this, callInfo);
        float speed = Vector2.Distance(new Vector2(lastPos.x,lastPos.z), new Vector2(transform.position.x,transform.position.z)) / Time.deltaTime;
        if (speed > maxSpeed)
        {
            maxSpeed = speed;
        }
        CheckDistance();
        CheckDuration();
        if (noHitTime <= 0f)
            CheckCollision();
        lastPos = transform.position;

        callInfo.caster.characterAbilities.AddTarget(callInfo.ability, callInfo.itemID, TargetOverride, true, transform.position);
    }

    public void CheckDistance()
	{
        if (destroyUponReachingTargetPoint)
        {
           
            if (callInfo.targetPoint.HasValue && Vector2.Distance(new Vector2(transform.position.x, transform.position.z), 
                new Vector2(callInfo.targetPoint.Value.x, callInfo.targetPoint.Value.z)) < maxSpeed * Time.deltaTime * 1.1f)
            {
                //transform.position = new Vector3(point.x, transform.position.y, point.z);
                DestroyThis(new Vector3(callInfo.targetPoint.Value.x, transform.position.y, callInfo.targetPoint.Value.z));
            }
                
        }
        if (destroyUponReachingTargetCharacter)
		{
            if (callInfo.target != null)
            {
                
                if (Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(callInfo.target.transform.position.x, callInfo.target.transform.position.z)) < maxSpeed * Time.deltaTime * 1.1f)
                {
                    //transform.position = new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z);
                    DestroyThis(new Vector3(callInfo.target.transform.position.x, transform.position.y, callInfo.target.transform.position.z));
                }  
            }
                
        }

        if (maxDistance <= 0)
            return;
        
        if (distanceFromPoint && callInfo.originPoint.HasValue)
		{
            if (Vector3.Distance(callInfo.originPoint.Value, transform.position) >= maxDistance-.1f)
            {
                //transform.position = CharacterAbilities.BoundVector3InRange(transform.position, 0f, maxDistance, originPoint);
                DestroyThis(CharacterAbilities.BoundVector3InRange(transform.position, 0f, maxDistance, callInfo.originPoint.Value));
            }
                
		}
        else
		{
            if (Vector3.Distance(callInfo.origin.transform.position, transform.position) >= maxDistance-.1f)
            {
                //transform.position = CharacterAbilities.BoundVector3InRange(transform.position, 0f, maxDistance, origin.transform.position);
                DestroyThis(CharacterAbilities.BoundVector3InRange(transform.position, 0f, maxDistance, callInfo.origin.transform.position));
            }
                
        }
	}

    public void CheckDuration()
	{
        currentDuration -= effect.InheritCasterTime ? Time.deltaTime * callInfo.caster.LocalTimeMult : Time.deltaTime;
        if (currentDuration <= 0)
            DestroyThis();
    }

	#region Collisions
	Vector3 lastPos;

    void CheckCollision()
	{
        if (Vector3.Distance(transform.position, lastPos) < .01f && CanHitTargetsBehind)
		{
            if (Physics.OverlapSphere(transform.position, width / 2f, stopLayerMask).Length > 0)
			{
                DestroyThis();
                return;
			}

            Collider[] colliders;
            if (StoppedByShields)
            {
                colliders = Physics.OverlapSphere(transform.position, width / 2f, LayerMask.GetMask("Shields"));
                foreach (Collider collider in colliders)
                {
                    if (HitShield(collider.gameObject))
                    {
                        DestroyThis();
                        return;
                    }
                }
            }

            colliders = Physics.OverlapSphere(transform.position, width / 2f, layerMask);
            foreach (Collider collider in colliders)
                HitObj(collider.gameObject);
		}
        else
		{
            RaycastHit[] wallHits = Physics.SphereCastAll(lastPos, width / 2f, (transform.position - lastPos).normalized, Vector3.Distance(transform.position, lastPos), stopLayerMask);
            if (wallHits.Length > 0)
			{
                DestroyThis();
                return;
			}
            
            if (StoppedByShields)
            {
                RaycastHit[] shieldhits = Physics.SphereCastAll(lastPos, width / 2f, (transform.position - lastPos).normalized, Vector3.Distance(transform.position, lastPos), LayerMask.GetMask("Shields"));
                foreach (RaycastHit rh in shieldhits)
                {
                    if (HitShield(rh.collider.gameObject))
                    {
                        DestroyThis();
                        return;
                    }
                }
            }
            

            RaycastHit[] hits = Physics.SphereCastAll(lastPos, width / 2f, (transform.position - lastPos).normalized, Vector3.Distance(transform.position, lastPos), layerMask);
            foreach (RaycastHit hit in hits)
                HitObj(hit.collider.gameObject);
        }
        
        
	}

    bool HitShield(GameObject objectHit)
    {
        LocalShield ls  = LocalShield.GetShield(objectHit);
        if (ls == null)
            return false;
        return ls.ShouldProjectileStop(callInfo.caster);
    }

    void HitObj(GameObject objectHit)
	{
        if (canOnlyHitTarget && objectHit != callInfo.target.gameObject)
            return;
        if (numberOfCollisions == 0)
            return;


        if (!CanHitTargetsBehind)
		{
            Vector2 my2 = new Vector2(transform.position.x, transform.position.z);
            Vector2 origin2 = new Vector2(0, 0);
            if (callInfo.originPoint.HasValue)
            {
                origin2 = new Vector2(callInfo.originPoint.Value.x, callInfo.originPoint.Value.z);
            }
            Vector2 target2 = new Vector2(objectHit.transform.position.x, objectHit.transform.position.z);

            if (Vector2.Angle(my2 - origin2, target2 - origin2) > 90)
			{
                return;
			}


        }

        if (!hitObjects.ContainsKey(objectHit))
        {
            Character hit = Character.GetCharacter(objectHit);

            if (hit == null)
                return;

            bool isUntargettable = hit.HasStatus(Status.Type.Untargettability);
            if (canTargetUntargettable || !isUntargettable)
			{
                 HitCharacter(hit, objectHit);
			}
           
        }
    }

    Character[] noCharacters = new Character[0];
    void HitCharacter(Character characterHit, GameObject objectHit)
	{
        hitObjects.Add(objectHit, Time.time);

        if (!canMultiHit && RepeatTargetID != null && RepeatTargetID.Length != 0 
            && !callInfo.caster.characterAbilities.CheckRepeatHits(callInfo.ability, callInfo.item, RepeatTargetID, characterHit))
            return;
        if (!canMultiHit && RepeatTargetID != null && RepeatTargetID.Length != 0)
        {
            callInfo.caster.characterAbilities.AddRepeatHits(callInfo.ability, callInfo.item, RepeatTargetID, characterHit);
        }
        if (callInfo.origin != null && CantHitOriginCharacter && characterHit == callInfo.origin)
            return;

        if (!callInfo.caster.FitsMask(characterHit, targetMask))
            return;
        if (characterHit != null)
            callInfo.caster.characterAbilities.AddTarget(callInfo.ability, callInfo.itemID, TargetOverride,true, characterHit);
        else
		{
            callInfo.caster.characterAbilities.AddTarget(callInfo.ability, callInfo.itemID, TargetOverride, true, noCharacters);
		}

        lastTarget = characterHit;

        CallInfo newInfo = callInfo.Duplicate();
        newInfo.target = characterHit;
        newInfo.targetPoint = transform.position;

        newInfo.activation.OnHit?.Invoke(newInfo);
        numberOfCollisions--;
        if (numberOfCollisions == 0)
		{


            DestroyThis(hitTransform: objectHit.transform);
        }

    }

    public Vector2? IntersectionPoint(Vector2 line1Point1, Vector2 line1Point2, Vector2 line2Point1, Vector2 line2Point2)
    {
        (float, float, float) sf1 = StandardForm(line1Point1, line1Point2);
        (float, float, float) sf2 = StandardForm(line2Point1, line2Point2);

        float delta = sf1.Item1 *sf2.Item2 - sf1.Item2 * sf2.Item1;
        if (delta == 0f)
            return null;

        float x = (sf2.Item2 * sf1.Item3 - sf1.Item2 * sf2.Item3) / delta;
        float y = (sf1.Item1 * sf2.Item3 - sf2.Item1 * sf1.Item3) / delta;
        return new Vector2(x, y);

    }
    /// <summary>
    /// Returns Ax + By = C
    /// A,B,C
    /// </summary>
    /// <param name="point1"></param>
    /// <param name="point2"></param>
    /// <returns>(A,B,C)</returns>
    public (float,float,float) StandardForm(Vector2 point1, Vector2 point2)
    {
        float num = point1.y - point2.y;
        float den = point1.x - point2.x;
        if (den == 0f)
        {
            return (1f,0f,point1.x);
        }
        float slope = num / den;
        //y - point1.y = (slope) (x - point1.x)
        //y = point1.y + slope x - slope * point1.x
        //-slope x + y = point1.y - slope * point1.x
        return (-slope, 1f, point1.y - slope * point1.x);
    }

    #endregion
    private void OnDestroy()
    {
        Static.I.projectiles.Remove(this);
        
    }
    
    public void DestroyThis(Vector3? overridePoint = null, Transform hitTransform = null)
    {
        if (hitTransform != null)
        {
            Vector3 finalPos3 = transform.position;
            //transform.position = objectHit.transform.position; //??
            if (lastPos != transform.position)
            {
                Vector2 finalPosition = new Vector2(transform.position.x, transform.position.z);
                Vector2 moveDirection = new Vector2((-transform.position + lastPos).x, (-transform.position + lastPos).z);

                Vector2 perpDirection = new Vector2(-moveDirection.y, moveDirection.x);
                Vector2 objPosition = new Vector2(hitTransform.transform.position.x, hitTransform.transform.position.z);

                Vector2? inter = IntersectionPoint(finalPosition, finalPosition + moveDirection, objPosition, objPosition + perpDirection);
                if (inter.HasValue)
                {
                    float distance = Vector2.Distance(finalPosition, inter.Value);
                    if (distance < Vector3.Distance(lastPos, transform.position))
                    {
                        finalPos3 = new Vector3(inter.Value.x, finalPos3.y, inter.Value.y);
                        transform.position = finalPos3;
                    }
                }
            }
        }

        CallInfo newInfo = callInfo.Duplicate();
        newInfo.target = lastTarget;
        newInfo.targetPoint = transform.position;

        callInfo.caster.characterAbilities.AddTarget(callInfo.ability, callInfo.itemID, TargetOverride, true, overridePoint.HasValue ? overridePoint.Value : transform.position);
        callInfo.activation.OnDestroy?.Invoke(newInfo);
        Destroy(gameObject);
    }


    void AddAppearance(GameObject appearance)
    {
        if (appearance == null)
            return;
        GameObject newAppearance = Instantiate(appearance, this.transform);
        newAppearance.name = appearance.name;

        this.appearance = newAppearance;
        newAppearance.transform.SetParent(transform);
        newAppearance.transform.localScale = appearance.transform.localScale;
        newAppearance.transform.localRotation = Quaternion.identity;
        newAppearance.transform.localPosition = new Vector3();
        newAppearance.SetActive(true);
    }

    void EnableThis(LayerMask layerMask, string TargetOverride, CallInfo callInfo, ProjectileMotion motion,
        float width, GameObject appearance, int numberOfCollisions, bool CanMultiHit, float maxDistance, float maxDuration, bool distanceFromPoint,
        bool canOnlyHitTarget, string effectID, string callbackID, bool destroyUponReachingTargetPoint, bool destroyUponReachingTargetCharacter,
        TargetMask targetMask, LayerMask stopLayerMask, bool CanHitTargetsBehind, bool StoppedByShields, float MultiHitTickRate, string RepeatTargetID, bool InitializeRepeatTarget,
        bool CantHitOriginCharacter, bool canTargetUntargettable, Projectile effect)
    {
        if (effect.InheritItemModel && callInfo.item != null)
        {
            if (callInfo.item != null && callInfo.item.myItemGenerator != null)
            {
                appearance = callInfo.item.myItemGenerator.replaceProjectileModel == null ? callInfo.item.model : callInfo.item.myItemGenerator.replaceProjectileModel;
            }

        }
        AddAppearance(appearance);
        if (callInfo.originPoint.HasValue)
        {
            this.transform.position = callInfo.originPoint.Value;
        }
        this.callInfo = callInfo.Duplicate();
        this.callInfo.CallbackID = callbackID; 
        
        this.transform.localScale = new Vector3(width, width, width);
        this.stopLayerMask = stopLayerMask;
        this.width = width;
        this.motion = motion;
        this.maxDistance = maxDistance;
        this.maxDuration = (maxDuration < 0 || maxDuration > 100) ? 100 : maxDuration;
        this.currentDuration = this.maxDuration;
        this.distanceFromPoint = distanceFromPoint;
        this.numberOfCollisions = numberOfCollisions;
        this.canMultiHit = CanMultiHit;
        this.MultiHitTickRate = MultiHitTickRate;
        this.layerMask = layerMask;
        this.canOnlyHitTarget = canOnlyHitTarget;
        this.TargetOverride = TargetOverride;
        this.effectID = effectID;
        this.callbackID = callbackID;
        this.destroyUponReachingTargetPoint = destroyUponReachingTargetPoint;
        this.destroyUponReachingTargetCharacter = destroyUponReachingTargetCharacter;
        this.targetMask = targetMask;
        this.CanHitTargetsBehind = CanHitTargetsBehind;
        this.StoppedByShields = StoppedByShields;
        this.RepeatTargetID = RepeatTargetID;
        this.InitializeRepeatTarget = InitializeRepeatTarget;
        this.CantHitOriginCharacter = CantHitOriginCharacter;
        this.canTargetUntargettable = canTargetUntargettable;
        //this.motion.UpdateThis(this, callInfo, true);
        this.effect = effect;
        this.speedMotion = null;
        this.lastPositionMotion = transform.position;
        this.noHitTime = (effect.noHitTime == null || effect.noHitTime.NaiveIsZero()) ? 0f : effect.noHitTime.GetValue(callInfo);

        if (effect.ChangeWidthOverLifetime)
            this.finalWidth = effect.endWidthValue.GetValue(callInfo);
        else
            this.finalWidth = this.width;
        this.startingWidth = this.width;
        this.targetStartPoint = callInfo.target == null ? new Vector3() : callInfo.target.transform.position;

        
        active = true;

        Static.I.projectiles.Add(this);

        this.Initialize();
    }

	#region Static References


    static GameObject prefab;
    public static void CreateProjectile(LayerMask layerMask, string TargetOverride, CallInfo callInfo, ProjectileMotion motion,
        float width, GameObject appearance, int numberOfCollisions, bool CanMultiHit, float maxDistance, float maxDuration, bool distanceFromPoint,
        bool canOnlyHitTarget, string effectID, string callbackID, bool destroyUponReachingTargetPoint, bool destroyUponReachingTargetCharacter,
        TargetMask targetMask, LayerMask stopLayerMask, bool CanHitTargetsBehind, bool StoppedByShields, float MultiHitTickRate, string RepeatTargetID, bool InitializeRepeatTarget,
        bool CantHitOriginCharacter, bool canTargetUntargettable, Projectile effect)
    {
        if (!callInfo.originPoint.HasValue)
            return;
        LocalProjectile local;
        if (prefab == null)
        {
            prefab = (GameObject)Resources.Load("ProjectilePrefab");
        }

        GameObject newObj = Instantiate(prefab, callInfo.originPoint.Value, Quaternion.identity);
        local = newObj.GetComponent<LocalProjectile>();

        local.EnableThis( layerMask, TargetOverride, callInfo, motion,
            width, appearance, numberOfCollisions, CanMultiHit, maxDistance, maxDuration, distanceFromPoint, canOnlyHitTarget, effectID, callbackID,
            destroyUponReachingTargetPoint, destroyUponReachingTargetCharacter, targetMask, stopLayerMask, CanHitTargetsBehind, StoppedByShields, MultiHitTickRate,
            RepeatTargetID, InitializeRepeatTarget, CantHitOriginCharacter, canTargetUntargettable, effect);
    }

    static List<LocalProjectile> tempProjectiles = new List<LocalProjectile>();
    public static void DestroyProjectile(Ability ability, Character character, string EffectID)
	{
        tempProjectiles.Clear();
        tempProjectiles.AddRange(Static.I.projectiles);
        foreach (LocalProjectile projectile in tempProjectiles)
		{
            if (projectile.callInfo.ability == ability && projectile.callInfo.caster == character && projectile.effectID == EffectID)
			{
                projectile.DestroyThis();
			}
		}
	}

    public static void DestroyProjectile(Character character)
    {
        tempProjectiles.Clear();
        tempProjectiles.AddRange(Static.I.projectiles);
        foreach (LocalProjectile projectile in tempProjectiles)
        {
            if (projectile.callInfo.caster == character)
            {
                projectile.DestroyThis();
            }
        }
    }
    #endregion
}
