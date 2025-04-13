using UnityEngine;
using TMPro;
using System.Threading.Tasks;
using System.Threading;
using CustomInspector;

[System.Serializable]
public class TimerUtilities
{
    [SerializeField] public bool showDecimal;
    [SerializeField] public TextMeshProUGUI textReference;
    [SerializeField] public string addonText;
    Color textStartColor;

    // holds the time reference for the type of time , it could be elapsedTime or timeLeft
    [Resettable] float timeReference;
    [Resettable][field:SerializeField][ReadOnly] public  float elapsedTime { get; private set; }
    [Resettable] float targetStopTime;
    [Resettable][field:SerializeField][ReadOnly] public float timeLeft { get; private set; }
    [Resettable][field: SerializeField] public bool StopWatchEnabled { get; private set; }
    [Resettable][field: SerializeField] public bool CountDownEnabled { get; private set; }

    CancellationTokenSource cts = new CancellationTokenSource();

    public TimerUtilities()
    {

    }

    public TimerUtilities(bool _showDecimal, TextMeshProUGUI _textReference, string _addonText = "")
    {
        showDecimal = _showDecimal;
        textReference = _textReference;
        addonText = _addonText;
    }

    public void InitializeUnlimitedStopWatch()
    {
        StopWatchEnabled = true;
        elapsedTime = 0;
        textStartColor = textReference is not null ? textReference.color : Color.white;
    }

    public void InitializeNormalStopWatch(float _targetStopTime)
    {
        StopWatchEnabled = true;
        elapsedTime = 0;
        targetStopTime = _targetStopTime;
        textStartColor = textReference is not null ? textReference.color : Color.white;
    }

    public void InitializeCountDownTimer(float _timeLeft)
    {
        timeLeft = _timeLeft;
        textStartColor = textReference is not null ? textReference.color : Color.white;
        CountDownEnabled = true;
    }

    public float UpdateUnlimitedStopWatch(bool updateText =true)
    {
        if (StopWatchEnabled == false) { return elapsedTime; }
        elapsedTime += Time.deltaTime;
        timeReference = elapsedTime;
        if(!updateText) return elapsedTime;
        SetTimerText(textReference, elapsedTime);
        return elapsedTime;
    }

    public (float elapsedTime, bool isRunning) UpdateStopWatch(bool _resetWhenDone)
    {
        if (elapsedTime <= targetStopTime && StopWatchEnabled)
        {
            elapsedTime += Time.deltaTime;
            timeReference = elapsedTime;
            SetTimerText(textReference, elapsedTime);
            return (elapsedTime, StopWatchEnabled);
        }
        else
        { EndStopWatch(_resetWhenDone); }

        return (elapsedTime, StopWatchEnabled);
    }

    public float EndStopWatch(bool resetElapsedTime)
    {
        cts.Cancel();
        StopWatchEnabled = false;
        elapsedTime = resetElapsedTime ? 0 : elapsedTime;
        return elapsedTime;
    }

    public (float timeRemaining, bool isRunning) UpdateCountDown()
    {
        if (timeLeft > 0)
        {
            timeLeft -= Time.deltaTime;
            timeReference = timeLeft;
            SetTimerText(textReference, timeLeft);
            return (timeLeft, true);
        }
        CountDownEnabled = false;
        return (timeLeft, false);
    }

    public async void ChangeTextColor(float colorChangeInterval, Color newColor, float colorChangeDuration)
    {
        if (cts.IsCancellationRequested) return;
        if (textReference is null) return;
        if (textReference.color == newColor) return;
        if ((int)timeReference % colorChangeInterval is not 0 || (int)timeReference is 0) return;

        textReference.color = newColor;

        await Task.Delay((int)(colorChangeDuration * 1000));
        textReference.color = textStartColor;
    }

    void SetTimerText(TextMeshProUGUI _textReference, float _timeReference)
    {
        if (_textReference is null) return;
        _textReference.text = showDecimal ? addonText + _timeReference.ToString("F2") : addonText + _timeReference.ToString("F0");
    }
}
