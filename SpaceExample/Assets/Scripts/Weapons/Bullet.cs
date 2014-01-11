using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

    public float damage;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter2D(Collider2D other)
    {
        print("Trigger");
        HealthBehaviour healthOther = other.GetComponent<HealthBehaviour>();
        if(healthOther != null)
        {
            healthOther.ApplyDamage(damage);
        }
        Destroy(gameObject);
    }
}
