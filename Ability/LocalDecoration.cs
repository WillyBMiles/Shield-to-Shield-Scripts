using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalDecoration : MonoBehaviour
{
    GameObject appearance;
    public float duration;
    public float maxDuration;
    Character target;
    float height;
    Effect_Decoration effect;
    Character origin;

    public Color color;
    bool active;

    Vector3 startingScale;

    // Start is called before the first frame update

    // Update is called once per frame
    void Update()
    {
        if (!active)
            return;
        if (target != null)
        {
            transform.position = target.transform.position;
            if (effect.FollowCharacter && effect.FollowCharacterRotation)
            {
                transform.rotation = target.transform.rotation;
            }
            if (target.Dead && !effect.FollowAfterDeath)
            {
                DestroyThis();
            }
        }
            
        transform.position = new Vector3(transform.position.x, height, transform.position.z);
        if (maxDuration > -1)
        {
            duration -= Time.deltaTime;
            if (duration <= 0)
                DestroyThis();
        }
        if (effect.Pop)
        {
            if (duration < maxDuration / 2f)
            {
                float proportion = duration / (maxDuration / 2f);
                transform.localScale = proportion * startingScale;
            }
            else
            {
                float proportion = 2 - duration / (maxDuration / 2f) ;
                transform.localScale = proportion * startingScale;
            }
        }
    }


    static List<LocalDecoration> decorations = new List<LocalDecoration>();
    void EnableDecoration(Vector3 originPoint, float width, float height, GameObject appearance, float duration, Character target, bool followCharacter,
        Color color, Effect_Decoration effect, Character origin)
    {
        AttachAppearance(appearance);
        this.transform.position = new Vector3(originPoint.x, height, originPoint.z);
        this.appearance.transform.localScale = new Vector3(width, width, width);
        this.duration = duration;
        this.height = height;
        this.color = color;
        this.maxDuration = duration;
        this.effect = effect;
        this.origin = origin;

        this.startingScale = new Vector3(width, width, width);
        if (followCharacter)
            this.target = target;
        else
            this.target = null;
        decorations.Add(this);
        active = true;

        if (effect.PointTowardsOrigin && origin != null)
        {
            Vector3 pointDirection = transform.position - origin.transform.position;
            pointDirection = new Vector3(pointDirection.x, 0f, pointDirection.z);
            transform.rotation = Quaternion.LookRotation(pointDirection, Vector3.up);
        }
    }

    public void DestroyThis()
    {
        decorations.Remove(this);
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        decorations.Remove(this);
    }


    void AttachAppearance(GameObject appearance)
    {
        if (appearance == null)
            return;
        GameObject newAppearance = Instantiate(appearance, this.transform);


        this.appearance = newAppearance;
        newAppearance.transform.SetParent(transform);
        newAppearance.transform.localScale = appearance.transform.localScale;
        newAppearance.transform.localRotation = Quaternion.identity;
        newAppearance.transform.localPosition = new Vector3();
        newAppearance.SetActive(true);
    }

    static GameObject prefab;
    public static GameObject CreateDecoration(Vector3 originPoint, float width, float height, GameObject appearance, float duration, Character target, bool followCharacter, 
        Color color, Effect_Decoration effect, Character origin)
	{
        if (prefab == null)
        {
            prefab = (GameObject)Resources.Load("decorationPrefab");
        }
        if (appearance == null)
            return null;
        GameObject newObj;
        LocalDecoration local;
        newObj = Instantiate(prefab, new Vector3(originPoint.x, height, originPoint.z), Quaternion.identity);
        local = newObj.GetComponent<LocalDecoration>();

        local.EnableDecoration(originPoint, width, height, appearance, duration, target, followCharacter, color, effect, origin);
        return newObj;
    }
}
