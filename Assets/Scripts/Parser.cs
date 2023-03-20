using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using HtmlAgilityPack;
using TMPro;
using UnityEngine.UI;

public class Parser : MonoBehaviour
{
    [SerializeField] private Transform placeholder;
    [SerializeField] private Image prefab;
    [SerializeField] private TMP_Text textCount;
    [SerializeField] private TMP_Text textResult;
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private Button button;

    private readonly List<Image> _imagesFromSite = new();
    private List<string> _imagesUrlsList = new();
    private string _path;

    private void Awake()
    {
        button.onClick.AddListener(TryLoadAndShowImages);
    }

    private void TryLoadAndShowImages()
    {
        if (string.IsNullOrEmpty(inputField.text))
        {
            textCount.text = "can`t be null";
            return;
        }

        ClearPreviousSearch();
        GetImages(inputField.text);
    }

    private void ClearPreviousSearch()
    {
        foreach (var im in _imagesFromSite)
        {
            Destroy(im.gameObject);
        }
        _imagesFromSite.Clear();
    }

    private void GetImages(string url)
    {
        var document = new HtmlWeb().Load(url);
        var urls = document.DocumentNode.Descendants("img")
            .Select(e => e.GetAttributeValue("src", null))
            .Where(s => !String.IsNullOrEmpty(s));

        _imagesUrlsList = urls.ToList();
        

        textCount.text = urls.Count().ToString();

        for (int i = 0; i < _imagesUrlsList.Count; i++)
        {
            _imagesFromSite.Add(Instantiate(prefab, placeholder));
        }

        SetImages();
    }

    private void SetImages()
    {
        for (int i = 0; i < _imagesFromSite.Count; i++)
        {
            var i1 = i;

            _path = _imagesUrlsList[i].StartsWith("https") ? "" + _imagesUrlsList[i] : "https:" + _imagesUrlsList[i];

            Davinci.get().
                load(_path).
                into(_imagesFromSite[i1]).
                withErrorAction(s => textResult.text = s).
                withDownloadedAction(() => textResult.text ="Success").
                start();
        }
    }
}
