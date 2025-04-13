using Dreamteck.Splines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplineFollowerState : ComponentState
{
    SplineFollower splineFollower;
    SplineComputer assignedSpline;
    Spline.Direction direction;
    bool follow;
    float followSpeed;
    double resultPercent;

    public override void CaptureState(object component)
    {
        splineFollower = component as SplineFollower;
        if (splineFollower == null) { return; }
        assignedSpline = splineFollower.spline;
        direction = splineFollower.direction;
        followSpeed = splineFollower.followSpeed;
        resultPercent = splineFollower.result.percent;
        follow = false;
    }

    public override void ResetState()
    {
        if (splineFollower == null) { return; }
        splineFollower.spline = assignedSpline;
        splineFollower.direction = direction;
        splineFollower.followSpeed = followSpeed;
        splineFollower.SetPercent(resultPercent);
        splineFollower.follow = follow;
    }
}
