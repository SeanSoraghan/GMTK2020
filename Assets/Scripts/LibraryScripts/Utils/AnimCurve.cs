using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimCurve
{
	public enum MotionType
	{
		Linear,
		Exponential,
		Logarithmic
	}

	public MotionType currentMotionType { private get; set; } = MotionType.Linear;

	float animationSpeed = 0.0f;
	float _animationTimeSeconds;
	public float animationTimeSeconds
	{
		private get { return _animationTimeSeconds; }
		set
		{
			_animationTimeSeconds = value;
			animationSpeed = _animationTimeSeconds == 0.0f ? 0.0f : 1.0f / _animationTimeSeconds;
		}
	}
	public float portionThisFrame { get; private set; } = 0.0f;
	public float animLinearCounter { get; private set; } = 0.0f;
	public float animCurveCounter { get; private set; } = 0.0f;
	float prevAnimCurveCounter = 0.0f;

	public void Reset()
	{
		animLinearCounter = 0.0f;
		prevAnimCurveCounter = 0.0f;
	}

	// Update is called once per frame
	public bool UpdateCurve(float deltaTime)
    {
		animLinearCounter += Time.deltaTime * animationSpeed;
		animCurveCounter = animLinearCounter;
		if (currentMotionType == MotionType.Exponential)
			animCurveCounter = (Mathf.Pow(2.0f, animLinearCounter * 4.0f) - 1.0f) / 15.0f;
		else if (currentMotionType == MotionType.Logarithmic)
			animCurveCounter = Mathf.Log10(9.0f * animLinearCounter + 1.0f);

		float remaining = 1.0f - prevAnimCurveCounter;
		portionThisFrame = (animCurveCounter - prevAnimCurveCounter) / remaining;

		prevAnimCurveCounter = animCurveCounter;

		return animLinearCounter >= 1.0f;
	}
}

public class AnimatedValue
{
	public AnimCurve animCurve = new AnimCurve();
	float _animTimeSeconds = 1.0f;
	public float animTimeSeconds
	{
		get { return _animTimeSeconds; }
		set
		{
			_animTimeSeconds = value;
			animCurve.animationTimeSeconds = _animTimeSeconds;
		}
	}

	public float start = 0.0f;
	public float target = 0.0f;
	public float value { private set; get; } = 0.0f;

	public void Reset()
	{
		animCurve.Reset();
		value = start;
	}

	public void SetTargetFromCurrent(float newTarget)
	{
		start = value;
		target = newTarget;
	}

	public void SetStartAndTarget(float newStart, float newTarget)
	{
		start = newStart;
		target = newTarget;
	}

	public bool Update(float deltaTime)
	{
		bool curveComplete = animCurve.UpdateCurve(deltaTime);
		value = Mathf.Lerp(start, target, animCurve.animCurveCounter);
		if (curveComplete)
			value = target;
		return curveComplete;
	}
}

public class InterpedValue
{
	public float start = 0.0f;
	public float target = 0.0f;
	public float value = 0.0f;
}

public class AnimatedValueList
{
	public AnimCurve animCurve = new AnimCurve();
	float _animTimeSeconds = 1.0f;
	public float animTimeSeconds
	{
		get { return _animTimeSeconds; }
		set
		{
			_animTimeSeconds = value;
			animCurve.animationTimeSeconds = _animTimeSeconds;
		}
	}

	public List<InterpedValue> interpedValues = new List<InterpedValue>();

	public void AddAnimatedValue(float start = 0.0f, float target = 0.0f)
	{
		interpedValues.Add(new InterpedValue());
		interpedValues[interpedValues.Count - 1].start = start;
		interpedValues[interpedValues.Count - 1].target = target;
		interpedValues[interpedValues.Count - 1].value = start;
	}

	public void Reset()
	{
		animCurve.Reset();
		for (int i = 0; i < interpedValues.Count; ++i)
		{
			interpedValues[i].value = interpedValues[i].start;
		}
	}

	public void SetTargetFromCurrent(int index, float newTarget)
	{
		interpedValues[index].start = interpedValues[index].value;
		interpedValues[index].target = newTarget;
	}

	public void SetStartAndTarget(int index, float newStart, float newTarget)
	{
		interpedValues[index].start = newStart;
		interpedValues[index].target = newTarget;
	}

	public bool Update(float deltaTime)
	{
		bool curveComplete = animCurve.UpdateCurve(deltaTime);
		foreach (InterpedValue interpedValue in interpedValues)
		{
			if (curveComplete)
				interpedValue.value = interpedValue.target;
			else
				interpedValue.value = Mathf.Lerp(interpedValue.start, interpedValue.target, animCurve.animCurveCounter);
		}
		return curveComplete;
	}
}
