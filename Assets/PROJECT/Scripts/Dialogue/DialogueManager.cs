using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour, IResettable
{
    public static DialogueManager Instance { get; private set; }
    [Header("References")]
    [SerializeField] CanvasGroup dialogueWindow;
    [SerializeField] Image characterImage;
    [SerializeField] TextMeshProUGUI characterName;
    [SerializeField] TextMeshProUGUI dialogueLine;

    [Header("Dialogue Flow")]
    [SerializeField] float windowFadeTime = 1f;
    [SerializeField] float timePerWord = 0.2f;
    [SerializeField] float waitAfterLine = 2f;
    [SerializeField] RectTransform origin;
    RectTransform windowRect;
    Vector2 startPosition;
    Vector2 endPosition;

    Queue<Line> lineQueue;
    Coroutine dialogue;
    Coroutine subroutine;

    [Resettable] GameObject _ => dialogueWindow.gameObject;

    private void Start()
    {
        if (Instance == null) { Instance = this; } else { Destroy(this); }
        dialogueWindow.gameObject.SetActive(false);
        dialogueWindow.alpha = 0;
        windowRect = dialogueWindow.GetComponent<RectTransform>();
        endPosition = windowRect.anchoredPosition;
        startPosition = origin.anchoredPosition + endPosition;
    }

    public void StartDialogue(Line[] lines)
    {
        if (dialogue != null)
        {
            StopCoroutine(dialogue);
            dialogue = null;
        }
        if (subroutine != null)
        {
            StopCoroutine(subroutine);
            subroutine = null;
        }

        StopAllCoroutines();
        lineQueue = new Queue<Line>(lines);
        dialogue = StartCoroutine(PlayConversation());
    }

    IEnumerator PlayConversation()
    {
        if (dialogueWindow.alpha < 1 || !dialogueWindow.gameObject.activeSelf || Vector2.Distance(windowRect.anchoredPosition, endPosition) > 1f)
        {
            UpdateImage(lineQueue.Peek().Character.Avatar);
            dialogueLine.text = "";
            characterName.text = lineQueue.Peek().Character.CharacterName;
            subroutine = StartCoroutine(ToggleUI(true, dialogueWindow, windowRect, startPosition, endPosition, windowFadeTime));
            yield return subroutine;
        }

        while (lineQueue.Count > 0)
        {
            Line line = lineQueue.Dequeue();
            UpdateImage(line.Character.Avatar);
            characterName.text = line.Character.CharacterName;
            subroutine = StartCoroutine(AnimateLine(line.Message));
            yield return subroutine;
            yield return new WaitForSeconds(waitAfterLine);
        }

        subroutine = StartCoroutine(ToggleUI(false, dialogueWindow, windowRect, startPosition, endPosition, windowFadeTime));
        yield return subroutine;
    }

    void UpdateImage(Sprite sprite)
    {
        characterImage.gameObject.SetActive(sprite != null);
        characterImage.sprite = sprite;
    }

    // Instead of adding to the text character by character, this makes the entire text transparent then fades it in one word at a time.
    IEnumerator AnimateLine(string line)
    {
        dialogueLine.text = line;
        dialogueLine.ForceMeshUpdate();

        SetTextTransparency(dialogueLine);

        string[] words = line.Split(' ');
        int wordStartIndex = 0;
        int wordEndIndex = 0;
        for (int wordIndex = 0; wordIndex < words.Length; wordIndex++)
        {
            wordEndIndex += words[wordIndex].Length;
            float time = 0;

            while (time < timePerWord)
            {
                time += Time.deltaTime;
                byte alpha = (byte)(Mathf.Lerp(0, 1, time / timePerWord) * 255);
                SetTextTransparency(dialogueLine, alpha, wordStartIndex, wordEndIndex);
                yield return null;
            }

            wordStartIndex = wordEndIndex + 1;
            wordEndIndex += 1;
        }
    }

    public static void SetTextTransparency(TextMeshProUGUI dialogueLine, byte alpha = 0, int startIndex = 0, int endIndex = int.MaxValue)
    {
        TMP_TextInfo textInfo = dialogueLine.textInfo;
        endIndex = Mathf.Min(endIndex, textInfo.characterCount - 1);

        for (int i = startIndex; i <= endIndex; i++)
        {
            if (!textInfo.characterInfo[i].isVisible) continue;

            int materialIndex = textInfo.characterInfo[i].materialReferenceIndex;
            Color32[] newVertexColors = textInfo.meshInfo[materialIndex].colors32;
            int vertexIndex = textInfo.characterInfo[i].vertexIndex;

            Color32 color = newVertexColors[vertexIndex];
            color.a = alpha;

            newVertexColors[vertexIndex + 0] = color;
            newVertexColors[vertexIndex + 1] = color;
            newVertexColors[vertexIndex + 2] = color;
            newVertexColors[vertexIndex + 3] = color;
        }

        dialogueLine.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
    }

    public static IEnumerator ToggleUI(bool enable, CanvasGroup window, RectTransform rectTransform, Vector3 startPosition, Vector3 endPosition, float fadeTime)
    {
        if (enable && !window.gameObject.activeSelf) { window.gameObject.SetActive(true); }
        float target = enable ? 1f : 0f;
        float initial = window.alpha;
        Vector3 targetPosition = enable ? endPosition : startPosition;
        Vector3 initialPosition = enable ? startPosition : endPosition;
        float time = 0;
        while (time < fadeTime)
        {
            time += Time.deltaTime;
            window.alpha = Mathf.Lerp(initial, target, time / fadeTime);
            rectTransform.anchoredPosition = Vector3.Lerp(initialPosition, targetPosition, time / fadeTime);
            yield return null;
        }
        window.alpha = target;
        rectTransform.anchoredPosition = targetPosition;
    }
}