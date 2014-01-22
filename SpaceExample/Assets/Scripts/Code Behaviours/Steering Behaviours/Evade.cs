using UnityEngine;
using System.Collections;
using kAI.Core;

public class Evade : kAICodeBehaviour {

    bool started;
    float startAngle;
    float direction;

    public Evade()
    {
        started = false;
    }

    protected override void OnActivate()
    {
        base.OnActivate();
        started = false;
    }

    protected override void InternalUpdate(float lDeltaTime, object lUserData)
    {
        GameObject shipObject = lUserData as GameObject;
        if(shipObject != null)
        {
            ShipEngine engine = shipObject.GetComponent<ShipEngine>();

            if(!started)
            {
                startAngle = shipObject.transform.rotation.eulerAngles.z + 45.0f;
                direction = 1;
                started = true;
            }

            float currentAngle = shipObject.transform.rotation.eulerAngles.z - startAngle;
            LogMessage("Current angle: " + currentAngle);
            if((currentAngle >= 0 && currentAngle < 180.0f && direction > 0 ) || 
                (currentAngle <= 360.0f && currentAngle > 180.0f && direction < 0 ))
            {
                direction *= -1;
            }

            engine.ApplyTorque(engine.torqueForce * direction);
            engine.ApplyAccelerateForce(engine.accelerationForce * 0.01f);
        }
    }
}

public class GenerateEvadePoint : kAICodeBehaviour
{
    bool shouldCalculatePoint;

    kAIDataPort<Vector2> targetPort;
    kAIDataPort<Vector2> hitDirection;

    kAIDataPort<float> scalePort;

    public GenerateEvadePoint()
    {
        kAITriggerPort calculateNewPoint = new kAITriggerPort("CalculatePoint", kAIPort.ePortDirection.PortDirection_In);
        AddExternalPort(calculateNewPoint);
        calculateNewPoint.OnTriggered += calculateNewPoint_OnTriggered;

        targetPort = new kAIDataPort<Vector2>("Target", kAIPort.ePortDirection.PortDirection_Out);
        AddExternalPort(targetPort);

        hitDirection = new kAIDataPort<Vector2>("OnDamageDirection", kAIPort.ePortDirection.PortDirection_In);
        AddExternalPort(hitDirection);

        scalePort = new kAIDataPort<float>("Scale", kAIPort.ePortDirection.PortDirection_In);
        AddExternalPort(scalePort);
    }

    void calculateNewPoint_OnTriggered(kAIPort lSender)
    {
        shouldCalculatePoint = true;
    }

    protected override void InternalUpdate(float lDeltaTime, object lUserData)
    {
        if (shouldCalculatePoint)
        {
            GameObject shipObject = lUserData as GameObject;
            if (shipObject != null)
            {
                targetPort.Data = EvadeFunctions.GenerateNewEvadePoint(shipObject, scalePort.Data, hitDirection.Data);
            }

            shouldCalculatePoint = false;
        }
    }
}

public static class EvadeFunctions
{
    public static Vector2 GenerateNewEvadePoint(GameObject currentObject, float scale, Vector2 directionOfDamage)
    {
        if(directionOfDamage != Vector2.zero)
        {
            Vector2 minusX = new Vector2(-directionOfDamage.x, directionOfDamage.y);
            Vector2 minusY = new Vector2(directionOfDamage.x, -directionOfDamage.y);
            float angleX = Vector2.Angle(currentObject.transform.right, minusX);
            float angleY = Vector2.Angle(currentObject.transform.right, minusY);

            if(angleX < angleY  )
            {
                return currentObject.transform.GetPosition2D() + (minusX.normalized * scale);
            }
            else
            {
                return currentObject.transform.GetPosition2D() + (minusY.normalized * scale);
            }
        }
        else
        {
            Vector2 facing = currentObject.transform.right;

            float angle = Random.Range(-Mathf.PI / 2.0f, Mathf.PI / 2.0f);

            Vector2 newFacing = new Vector2((facing.x * Mathf.Cos(angle)) - (facing.y * Mathf.Sin(angle)),
                (facing.x * Mathf.Sin(angle)) + (facing.y * Mathf.Cos(angle)));

            return (Vector2)currentObject.transform.position + (scale * newFacing);
        }
    }
}
//public class RotateTo : kAICodeBehaviour