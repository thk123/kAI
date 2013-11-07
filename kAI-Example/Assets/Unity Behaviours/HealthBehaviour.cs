using UnityEngine;
using System.Collections;

public class HealthBehaviour : MonoBehaviour {
	
	public float startingHealth;
	float remainingHealth;
	
	public float regenHealthTime = 1.0f;
	public float regenRate= 5.0f;
	float timeSinceLastDamage;
	
	public int xPos;
	
	public float CurrentHealth
	{
		get
		{
			return remainingHealth;
		}
	}
	
	// Use this for initialization
	void Start () {
		remainingHealth = startingHealth;
		timeSinceLastDamage = regenHealthTime;
	}
	
	// Update is called once per frame
	void Update () {
		if(remainingHealth <= 0.0f)
		{
			DestroyObject(gameObject);	
		}
		
		timeSinceLastDamage += Time.deltaTime;
		
		if(timeSinceLastDamage > regenHealthTime && remainingHealth < startingHealth)
		{
			remainingHealth += regenRate * Time.deltaTime;
			remainingHealth = Mathf.Min(startingHealth, remainingHealth);
		}
	}
	
	void OnGUI()
	{
		if(xPos > 0)
		{
			GUI.Box(new Rect(xPos, Screen.height - 100, 200, 50), "Health " + remainingHealth.ToString("0"));	
		}
		else	
		{
			Vector2 size = GUIStyle.none.CalcSize(new GUIContent("Health " + remainingHealth.ToString("0")));
			GUI.Box(new Rect(Screen.width- size.x -200 + xPos, Screen.height - 100, 200, 50), "Health " + remainingHealth.ToString("0"));	
		}
	}
	
	public void ApplyDamage(float lDamage)
	{
		remainingHealth -= lDamage;	
		timeSinceLastDamage = 0.0f;
	}
	
	public void OnTriggerEnter(Collider lCollider)
	{
		if(!lCollider.transform.IsChildOf(transform))	
		{
			SwordTip tid = lCollider.GetComponent<SwordTip>();
			if(tid != null)
			{
				ApplyDamage(tid.hitDamage);
			}
		}
	}
	
	
	public void OnTriggerStay(Collider lCollider)
	{
		if(!lCollider.transform.IsChildOf(transform))	
		{
			SwordTip tid = lCollider.GetComponent<SwordTip>();
			if(tid != null)
			{
				ApplyDamage(tid.continuousDamage);
			}
		}
	}
}
