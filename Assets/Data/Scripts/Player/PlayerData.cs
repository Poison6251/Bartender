using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName ="Player/PlayerData")]
public class PlayerData : ScriptableObject 
{
    public PD pd;
    public float Money 
    {
        get 
        {
            return pd.Money; 
        }
        set
        {
            pd.Money = value;
            UpdatePlayTime();
        } 
    }

    private float lastTime;
    private static string filePath = @"Log";
    private char spaceWord = ' ';
    public static bool CheckDuplicateNickname(string nickname)
    {
        List<PD> pdList = ReadLogPD();
        foreach(var i in pdList)
            if (i.NickName == nickname) 
                return true;
        return false;
    }
    public bool Init(string nickname)
    {
        if (CheckDuplicateNickname(nickname)) return false;
        lastTime = Time.time;
        pd = new PD();

        pd.NickName = nickname;
        return true;
    }
    public void SendData()
    {
        using (StreamWriter sw = new StreamWriter(new FileStream(filePath, FileMode.Append))) 
        {
            sw.WriteLine(pd.NickName.ToString() + spaceWord + Money.ToString() + spaceWord + pd.PlaytimeOfLastSale.ToString());
            
        }
    }
    private void UpdatePlayTime()
    {
        pd.PlaytimeOfLastSale += Time.time - lastTime;
    }
    public static List<PD> ReadLogPD()
    {
        var result = new List<PD>();
        // 데이터 읽기
        using (StreamReader sr = new StreamReader(new FileStream(filePath, FileMode.OpenOrCreate)))
        {
            while (sr.Peek() >= 0)
            {
                var line = sr.ReadLine();
                if (line != "")
                {
                    PD temp = new PD();
                    var playerData = line.Split();
                    temp.NickName = playerData[0];
                    temp.Money = float.Parse(playerData[1]);
                    temp.PlaytimeOfLastSale = float.Parse(playerData[2]);
                    result.Add(temp);
                }
            }
        }
        return result;
    }
}
public struct PD
{
    public string NickName;
    public float Money;
    public float PlaytimeOfLastSale;
}