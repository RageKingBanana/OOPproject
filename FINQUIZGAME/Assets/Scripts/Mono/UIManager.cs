using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[Serializable()]
public struct UIManagerParameters
{
    [Header("Answers Options")]
    [SerializeField] float margins;
    public float Margins { get { return margins; } }

    [Header("Resolution Screen Options")]
    [SerializeField] Color correctBGColor;
    public Color CorrectBGColor { get { return correctBGColor; } }
    [SerializeField] Color incorrectBGColor;
    public Color IncorrectBGColor { get { return incorrectBGColor; } }
    [SerializeField] Color finalBGColor;
    public Color FinalBGColor { get { return finalBGColor; } }
}
[Serializable()]
public struct UIElements
{
    [SerializeField] RectTransform answersContentArea;
    public RectTransform AnswersContentArea { get { return answersContentArea; } }

    [SerializeField] Image questionInfoImage;//image
    public Image QuestionInfoImage { get { return questionInfoImage; } }

    [SerializeField] TextMeshProUGUI questionInfoTextObject;
    public TextMeshProUGUI QuestionInfoTextObject { get { return questionInfoTextObject; } }

    [SerializeField] TextMeshProUGUI scoreText;
    public TextMeshProUGUI ScoreText { get { return scoreText; } }

    [Space]

    [SerializeField] Animator resolutionScreenAnimator;
    public Animator ResolutionScreenAnimator { get { return resolutionScreenAnimator; } }

    [SerializeField] Image resolutionBG;
    public Image ResolutionBG { get { return resolutionBG; } }

    [SerializeField] TextMeshProUGUI resolutionStateInfoText;
    public TextMeshProUGUI ResolutionStateInfoText { get { return resolutionStateInfoText; } }

    [SerializeField] TextMeshProUGUI resolutionScoreText;
    public TextMeshProUGUI ResolutionScoreText { get { return resolutionScoreText; } }

    [Space]

    [SerializeField] TextMeshProUGUI highScoreText;
    public TextMeshProUGUI HighScoreText { get { return highScoreText; } }

    [SerializeField] CanvasGroup mainCanvasGroup;
    public CanvasGroup MainCanvasGroup { get { return mainCanvasGroup; } }

    [SerializeField] RectTransform finishUIElements;
    public RectTransform FinishUIElements { get { return finishUIElements; } }
}
public class UIManager : MonoBehaviour {

    #region Variables

    public enum         ResolutionScreenType   { Correct, Incorrect, Finish }

    [Header("References")]
    [SerializeField]    GameEvents             events                       = null;

    [Header("UI Elements (Prefabs)")]
    [SerializeField]    AnswerData             answerPrefab                 = null;

    [SerializeField]    UIElements             uIElements                   = new UIElements();

    [Space]
    [SerializeField]    UIManagerParameters    parameters                   = new UIManagerParameters();

    private             List<AnswerData>       currentAnswers               = new List<AnswerData>();
    private             int                    resStateParaHash             = 0;

    private             IEnumerator            IE_DisplayTimedResolution    = null;
    public int AnsQues;
    private bool pressedinc = false;

    #endregion

    #region Default Unity methods

    /// <summary>
    /// Function that is called when the object becomes enabled and active
    /// </summary>
    void OnEnable()
    {
        events.UpdateQuestionUI         += UpdateQuestionUI;
        events.DisplayResolutionScreen  += DisplayResolution;
        events.ScoreUpdated             += UpdateScoreUI;
    }
    /// <summary>
    /// Function that is called when the behaviour becomes disabled
    /// </summary>
    void OnDisable()
    {
        events.UpdateQuestionUI         -= UpdateQuestionUI;
        events.DisplayResolutionScreen  -= DisplayResolution;
        events.ScoreUpdated             -= UpdateScoreUI;
    }

    /// <summary>
    /// Function that is called when the script instance is being loaded.
    /// </summary>
    void Start()
    {
        UpdateScoreUI();
        resStateParaHash = Animator.StringToHash("ScreenState");
    }

    #endregion

