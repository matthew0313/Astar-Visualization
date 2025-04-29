using System;
using UnityEngine;

public static class Settings
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
    static void Init()
    {
        m_language = Language.Korean;
    }

    static Language m_language;
    public static Language language
    {
        get => m_language;
        set
        {
            if (m_language == value) return;
            m_language = value;
            onLanguageChange?.Invoke();
        }
    }
    public static Action onLanguageChange;
}