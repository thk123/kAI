using UnityEngine;
using System.Collections;

public class SwordController : MonoBehaviour {
	
	public GameObject sword;
	
	public bool swordSwinging;
	
	float progress = 0.0f;
	public float swingDuration = 2.0f;

	/// <summary>
	/// The length of the sword.
	/// </summary>
	public float swordLength;

	/// <summary>
	/// The swing radius.
	/// </summary>
	public float swingRadius;
	
	public float direction = 1.0f;
	
	
	// Use this for initialization
	void Start () {
		swordSwinging = false;
	}
	
	// Update is called once per frame
	void Update () {
		if(swordSwinging)
		{
			progress += Time.deltaTime;
			float normalisedProgress = progress / swingDuration;
			
			if(normalisedProgress >= 1.0f)
			{
				swordSwinging  = false;	
				sword.transform.RotateAround(sword.transform.position + (swordLength * -1.0f * sword.transform.up),Vector3.forward * direction, swingRadius);
			}
			else
			{
				float speed = swingRadius / swingDuration;
				sword.transform.RotateAround(sword.transform.position + (swordLength * -1.0f * sword.transform.up), Vector3.forward * direction, -1.0f *speed  * Time.deltaTime);
			}
		}
	}
	
	public void SwingSword()
	{
		if(!swordSwinging)
		{
			progress = 0.0f;
			swordSwinging = true;
		}
	}
}
