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
    public string questiooon;
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
        PlayerPrefs.SetString("Questioon", uIElements.QuestionInfoTextObject.text);
          
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
        questiooon = PlayerPrefs.GetString("Questioon");
        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            switch (type)
            {
                case ResolutionScreenType.Correct:
                    uIElements.ResolutionBG.color = parameters.CorrectBGColor;
                    if (questiooon == "What are the natural cause of earthquakes?")
                    {
                        uIElements.ResolutionStateInfoText.text = "The natural causes of earthquakes are sliding of tectonic plates and volcanic activities.";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (questiooon == "What is the necessary skill to be learned in case of emergency?")
                    {
                        uIElements.ResolutionStateInfoText.text = "Learning simple first aid techniques can be very advantageous. ";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (questiooon == "Emergency kits should contain food in case of emergency, what is not needed?")
                    {
                        uIElements.ResolutionStateInfoText.text = "Freezed Ham is not included because it is needed to be processed first in order to eat it.";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    } 
                    else if (questiooon == "Emergency kits should contain all of the essential tools that is needed for survival.What is included ?")
                    {
                        uIElements.ResolutionStateInfoText.text = "Spare Batteries and Flashlight are included in the emergency kit.";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (questiooon == "Why is the Philippines is considered to be an earthquake prone country?")
                    {
                        uIElements.ResolutionStateInfoText.text = "The Philippines is considered to be an earthquake prone country because it is located near the Pacific Ring of Fire.";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (questiooon == "NDRRMC mad a mobile application to provide a handy electronic resources to the public that can be utilize in case of emergency. What is the application called?")
                    {
                        uIElements.ResolutionStateInfoText.text = "“Batingaw” is the mobile application developed by NDRRMC for public use.";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (questiooon == "What is the agency responsible for ensuring the protection and welfare of the people?")
                    {
                        uIElements.ResolutionStateInfoText.text = "National Disaster Risk Reduction Management Council(NDRRMC) is the agency that is responsible for ensuring the protection and welfare of the people.";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (questiooon == "What government agency is responsible for mitigating disasters that arises from geotectonic phenomena? ")
                    {
                        uIElements.ResolutionStateInfoText.text = "Philippine Institute of Volcanology and Seismology is the agency responsible for mitigation of disasters that arises from geotectonic phenomenas like volcanic eruptions, earthquakes, and tsunamis.";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (questiooon == "What government agency is responsible for monitoring the weather changes in the Philippines?")
                    {
                        uIElements.ResolutionStateInfoText.text = "Philippine Atmospheric,Geophysical and Astronomical Services Administration(PAG-ASA) is the agency responsible for assessing and forecasting weather, flood, and other conditions essential for the welfare of the people.";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (questiooon == "What are the possible effects of earthquakes?")
                    {
                        uIElements.ResolutionStateInfoText.text = "Aftershocks, Tsunamis, Landslides are all possible effects of earthquakes.";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }

                    break;
                case ResolutionScreenType.Incorrect:
                    uIElements.ResolutionBG.color = parameters.IncorrectBGColor;
                    if (questiooon == "What are the natural cause of earthquakes?")
                    {
                        uIElements.ResolutionStateInfoText.text = "The natural causes of earthquakes are sliding of tectonic plates and volcanic activities.";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (questiooon == "What is the necessary skill to be learned in case of emergency?")
                    {
                        uIElements.ResolutionStateInfoText.text = "Learning simple first aid techniques can be very advantageous. ";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (questiooon == "Emergency kits should contain food in case of emergency, what is not needed ?")
                    {
                        uIElements.ResolutionStateInfoText.text = "Freezed Ham is not included because it is needed to be processed first in order to eat it.";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (questiooon == "Emergency kits should contain all of the essential tools that is needed for survival.What is included ?")
                    {
                        uIElements.ResolutionStateInfoText.text = "Spare Batteries and Flashlight are included in the emergency kit.";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (questiooon == "Why is the Philippines is considered to be an earthquake prone country?")
                    {
                        uIElements.ResolutionStateInfoText.text = "The Philippines is considered to be an earthquake prone country because it is located near the Pacific Ring of Fire.";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (questiooon == "NDRRMC mad a mobile application to provide a handy electronic resources to the public that can be utilize in case of emergency. What is the application called?")
                    {
                        uIElements.ResolutionStateInfoText.text = "“Batingaw” is the mobile application developed by NDRRMC for public use.";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (questiooon == "What is the agency responsible for ensuring the protection and welfare of the people?")
                    {
                        uIElements.ResolutionStateInfoText.text = "National Disaster Risk Reduction Management Council(NDRRMC) is the agency that is responsible for ensuring the protection and welfare of the people.";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (questiooon == "What government agency is responsible for mitigating disasters that arises from geotectonic phenomena? ")
                    {
                        uIElements.ResolutionStateInfoText.text = "Philippine Institute of Volcanology and Seismology is the agency responsible for mitigation of disasters that arises from geotectonic phenomenas like volcanic eruptions, earthquakes, and tsunamis.";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (questiooon == "What government agency is responsible for monitoring the weather changes in the Philippines?")
                    {
                        uIElements.ResolutionStateInfoText.text = "Philippine Atmospheric,Geophysical and Astronomical Services Administration(PAG-ASA) is the agency responsible for assessing and forecasting weather, flood, and other conditions essential for the welfare of the people.";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (questiooon == "What are the possible effects of earthquakes?")
                    {
                        uIElements.ResolutionStateInfoText.text = "Aftershocks, Tsunamis, Landslides are all possible effects of earthquakes.";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    break;
                case ResolutionScreenType.Finish:
                    if (questiooon == "What are the natural cause of earthquakes?")
                    {
                        uIElements.ResolutionStateInfoText.text = "The natural causes of earthquakes are sliding of tectonic plates and volcanic activities.";
                    }
                    else if (questiooon == "What is the necessary skill to be learned in case of emergency?")
                    {
                        uIElements.ResolutionStateInfoText.text = "Learning simple first aid techniques can be very advantageous. ";
                    }
                    else if (questiooon == "Emergency kits should contain food in case of emergency, what is not needed ?")
                    {
                        uIElements.ResolutionStateInfoText.text = "Freezed Ham is not included because it is needed to be processed first in order to eat it.";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (questiooon == "Emergency kits should contain all of the essential tools that is needed for survival.What is included ?")
                    {
                        uIElements.ResolutionStateInfoText.text = "Spare Batteries and Flashlight are included in the emergency kit.";
                    }
                    else if (questiooon == "Why is the Philippines is considered to be an earthquake prone country?")
                    {
                        uIElements.ResolutionStateInfoText.text = "The Philippines is considered to be an earthquake prone country because it is located near the Pacific Ring of Fire.";
                    }
                    else if (questiooon == "NDRRMC mad a mobile application to provide a handy electronic resources to the public that can be utilize in case of emergency. What is the application called?")
                    {
                        uIElements.ResolutionStateInfoText.text = "“Batingaw” is the mobile application developed by NDRRMC for public use.";
                    }
                    else if (questiooon == "What is the agency responsible for ensuring the protection and welfare of the people?")
                    {
                        uIElements.ResolutionStateInfoText.text = "National Disaster Risk Reduction Management Council(NDRRMC) is the agency that is responsible for ensuring the protection and welfare of the people.";
                    }
                    else if (questiooon == "What government agency is responsible for mitigating disasters that arises from geotectonic phenomena? ")
                    {
                        uIElements.ResolutionStateInfoText.text = "Philippine Institute of Volcanology and Seismology is the agency responsible for mitigation of disasters that arises from geotectonic phenomenas like volcanic eruptions, earthquakes, and tsunamis.";
                    }
                    else if (questiooon == "What government agency is responsible for monitoring the weather changes in the Philippines?")
                    {
                        uIElements.ResolutionStateInfoText.text = "Philippine Atmospheric,Geophysical and Astronomical Services Administration(PAG-ASA) is the agency responsible for assessing and forecasting weather, flood, and other conditions essential for the welfare of the people.";
                    }
                    else if (questiooon == "What are the possible effects of earthquakes?")
                    {
                        uIElements.ResolutionStateInfoText.text = "Aftershocks, Tsunamis, Landslides are all possible effects of earthquakes.";
                    }

                    uIElements.ResolutionBG.color = parameters.FinalBGColor;

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
                    if (questiooon == "You are currently celebrating your birthday when you found out that the water in your area is rapidly rising. What should you do?")
                    {
                        uIElements.ResolutionStateInfoText.text = "Be alert. Being cautious about your surroundings can be prevent damage to your family and your property.";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (questiooon == "A person can get this disease through a mosquito bite causing a flu-like illness and sometimes lethal complications.")
                    {
                        uIElements.ResolutionStateInfoText.text = "Dengue";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (questiooon == "A person can get this disease by consuming contaminated food or water.")
                    {
                        uIElements.ResolutionStateInfoText.text = "Cholera";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (questiooon == "A person can get this disease simply by wading or swimming in floods contaminated with animal urine? ")
                    {
                        uIElements.ResolutionStateInfoText.text = "Leptospirosis is spread mainly by contact upon water or soil contaminated by the urine of infected animals.";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (questiooon == "According to waze, what are the top three cities  in metro manila are considered to be the most flood prone areas?")
                    {
                        uIElements.ResolutionStateInfoText.text = "According to waze, Manila is considered to be the most flood prone area in metro manila followed by Makati and Mandaluyong.";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (questiooon == "What are the alternative solution/s to reduce flood damage?")
                    {
                        uIElements.ResolutionStateInfoText.text = "All, The more trees there are, the slower the water flow = less damage, Proper waste management helps in avoiding blocked irrigation systems and Investing in flood defences helps prepare for future damage control";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (questiooon == "If a family who's always suffer from floods, had a newborn baby recently. What should they do?")
                    {
                        uIElements.ResolutionStateInfoText.text = " If there is a baby, persons with disabilities (PWD) in your family. You should put their safety as a top priority.";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (questiooon == "A family is currently living in a household located in a low ground location. The area is currently suffering the onslaught of a super typhoon. The flood started to fill the premises. There are extension cords that are just laying on the ground ? What is the danger that the family may suffer?")
                    {
                        uIElements.ResolutionStateInfoText.text = "Shut off the electricity at the circuit breakers. Water conducts electricity and loose electric connection can result in someone being electrocuted.";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (questiooon == "A family of illegal settlers dwells near an estero. An unexpected rise of water level woke them up. What they should not do is ?")
                    {
                        uIElements.ResolutionStateInfoText.text = "NEVER try to walk or swim through flowing water. If the water is moving swiftly, water 6 inches deep can knock you off your feet.";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (questiooon == "You woke up early in the morning and you notice that the water is starting to rise. The local government failed to inform the citizens about the flood.  What should you do?")
                    {
                        uIElements.ResolutionStateInfoText.text = "Evacuate immediately. Move to a safe area as soon as possible before access is cut off by rising water.";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (questiooon == "You own a farm and a waist deep flood is currently devastating your farm. You should not?")
                    {
                        uIElements.ResolutionStateInfoText.text = "You should not prioritze to save crops and animals. Get out of low areas that may be subject to flooding,prioritze your safety above all else";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (questiooon == "You saw in a documentary the danger of flash floods. The local government warns every Filipino family regarding this issue. What should you do?")
                    {
                        uIElements.ResolutionStateInfoText.text = "Discuss a disaster plan to your family. Discuss flood plans with your family. Decide where you will meet if separated.";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (questiooon == "The rain is falling so hard due to the super typhoon in your area. There is a chance of flash floods. What should you do?")
                    {
                        uIElements.ResolutionStateInfoText.text = "Be prepared to evacuate. If you have a place you can stay, identify alternative routes that are not prone to flooding and immediately evacuate. If not, go to the designated evacuation assigned by the local government.";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (questiooon == "You recently attended a seminar about survival tips conducted by your barangay. What should you do?")
                    {
                        uIElements.ResolutionStateInfoText.text = "Assemble disaster supplies. Emergency Kits are a MUST and can comes very handy in emergency situations.";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (questiooon == "You are currently residing in Brgy. Tomana in Marikina. You heard that the current water level in Marikina river has the potential to reach it’s critical level. What should you do?")
                    {
                        uIElements.ResolutionStateInfoText.text = "Climb to safety immediately. Flash floods develop quickly. Do not wait until you see rising water.";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    break;
                case ResolutionScreenType.Incorrect:
                    uIElements.ResolutionBG.color = parameters.IncorrectBGColor;
                    if (questiooon == "You are currently celebrating your birthday when you found out that the water in your area is rapidly rising. What should you do?")
                    {
                        uIElements.ResolutionStateInfoText.text = "Be alert. Being cautious about your surroundings can be prevent damage to your family and your property.";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (questiooon == "A person can get this disease through a mosquito bite causing a flu-like illness and sometimes lethal complications.")
                    {
                        uIElements.ResolutionStateInfoText.text = "Dengue";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (questiooon == "A person can get this disease by consuming contaminated food or water.")
                    {
                        uIElements.ResolutionStateInfoText.text = "Cholera";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (questiooon == "A person can get this disease simply by wading or swimming in floods contaminated with animal urine? ")
                    {
                        uIElements.ResolutionStateInfoText.text = "Leptospirosis is spread mainly by contact upon water or soil contaminated by the urine of infected animals.";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (questiooon == "According to waze, what are the top three cities  in metro manila are considered to be the most flood prone areas?")
                    {
                        uIElements.ResolutionStateInfoText.text = "According to waze, Manila is considered to be the most flood prone area in metro manila followed by Makati and Mandaluyong.";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (questiooon == "What are the alternative solution/s to reduce flood damage?")
                    {
                        uIElements.ResolutionStateInfoText.text = "All, The more trees there are, the slower the water flow = less damage, Proper waste management helps in avoiding blocked irrigation systems and Investing in flood defences helps prepare for future damage control";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (questiooon == "If a family who's always suffer from floods, had a newborn baby recently. What should they do?")
                    {
                        uIElements.ResolutionStateInfoText.text = "Put the baby’s need and evacuate immediately. Safety is always the first priority in emergencies.Put the baby’s need and evacuate immediately. Safety is always the first priority in emergencies.";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (questiooon == "A family is currently living in a household located in a low ground location. The area is currently suffering the onslaught of a super typhoon. The flood started to fill the premises. There are extension cords that are just laying on the ground ? What is the danger that the family may suffer?")
                    {
                        uIElements.ResolutionStateInfoText.text = "Shut off the electricity at the circuit breakers. Water conducts electricity and loose electric connection can result in someone being electrocuted.";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (questiooon == "A family of illegal settlers dwells near an estero. An unexpected rise of water level woke them up. What they should not do is ?")
                    {
                        uIElements.ResolutionStateInfoText.text = "NEVER try to walk or swim through flowing water. If the water is moving swiftly, water 6 inches deep can knock you off your feet.";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (questiooon == "You woke up early in the morning and you notice that the water is starting to rise. The local government failed to inform the citizens about the flood.  What should you do?")
                    {
                        uIElements.ResolutionStateInfoText.text = "Evacuate immediately. Move to a safe area as soon as possible before access is cut off by rising water.";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (questiooon == "You own a farm and a waist deep flood is currently devastating your farm. You should not?")
                    {
                        uIElements.ResolutionStateInfoText.text = "You should not prioritze to save crops and animals. Get out of low areas that may be subject to flooding,prioritze your safety above all else";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (questiooon == "You saw in a documentary the danger of flash floods. The local government warns every Filipino family regarding this issue. What should you do?")
                    {
                        uIElements.ResolutionStateInfoText.text = "Discuss a disaster plan to your family. Discuss flood plans with your family. Decide where you will meet if separated.";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (questiooon == "The rain is falling so hard due to the super typhoon in your area. There is a chance of flash floods. What should you do?")
                    {
                        uIElements.ResolutionStateInfoText.text = "Be prepared to evacuate. If you have a place you can stay, identify alternative routes that are not prone to flooding and immediately evacuate. If not, go to the designated evacuation assigned by the local government.";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (questiooon == "You recently attended a seminar about survival tips conducted by your barangay. What should you do?")
                    {
                        uIElements.ResolutionStateInfoText.text = "Assemble disaster supplies. Emergency Kits are a MUST and can comes very handy in emergency situations.";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (questiooon == "You are currently residing in Brgy. Tomana in Marikina. You heard that the current water level in Marikina river has the potential to reach it’s critical level. What should you do?")
                    {
                        uIElements.ResolutionStateInfoText.text = "Climb to safety immediately. Flash floods develop quickly. Do not wait until you see rising water.";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    break;
                case ResolutionScreenType.Finish:
                    uIElements.ResolutionBG.color = parameters.FinalBGColor;
                    if (questiooon == "You are currently celebrating your birthday when you found out that the water in your area is rapidly rising. What should you do?")
                    {
                        uIElements.ResolutionStateInfoText.text = "Be alert. Being cautious about your surroundings can be prevent damage to your family and your property.";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (questiooon == "A person can get this disease through a mosquito bite causing a flu-like illness and sometimes lethal complications.")
                    {
                        uIElements.ResolutionStateInfoText.text = "Dengue";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (questiooon == "A person can get this disease by consuming contaminated food or water.")
                    {
                        uIElements.ResolutionStateInfoText.text = "Cholera";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (questiooon == "A person can get this disease simply by wading or swimming in floods contaminated with animal urine? ")
                    {
                        uIElements.ResolutionStateInfoText.text = "Leptospirosis is spread mainly by contact upon water or soil contaminated by the urine of infected animals.";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (questiooon == "According to waze, what are the top three cities  in metro manila are considered to be the most flood prone areas?")
                    {
                        uIElements.ResolutionStateInfoText.text = "According to waze, Manila is considered to be the most flood prone area in metro manila followed by Makati and Mandaluyong.";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (questiooon == "What are the alternative solution/s to reduce flood damage?")
                    {
                        uIElements.ResolutionStateInfoText.text = "All, The more trees there are, the slower the water flow = less damage, Proper waste management helps in avoiding blocked irrigation systems and Investing in flood defences helps prepare for future damage control";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (questiooon == "If a family who's always suffer from floods, had a newborn baby recently. What should they do?")
                    {
                        uIElements.ResolutionStateInfoText.text = "Put the baby’s need and evacuate immediately. Safety is always the first priority in emergencies.Put the baby’s need and evacuate immediately. Safety is always the first priority in emergencies.";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (questiooon == "A family is currently living in a household located in a low ground location. The area is currently suffering the onslaught of a super typhoon. The flood started to fill the premises. There are extension cords that are just laying on the ground ? What is the danger that the family may suffer?")
                    {
                        uIElements.ResolutionStateInfoText.text = "Shut off the electricity at the circuit breakers. Water conducts electricity and loose electric connection can result in someone being electrocuted.";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (questiooon == "A family of illegal settlers dwells near an estero. An unexpected rise of water level woke them up. What they should not do is ?")
                    {
                        uIElements.ResolutionStateInfoText.text = "NEVER try to walk or swim through flowing water. If the water is moving swiftly, water 6 inches deep can knock you off your feet.";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (questiooon == "You woke up early in the morning and you notice that the water is starting to rise. The local government failed to inform the citizens about the flood.  What should you do?")
                    {
                        uIElements.ResolutionStateInfoText.text = "Evacuate immediately. Move to a safe area as soon as possible before access is cut off by rising water.";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (questiooon == "You own a farm and a waist deep flood is currently devastating your farm. You should not?")
                    {
                        uIElements.ResolutionStateInfoText.text = "You should not prioritze to save crops and animals. Get out of low areas that may be subject to flooding,prioritze your safety above all else";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (questiooon == "You saw in a documentary the danger of flash floods. The local government warns every Filipino family regarding this issue. What should you do?")
                    {
                        uIElements.ResolutionStateInfoText.text = "Discuss a disaster plan to your family. Discuss flood plans with your family. Decide where you will meet if separated.";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (questiooon == "The rain is falling so hard due to the super typhoon in your area. There is a chance of flash floods. What should you do?")
                    {
                        uIElements.ResolutionStateInfoText.text = "Be prepared to evacuate. If you have a place you can stay, identify alternative routes that are not prone to flooding and immediately evacuate. If not, go to the designated evacuation assigned by the local government.";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (questiooon == "You recently attended a seminar about survival tips conducted by your barangay. What should you do?")
                    {
                        uIElements.ResolutionStateInfoText.text = "Assemble disaster supplies. Emergency Kits are a MUST and can comes very handy in emergency situations.";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (questiooon == "You are currently residing in Brgy. Tomana in Marikina. You heard that the current water level in Marikina river has the potential to reach it’s critical level. What should you do?")
                    {
                        uIElements.ResolutionStateInfoText.text = "Climb to safety immediately. Flash floods develop quickly. Do not wait until you see rising water.";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }

                    //uIElements.ResolutionStateInfoText.text = "FINAL SCORE";

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
                    if (questiooon == "The local government warns the public about the possibility of an earthquake. What should you do?")
                    {
                        uIElements.ResolutionStateInfoText.text = "Familiarize yourself with all possible exit routes.";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (questiooon == "You and your family are driving for your vacation when suddenly an earthquake hits.What should you do?")
                    {
                        uIElements.ResolutionStateInfoText.text = "Step out and move towards a safer area.";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (questiooon == "An earthquake suddenly hits and you are able to exit the building safely. Once you are outside of the building, What you should do?")
                    {
                        uIElements.ResolutionStateInfoText.text = "Evacuate to an open area.You should always be on the lookout for any structures that might collapse.";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (questiooon == "While running towards the exit during an earthquake. You should avoid?")
                    {
                        uIElements.ResolutionStateInfoText.text = "During an earthquake, avoid using electricity powered machines like elevators and escalators.";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (questiooon == "You and your family are shopping in a mall when an earthquake suddenly hits. You did the duck, cover and hold and are ready to exit the building. What should you look out for?")
                    {
                        uIElements.ResolutionStateInfoText.text = "All of the above, Stay away from glass windows, shelves, hanging objects or any sharp or heavy objects that can cause severe injuries.";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (questiooon == "What is the most important thing to do during an earthquake?")
                    {
                        uIElements.ResolutionStateInfoText.text = " Although all of the choices are all important, doing the duck, cover and hold can save someone’s life from collapsing debris.";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (questiooon == "You were in a public mall when an earthquake suddenly hits. What should you do?")
                    {
                        uIElements.ResolutionStateInfoText.text = "A person should stay calm first in order not to spread panic to others";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (questiooon == "This is the most active volcano in asia. ")
                    {
                        uIElements.ResolutionStateInfoText.text = "According to Forbes, Mount Agung is considered to be the most dangerous volcano in asia.";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (questiooon == "What country has experienced the strongest earthquake that the world has seen.")
                    {
                        uIElements.ResolutionStateInfoText.text = "“The Great Chilean Earthquake” is the world’s strongest earthquake with a magnitude of 9.5 in 1960.";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (questiooon == "What can the local baranggay do to help prepare their subjects for a possible earthquake?")
                    {
                        uIElements.ResolutionStateInfoText.text = "Participate in office and community earthquake drills.";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (questiooon == "What country has the most earthquakes?")
                    {
                        uIElements.ResolutionStateInfoText.text = "Indonesia";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (questiooon == "The local government recently warned about the possibility of an earthquake. You try to decide what to do to your furniture.")
                    {
                        uIElements.ResolutionStateInfoText.text = "Secure heavy furniture and hanging objects.";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (questiooon == "You own a store that sells different kinds of chemicals. In part with earthquake safety, what should you do?")
                    {
                        uIElements.ResolutionStateInfoText.text = "Store harmful chemicals and flammable materials properly.";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (questiooon == "You noticed that your friend’s house is not in a very good condition. The Phivolcs recently warned the public about the possibility of an earthquake. What should you do?")
                    {
                        uIElements.ResolutionStateInfoText.text = "Point out the current condition of the house and advice them for repairs.";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    break;
                case ResolutionScreenType.Incorrect:
                    uIElements.ResolutionBG.color = parameters.IncorrectBGColor;
                    if (questiooon == "The local government warns the public about the possibility of an earthquake. What should you do?")
                    {
                        uIElements.ResolutionStateInfoText.text = "Familiarize yourself with all possible exit routes.";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (questiooon == "You and your family are driving for your vacation when suddenly an earthquake hits.What should you do?")
                    {
                        uIElements.ResolutionStateInfoText.text = "Step out and move towards a safer area.";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (questiooon == "An earthquake suddenly hits and you are able to exit the building safely. Once you are outside of the building, What you should do?")
                    {
                        uIElements.ResolutionStateInfoText.text = "Evacuate to an open area.You should always be on the lookout for any structures that might collapse.";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (questiooon == "While running towards the exit during an earthquake. You should avoid?")
                    {
                        uIElements.ResolutionStateInfoText.text = "During an earthquake, avoid using electricity powered machines like elevators and escalators.";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (questiooon == "You and your family are shopping in a mall when an earthquake suddenly hits. You did the duck, cover and hold and are ready to exit the building. What should you look out for?")
                    {
                        uIElements.ResolutionStateInfoText.text = "All of the above, Stay away from glass windows, shelves, hanging objects or any sharp or heavy objects that can cause severe injuries.";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (questiooon == "What is the most important thing to do during an earthquake?")
                    {
                        uIElements.ResolutionStateInfoText.text = " Although all of the choices are all important, doing the duck, cover and hold can save someone’s life from collapsing debris.";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (questiooon == "You were in a public mall when an earthquake suddenly hits. What should you do?")
                    {
                        uIElements.ResolutionStateInfoText.text = "A person should stay calm first in order not to spread panic to others";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (questiooon == "This is the most active volcano in asia. ")
                    {
                        uIElements.ResolutionStateInfoText.text = "According to Forbes, Mount Agung is considered to be the most dangerous volcano in asia.";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (questiooon == "What country has experienced the strongest earthquake that the world has seen.")
                    {
                        uIElements.ResolutionStateInfoText.text = "“The Great Chilean Earthquake” is the world’s strongest earthquake with a magnitude of 9.5 in 1960.";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (questiooon == "What can the local baranggay do to help prepare their subjects for a possible earthquake?")
                    {
                        uIElements.ResolutionStateInfoText.text = "Participate in office and community earthquake drills.";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (questiooon == "What country has the most earthquakes?")
                    {
                        uIElements.ResolutionStateInfoText.text = "Indonesia";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (questiooon == "The local government recently warned about the possibility of an earthquake. You try to decide what to do to your furniture.")
                    {
                        uIElements.ResolutionStateInfoText.text = "Secure heavy furniture and hanging objects.";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (questiooon == "You own a store that sells different kinds of chemicals. In part with earthquake safety, what should you do?")
                    {
                        uIElements.ResolutionStateInfoText.text = "Store harmful chemicals and flammable materials properly.";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (questiooon == "You noticed that your friend’s house is not in a very good condition. The Phivolcs recently warned the public about the possibility of an earthquake. What should you do?")
                    {
                        uIElements.ResolutionStateInfoText.text = "Point out the current condition of the house and advice them for repairs.";
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
                    if (questiooon == "What should you do AFTER an earthquake?")
                    {
                        uIElements.ResolutionStateInfoText.text = "Stay alert in the event of aftershocks.";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (questiooon == "What do you call the unsubstantiated theory about how to survive a major earthquake wherein the theory advocates methods of protection very different from ''drop, cover, and hold on'' ? ")
                    {
                        uIElements.ResolutionStateInfoText.text = "Triangle of Life";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (questiooon == "What to do DURING earthquakes?")
                    {
                        uIElements.ResolutionStateInfoText.text = "Stay calm and stay put.";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (questiooon == "What do you call a kit that includes bath soap, toothbrush, toothpaste and sanitary pads?")
                    {
                        uIElements.ResolutionStateInfoText.text = "Toiletries";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (questiooon == "These are considered essential emergency kits except:")
                    {
                        uIElements.ResolutionStateInfoText.text = "Hiking equipment";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (questiooon == "What does PHILVOCS stands for?")
                    {
                        uIElements.ResolutionStateInfoText.text = "Philippine Institute of Volcanology and Seismology";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (questiooon == "The local government agency that has been urging the public to be prepared for earthquakes, considering that they are unpredictable and may strike anytime is?")
                    {
                        uIElements.ResolutionStateInfoText.text = "PHILVOCS";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (questiooon == "You are recently a victim of a super typhoon. In the process your house was recently destroyed by flash floods. What should you do? ")
                    {
                        uIElements.ResolutionStateInfoText.text = "Elevate electrical components, loose electrical cords might cause electrocution";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (questiooon == " An earthquake just recently hit and there so many injured what should you do? ")
                    {
                        uIElements.ResolutionStateInfoText.text = "provide first aid for the injured";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (questiooon == "You recently experienced an earthquake and are safe during the earthquake, what you should do?")
                    {
                        uIElements.ResolutionStateInfoText.text = "Stay alert in the event of aftershocks.";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (questiooon == "You are living near the beach, then suddenly an earthquake hits. What you should do?")
                    {
                        uIElements.ResolutionStateInfoText.text = "Evacuate immediately, because it is most likely that a tsunami will hit.";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (questiooon == "2-What should you do AFTER a flood?")//9
                    {
                        uIElements.ResolutionStateInfoText.text = "protect your property from future flood damage. Elevation of utilities and electrical components above the potential flood height as consideration of the elevation of the entire structure can be reduce the damage next time around";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (questiooon == "1-What should you do AFTER a flood?")//8
                    {
                        uIElements.ResolutionStateInfoText.text = "Use extreme caution when entering buildings. Examine walls, floors, doors, windows, and ceilings for risk of collapsing.";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (questiooon == "What should you do AFTER a flood?")//7
                    {
                        uIElements.ResolutionStateInfoText.text = "Wait until it is safe to return. Do not return to flooded areas until the authorities indicate it is safe to do so.";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (questiooon == "5-What should you do AFTER an earthquake?")//6
                    {
                        uIElements.ResolutionStateInfoText.text = "Message your loved ones of your state and where you are.";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (questiooon == "4-What should you do AFTER an earthquake?")//5
                    {
                        uIElements.ResolutionStateInfoText.text = "Stay updated through a battery-operated radio.";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (questiooon == "3-What should you do AFTER an earthquake?")//4
                    {
                        uIElements.ResolutionStateInfoText.text = "Check for water, electrical, gas, or LPG leaks and damages.";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (questiooon == "2-What should you do AFTER an earthquake?")//3
                    {
                        uIElements.ResolutionStateInfoText.text = "Stay out of buildings until advised.";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (questiooon == "If you are living in the coastal area, what should you do AFTER an earthquake?")//2
                    {
                        uIElements.ResolutionStateInfoText.text = "Evacuate to higher ground immediately.";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (questiooon == "1-What should you do AFTER an earthquake?")//1
                    {
                        uIElements.ResolutionStateInfoText.text = "Provide first aid for any possible injuries.";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    break;
                case ResolutionScreenType.Incorrect:
                    uIElements.ResolutionBG.color = parameters.IncorrectBGColor;
                    if (questiooon == "What should you do AFTER an earthquake?")
                    {
                        uIElements.ResolutionStateInfoText.text = "Stay alert in the event of aftershocks.";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (questiooon == "What do you call the unsubstantiated theory about how to survive a major earthquake wherein the theory advocates methods of protection very different from ''drop, cover, and hold on'' ? ")
                    {
                        uIElements.ResolutionStateInfoText.text = "Triangle of Life";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (questiooon == "What to do DURING earthquakes?")
                    {
                        uIElements.ResolutionStateInfoText.text = "Stay calm and stay put.";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (questiooon == "What do you call a kit that includes bath soap, toothbrush, toothpaste and sanitary pads?")
                    {
                        uIElements.ResolutionStateInfoText.text = "Toiletries";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (questiooon == "These are considered essential emergency kits except:")
                    {
                        uIElements.ResolutionStateInfoText.text = "Hiking equipment";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (questiooon == "What does PHILVOCS stands for?")
                    {
                        uIElements.ResolutionStateInfoText.text = "Philippine Institute of Volcanology and Seismology";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (questiooon == "The local government agency that has been urging the public to be prepared for earthquakes, considering that they are unpredictable and may strike anytime is?")
                    {
                        uIElements.ResolutionStateInfoText.text = "PHILVOCS";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (questiooon == "You are recently a victim of a super typhoon. In the process your house was recently destroyed by flash floods. What should you do? ")
                    {
                        uIElements.ResolutionStateInfoText.text = "Elevate electrical components, loose electrical cords might cause electrocution";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (questiooon == " An earthquake just recently hit and there so many injured what should you do? ")
                    {
                        uIElements.ResolutionStateInfoText.text = "provide first aid for the injured";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (questiooon == "You recently experienced an earthquake and are safe during the earthquake, what you should do?")
                    {
                        uIElements.ResolutionStateInfoText.text = "Stay alert in the event of aftershocks.";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (questiooon == "You are living near the beach, then suddenly an earthquake hits. What you should do?")
                    {
                        uIElements.ResolutionStateInfoText.text = "Evacuate immediately, because it is most likely that a tsunami will hit.";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (questiooon == "2-What should you do AFTER a flood?")//9
                    {
                        uIElements.ResolutionStateInfoText.text = "protect your property from future flood damage. Elevation of utilities and electrical components above the potential flood height as consideration of the elevation of the entire structure can be reduce the damage next time around";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (questiooon == "1-What should you do AFTER a flood?")//8
                    {
                        uIElements.ResolutionStateInfoText.text = "Use extreme caution when entering buildings. Examine walls, floors, doors, windows, and ceilings for risk of collapsing.";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (questiooon == "What should you do AFTER a flood?")//7
                    {
                        uIElements.ResolutionStateInfoText.text = "Wait until it is safe to return. Do not return to flooded areas until the authorities indicate it is safe to do so.";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (questiooon == "5-What should you do AFTER an earthquake?")//6
                    {
                        uIElements.ResolutionStateInfoText.text = "Message your loved ones of your state and where you are.";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (questiooon == "4-What should you do AFTER an earthquake?")//5
                    {
                        uIElements.ResolutionStateInfoText.text = "Stay updated through a battery-operated radio.";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (questiooon == "3-What should you do AFTER an earthquake?")//4
                    {
                        uIElements.ResolutionStateInfoText.text = "Check for water, electrical, gas, or LPG leaks and damages.";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (questiooon == "2-What should you do AFTER an earthquake?")//3
                    {
                        uIElements.ResolutionStateInfoText.text = "Stay out of buildings until advised.";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (questiooon == "If you are living in the coastal area, what should you do AFTER an earthquake?")//2
                    {
                        uIElements.ResolutionStateInfoText.text = "Evacuate to higher ground immediately.";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (questiooon == "1-What should you do AFTER an earthquake?")//1
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
                    if (questiooon == "What refers to several forms of mass wasting that include a wide range of ground movements, such as rock falls, deep-seated slope failures, mud flows, and debris flows?")
                    {
                        uIElements.ResolutionStateInfoText.text = "Landslide";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (questiooon == "NDRRMC stands for:")
                    {
                        uIElements.ResolutionStateInfoText.text = "National Disaster Risk Reduction & Management Council";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (questiooon == "It is a way wherein local government agencies provide safety warnings through television broadcasting, text messages alerts, and social media. What is it?")
                    {
                        uIElements.ResolutionStateInfoText.text = "Early warning systems";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (questiooon == "These are ways to prevent casualties during landslide except one. Which is it?")
                    {
                        uIElements.ResolutionStateInfoText.text = "Checking maximum sustained winds";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (questiooon == "All of these are important things to do AFTER a flood except one:")
                    {
                        uIElements.ResolutionStateInfoText.text = "Tell your neighbors that it is safe to return";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (questiooon == "It is an event in which the surface wind increases in magnitude above the mean by factors of 1.2 to 1.6 or higher")
                    {
                        uIElements.ResolutionStateInfoText.text = "Squall";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (questiooon == "What to do BEFORE a flood?")
                    {
                        uIElements.ResolutionStateInfoText.text = "Being cautious about your surroundings can  prevent damage to your family and your property.";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (questiooon == "What is a downslope viscous flow of fine-grained materials that have been saturated with water and moves under the pull of gravity?")
                    {
                        uIElements.ResolutionStateInfoText.text = "Earth flow";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (questiooon == "What do you call the oceanic event incorporated with abnormal rise of water due to a tropical cyclone?")
                    {
                        uIElements.ResolutionStateInfoText.text = "Storm surge";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (questiooon == "What to do DURING a flood?")
                    {
                        uIElements.ResolutionStateInfoText.text = "Get to high ground.";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (questiooon == "What to do AFTER a flood?")
                    {
                        uIElements.ResolutionStateInfoText.text = "Wait until the authorities indicate that it is safe to return.";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (questiooon == "What is a tropical cyclone with maximum wind speed exceeding 220 kph or more than 120 knots?")
                    {
                        uIElements.ResolutionStateInfoText.text = "Super typhoon";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (questiooon == "What is a tropical cyclone with maximum wind speed of 89 to 117 kph or 48 - 63 knots?")
                    {
                        uIElements.ResolutionStateInfoText.text = "Severe tropical storm";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (questiooon == "What is a tropical cyclone with maximum wind speed of 118 to 220 kph or 64 - 120 knots?")
                    {
                        uIElements.ResolutionStateInfoText.text = "Typhoon";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (questiooon == "What do you call a tropical cyclone with maximum wind speed of 62 to 88 kph or 34 - 47 knots?")
                    {
                        uIElements.ResolutionStateInfoText.text = "Tropical storm";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (questiooon == "What do you call a tropical cyclone with maximum sustained winds of up to 61 kilometers per hour (kph) or less than 33 nautical miles per hour (knots)?")
                    {
                        uIElements.ResolutionStateInfoText.text = "Tropical depression";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (questiooon == "These are some ways to do BEFORE a flood except one. What is it?")
                    {
                        uIElements.ResolutionStateInfoText.text = "Discuss an outing plan to your family.";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (questiooon == "Each typhoon classification are based on what reference?")
                    {
                        uIElements.ResolutionStateInfoText.text = "Maximum sustained winds";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (questiooon == "What is the local government agency that provides safety warnings through television broadcasting regarding typhoons and floods?")
                    {
                        uIElements.ResolutionStateInfoText.text = "PAG-ASA";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    else if (questiooon == "What do you call the way of highlighting areas that were affected by or vulnerable to a particular hazard?")
                    {
                        uIElements.ResolutionStateInfoText.text = "Hazard mapping";
                        uIElements.ResolutionScoreText.text = "+" + score;
                    }
                    break;
                case ResolutionScreenType.Incorrect:
                    uIElements.ResolutionBG.color = parameters.IncorrectBGColor;
                    if (questiooon == "What refers to several forms of mass wasting that include a wide range of ground movements, such as rock falls, deep-seated slope failures, mud flows, and debris flows?")
                    {
                        uIElements.ResolutionStateInfoText.text = "Landslide";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (questiooon == "NDRRMC stands for:")
                    {
                        uIElements.ResolutionStateInfoText.text = "National Disaster Risk Reduction & Management Council";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (questiooon == "It is a way wherein local government agencies provide safety warnings through television broadcasting, text messages alerts, and social media. What is it?")
                    {
                        uIElements.ResolutionStateInfoText.text = "Early warning systems";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (questiooon == "These are ways to prevent casualties during landslide except one. Which is it?")
                    {
                        uIElements.ResolutionStateInfoText.text = "Checking maximum sustained winds";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (questiooon == "All of these are important things to do AFTER a flood except one:")
                    {
                        uIElements.ResolutionStateInfoText.text = "Tell your neighbors that it is safe to return";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (questiooon == "It is an event in which the surface wind increases in magnitude above the mean by factors of 1.2 to 1.6 or higher")
                    {
                        uIElements.ResolutionStateInfoText.text = "Squall";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (questiooon == "What to do BEFORE a flood?")
                    {
                        uIElements.ResolutionStateInfoText.text = "Being cautious about your surroundings can  prevent damage to your family and your property.";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (questiooon == "What is a downslope viscous flow of fine-grained materials that have been saturated with water and moves under the pull of gravity?")
                    {
                        uIElements.ResolutionStateInfoText.text = "Earth flow";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (questiooon == "What do you call the oceanic event incorporated with abnormal rise of water due to a tropical cyclone?")
                    {
                        uIElements.ResolutionStateInfoText.text = "Storm surge";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (questiooon == "What to do DURING a flood?")
                    {
                        uIElements.ResolutionStateInfoText.text = "Get to high ground.";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (questiooon == "What to do AFTER a flood?")
                    {
                        uIElements.ResolutionStateInfoText.text = "Wait until the authorities indicate that it is safe to return.";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (questiooon == "What is a tropical cyclone with maximum wind speed exceeding 220 kph or more than 120 knots?")
                    {
                        uIElements.ResolutionStateInfoText.text = "Super typhoon";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (questiooon == "What is a tropical cyclone with maximum wind speed of 89 to 117 kph or 48 - 63 knots?")
                    {
                        uIElements.ResolutionStateInfoText.text = "Severe tropical storm";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (questiooon == "What is a tropical cyclone with maximum wind speed of 118 to 220 kph or 64 - 120 knots?")
                    {
                        uIElements.ResolutionStateInfoText.text = "Typhoon";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (questiooon == "What do you call a tropical cyclone with maximum wind speed of 62 to 88 kph or 34 - 47 knots?")
                    {
                        uIElements.ResolutionStateInfoText.text = "Tropical storm";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (questiooon == "What do you call a tropical cyclone with maximum sustained winds of up to 61 kilometers per hour (kph) or less than 33 nautical miles per hour (knots)?")
                    {
                        uIElements.ResolutionStateInfoText.text = "Tropical depression";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (questiooon == "These are some ways to do BEFORE a flood except one. What is it?")
                    {
                        uIElements.ResolutionStateInfoText.text = "Discuss an outing plan to your family.";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (questiooon == "Each typhoon classification are based on what reference?")
                    {
                        uIElements.ResolutionStateInfoText.text = "Maximum sustained winds";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (questiooon == "What is the local government agency that provides safety warnings through television broadcasting regarding typhoons and floods?")
                    {
                        uIElements.ResolutionStateInfoText.text = "PAG-ASA";
                        uIElements.ResolutionScoreText.text = "-" + score;
                    }
                    else if (questiooon == "What do you call the way of highlighting areas that were affected by or vulnerable to a particular hazard?")
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