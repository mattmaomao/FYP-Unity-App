using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class MLPoseClassifier : Agent
{
    [SerializeField] PoseIdentifier poseIdentifier;
    [SerializeField] List<Transform> dots = new List<Transform>();
    bool isSetup = false;

    public override void CollectObservations(VectorSensor sensor)
    {
        // dun do anything before setup
        if (!isSetup) return;

        for (int i = 0; i < dots.Count; i++)
            sensor.AddObservation(dots[i].localPosition);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        // dun do anything before setup
        if (!isSetup || dots.Count <= 0) return;

        int guess = actions.DiscreteActions[0];
        if ((Pose)guess == poseIdentifier.currentPose)
        {
            if (poseIdentifier.currentPose == Pose.Idle)
                SetReward(1f);
            else if (poseIdentifier.currentPose == Pose.Draw_Ready)
                SetReward(1f);
            else {
                Debug.Log("correct!!");
                SetReward(3f);
            }
        }
        else
        {
            Debug.Log("wrong!!");
            SetReward(-3f);
        }
        EndEpisode();
    }

    public void Setup(List<Transform> dots)
    {
        // connect all dots
        this.dots = dots;
        isSetup = true;
    }
}