    /// <summary>
    /// Function that is used to update new question UI information.
    /// </summary>
    void UpdateQuestionUI(Question question)
    {
        uIElements.QuestionInfoImage.sprite = question.InfoImage;
        uIElements.QuestionInfoTextObject.text = question.Info;
        CreateAnswers(question);
    }
    /// <summary>
    /// Function that is used to display resolution screen.
    /// </summary>
    void DisplayResolution(ResolutionScreenType type, int score)
    {
        UpdateResUI(type, score);
        pressedinc = false;
        uIElements.ResolutionScreenAnimator.SetInteger(resStateParaHash, 2);
        uIElements.MainCanvasGroup.blocksRaycasts = false;
        if (type != ResolutionScreenType.Finish)
        {
            
            if (IE_DisplayTimedResolution != null)
            {
                StopCoroutine(IE_DisplayTimedResolution);
            }
            IE_DisplayTimedResolution = DisplayTimedResolution();
            StartCoroutine(IE_DisplayTimedResolution);
        }
    }
    IEnumerator DisplayTimedResolution()
    {
        yield return new WaitForSeconds(GameUtility.ResolutionDelayTime);
        while (!pressedinc)
        {
            if (Input.touchCount > 0 || Input.GetMouseButton(0))
                pressedinc = true;
            yield return null;
        }
        uIElements.ResolutionScreenAnimator.SetInteger(resStateParaHash, 1);
        uIElements.MainCanvasGroup.blocksRaycasts = true;
    }

