using CartoonFX;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Dart_Plane : MonoBehaviour
{
    public int TotalScore;
    public bool isDelay;
    [SerializeField] CFXR_Demo_RandomText scoreEffect;
    [SerializeField] List<Dart_Score> Scores;
    [SerializeField] TextMeshProUGUI ui;
    public string scoreSound, highScoreSound, perfectSound;
    void AddScore(int score, Vector3 pos)
    {
        TotalScore += score;
        scoreEffect.SetText(score);
        var tmp = pos;
        tmp.y = scoreEffect.transform.position.y;
        scoreEffect.transform.position= tmp;
        ui.text = TotalScore.ToString();

        if(score == 50) { SoundManager.Instance.PlaySound(perfectSound); }
        else SoundManager.Instance.PlaySound(scoreSound);
    }
    private void Awake()
    {
        foreach (var item in Scores)
        {
            item.AddScore.AddListener(AddScore);
            item.plane = this;
        }
    }
    public void Reset()
    {
        TotalScore = 0;
        ui.text = TotalScore.ToString();
    }
    private void OnEnable()
    {
        Reset();
    }
}
