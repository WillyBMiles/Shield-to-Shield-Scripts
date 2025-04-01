using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[RequireComponent(typeof(Ara.AraTrail))]
public class ScaleTrail : MonoBehaviour
{
    [HideIf(nameof(ScaleWithLossyScale))]
    public bool ScaleWithAttackRange;
    public float Scale;
    [HideIf(nameof(ScaleWithAttackRange))]
    public bool ScaleWithLossyScale;

    Ara.AraTrail trail;
    Character character;
    // Start is called before the first frame update
    void Start()
    {
        character = GetComponentInParent<Character>();
        trail = GetComponent<Ara.AraTrail>();
    }
    Ara.ElasticArray<Ara.AraTrail.Point> pointArray = new Ara.ElasticArray<Ara.AraTrail.Point>();
    Ara.ElasticArray<Ara.AraTrail.Point> tempPoints = new Ara.ElasticArray<Ara.AraTrail.Point>();

    // Update is called once per frame
    void Update()
    {
        if (ScaleWithLossyScale)
        {
            trail.thickness = Scale * transform.lossyScale.x;
        }
        else
        {
            if (ScaleWithAttackRange && character != null)
            {
                trail.thickness = character.CheckTrait(Trait.AttackRange, multiplier: Scale);
            }
            else
            {
                trail.thickness = Scale;
            }
        }

        if (character != null && character.LocalTimeMult == 0f)
        {
            tempPoints.Clear();
            foreach (var point in trail.points)
            {
                tempPoints.Add(point);
                
            }
            pointArray.Clear();
            foreach (var point in tempPoints)
            {
                Ara.AraTrail.Point newPoint = new Ara.AraTrail.Point(point.position, point.velocity, point.tangent, point.normal,
                    point.color, point.thickness, point.texcoord, point.life + Time.deltaTime);
                pointArray.Add(newPoint);
            }
            trail.timeInterval = 1000f;
            trail.points = pointArray;
        }
        else
        {
            trail.timeInterval = 0f;
        }
        
    }
}
