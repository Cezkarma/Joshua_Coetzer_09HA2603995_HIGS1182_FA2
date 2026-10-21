using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HUDManager : MonoBehaviour
{
    [Tooltip("Small text that lists recent events, e.g. '+1 Scrap' or 'Enemy hit'.")]
    [SerializeField] private TMP_Text messageText;
    [Tooltip("Large centred text for big moments, e.g. 'Wave 2'.")]
    [SerializeField] private TMP_Text announcementText;

    [Header("Timing")]
    [Tooltip("How many event messages can be on screen at once. The oldest is dropped first.")]
    [SerializeField] private int maxMessages = 4;
    [SerializeField] private float messageDuration = 2f;
    [SerializeField] private float announcementDuration = 3f;

    private static HUDManager instance;

    private readonly List<string> messages = new List<string>();
    private Coroutine announcementRoutine;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Debug.LogWarning("HUDManager: more than one in the scene, so this one is ignored.", this);
            enabled = false;
            return;
        }

        instance = this;

        if (messageText == null || announcementText == null)
        {
            Debug.LogWarning("HUDManager: assign both the message and announcement texts, otherwise some feedback will not show.", this);
        }

        SetText(messageText, string.Empty);
        SetText(announcementText, string.Empty);
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }

    public static void ShowMessage(string message)
    {
        if (instance != null)
        {
            instance.AddMessage(message);
        }
    }

    public static void ShowMessage(string message, Color colour)
    {
        ShowMessage(Colourise(message, colour));
    }

    public static void ShowAnnouncement(string announcement)
    {
        if (instance != null)
        {
            instance.Announce(announcement);
        }
    }

    private void AddMessage(string message)
    {
        messages.Add(message);

        if (messages.Count > maxMessages)
        {
            messages.RemoveAt(0);
        }

        RefreshMessages();
        StartCoroutine(ExpireMessage(message));
    }

    private IEnumerator ExpireMessage(string message)
    {
        yield return new WaitForSeconds(messageDuration);

        messages.Remove(message);
        RefreshMessages();
    }

    private void RefreshMessages()
    {
        SetText(messageText, string.Join("\n", messages));
    }

    private void Announce(string announcement)
    {
        if (announcementRoutine != null)
        {
            StopCoroutine(announcementRoutine);
        }

        announcementRoutine = StartCoroutine(AnnouncementRoutine(announcement));
    }

    private IEnumerator AnnouncementRoutine(string announcement)
    {
        SetText(announcementText, announcement);
        yield return new WaitForSeconds(announcementDuration);
        SetText(announcementText, string.Empty);

        announcementRoutine = null;
    }

    private static void SetText(TMP_Text label, string text)
    {
        if (label != null)
        {
            label.text = text;
        }
    }

    private static string Colourise(string text, Color colour)
    {
        return $"<color=#{ColorUtility.ToHtmlStringRGB(colour)}>{text}</color>";
    }
}
