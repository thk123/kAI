using UnityEngine;
using System.Collections;

public class LaserWeaponSystem : MonoBehaviour, IWeaponSystem {

    public float maxDistance;
    public float firingDuration;
    public float damagePerSecond;


    public LineRenderer laserRenderer;

    public Transform laserStartPoint;

    float currentFireDuration;

	// Use this for initialization
	void Start () {
        currentFireDuration = firingDuration;	
	}
	
	// Update is called once per frame
	void Update () {
	    if(currentFireDuration >= firingDuration)
        {
            laserRenderer.SetVertexCount(0);
        }
        else
        {
            UpdateLaser();
            //currentFireDuration += Time.deltaTime;
        }
	}

    void UpdateLaser()
    {
        RaycastHit2D result = Physics2D.Raycast(laserStartPoint.position, transform.right, maxDistance);

        if (result)
        {
            laserRenderer.SetVertexCount(2);
            laserRenderer.SetPosition(0, laserStartPoint.position);
            laserRenderer.SetPosition(1, new Vector3(result.point.x, result.point.y, transform.position.z));

            HealthBehaviour health = result.collider.GetComponent<HealthBehaviour>();
            if(health != null)
            {
                health.ApplyDamage(damagePerSecond * Time.deltaTime);
            }

        }
        else
        {
            laserRenderer.SetVertexCount(2);
            laserRenderer.SetPosition(0, transform.position);
            laserRenderer.SetPosition(1, transform.position + (laserStartPoint.right * maxDistance));
        }
    }

    public void Fire()
    {
        currentFireDuration = 0.0f;        
    }
}
