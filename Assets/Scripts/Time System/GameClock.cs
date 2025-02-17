using TMPro;
using UnityEngine;

public class GameClock : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timeText = null;
    [SerializeField] private TextMeshProUGUI dateText = null;
    [SerializeField] private TextMeshProUGUI SeasonText = null;
    [SerializeField] private TextMeshProUGUI yearText = null;

    private void OnEnable()
    {
        EventHandler.AdvanceGameMinuteEvent += UpdateGameTime;
    }

   

    private void OnDisable()
    {
        EventHandler.AdvanceGameMinuteEvent -= UpdateGameTime;
    }

    private void UpdateGameTime(int y, Season season, int d, string dow, int h, int m, int second)
    {
        m = m - (m % 10);

        string ampm = "";
        string minute;

        if(h >= 12)
        {
            ampm = " pm";
        }
        else
        {
            ampm = " am";
        }

        if(h >= 13)
        {
            h -= 12;
        }
        if(m < 10)
        {
            minute = "0" + h.ToString();
        }
        else
        {
            minute = m.ToString();
        }

        string time = h.ToString() + ":" + minute + ampm;

        timeText.SetText(time);
        dateText.SetText(dow + ". " + d.ToString());
        SeasonText.SetText(season.ToString());
        yearText.SetText("Year" + y);
    }
}
