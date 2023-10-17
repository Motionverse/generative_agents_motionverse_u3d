using MotionverseSDK;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sample : MonoBehaviour
{
    public Player player;

    private void Start()
    {
        player.OnPlayComplete += OnPlayComplete;
        player.OnPlayStart += OnPlayStart;
        player.OnPlayError += OnPlayError;
    }
    public void OnTextDrive(string text)
    {
        DriveTask driveTask = new DriveTask() { player = player, text = text};

        TextDrive.GetDrive(driveTask, false);
    }

    public void OnAudioUrlDrive(string url)
    {

        DriveTask driveTask = new DriveTask() { player = player, text = url };


        AudioUrlDrive.GetDrive(driveTask, false);
    }


    public void OnNLPDrive(string text)
    {
        DriveTask driveTask = new DriveTask() { player = player, text = text };
        NLPDrive.GetDrive(driveTask, false);
    }
    
    public void OnTest()
    {
        player.StopPlay();
    }
    public void OnPlayComplete()
    {
        Debug.Log("OnPlayComplete");
    }
    public void OnPlayStart()
    {
        Debug.Log("OnPlayStart");
    }

    private void OnDestroy()
    {
        player.OnPlayComplete -= OnPlayComplete;
        player.OnPlayStart -= OnPlayStart;
        player.OnPlayError -= OnPlayError;
    }
    public void OnPlayError(string msg)
    {
        Debug.Log($"OnPlayError:{ msg}");

    }
}
