using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ArcherLvl { Beginner, Elementary, Intermidate, Advanced, Null }

public class PostureScoreUtils : MonoBehaviour
{
    #region Singleton
    public static PostureScoreUtils instance;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        // DontDestroyOnLoad(gameObject);
        else
            Destroy(gameObject);
    }
    #endregion

    // hardcode score range
    float minFrontWristFluctuate = 0f;
    float maxFrontWristFluctuate = 50f;

    float minBackWristFluctuate = 40f;
    float maxBackWristFluctuate = 150f;

    float minFrontElbowAngleFluctuate = 0f;
    float maxFrontElbowAngleFluctuate = 5f;

    float minBackElbowAngleFluctuate = 0f;
    float maxBackElbowAngleFluctuate = 16f;

    float minFrontShoulderAngleFluctuate = 0f;
    float maxFrontShoulderAngleFluctuate = 5f;

    float minBackShoulderAngleFluctuate = 2f;
    float maxBackShoulderAngleFluctuate = 5f;

    float[] rankMultiplier = { 1.5f, 1f, 0.75f, 0.5f };
    public float absoulteScore_Beginner => (rankMultiplier[0] - rankMultiplier[0]) / rankMultiplier[0] * 100;
    public float absoulteScore_Elementary => (rankMultiplier[0] - rankMultiplier[1]) / rankMultiplier[0] * 100;
    public float absoulteScore_Intermidate => (rankMultiplier[0] - rankMultiplier[2]) / rankMultiplier[0] * 100;
    public float absoulteScore_Advanced => (rankMultiplier[0] - rankMultiplier[3]) / rankMultiplier[0] * 100;

    // cal score with level adjustment
    /* no adjust => get the score from the max range (lowest level)
    the smaller the score, the better the performance */
    public List<float> adjustedScore(List<float> scores, int archerLvl = 0)
    {
        float offset = rankMultiplier[archerLvl];

        scores[0] = (scores[0] - minFrontWristFluctuate) /
                        (maxFrontWristFluctuate * offset - minFrontWristFluctuate) * 100;

        scores[1] = (scores[1] - minBackWristFluctuate) /
                        (maxBackWristFluctuate * offset - minBackWristFluctuate) * 100;

        scores[2] = (scores[2] - minFrontElbowAngleFluctuate) /
                        (maxFrontElbowAngleFluctuate * offset - minFrontElbowAngleFluctuate) * 100;

        scores[3] = (scores[3] - minBackElbowAngleFluctuate) /
                        (maxBackElbowAngleFluctuate * offset - minBackElbowAngleFluctuate) * 100;

        scores[4] = (scores[4] - minFrontShoulderAngleFluctuate) /
                        (maxFrontShoulderAngleFluctuate * offset - minFrontShoulderAngleFluctuate) * 100;

        scores[5] = (scores[5] - minBackShoulderAngleFluctuate) /
                        (maxBackShoulderAngleFluctuate * offset - minBackShoulderAngleFluctuate) * 100;

        scores[6] = (scores[0] * 2 +
                     scores[1] +
                     scores[2] * 2 +
                     scores[3] +
                     scores[4] * 2 +
                     scores[5]) / 9;

        return scores;
    }
}