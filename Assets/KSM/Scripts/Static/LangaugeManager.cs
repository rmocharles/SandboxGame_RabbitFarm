using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class LangaugeManager : MonoBehaviour
{

    //언어 시트
    private const string langURL = "https://docs.google.com/spreadsheets/d/1GwoBBv2NMYSWTo_VgDINOyslpUTg_axrZK4VywcAyDM/export?format=tsv";
    public int curLangIndex;
    public List<Lang> Langs;

    public class Lang
    {
        public string lang, langLocalize;
        public List<string> value = new List<string>();
    }

    private IEnumerator GetLangCo()
    {
        UnityWebRequest www = UnityWebRequest.Get(langURL);
        yield return www.SendWebRequest();
        try
        {
            Debug.LogWarning("언어 가져오기 성공");
            SetLangList(www.downloadHandler.text);
        }
        catch (Exception e)
        {
            
        }
    }

    private void SetLangList(string tsv)
    {
        string[] row = tsv.Split('\n');
        int rowSize = row.Length;
        int columnSize = row[0].Split('\t').Length;
        string[,] sentence = new string[rowSize, columnSize];

        for (int i = 0; i < rowSize; i++)
        {
            string[] column = row[i].Split('\t');
            for (int j = 0; j < columnSize; j++)
                sentence[i, j] = column[j];
        }
        
        //클래스 리스트
        Langs = new List<Lang>();
        for (int i = 0; i < columnSize; i++)
        {
            Lang lang = new Lang();
            lang.lang = sentence[0, i];
            lang.langLocalize = sentence[1, i];
            
            for(int j = 2; j < rowSize; j++)
                lang.value.Add(sentence[j, i]);
            Langs.Add(lang);
        }
        InitLang();
    }

    private void InitLang()
    {
        if (!PlayerPrefs.HasKey("LangIndex"))
        {
            curLangIndex = Application.systemLanguage == SystemLanguage.Korean ? 0 : 1;
            PlayerPrefs.SetInt("LangIndex", curLangIndex);
        }
        else
            curLangIndex = PlayerPrefs.GetInt("LangIndex");
    }

    public void Initialize()
    {
        StartCoroutine(GetLangCo());
    }

    public string Localize(string key)
    {
        int keyIndex = Langs[0].value.FindIndex(x => x.ToLower() == key.ToLower());
        return Langs[curLangIndex].value[keyIndex];
    }

    public string Localize(int keyIndex)
    {
        return Langs[curLangIndex].value[keyIndex];
    }

    public void SwitchLangauge(int code)
    {
        curLangIndex = code;
        PlayerPrefs.SetInt("LangIndex", curLangIndex);
    }
}
