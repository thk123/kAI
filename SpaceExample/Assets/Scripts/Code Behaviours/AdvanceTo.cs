using System;
using System.Collections.Generic;
using kAI.Core;
using UnityEngine;

public class AdvanceTo : kAICodeBehaviour
{
	kAIDataPort<float> targetAngle;

	float time;
	float totalTime;
	float startTime;
	public float applyingForce
	{
		get;
		private set;
	}
	
	bool firstRun = true;

	MathFunction forceFunction;

	public AdvanceTo() 
		:base(null)
	{
		targetAngle = new kAIDataPort<float>("Data", kAIPort.ePortDirection.PortDirection_In, null);
		AddExternalPort(targetAngle);

	}
	
	protected override void InternalUpdate (float lDeltaTime, object lUserData)
	{	
		GameObject ship = lUserData as GameObject;
		if(ship != null)
		{
			ShipEngine engine = ship.GetComponent<ShipEngine>();
			
			if(firstRun)
			{
				ComputeForces(engine);
				startTime = Time.fixedTime;
				firstRun = false;
			}


			time = Time.fixedTime - startTime;
			if(time <= totalTime)
			{
				applyingForce = forceFunction.GetFunctionForPoint(time)(time);
				
				
				Vector3 currentVelocity = ship.rigidbody.velocity;
				Vector3 direction = ship.transform.right;

				float velocityAlongDir = Vector3.Dot(currentVelocity, direction);
				LogMessage("Actual Velocity: " + velocityAlongDir);
				float velocityPrediction = velocityAlongDir + (applyingForce * lDeltaTime);
				LogMessage("Predicted Velocity: " + velocityPrediction);

				if((velocityPrediction < 0.0f  && velocityAlongDir > 0.0f ) ||
				   (velocityPrediction > 0.0f  && velocityAlongDir < 0.0f ) )
				{
					LogMessage("Low Power");
					float ratio = Mathf.Abs((velocityAlongDir / (applyingForce * lDeltaTime)));
					applyingForce = applyingForce * ratio;
					/*time = Mathf.PI  * 2.0f;*/
					Deactivate();
				}
				
				engine.ApplyAccelerateForce(applyingForce * ship.rigidbody.mass);
			}
		}

	}

	void ComputeForces(ShipEngine engine)
	{
		// We want to minimize the time given the force of the egine
		
		float delta = targetAngle.Data;

		float maxForce = engine.accelerationForce;
		if(delta <= 2 * maxForce)
		{
			totalTime = Mathf.Acos((maxForce - delta)/maxForce);

			forceFunction = new MathFunction(0.0f, totalTime);
			forceFunction.AddSegment((point) => {
				return ((Mathf.Cos(time * (Mathf.PI / totalTime)) * maxForce));
			}, 0.0f, totalTime);
		}
		else
		{
			Func<float, float> fullAccel = (point) => { Debug.Log("Full acceleration"); return maxForce; };
			Func<float, float> fullDeccel = (point) => { Debug.Log("Full deceleration"); return -maxForce; };

			float t0 = ((-Mathf.PI * maxForce) + (Mathf.Sqrt(
							(((Mathf.PI * Mathf.PI) - 8) * maxForce) + 4.0f * delta)) * Mathf.Sqrt(maxForce)) /
							(2.0f * maxForce);

			forceFunction = new MathFunction(0.0f, t0 + Mathf.PI + t0);
			forceFunction.AddSegment(fullAccel, 0.0f, t0);
			forceFunction.AddSegment((lPoint) => {
				Debug.Log("Curved acceleration");
				return (Mathf.Cos(time-t0) * maxForce);
			}, t0, t0 + Mathf.PI);
			forceFunction.AddSegment(fullDeccel, t0 + Mathf.PI, Mathf.PI + (2 * t0));
			totalTime = t0 + Mathf.PI + t0;
		}
	
		forceFunction.EndAddSegment();
		time = 0.0f;

		LogMessage("Time:" + totalTime);
	}

}

public class MathFunction
{
	float MinValue;
	float MaxValue;

	public class FunctionSegment
	{
		public Func<float, float> segmentFunction
		{
			get;
			private set;
		}
		public float startPoint
		{
			get;
			private set;
		}
		public float endPoint
		{
			get;
			private set;
		}

		public static bool operator<=(FunctionSegment a, FunctionSegment b)
		{
			return a.startPoint <= b.startPoint;
		}

		public static bool operator>=(FunctionSegment a, FunctionSegment b)
		{
			return a.startPoint >= b.startPoint;
		}

		public static bool Disjoint(FunctionSegment a, FunctionSegment b)
		{
			return a.endPoint <= b.startPoint || b.endPoint <= a.startPoint;
		}

		public FunctionSegment(Func<float, float> segFunc, float startValue, float endValue)
		{
			segmentFunction = segFunc;
			startPoint = startValue;
			endPoint = endValue;
		}
	}

	List<FunctionSegment> functionSegments;



	public MathFunction(float minValue, float maxValue)
	{
		MinValue = minValue;
		MaxValue = maxValue;
		functionSegments = new List<FunctionSegment>();
	}

	public void AddSegment(Func<float, float> segFunc, float startValue, float endValue)
	{
		functionSegments.Add(new FunctionSegment(segFunc, startValue, endValue));
	}

	public void EndAddSegment()
	{
		functionSegments.Sort(new Comparison<FunctionSegment>((lhs, rhs) =>
		{
			return lhs <= rhs ? -1 : 1;
		}));

		float positionDefined = MinValue;

		foreach(FunctionSegment segment in functionSegments)
		{
			if(segment.startPoint != positionDefined)
			{
				Debug.LogError("Gap between: " + segment.startPoint + " and " + positionDefined);
				return;
			}
			else
			{
				positionDefined = segment.endPoint;
			}
		}

		if(positionDefined != MaxValue)
		{
			Debug.LogError("Gap between: " + positionDefined + " and " + MaxValue);
			return;
		}


	}

	public float GetValue(float point)
	{
		if(point < MinValue || point > MaxValue)
		{
			Debug.LogError("Out side defined range of function");
			return float.NaN;
		}

		Func<float, float> func = GetFunctionForPoint(point);

		if(func != null)
		{
			return func(point);
		}
		else
		{
			return float.NaN;
		}
	}

	public Func<float, float> GetFunctionForPoint(float point)
	{
		foreach(FunctionSegment seg in functionSegments)
		{
			if(point <= seg.endPoint)
			{
				return seg.segmentFunction;
			}
		}

		Debug.LogError("Could not find function for point " + point);
		return null;
	}
}






