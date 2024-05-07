using System;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

public static class Utilities
{
    #region NumberConvert

    public static string ConvertToString(int number) => Convert(number);
    public static string ConvertToString(long number) => Convert(number);
    private static string Convert(long number)
    {
        //만 자리 이하일 경우 그냥 반환
        if (number < 1_000)
        {
            return number.ToString();
        }
        string[] numSymbol = { "", "A ", "B  ", "C ", "D ", "E ", "F", "G", "H", "I", "J", "K" };
        int magnitudeIndex = (int)Mathf.Log10(number) / 3;
        StringBuilder sb = new StringBuilder()
        
            .Append((number * Mathf.Pow(0.001f, magnitudeIndex)).ToString("N2"))
            .Append(numSymbol[magnitudeIndex]);

        return sb.ToString();
    }

    #endregion

    #region ColorCollect
    private static string SetTierColor(ItemTier itemTier)
    {
        switch (itemTier)
        {
            case ItemTier.Common:
                return "#C3C2C5";
            case ItemTier.Uncommon:
                return "#93DCC3";
            case ItemTier.Rare:
                return "#56BAF8";
            case ItemTier.epic:
                return "#D500FF";
            case ItemTier.Legendary:
                return "#FFD150";
            default:
                return "등록되지 않은 티어";
        }
    }

    public static Color SetSlotTierColor(ItemTier itemTier)
    {
        ColorUtility.TryParseHtmlString(SetTierColor(itemTier), out Color color);
        return color;
    }

    #endregion
}