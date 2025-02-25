using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PostureData
{
    public System.DateTime dateTime;
    public string postureName;
    public int archerLvl;

    // front wrist
    public float frontWristFluctuate;
    public Vector3 frontWristStart;
    public Vector3 frontWristEnd;
    // back wrist
    public float backWristFluctuate;
    public Vector3 backWristStart;
    public Vector3 backWristEnd;
    // front elbow
    public float frontElbowAngleFluctuate;
    public float frontElbowAngleStart;
    public float frontElbowAngleEnd;
    // back elbow
    public float backElbowAngleFluctuate;
    public float backElbowAngleStart;
    public float backElbowAngleEnd;
    // front shoulder
    public float frontShoulderAngleFluctuate;
    public float frontShoulderAngleStart;
    public float frontShoulderAngleEnd;
    // back shoulder
    public float backShoulderAngleFluctuate;
    public float backShoulderAngleStart;
    public float backShoulderAngleEnd;

    public List<TimedPos> frontWristPts = new();
    public List<TimedPos> backWristPts = new();
}