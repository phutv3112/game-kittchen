using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScore : MonoBehaviour
{
    public ulong playerId;    // Thêm playerId vào lớp PlayerScore
    public string playerName;
    public int score;

    public PlayerScore(ulong playerId, string playerName, int score)
    {
        this.playerId = playerId;
        this.playerName = playerName;
        this.score = score;
    }
}
