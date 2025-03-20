using System.Collections;
using System.Collections.Generic;
using Unity.Theme;
using UnityEngine;

public static class ThemeManager
{
    public static void ChangeTheme(int idx)
    {
        if (idx >= 0 && idx < Theme.Instance.Themes.Count)
            Theme.Instance.CurrentThemeIndex = idx;

        Debug.Log("change theme to " + Theme.Instance.CurrentThemeIndex);
    }

    public static void ChangeTheme(string themeName)
    {
        for (int i = 0; i < Theme.Instance.Themes.Count; i++)
        {
            if (Theme.Instance.Themes[i].themeName == themeName)
            {
                Theme.Instance.CurrentThemeIndex = i;
                break;
            }
        }
    }

    public static int getThemeIndex() {
        return Theme.Instance.CurrentThemeIndex;        
    }
}