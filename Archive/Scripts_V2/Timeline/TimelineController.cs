using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class TimelineController : MonoBehaviour
{
    public PlayableDirector timeline;

    public void PauseTimeline()
    {
        timeline.Pause();
        DialogueManager.Instance.SetActiveTimeline(this);
    }
    public void ResumeTimeline()
    {
        Debug.Log("Пробую запустить таймлайн");
        timeline.Play();
    }
}
