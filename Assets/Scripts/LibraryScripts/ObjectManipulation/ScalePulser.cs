using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScalePulser : MonoBehaviour
{
	public enum AnimationState
	{
		Attack,
		Release,
		Idle
	}

	AnimationState _animState = AnimationState.Idle;
	AnimationState animState
	{
		get { return _animState; }
		set
		{
			_animState = value;
			if (_animState == AnimationState.Attack)
			{
				animCurveAttack.Reset();
				transform.localScale = startScale;
			}
			if (_animState == AnimationState.Release)
			{
				animCurveRelease.Reset();
				transform.localScale = startScale * pulseScale;
			}
			if (_animState == AnimationState.Idle)
			{
				transform.localScale = startScale;
			}
		}
	}

	public float PulseAttackTime = 0.1f;
	public float PulseReleaseTime = 0.2f;
	public bool loop = true;

	AnimCurve animCurveAttack = new AnimCurve();
	AnimCurve animCurveRelease = new AnimCurve();

	Vector3 startScale;
	// scale will pulse out (or in) to startScale * pulseScale and back again.
	float pulseScale = 1.1f;

	void Awake()
    {
		animCurveAttack.animationTimeSeconds = PulseAttackTime;
		animCurveRelease.animationTimeSeconds = PulseReleaseTime;
		animCurveAttack.currentMotionType = AnimCurve.MotionType.Logarithmic;
		animCurveRelease.currentMotionType = AnimCurve.MotionType.Linear;
		startScale = transform.localScale;
    }

	public void StartLooping()
	{
		loop = true;
		animState = AnimationState.Attack;
	}
	public void StopLooping() { loop = false; }
	public void PulseOnce()
	{
		loop = false;
		animState = AnimationState.Attack;
	}

    // Update is called once per frame
    void Update()
    {
        switch (animState)
		{
			case AnimationState.Attack:
				animCurveAttack.UpdateCurve(Time.deltaTime);
				float attackScale = Mathf.Lerp(1.0f, pulseScale, animCurveAttack.animCurveCounter);
				transform.localScale = attackScale * startScale;
				if (animCurveAttack.animLinearCounter >= 1.0f)
				{
					animState = AnimationState.Release;
				}
				break;
			case AnimationState.Release:
				animCurveRelease.UpdateCurve(Time.deltaTime);
				float releaseScale = Mathf.Lerp(pulseScale, 1.0f, animCurveRelease.animCurveCounter);
				transform.localScale = releaseScale * startScale;
				if (animCurveRelease.animLinearCounter >= 1.0f)
				{
					if (loop)
						animState = AnimationState.Attack;
					else
						animState = AnimationState.Idle;
				}
				break;
			default: break;
		}
    }
}
