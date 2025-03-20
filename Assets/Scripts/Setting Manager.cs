using System.Collections;
using System.Collections.Generic;
using System.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingManager : MonoBehaviour
{
    [SerializeField] RectTransform pageLayout;

    [Header("Setting")]
    [SerializeField] Slider volumeSlider;
    [SerializeField] List<Image> selectedThemeIndicators;

    [Header("FAQ")]
    [SerializeField] List<bool> faqIsOpen = new();
    [SerializeField] List<TextMeshProUGUI> faqArrows = new();
    [SerializeField] List<RectTransform> faqMasks = new();


    void Start()
    {
        // set volume
        volumeSlider.value = AudioManager.instance.AudioSource_SE.volume;
        volumeSlider.onValueChanged.AddListener((float value) => AudioManager.instance.SetSEVolume(value));

        // set theme
        selectedThemeIndicators.ForEach((Image img) => img.enabled = false);
        selectedThemeIndicators[ThemeManager.getThemeIndex()].enabled = true;


        // init faq
        for (int i = 0; i < 5; i++)
            faqIsOpen.Add(false);
    }

    #region Setting
    // change theme color
    public void changeThemeColor(int idx)
    {
        selectedThemeIndicators[ThemeManager.getThemeIndex()].enabled = false;
        ThemeManager.ChangeTheme(idx);
        selectedThemeIndicators[ThemeManager.getThemeIndex()].enabled = true;
    }

    #endregion

    #region FAQ
    // select btn
    public void selectFAQ(int idx)
    {
        faqIsOpen[idx] = !faqIsOpen[idx];
        faqArrows[idx].text = faqIsOpen[idx] ? "v" : "<";
        StartCoroutine(showFAQ(idx));
    }

    IEnumerator showFAQ(int idx)
    {
        float target = faqIsOpen[idx] ? 0 : faqMasks[idx].GetChild(0).GetComponent<RectTransform>().sizeDelta.y;
        float animationSpeed = 10f;
        float speed = (target - faqMasks[idx].sizeDelta.y) / animationSpeed;

        while (Mathf.Abs(faqMasks[idx].sizeDelta.y - target) > 0.1f)
        {
            faqMasks[idx].sizeDelta = new Vector2(faqMasks[idx].sizeDelta.x, faqMasks[idx].sizeDelta.y + speed);
            yield return 0;
        }

        yield return 0;
    }

    #endregion

}
