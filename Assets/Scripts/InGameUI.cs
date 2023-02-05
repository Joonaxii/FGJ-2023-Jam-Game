using System;
using TMPro;

[System.Serializable]
public class InGameUI
{
    public TextMeshProUGUI bitAmountText;
    public TextMeshProUGUI scanTimeText;
    public TextMeshProUGUI scanTimeFullText;
    public TextMeshProUGUI detectionsText;

    public void UpdateBitAmount(int bits, int max, float bps, int minus)
    {
        if(minus > 0)
        {
            bitAmountText.text = $"Bits - {bits} (<color=#FF0000>-{minus}</color>) /{max} ({bps:F1} bps)";
            return;
        }

        bitAmountText.text = $"Bits - {bits} / {max} ({bps:F1} bps)";
    }

    public void UpdateScanTime(float nextScan, float corruption)
    {
        if (nextScan <= 0)
        {
            scanTimeText.text = $"<color=#00F0FF>Quick scan in progress...</color> ({corruption * 100.0f:F1}% infected!)";
            return;
        }
        string color = "FFFFFF";

        if(nextScan < 2.0f)
        {
            color = "FF0000";
        }
        else if(nextScan < 4.0f)
        {
            color = "F0F000";
        }

        TimeSpan span = TimeSpan.FromSeconds(nextScan);
        scanTimeText.text = $"<color=#{color}>Next quick scan - {span.ToString(@"mm\:ss")}</color> ({corruption * 100.0f:F1}% infected!)";
    }

    public void UpdateFullScanTime(float nextScan)
    {
        if (nextScan <= 0)
        {
            scanTimeFullText.text = $"<color=#FF0000>You're boned!</color>";
            return;
        }
        string color = "FFFFFF";

        if (nextScan < 30.0f)
        {
            color = "FFA000";
        }
        else if (nextScan < 4.0f)
        {
            color = "FFF00";
        }

        TimeSpan span = TimeSpan.FromSeconds(nextScan);
        scanTimeFullText.text = $"<color=#{color}>Full scan - {span.ToString(@"mm\:ss")}</color>";
    }

    public void UpdateDetections(int detections, int max)
    {
        if (detections > max)
        {
            detectionsText.text = $"<color=#FF0000>!!!You've been found!!!</color>";
            return;
        }

        if (detections == max)
        {
            detectionsText.text = $"<color=#FFFF00>!!Danger - Danger!!</color>";
            return;
        }

        detectionsText.text = $"Detections - {detections}/{max}";
    }
}