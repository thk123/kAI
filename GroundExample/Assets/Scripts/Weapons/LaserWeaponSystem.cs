using UnityEngine;
using System.Collections;

[RequireComponent(typeof(LineRenderer))]
public class LaserWeaponSystem : MonoBehaviour, IWeaponSystem {

    public float maxDistance;
    public float firingDuration;
    public float damagePerSecond;


    LineRenderer laserRenderer;

    public Transform laserStartPoint;

    float currentFireDuration;

	// Use this for initialization
	void Start () {
        currentFireDuration = firingDuration;
        laserRenderer = GetComponent<LineRenderer>();
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
            currentFireDuration += Time.deltaTime;
        }
	}

    void UpdateLaser()
    {
        RaycastHit result;

        if (Physics.Raycast(laserStartPoint.position, laserStartPoint.forward, out result, maxDistance))
        {
            laserRenderer.SetVertexCount(2);
            laserRenderer.SetPosition(0, laserStartPoint.position);
            laserRenderer.SetPosition(1, result.point);

            HealthBehaviour health = result.collider.GetComponent<HealthBehaviour>();
            if (health != null)
            {
                health.ApplyDamage(damagePerSecond * Time.deltaTime, transform.forward, transform.parent.gameObject);
            }

        }
        else
        {
            laserRenderer.SetVertexCount(2);
            laserRenderer.SetPosition(0, laserStartPoint.position);
            laserRenderer.SetPosition(1, laserStartPoint.position + (laserStartPoint.forward * maxDistance));
        }
        /*RaycastHit2D result = Physics2D.Raycast(laserStartPoint.position, transform.forward, maxDistance);

        if (result)
        {
            laserRenderer.SetVertexCount(2);
            laserRenderer.SetPosition(0, laserStartPoint.position);
            laserRenderer.SetPosition(1, new Vector3(result.point.x, result.point.y, transform.position.z));

            HealthBehaviour health = result.collider.GetComponent<HealthBehaviour>();
            if(health != null)
            {
                health.ApplyDamage(damagePerSecond * Time.deltaTime, transform.right);  
            }

        }
        else
        {
            laserRenderer.SetVertexCount(2);
            laserRenderer.SetPosition(0, laserStartPoint.position);
            laserRenderer.SetPosition(1, laserStartPoint.position + (laserStartPoint.right * maxDistance));
        }*/
    }

    public void Fire()
    {
        currentFireDuration = 0.0f;        
    }


    public void SetSpawnPoint(Transform spawn)
    {
        laserStartPoint = spawn;
    }

    public float Range
    {
        get { return maxDistance; }
    }
}
