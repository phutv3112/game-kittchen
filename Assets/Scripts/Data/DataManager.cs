using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Diagnostics;
using UnityEngine.InputSystem;
using UnityEngine.Playables;
using Pixelplacement;

public class DataManager : Pixelplacement.Singleton<DataManager>
{
    // Start is called before the first frame update
    public int hightScore;
    public void SaveHighScore(int score)
    {
        // Kiểm tra nếu điểm hiện tại lớn hơn high score đã lưu trước đó
        int currentHighScore = PlayerPrefs.GetInt("HighScore", 0);
        if (score > currentHighScore)
        {
            PlayerPrefs.SetInt("HighScore", score); // Lưu high score mới
            PlayerPrefs.Save(); // Lưu lại dữ liệu đã thay đổi
            Debug.Log("New High Score saved: " + score);
        }
    }
    public int LoadHighScore()
    {
        int highScore = PlayerPrefs.GetInt("HighScore", 0); // Lấy high score đã lưu, mặc định là 0 nếu chưa có
        Debug.Log("High Score loaded: " + highScore);
        return highScore;
    }
    public void ResetHighScore()
    {
        PlayerPrefs.DeleteKey("HighScore"); // Xóa high score đã lưu
        PlayerPrefs.Save();
        Debug.Log("High Score reset.");
    }

}
