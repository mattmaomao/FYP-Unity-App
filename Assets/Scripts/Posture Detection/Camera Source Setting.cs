// Copyright (c) 2021 homuler
//
// Use of this source code is governed by an MIT-style
// license that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Mediapipe.Unity.Sample;
using Mediapipe.Unity.Sample.PoseLandmarkDetection;
using TMPro;

public class CaneraSourceSetting : Mediapipe.Unity.Sample.UI.ModalContents
{
    [SerializeField] MyLandmarkerRunner myLandmarkerRunner;
    [SerializeField] GameObject settingPanel;
    [SerializeField] TMP_Dropdown sourceTypeInput;
    [SerializeField] TMP_Dropdown sourceInput;
    [SerializeField] TMP_Dropdown resolutionInput;
    [SerializeField] Toggle isHorizontallyFlippedInput;

    bool isChanged;

    void Start()
    {
        InitializeContents();
        settingPanel.SetActive(false);
    }

    void InitializeContents()
    {
        InitializeSourceType();
        InitializeSource();
        InitializeResolution();
        InitializeIsHorizontallyFlipped();
        isChanged = false;
    }

    #region init settings
    void InitializeSourceType()
    {
        sourceTypeInput.ClearOptions();
        sourceTypeInput.onValueChanged.RemoveAllListeners();

        var options = Enum.GetNames(typeof(ImageSourceType)).Where(x => x != ImageSourceType.Unknown.ToString()).ToList();
        sourceTypeInput.AddOptions(options);

        var currentSourceType = ImageSourceProvider.CurrentSourceType;
        var defaultValue = options.FindIndex(option => option == currentSourceType.ToString());

        if (defaultValue >= 0)
        {
            sourceTypeInput.value = defaultValue;
        }

        sourceTypeInput.onValueChanged.AddListener(delegate
        {
            ImageSourceProvider.Switch((ImageSourceType)sourceTypeInput.value);
            isChanged = true;
            InitializeContents();
        });
    }

    void InitializeSource()
    {
        sourceInput.ClearOptions();
        sourceInput.onValueChanged.RemoveAllListeners();

        var imageSource = ImageSourceProvider.ImageSource;
        var sourceNames = imageSource.sourceCandidateNames;

        if (sourceNames == null)
        {
            sourceInput.enabled = false;
            return;
        }

        var options = new List<string>(sourceNames);
        sourceInput.AddOptions(options);

        var currentSourceName = imageSource.sourceName;
        var defaultValue = options.FindIndex(option => option == currentSourceName);

        if (defaultValue >= 0)
        {
            sourceInput.value = defaultValue;
        }

        sourceInput.onValueChanged.AddListener(delegate
        {
            imageSource.SelectSource(sourceInput.value);
            isChanged = true;
            InitializeResolution();
        });
    }

    void InitializeResolution()
    {
        resolutionInput.ClearOptions();
        resolutionInput.onValueChanged.RemoveAllListeners();

        var imageSource = ImageSourceProvider.ImageSource;
        var resolutions = imageSource.availableResolutions;

        if (resolutions == null)
        {
            resolutionInput.enabled = false;
            return;
        }

        var options = resolutions.Select(resolution => resolution.ToString()).ToList();
        resolutionInput.AddOptions(options);

        var currentResolutionStr = imageSource.resolution.ToString();
        var defaultValue = options.FindIndex(option => option == currentResolutionStr);

        if (defaultValue >= 0)
        {
            resolutionInput.value = defaultValue;
        }

        resolutionInput.onValueChanged.AddListener(delegate
        {
            imageSource.SelectResolution(resolutionInput.value);
            isChanged = true;
        });
    }

    void InitializeIsHorizontallyFlipped()
    {
        var imageSource = ImageSourceProvider.ImageSource;
        isHorizontallyFlippedInput.isOn = imageSource.isHorizontallyFlipped;
        isHorizontallyFlippedInput.onValueChanged.AddListener(delegate
        {
            imageSource.isHorizontallyFlipped = isHorizontallyFlippedInput.isOn;
            isChanged = true;
        });
    }
    #endregion

    #region buttons
    public void toggleSettingPanel()
    {
        if (!settingPanel.activeSelf)
            myLandmarkerRunner.Pause();
        else
        {
            if (isChanged)
                myLandmarkerRunner.Play();
            else
                myLandmarkerRunner.Resume();
        }
        isChanged = false;

        settingPanel.SetActive(!settingPanel.activeSelf);
    }
    #endregion
}
