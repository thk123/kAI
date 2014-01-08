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

            float oldTime = time;
			time = Time.fixedTime - startTime;

            float variableFixedDeltaTime = time - oldTime;
            float fixedRatio = variableFixedDeltaTime / Time.fixedDeltaTime;
			if(time <= totalTime)
			{
                applyingForce = forceFunction.GetValue(time) * fixedRatio;
				
				Vector2 currentVelocity = ship.rigidbody2D.velocity;
				Vector2 direction = new Vector2(ship.transform.right.x, ship.transform.right.y);

				float velocityAlongDir = Vector2.Dot(currentVelocity, direction);
				float velocityPrediction = velocityAlongDir + (applyingForce * lDeltaTime);

                // TODO: we really should have the second param >= 0.0f but have an issue with only starting moving on the second frame. 
				if(time > 0.0f && // we are not interested if we are just starting
				   ((velocityPrediction < 0.0f  && velocityAlongDir > 0.0f ) ||
				   (velocityPrediction > 0.0f  && velocityAlongDir < 0.0f ) ))
				{
					float ratio = Mathf.Abs((velocityAlongDir / (applyingForce * lDeltaTime)));
					applyingForce = applyingForce * ratio;
	
                    // We have reversed the direction so we must have arrived
					Deactivate();
				}
			
				engine.ApplyAccelerateForce(applyingForce);
			}
		}

	}

	void ComputeForces(ShipEngine engine)
	{
		// We want to minimize the time given the force of the engine
		
		float delta = targetAngle.Data;

		float maxForce = engine.accelerationForce;

        // if we are under this limit, then the optimal acceleration is just the cos curve, compressed if we can do it in less time
		if(delta <= 2 * maxForce)
		{
			totalTime = Mathf.Acos((maxForce - delta)/maxForce);
            LogMessage("Time:" + totalTime);
			forceFunction = new MathFunction(0.0f, totalTime);

            // The time factor represents how much we have compressed the cos curve
            float timeFactor = Mathf.PI / totalTime;

			forceFunction.AddSegment((point) => {
				return ((timeFactor * Mathf.Cos(point * timeFactor) * maxForce)); 
			}, 0.0f, totalTime);


		}
		else
		{
			Func<float, float> fullAccel = (point) => { return maxForce; };
			Func<float, float> fullDeccel = (point) => { return -maxForce; };

            // we compute the time to do full accerlation (and deceleration)
			float t0 = ((-Mathf.PI * maxForce) + (Mathf.Sqrt(
							(((Mathf.PI * Mathf.PI) - 8) * maxForce) + 4.0f * delta)) * Mathf.Sqrt(maxForce)) /
							(2.0f * maxForce);

			forceFunction = new MathFunction(0.0f, t0 + Mathf.PI + t0);

            // We do full accleration between 0 and t0
			forceFunction.AddSegment(fullAccel, 0.0f, t0);

            // Do a cos acceleration as above between t0 and t0 + pi
			forceFunction.AddSegment((lPoint) => {
				return (Mathf.Cos(lPoint-t0) * maxForce);
			}, t0, t0 + Mathf.PI);

            // Then a full deceleration at the end 
			forceFunction.AddSegment(fullDeccel, t0 + Mathf.PI, Mathf.PI + (2 * t0));

            // The total time is the time spend doing full acceleration, plus our cos section plus time spent doing full deceleration.
			totalTime = t0 + Mathf.PI + t0;
		}
	
		forceFunction.EndAddSegment();
		time = 0.0f;
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
				//Debug.Log("Getting function for " + point + ", which runs from " + seg.startPoint + " to " + seg.endPoint);
				return seg.segmentFunction;
			}
		}

		Debug.LogError("Could not find function for point " + point);
		return null;
	}
}






