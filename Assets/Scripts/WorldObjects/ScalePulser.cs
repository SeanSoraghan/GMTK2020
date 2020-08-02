using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScalePulser : MonoBehaviour
{
	public enum AnimationState
	{
		Attack,
		Release
	}

	public float PulseAttackTime = 0.1f;
	public float PulseReleaseTime = 0.2f;

	AnimCurve animCurve = new AnimCurve();
	// Start is called before the first frame update
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
