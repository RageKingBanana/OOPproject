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
    public int i;
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
                        uIElements.ResolutionStateInfoText.text = "National Disaster Risk Reduction Management Council(NDRRMC) is the agency that is responsible for ensuring the protection and welfare of the people.";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (i == 3)
                    {
                        uIElements.ResolutionStateInfoText.text = "Philippine Institute of Volcanology and Seismology is the agency responsible for mitigation of disasters that arises from geotectonic phenomenas like volcanic eruptions, earthquakes, and tsunamis.";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (i == 2)
                    {
                        uIElements.ResolutionStateInfoText.text = "Philippine Atmospheric, Geophysical and Astronomical Services Administration(PAG-ASA) is the agency responsible for assessing and forecasting weather, flood, and other conditions essential for the welfare of the people.";
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
                        uIElements.ResolutionStateInfoText.text = "Be alert. Being cautious about your surroundings can be prevent damage to your family and your property.";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (i == 14)
                    {
                        uIElements.ResolutionStateInfoText.text = "nine";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (i == 13)
                    {
                        uIElements.ResolutionStateInfoText.text = "Immediately shut off the electricity. The risk of getting electrocuted is high so this is the top priority at the moment.";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (i == 12)
                    {
                        uIElements.ResolutionStateInfoText.text = "All of the choices are correct.";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (i == 11)
                    {
                        uIElements.ResolutionStateInfoText.text = "Do not try to swim to cross the river. Instead you can find help to attach a rope to the other side of the river to cross safely.";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (i == 10)
                    {
                        uIElements.ResolutionStateInfoText.text = "Get your emergency kit and evacuate your house immediately.";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (i == 9)
                    {
                        uIElements.ResolutionStateInfoText.text = "Put the baby’s need and evacuate immediately. Safety is always the first priority in emergencies.Put the baby’s need and evacuate immediately. Safety is always the first priority in emergencies.";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (i == 8)
                    {
                        uIElements.ResolutionStateInfoText.text = "Shut off the electricity at the circuit breakers. Water conducts electricity and loose electric connection can result in someone being electrocuted.";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (i == 7)
                    {
                        uIElements.ResolutionStateInfoText.text = "NEVER try to walk or swim through flowing water. If the water is moving swiftly, water 6 inches deep can knock you off your feet.";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (i == 6)
                    {
                        uIElements.ResolutionStateInfoText.text = "Evacuate immediately. Move to a safe area as soon as possible before access is cut off by rising water.";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (i == 5)
                    {
                        uIElements.ResolutionStateInfoText.text = "Get to high ground. Get out of low areas that may be subject to flooding,";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (i == 4)
                    {
                        uIElements.ResolutionStateInfoText.text = "Discuss a disaster plan to your family. Discuss flood plans with your family. Decide where you will meet if separated.";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (i == 3)
                    {
                        uIElements.ResolutionStateInfoText.text = "Be prepared to evacuate. If you have a place you can stay, identify alternative routes that are not prone to flooding and immediately evacuate. If not, go to the designated evacuation assigned by the local government.";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (i == 2)
                    {
                        uIElements.ResolutionStateInfoText.text = "Assemble disaster supplies. Emergency Kits are a MUST and can comes very handy in emergency situations.";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (i == 1)
                    {
                        uIElements.ResolutionStateInfoText.text = "Climb to safety immediately. Flash floods develop quickly. Do not wait until you see rising water.";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    break;
                case ResolutionScreenType.Incorrect:
                    uIElements.ResolutionBG.color = parameters.IncorrectBGColor;
                    if (i == 0)
                    {
                        uIElements.ResolutionStateInfoText.text = "Be alert. Being cautious about your surroundings can be prevent damage to your family and your property.";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (i == 14)
                    {
                        uIElements.ResolutionStateInfoText.text = "nine";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (i == 13)
                    {
                        uIElements.ResolutionStateInfoText.text = "Immediately shut off the electricity. The risk of getting electrocuted is high so this is the top priority at the moment.";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (i == 12)
                    {
                        uIElements.ResolutionStateInfoText.text = "All of the choices are correct.";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (i == 11)
                    {
                        uIElements.ResolutionStateInfoText.text = "Do not try to swim to cross the river. Instead you can find help to attach a rope to the other side of the river to cross safely.";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (i == 10)
                    {
                        uIElements.ResolutionStateInfoText.text = "Get your emergency kit and evacuate your house immediately.";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (i == 9)
                    {
                        uIElements.ResolutionStateInfoText.text = "Put the baby’s need and evacuate immediately. Safety is always the first priority in emergencies.Put the baby’s need and evacuate immediately. Safety is always the first priority in emergencies.";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (i == 8)
                    {
                        uIElements.ResolutionStateInfoText.text = "Shut off the electricity at the circuit breakers. Water conducts electricity and loose electric connection can result in someone being electrocuted.";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (i == 7)
                    {
                        uIElements.ResolutionStateInfoText.text = "NEVER try to walk or swim through flowing water. If the water is moving swiftly, water 6 inches deep can knock you off your feet.";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (i == 6)
                    {
                        uIElements.ResolutionStateInfoText.text = "Evacuate immediately. Move to a safe area as soon as possible before access is cut off by rising water.";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (i == 5)
                    {
                        uIElements.ResolutionStateInfoText.text = "Get to high ground. Get out of low areas that may be subject to flooding,";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (i == 4)
                    {
                        uIElements.ResolutionStateInfoText.text = "Discuss a disaster plan to your family. Discuss flood plans with your family. Decide where you will meet if separated.";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (i == 3)
                    {
                        uIElements.ResolutionStateInfoText.text = "Be prepared to evacuate. If you have a place you can stay, identify alternative routes that are not prone to flooding and immediately evacuate. If not, go to the designated evacuation assigned by the local government.";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (i == 2)
                    {
                        uIElements.ResolutionStateInfoText.text = "Assemble disaster supplies. Emergency Kits are a MUST and can comes very handy in emergency situations.";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (i == 1)
                    {
                        uIElements.ResolutionStateInfoText.text = "Climb to safety immediately. Flash floods develop quickly. Do not wait until you see rising water.";
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
                        uIElements.ResolutionStateInfoText.text = "Familiarize yourself with all possible exit routes.";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (i == 14)
                    {
                        uIElements.ResolutionStateInfoText.text = "nine";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (i == 13)
                    {
                        uIElements.ResolutionStateInfoText.text = "Step out and move towards a safer area.";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (i == 12)
                    {
                        uIElements.ResolutionStateInfoText.text = "Evacuate to an open area.";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (i == 11)
                    {
                        uIElements.ResolutionStateInfoText.text = "Exit the building via stairs.";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (i == 10)
                    {
                        uIElements.ResolutionStateInfoText.text = "Stay away from glass windows, shelves, and heavy and hanging objects.";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (i == 9)
                    {
                        uIElements.ResolutionStateInfoText.text = "Duck, cover and hold under somewhere stable.";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (i == 8)
                    {
                        uIElements.ResolutionStateInfoText.text = "Stay calm and stay put.";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (i == 7)
                    {
                        uIElements.ResolutionStateInfoText.text = "Learn simple first aid techniques.";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (i == 6)
                    {
                        uIElements.ResolutionStateInfoText.text = "Discuss a designated meet up location with family in case you’re separated.";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (i == 5)
                    {
                        uIElements.ResolutionStateInfoText.text = "Participate in office and community earthquake drills.";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (i == 4)
                    {
                        uIElements.ResolutionStateInfoText.text = "Prepare your emergency kit";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (i == 3)
                    {
                        uIElements.ResolutionStateInfoText.text = "Secure heavy furniture and hanging objects.";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (i == 2)
                    {
                        uIElements.ResolutionStateInfoText.text = "Store harmful chemicals and flammable materials properly.";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (i == 1)
                    {
                        uIElements.ResolutionStateInfoText.text = "Have necessary house repairs fixed to avoid further damage";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    break;
                case ResolutionScreenType.Incorrect:
                    uIElements.ResolutionBG.color = parameters.IncorrectBGColor;
                    if (i == 0)
                    {
                        uIElements.ResolutionStateInfoText.text = "Familiarize yourself with all possible exit routes.";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (i == 14)
                    {
                        uIElements.ResolutionStateInfoText.text = "nine";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (i == 13)
                    {
                        uIElements.ResolutionStateInfoText.text = "Step out and move towards a safer area.";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (i == 12)
                    {
                        uIElements.ResolutionStateInfoText.text = "Evacuate to an open area.";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (i == 11)
                    {
                        uIElements.ResolutionStateInfoText.text = "Exit the building via stairs.";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (i == 10)
                    {
                        uIElements.ResolutionStateInfoText.text = "Stay away from glass windows, shelves, and heavy and hanging objects.";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (i == 9)
                    {
                        uIElements.ResolutionStateInfoText.text = "Duck, cover and hold under somewhere stable.";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (i == 8)
                    {
                        uIElements.ResolutionStateInfoText.text = "Stay calm and stay put.";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (i == 7)
                    {
                        uIElements.ResolutionStateInfoText.text = "Learn simple first aid techniques.";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (i == 6)
                    {
                        uIElements.ResolutionStateInfoText.text = "Discuss a designated meet up location with family in case you’re separated.";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (i == 5)
                    {
                        uIElements.ResolutionStateInfoText.text = "Participate in office and community earthquake drills.";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (i == 4)
                    {
                        uIElements.ResolutionStateInfoText.text = "Prepare your emergency kit";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (i == 3)
                    {
                        uIElements.ResolutionStateInfoText.text = "Secure heavy furniture and hanging objects.";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (i == 2)
                    {
                        uIElements.ResolutionStateInfoText.text = "Store harmful chemicals and flammable materials properly.";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (i == 1)
                    {
                        uIElements.ResolutionStateInfoText.text = "Have necessary house repairs fixed to avoid further damage";
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
        else if (SceneManager.GetActiveScene().buildIndex == 4)
        {
            switch (type)
            {
                case ResolutionScreenType.Correct:
                    uIElements.ResolutionBG.color = parameters.CorrectBGColor;
                    if (i == 0)
                    {
                        uIElements.ResolutionStateInfoText.text = "Stay alert in the event of aftershocks.";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (i == 19)
                    {
                        uIElements.ResolutionStateInfoText.text = "Triangle of Life";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (i == 18)
                    {
                        uIElements.ResolutionStateInfoText.text = "Stay calm and stay put.";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (i == 17)
                    {
                        uIElements.ResolutionStateInfoText.text = "Toiletries";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (i == 16)
                    {
                        uIElements.ResolutionStateInfoText.text = "Hiking equipment";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (i == 15)
                    {
                        uIElements.ResolutionStateInfoText.text = "Philippine Institute of Volcanology and Seismology";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (i == 14)
                    {
                        uIElements.ResolutionStateInfoText.text = "PHILVOCS";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (i == 13)
                    {
                        uIElements.ResolutionStateInfoText.text = "Elevate electrical components, loose electrical cords might cause electrocution";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (i == 12)
                    {
                        uIElements.ResolutionStateInfoText.text = "provide first aid for the injured";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (i == 11)
                    {
                        uIElements.ResolutionStateInfoText.text = "Stay alert in the event of aftershocks.";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (i == 10)
                    {
                        uIElements.ResolutionStateInfoText.text = "Evacuate immediately, because it is most likely that a tsunami will hit.";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (i == 9)
                    {
                        uIElements.ResolutionStateInfoText.text = "protect your property from future flood damage. Elevation of utilities and electrical components above the potential flood height as consideration of the elevation of the entire structure can be reduce the damage next time around";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (i == 8)
                    {
                        uIElements.ResolutionStateInfoText.text = "Use extreme caution when entering buildings. Examine walls, floors, doors, windows, and ceilings for risk of collapsing.";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (i == 7)
                    {
                        uIElements.ResolutionStateInfoText.text = "Wait until it is safe to return. Do not return to flooded areas until the authorities indicate it is safe to do so.";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (i == 6)
                    {
                        uIElements.ResolutionStateInfoText.text = "Message your loved ones of your state and where you are.";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (i == 5)
                    {
                        uIElements.ResolutionStateInfoText.text = "Stay updated through a battery-operated radio.";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (i == 4)
                    {
                        uIElements.ResolutionStateInfoText.text = "Check for water, electrical, gas, or LPG leaks and damages.";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (i == 3)
                    {
                        uIElements.ResolutionStateInfoText.text = "Stay out of buildings until advised.";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (i == 2)
                    {
                        uIElements.ResolutionStateInfoText.text = "Evacuate to higher ground immediately.";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (i == 1)
                    {
                        uIElements.ResolutionStateInfoText.text = "Provide first aid for any possible injuries.";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    break;
                case ResolutionScreenType.Incorrect:
                    uIElements.ResolutionBG.color = parameters.IncorrectBGColor;
                    if (i == 0)
                    {
                        uIElements.ResolutionStateInfoText.text = "Stay alert in the event of aftershocks.";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (i == 19)
                    {
                        uIElements.ResolutionStateInfoText.text = "Triangle of Life";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (i == 18)
                    {
                        uIElements.ResolutionStateInfoText.text = "Stay calm and stay put.";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (i == 17)
                    {
                        uIElements.ResolutionStateInfoText.text = "Toiletries";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (i == 16)
                    {
                        uIElements.ResolutionStateInfoText.text = "Hiking equipment";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (i == 15)
                    {
                        uIElements.ResolutionStateInfoText.text = "Philippine Institute of Volcanology and Seismology";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (i == 14)
                    {
                        uIElements.ResolutionStateInfoText.text = "PHILVOCS";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (i == 13)
                    {
                        uIElements.ResolutionStateInfoText.text = "Elevate electrical components, loose electrical cords might cause electrocution";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (i == 12)
                    {
                        uIElements.ResolutionStateInfoText.text = "provide first aid for the injured";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (i == 11)
                    {
                        uIElements.ResolutionStateInfoText.text = "Stay alert in the event of aftershocks.";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (i == 10)
                    {
                        uIElements.ResolutionStateInfoText.text = "Evacuate immediately, because it is most likely that a tsunami will hit.";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (i == 9)
                    {
                        uIElements.ResolutionStateInfoText.text = "protect your property from future flood damage. Elevation of utilities and electrical components above the potential flood height as consideration of the elevation of the entire structure can be reduce the damage next time around";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (i == 8)
                    {
                        uIElements.ResolutionStateInfoText.text = "Use extreme caution when entering buildings. Examine walls, floors, doors, windows, and ceilings for risk of collapsing.";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (i == 7)
                    {
                        uIElements.ResolutionStateInfoText.text = "Wait until it is safe to return. Do not return to flooded areas until the authorities indicate it is safe to do so.";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (i == 6)
                    {
                        uIElements.ResolutionStateInfoText.text = "Message your loved ones of your state and where you are.";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (i == 5)
                    {
                        uIElements.ResolutionStateInfoText.text = "Stay updated through a battery-operated radio.";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (i == 4)
                    {
                        uIElements.ResolutionStateInfoText.text = "Check for water, electrical, gas, or LPG leaks and damages.";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (i == 3)
                    {
                        uIElements.ResolutionStateInfoText.text = "Stay out of buildings until advised.";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (i == 2)
                    {
                        uIElements.ResolutionStateInfoText.text = "Evacuate to higher ground immediately.";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (i == 1)
                    {
                        uIElements.ResolutionStateInfoText.text = "Provide first aid for any possible injuries.";
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
        else if (SceneManager.GetActiveScene().buildIndex == 5)
        {
            switch (type)
            {
                case ResolutionScreenType.Correct:
                    uIElements.ResolutionBG.color = parameters.CorrectBGColor;
                    if (i == 0)
                    {
                        uIElements.ResolutionStateInfoText.text = "Landslide";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (i == 19)
                    {
                        uIElements.ResolutionStateInfoText.text = "National Disaster Risk Reduction & Management Council";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (i == 18)
                    {
                        uIElements.ResolutionStateInfoText.text = "Early warning systems";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (i == 17)
                    {
                        uIElements.ResolutionStateInfoText.text = "Checking maximum sustained winds";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (i == 16)
                    {
                        uIElements.ResolutionStateInfoText.text = "Tell your neighbors that it is safe to return";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (i == 15)
                    {
                        uIElements.ResolutionStateInfoText.text = "Squall";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (i == 14)
                    {
                        uIElements.ResolutionStateInfoText.text = "Being cautious about your surroundings can  prevent damage to your family and your property.";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (i == 13)
                    {
                        uIElements.ResolutionStateInfoText.text = "Earth flow";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (i == 12)
                    {
                        uIElements.ResolutionStateInfoText.text = "Storm surge";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (i == 11)
                    {
                        uIElements.ResolutionStateInfoText.text = "Get to high ground.";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (i == 10)
                    {
                        uIElements.ResolutionStateInfoText.text = "Wait until the authorities indicate that it is safe to return.";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (i == 9)
                    {
                        uIElements.ResolutionStateInfoText.text = "Super typhoon";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (i == 8)
                    {
                        uIElements.ResolutionStateInfoText.text = "Severe tropical storm";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (i == 7)
                    {
                        uIElements.ResolutionStateInfoText.text = "Typhoon";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (i == 6)
                    {
                        uIElements.ResolutionStateInfoText.text = "Tropical storm";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (i == 5)
                    {
                        uIElements.ResolutionStateInfoText.text = "Tropical depression";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (i == 4)
                    {
                        uIElements.ResolutionStateInfoText.text = "Discuss an outing plan to your family.";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (i == 3)
                    {
                        uIElements.ResolutionStateInfoText.text = "Maximum sustained winds";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (i == 2)
                    {
                        uIElements.ResolutionStateInfoText.text = "PAG-ASA";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (i == 1)
                    {
                        uIElements.ResolutionStateInfoText.text = "Hazard mapping";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    break;
                case ResolutionScreenType.Incorrect:
                    uIElements.ResolutionBG.color = parameters.IncorrectBGColor;
                    if (i == 0)
                    {
                        uIElements.ResolutionStateInfoText.text = "Landslide";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (i == 19)
                    {
                        uIElements.ResolutionStateInfoText.text = "National Disaster Risk Reduction & Management Council";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (i == 18)
                    {
                        uIElements.ResolutionStateInfoText.text = "Early warning systems";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (i == 17)
                    {
                        uIElements.ResolutionStateInfoText.text = "Checking maximum sustained winds";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (i == 16)
                    {
                        uIElements.ResolutionStateInfoText.text = "Tell your neighbors that it is safe to return";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (i == 15)
                    {
                        uIElements.ResolutionStateInfoText.text = "Squall";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (i == 14)
                    {
                        uIElements.ResolutionStateInfoText.text = "Being cautious about your surroundings can  prevent damage to your family and your property.";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (i == 13)
                    {
                        uIElements.ResolutionStateInfoText.text = "Earth flow";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (i == 12)
                    {
                        uIElements.ResolutionStateInfoText.text = "Storm surge";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (i == 11)
                    {
                        uIElements.ResolutionStateInfoText.text = "Get to high ground.";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (i == 10)
                    {
                        uIElements.ResolutionStateInfoText.text = "Wait until the authorities indicate that it is safe to return.";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (i == 9)
                    {
                        uIElements.ResolutionStateInfoText.text = "Super typhoon";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (i == 8)
                    {
                        uIElements.ResolutionStateInfoText.text = "Severe tropical storm";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (i == 7)
                    {
                        uIElements.ResolutionStateInfoText.text = "Typhoon";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (i == 6)
                    {
                        uIElements.ResolutionStateInfoText.text = "Tropical storm";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (i == 5)
                    {
                        uIElements.ResolutionStateInfoText.text = "Tropical depression";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (i == 4)
                    {
                        uIElements.ResolutionStateInfoText.text = "Discuss an outing plan to your family.";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (i == 3)
                    {
                        uIElements.ResolutionStateInfoText.text = "Maximum sustained winds";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (i == 2)
                    {
                        uIElements.ResolutionStateInfoText.text = "PAG-ASA";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (i == 1)
                    {
                        uIElements.ResolutionStateInfoText.text = "Hazard mapping";
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