    /// <summary>
    /// Function that is used to display resolution UI information.
    /// </summary>
    void UpdateResUI(ResolutionScreenType type, int score)
    {
        var highscore = PlayerPrefs.GetInt(GameUtility.SavePrefKey);
        int i;
        AnsQues = PlayerPrefs.GetInt("QuesAns");
        i = AnsQues;
        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            switch (type)
            {
                case ResolutionScreenType.Correct:
                    uIElements.ResolutionBG.color = parameters.CorrectBGColor;
                    if (i == 0)
                    {
                        uIElements.ResolutionStateInfoText.text = "The natural causes of earthquakes are sliding of tectonic plates and volcanic activities.";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (i == 9)
                    {
                        uIElements.ResolutionStateInfoText.text = "Learning simple first aid techniques can be very advantageous. ";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (i == 8)
                    {
                        uIElements.ResolutionStateInfoText.text = "Freezed Ham is not included because it is needed to be processed first in order to eat it.";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (i == 7)
                    {
                        uIElements.ResolutionStateInfoText.text = "Spare Batteries and Flashlight are included in the emergency kit.";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (i == 6)
                    {
                        uIElements.ResolutionStateInfoText.text = "The Philippines is considered to be an earthquake prone country because it is located near the Pacific Ring of Fire.";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (i == 5)
                    {
                        uIElements.ResolutionStateInfoText.text = "“Batingaw” is the mobile application developed by NDRRMC for public use.";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (i == 4)
                    {
                        uIElements.ResolutionStateInfoText.text = "National Disaster Risk Reduction Management Council is the agency that is responsible for ensuring the protection and welfare of the people.";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (i == 3)
                    {
                        uIElements.ResolutionStateInfoText.text = "Philippine Institute of Volcanology and Seismology is the agency responsible for mitigation of disasters that arises from geotectonic phenomenas like volcanic eruptions, earthquakes, and tsunamis.";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (i == 2)
                    {
                        uIElements.ResolutionStateInfoText.text = "Philippine Atmospheric, Geophysical and Astronomical Services Administration is the agency responsible for assessing and forecasting weather, flood, and other conditions essential for the welfare of the people.";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (i == 1)
                    {
                        uIElements.ResolutionStateInfoText.text = "Aftershocks, Tsunamis, Landslides are all possible effects of earthquakes.";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    break;
                case ResolutionScreenType.Incorrect:
                    uIElements.ResolutionBG.color = parameters.IncorrectBGColor;
                    if (i == 0)
                    {
                        uIElements.ResolutionStateInfoText.text = "The natural causes of earthquakes are sliding of tectonic plates and volcanic activities.";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (i == 9)
                    {
                        uIElements.ResolutionStateInfoText.text = "Learning simple first aid techniques can be very advantageous. ";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (i == 8)
                    {
                        uIElements.ResolutionStateInfoText.text = "Freezed Ham is not included because it is needed to be processed first in order to eat it.";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (i == 7)
                    {
                        uIElements.ResolutionStateInfoText.text = "Spare Batteries and Flashlight are included in the emergency kit.";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (i == 6)
                    {
                        uIElements.ResolutionStateInfoText.text = "The Philippines is considered to be an earthquake prone country because it is located near the Pacific Ring of Fire.";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (i == 5)
                    {
                        uIElements.ResolutionStateInfoText.text = "“Batingaw” is the mobile application developed by NDRRMC for public use.";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (i == 4)
                    {
                        uIElements.ResolutionStateInfoText.text = "National Disaster Risk Reduction Management Council is the agency that is responsible for ensuring the protection and welfare of the people.";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (i == 3)
                    {
                        uIElements.ResolutionStateInfoText.text = "Philippine Institute of Volcanology and Seismology is the agency responsible for mitigation of disasters that arises from geotectonic phenomenas like volcanic eruptions, earthquakes, and tsunamis.";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (i == 2)
                    {
                        uIElements.ResolutionStateInfoText.text = "Philippine Atmospheric, Geophysical and Astronomical Services Administration is the agency responsible for assessing and forecasting weather, flood, and other conditions essential for the welfare of the people.";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (i == 1)
                    {
                        uIElements.ResolutionStateInfoText.text = "Aftershocks, Tsunamis, Landslides are all possible effects of earthquakes.";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    break;
                case ResolutionScreenType.Finish:
                    uIElements.ResolutionBG.color = parameters.FinalBGColor;
                    uIElements.ResolutionStateInfoText.text = "FINAL SCORE";

                    StartCoroutine(CalculateScore());
                    uIElements.FinishUIElements.gameObject.SetActive(true);
                    uIElements.HighScoreText.gameObject.SetActive(true);
                    uIElements.HighScoreText.text = ((highscore > events.StartupHighscore) ? "<color=yellow>new </color>" : string.Empty) + "Highscore: " + highscore;
                    break;
            }
        }
        else if (SceneManager.GetActiveScene().buildIndex == 2)
        {
            switch (type)
            {
                case ResolutionScreenType.Correct:
                    uIElements.ResolutionBG.color = parameters.CorrectBGColor;
                    if (i == 0)
                    {
                        uIElements.ResolutionStateInfoText.text = "zero";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (i == 14)
                    {
                        uIElements.ResolutionStateInfoText.text = "nine";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (i == 13)
                    {
                        uIElements.ResolutionStateInfoText.text = "nine";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (i == 12)
                    {
                        uIElements.ResolutionStateInfoText.text = "nine";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (i == 11)
                    {
                        uIElements.ResolutionStateInfoText.text = "nine";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (i == 10)
                    {
                        uIElements.ResolutionStateInfoText.text = "nine";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (i == 9)
                    {
                        uIElements.ResolutionStateInfoText.text = "nine";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (i == 8)
                    {
                        uIElements.ResolutionStateInfoText.text = "eight";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (i == 7)
                    {
                        uIElements.ResolutionStateInfoText.text = "seven";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (i == 6)
                    {
                        uIElements.ResolutionStateInfoText.text = "six";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (i == 5)
                    {
                        uIElements.ResolutionStateInfoText.text = "five";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (i == 4)
                    {
                        uIElements.ResolutionStateInfoText.text = "four";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (i == 3)
                    {
                        uIElements.ResolutionStateInfoText.text = "three";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (i == 2)
                    {
                        uIElements.ResolutionStateInfoText.text = "two";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (i == 1)
                    {
                        uIElements.ResolutionStateInfoText.text = "one";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    break;
                case ResolutionScreenType.Incorrect:
                    uIElements.ResolutionBG.color = parameters.IncorrectBGColor;
                    if (i == 0)
                    {
                        uIElements.ResolutionStateInfoText.text = "zero";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (i == 14)
                    {
                        uIElements.ResolutionStateInfoText.text = "nine";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (i == 13)
                    {
                        uIElements.ResolutionStateInfoText.text = "nine";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (i == 12)
                    {
                        uIElements.ResolutionStateInfoText.text = "nine";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (i == 11)
                    {
                        uIElements.ResolutionStateInfoText.text = "nine";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (i == 10)
                    {
                        uIElements.ResolutionStateInfoText.text = "nine";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (i == 9)
                    {
                        uIElements.ResolutionStateInfoText.text = "nine";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (i == 8)
                    {
                        uIElements.ResolutionStateInfoText.text = "eight";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (i == 7)
                    {
                        uIElements.ResolutionStateInfoText.text = "seven";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (i == 6)
                    {
                        uIElements.ResolutionStateInfoText.text = "six";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (i == 5)
                    {
                        uIElements.ResolutionStateInfoText.text = "five";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (i == 4)
                    {
                        uIElements.ResolutionStateInfoText.text = "four";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (i == 3)
                    {
                        uIElements.ResolutionStateInfoText.text = "three";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (i == 2)
                    {
                        uIElements.ResolutionStateInfoText.text = "two";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (i == 1)
                    {
                        uIElements.ResolutionStateInfoText.text = "one";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    break;
                case ResolutionScreenType.Finish:
                    uIElements.ResolutionBG.color = parameters.FinalBGColor;
                    uIElements.ResolutionStateInfoText.text = "FINAL SCORE";

                    StartCoroutine(CalculateScore());
                    uIElements.FinishUIElements.gameObject.SetActive(true);
                    uIElements.HighScoreText.gameObject.SetActive(true);
                    uIElements.HighScoreText.text = ((highscore > events.StartupHighscore) ? "<color=yellow>new </color>" : string.Empty) + "Highscore: " + highscore;
                    break;
            }
        }
        else if (SceneManager.GetActiveScene().buildIndex == 3)
        {
            switch (type)
            {
                case ResolutionScreenType.Correct:
                    uIElements.ResolutionBG.color = parameters.CorrectBGColor;
                    if (i == 0)
                    {
                        uIElements.ResolutionStateInfoText.text = "zero";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (i == 14)
                    {
                        uIElements.ResolutionStateInfoText.text = "nine";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (i == 13)
                    {
                        uIElements.ResolutionStateInfoText.text = "nine";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (i == 12)
                    {
                        uIElements.ResolutionStateInfoText.text = "nine";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (i == 11)
                    {
                        uIElements.ResolutionStateInfoText.text = "nine";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (i == 10)
                    {
                        uIElements.ResolutionStateInfoText.text = "nine";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (i == 9)
                    {
                        uIElements.ResolutionStateInfoText.text = "nine";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (i == 8)
                    {
                        uIElements.ResolutionStateInfoText.text = "eight";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (i == 7)
                    {
                        uIElements.ResolutionStateInfoText.text = "seven";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (i == 6)
                    {
                        uIElements.ResolutionStateInfoText.text = "six";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (i == 5)
                    {
                        uIElements.ResolutionStateInfoText.text = "five";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (i == 4)
                    {
                        uIElements.ResolutionStateInfoText.text = "four";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (i == 3)
                    {
                        uIElements.ResolutionStateInfoText.text = "three";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (i == 2)
                    {
                        uIElements.ResolutionStateInfoText.text = "two";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (i == 1)
                    {
                        uIElements.ResolutionStateInfoText.text = "one";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    break;
                case ResolutionScreenType.Incorrect:
                    uIElements.ResolutionBG.color = parameters.IncorrectBGColor;
                    if (i == 0)
                    {
                        uIElements.ResolutionStateInfoText.text = "zero";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (i == 14)
                    {
                        uIElements.ResolutionStateInfoText.text = "nine";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (i == 13)
                    {
                        uIElements.ResolutionStateInfoText.text = "nine";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (i == 12)
                    {
                        uIElements.ResolutionStateInfoText.text = "nine";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (i == 11)
                    {
                        uIElements.ResolutionStateInfoText.text = "nine";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (i == 10)
                    {
                        uIElements.ResolutionStateInfoText.text = "nine";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (i == 9)
                    {
                        uIElements.ResolutionStateInfoText.text = "nine";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (i == 8)
                    {
                        uIElements.ResolutionStateInfoText.text = "eight";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (i == 7)
                    {
                        uIElements.ResolutionStateInfoText.text = "seven";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (i == 6)
                    {
                        uIElements.ResolutionStateInfoText.text = "six";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (i == 5)
                    {
                        uIElements.ResolutionStateInfoText.text = "five";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (i == 4)
                    {
                        uIElements.ResolutionStateInfoText.text = "four";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (i == 3)
                    {
                        uIElements.ResolutionStateInfoText.text = "three";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (i == 2)
                    {
                        uIElements.ResolutionStateInfoText.text = "two";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (i == 1)
                    {
                        uIElements.ResolutionStateInfoText.text = "one";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    break;
                case ResolutionScreenType.Finish:
                    uIElements.ResolutionBG.color = parameters.FinalBGColor;
                    uIElements.ResolutionStateInfoText.text = "FINAL SCORE";

                    StartCoroutine(CalculateScore());
                    uIElements.FinishUIElements.gameObject.SetActive(true);
                    uIElements.HighScoreText.gameObject.SetActive(true);
                    uIElements.HighScoreText.text = ((highscore > events.StartupHighscore) ? "<color=yellow>new </color>" : string.Empty) + "Highscore: " + highscore;
                    break;
            }
        }
        if (SceneManager.GetActiveScene().buildIndex == 4)
        {
            switch (type)
            {
                case ResolutionScreenType.Correct:
                    uIElements.ResolutionBG.color = parameters.CorrectBGColor;
                    if (i == 0)
                    {
                        uIElements.ResolutionStateInfoText.text = "zero";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (i == 19)
                    {
                        uIElements.ResolutionStateInfoText.text = "nine";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (i == 18)
                    {
                        uIElements.ResolutionStateInfoText.text = "nine";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (i == 17)
                    {
                        uIElements.ResolutionStateInfoText.text = "nine";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (i == 16)
                    {
                        uIElements.ResolutionStateInfoText.text = "nine";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (i == 15)
                    {
                        uIElements.ResolutionStateInfoText.text = "nine";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (i == 14)
                    {
                        uIElements.ResolutionStateInfoText.text = "nine";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (i == 13)
                    {
                        uIElements.ResolutionStateInfoText.text = "nine";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (i == 12)
                    {
                        uIElements.ResolutionStateInfoText.text = "nine";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (i == 11)
                    {
                        uIElements.ResolutionStateInfoText.text = "nine";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (i == 10)
                    {
                        uIElements.ResolutionStateInfoText.text = "nine";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (i == 9)
                    {
                        uIElements.ResolutionStateInfoText.text = "nine";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (i == 8)
                    {
                        uIElements.ResolutionStateInfoText.text = "eight";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (i == 7)
                    {
                        uIElements.ResolutionStateInfoText.text = "seven";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (i == 6)
                    {
                        uIElements.ResolutionStateInfoText.text = "six";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (i == 5)
                    {
                        uIElements.ResolutionStateInfoText.text = "five";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (i == 4)
                    {
                        uIElements.ResolutionStateInfoText.text = "four";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (i == 3)
                    {
                        uIElements.ResolutionStateInfoText.text = "three";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (i == 2)
                    {
                        uIElements.ResolutionStateInfoText.text = "two";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (i == 1)
                    {
                        uIElements.ResolutionStateInfoText.text = "one";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    break;
                case ResolutionScreenType.Incorrect:
                    uIElements.ResolutionBG.color = parameters.IncorrectBGColor;
                    if (i == 0)
                    {
                        uIElements.ResolutionStateInfoText.text = "zero";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (i == 19)
                    {
                        uIElements.ResolutionStateInfoText.text = "nine";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (i == 18)
                    {
                        uIElements.ResolutionStateInfoText.text = "nine";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (i == 17)
                    {
                        uIElements.ResolutionStateInfoText.text = "nine";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (i == 16)
                    {
                        uIElements.ResolutionStateInfoText.text = "nine";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (i == 15)
                    {
                        uIElements.ResolutionStateInfoText.text = "nine";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (i == 14)
                    {
                        uIElements.ResolutionStateInfoText.text = "nine";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (i == 13)
                    {
                        uIElements.ResolutionStateInfoText.text = "nine";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (i == 12)
                    {
                        uIElements.ResolutionStateInfoText.text = "nine";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (i == 11)
                    {
                        uIElements.ResolutionStateInfoText.text = "nine";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (i == 10)
                    {
                        uIElements.ResolutionStateInfoText.text = "nine";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (i == 9)
                    {
                        uIElements.ResolutionStateInfoText.text = "nine";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (i == 8)
                    {
                        uIElements.ResolutionStateInfoText.text = "eight";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (i == 7)
                    {
                        uIElements.ResolutionStateInfoText.text = "seven";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (i == 6)
                    {
                        uIElements.ResolutionStateInfoText.text = "six";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (i == 5)
                    {
                        uIElements.ResolutionStateInfoText.text = "five";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (i == 4)
                    {
                        uIElements.ResolutionStateInfoText.text = "four";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (i == 3)
                    {
                        uIElements.ResolutionStateInfoText.text = "three";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (i == 2)
                    {
                        uIElements.ResolutionStateInfoText.text = "two";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (i == 1)
                    {
                        uIElements.ResolutionStateInfoText.text = "one";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    break;
                case ResolutionScreenType.Finish:
                    uIElements.ResolutionBG.color = parameters.FinalBGColor;
                    uIElements.ResolutionStateInfoText.text = "FINAL SCORE";

                    StartCoroutine(CalculateScore());
                    uIElements.FinishUIElements.gameObject.SetActive(true);
                    uIElements.HighScoreText.gameObject.SetActive(true);
                    uIElements.HighScoreText.text = ((highscore > events.StartupHighscore) ? "<color=yellow>new </color>" : string.Empty) + "Highscore: " + highscore;
                    break;
            }
        }
        if (SceneManager.GetActiveScene().buildIndex == 5)
        {
            switch (type)
            {
                case ResolutionScreenType.Correct:
                    uIElements.ResolutionBG.color = parameters.CorrectBGColor;
                    if (i == 0)
                    {
                        uIElements.ResolutionStateInfoText.text = "zero";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (i == 19)
                    {
                        uIElements.ResolutionStateInfoText.text = "nine";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (i == 18)
                    {
                        uIElements.ResolutionStateInfoText.text = "nine";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (i == 17)
                    {
                        uIElements.ResolutionStateInfoText.text = "nine";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (i == 16)
                    {
                        uIElements.ResolutionStateInfoText.text = "nine";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (i == 15)
                    {
                        uIElements.ResolutionStateInfoText.text = "nine";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (i == 14)
                    {
                        uIElements.ResolutionStateInfoText.text = "nine";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (i == 13)
                    {
                        uIElements.ResolutionStateInfoText.text = "nine";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (i == 12)
                    {
                        uIElements.ResolutionStateInfoText.text = "nine";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (i == 11)
                    {
                        uIElements.ResolutionStateInfoText.text = "nine";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (i == 10)
                    {
                        uIElements.ResolutionStateInfoText.text = "nine";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (i == 9)
                    {
                        uIElements.ResolutionStateInfoText.text = "nine";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (i == 8)
                    {
                        uIElements.ResolutionStateInfoText.text = "eight";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (i == 7)
                    {
                        uIElements.ResolutionStateInfoText.text = "seven";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (i == 6)
                    {
                        uIElements.ResolutionStateInfoText.text = "six";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (i == 5)
                    {
                        uIElements.ResolutionStateInfoText.text = "five";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (i == 4)
                    {
                        uIElements.ResolutionStateInfoText.text = "four";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (i == 3)
                    {
                        uIElements.ResolutionStateInfoText.text = "three";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (i == 2)
                    {
                        uIElements.ResolutionStateInfoText.text = "two";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (i == 1)
                    {
                        uIElements.ResolutionStateInfoText.text = "one";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    break;
                case ResolutionScreenType.Incorrect:
                    uIElements.ResolutionBG.color = parameters.IncorrectBGColor;
                    if (i == 0)
                    {
                        uIElements.ResolutionStateInfoText.text = "zero";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (i == 19)
                    {
                        uIElements.ResolutionStateInfoText.text = "nine";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (i == 18)
                    {
                        uIElements.ResolutionStateInfoText.text = "nine";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (i == 17)
                    {
                        uIElements.ResolutionStateInfoText.text = "nine";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (i == 16)
                    {
                        uIElements.ResolutionStateInfoText.text = "nine";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (i == 15)
                    {
                        uIElements.ResolutionStateInfoText.text = "nine";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (i == 14)
                    {
                        uIElements.ResolutionStateInfoText.text = "nine";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (i == 13)
                    {
                        uIElements.ResolutionStateInfoText.text = "nine";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (i == 12)
                    {
                        uIElements.ResolutionStateInfoText.text = "nine";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (i == 11)
                    {
                        uIElements.ResolutionStateInfoText.text = "nine";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (i == 10)
                    {
                        uIElements.ResolutionStateInfoText.text = "nine";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (i == 9)
                    {
                        uIElements.ResolutionStateInfoText.text = "nine";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (i == 8)
                    {
                        uIElements.ResolutionStateInfoText.text = "eight";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (i == 7)
                    {
                        uIElements.ResolutionStateInfoText.text = "seven";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (i == 6)
                    {
                        uIElements.ResolutionStateInfoText.text = "six";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (i == 5)
                    {
                        uIElements.ResolutionStateInfoText.text = "five";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (i == 4)
                    {
                        uIElements.ResolutionStateInfoText.text = "four";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (i == 3)
                    {
                        uIElements.ResolutionStateInfoText.text = "three";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (i == 2)
                    {
                        uIElements.ResolutionStateInfoText.text = "two";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (i == 1)
                    {
                        uIElements.ResolutionStateInfoText.text = "one";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    break;
                case ResolutionScreenType.Finish:
                    uIElements.ResolutionBG.color = parameters.FinalBGColor;
                    uIElements.ResolutionStateInfoText.text = "FINAL SCORE";

                    StartCoroutine(CalculateScore());
                    uIElements.FinishUIElements.gameObject.SetActive(true);
                    uIElements.HighScoreText.gameObject.SetActive(true);
                    uIElements.HighScoreText.text = ((highscore > events.StartupHighscore) ? "<color=yellow>new </color>" : string.Empty) + "Highscore: " + highscore;
                    break;
            }
        }
    }

    /// <summary>
    /// Function that is used to calculate and display the score.
    /// </summary>
    int scoreValue;
    IEnumerator CalculateScore()
    {
            uIElements.ResolutionScoreText.text = events.CurrentFinalScore.ToString();
            yield return null;
    }

    /// <summary>
    /// Function that is used to create new question answers.
    /// </summary>
    void CreateAnswers(Question question)
    {
        EraseAnswers();

        float offset = 0 - parameters.Margins;
        for (int i = 0; i < question.Answers.Length; i++)
        {
            AnswerData newAnswer = (AnswerData)Instantiate(answerPrefab, uIElements.AnswersContentArea);
            newAnswer.UpdateData(question.Answers[i].Info, i);

            newAnswer.Rect.anchoredPosition = new Vector2(0, offset);

            offset -= (newAnswer.Rect.sizeDelta.y + parameters.Margins);
            uIElements.AnswersContentArea.sizeDelta = new Vector2(uIElements.AnswersContentArea.sizeDelta.x, offset * -1);

            currentAnswers.Add(newAnswer);
        }
    }
    /// <summary>
    /// Function that is used to erase current created answers.
    /// </summary>
    void EraseAnswers()
    {
        foreach (var answer in currentAnswers)
        {
            Destroy(answer.gameObject);
        }
        currentAnswers.Clear();
    }

    /// <summary>
    /// Function that is used to update score text UI.
    /// </summary>
    void UpdateScoreUI()
    {
        Debug.Log(events.CurrentFinalScore);
        uIElements.ScoreText.text = "Score: " + events.CurrentFinalScore;
    }
}