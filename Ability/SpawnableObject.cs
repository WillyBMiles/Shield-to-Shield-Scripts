using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class SpawnableObject : NetworkBehaviour
{
    public Vector3 target;

    float timer = 0f;

    public float duration;
	Character character;
	public float startingHealth;
	public List<GameObject> body = new List<GameObject>();

	[SyncVar]
	public Character mySummoner;

	[HideInInspector]
	public bool InheritParentFaction;

	public bool InheritStats;

	public Effect_Spawn effect;


	private void Start()
	{
		character = GetComponent<Character>();
		character.mySpawnableObject = this;
		AnimationController ac = character.GetComponent<AnimationController>();
		if (body != null)
        {
			foreach (GameObject b in body)
            {
				b.gameObject.SetActive(false);
			}
        }
			
		if (ac != null)
			ac.PlayAnimation(DefaultAnimation.Awakened, PlayMode.StopAll, 1f);
	}

	public void ImplementStartingHealth()
    {
		character = GetComponent<Character>();
		character.MaxHealth = startingHealth;
		character.StartingHealth = startingHealth;
		character.CmdSetHealth(startingHealth);
    }

	bool inheritedStats = false;
	int tick;
	private void Update()
	{
		if (tick == 1 && body != null) { 
			if (body != null)
			{
				foreach (GameObject b in body)
				{
					b.gameObject.SetActive(true);
				}
			}
		}
		tick++;

		if (duration == 0 || !isServer)
			return;
		if (InheritStats && mySummoner != null && !inheritedStats)
		{
			//TODO???
			inheritedStats = true;

		}

		timer += Time.deltaTime;
		if (timer >= duration && duration > 0)
		{
			if (character == null)
			{
				Destroy(gameObject);
			}
			else
			{
				character.Die(false, true);
			}
		}

		if (effect.InheritColor && character.playerColor != null && mySummoner.playerColor != null)
        {
			character.playerColor.SetColor(mySummoner.playerColor.myColor);
        }
	}



}
