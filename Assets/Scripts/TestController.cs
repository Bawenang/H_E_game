using System.Text.RegularExpressions;

using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

using LoLSDK;

public class TestController : MonoBehaviour {

    public string bgm_str;
    //[SerializeField] private AudioClip _bgmClip;

    public string dialog_sfx_str;
    public string button_sfx_str;

    public string answer_correct_sfx_str;
    public string answer_wrong_sfx_str;

    [SerializeField] private StageDisplay stageDisplay;
    [SerializeField] private DialogController dialogCtrl;

    [SerializeField] private Button[] alternativeButtons = new Button[4];
    [SerializeField] private Text stemFirst;
    [SerializeField] private Text stemSecond;
    [SerializeField] private Image stemImage;
    [SerializeField] private Text stemNoImage;
    [SerializeField] private Text scoreText;

    [SerializeField] int maxQuestionsCounter;
    private int questionCounter = 0;
    private MultipleChoiceQuestion currentQuestion;

    private int rightAnswers = 0;

    private Color _altButtonColor;

    // Use this for initialization
    void Start() {
        stageDisplay.Show(4, "PlaySequenceWithSfx", dialogCtrl.gameObject, 0);

        LOLSubmitProgressWithCurrentScore(11);

        RefreshQuestion();


#if UNITY_WEBGL
        if (AudioController.Exists())
            AudioController.instance.SetBGM_Name(bgm_str);
#endif
    }


    private void RefreshQuestion()
    {
        ClearStem();

        if (this.questionCounter >= this.maxQuestionsCounter  || this.questionCounter >= LoLController.instance.questionList.questions.Length)
        {
            ShowExit();
            return;
        }

        this.currentQuestion = LoLController.instance.GetQuestion();
        if (this.currentQuestion == null)
        {
            ShowExit();
        }
        else {
            string pattern = @"([^\[\]]*)?(\[\S*\])?([^\[\]]*)?";
            MatchCollection matches = Regex.Matches(this.currentQuestion.stem, pattern);
            string stemStartValue = matches[0].Groups[1].Value;
            string stemImageValue = matches[0].Groups[2].Value;
            string stemEndValue = matches[0].Groups[3].Value;

            if (this.currentQuestion.imageURL != null && this.currentQuestion.imageURL.Length > 0)
            {
                SetFieldText(stemFirst, stemStartValue);
                SetFieldText(stemSecond, stemEndValue);
                StartCoroutine(LoadStemImage(this.currentQuestion.imageURL));
            }
            else {
                SetFieldText(stemNoImage, stemStartValue);
                HideStemImage();
            }

            for (int i = 0; i < 4; ++i)
            {
                SetFieldText(alternativeButtons[i].GetComponentInChildren<Text>(), this.currentQuestion.alternatives[i].text);
            }
        }
    }

    private void ClearStem()
    {
        SetFieldText(stemFirst, "");
        SetFieldText(stemSecond, "");
        SetFieldText(stemNoImage, "");
        HideStemImage();
    }

    private void ShowExit()
    {
        scoreText.text = "Your score is: " + Mathf.CeilToInt(((float)rightAnswers / (float)questionCounter) * 100.0f).ToString();

        dialogCtrl.PlaySequenceWithSfx(2);
    }

    public void EndGame()
    {
#if UNITY_WEBGL
        if (LoLController.Exists() && LoLController.instance.isUsingLoL)
        {
            LOLSubmitProgressWithCurrentScore(12);
            LoLSDK.LOLSDK.Instance.CompleteGame();
        }
#endif
        SceneManager.LoadScene("mainmenu");
    }

    private void SetFieldText(Text messageText, string message)
    {
        messageText.text = message;
    }

    private void HideStemImage()
    {
        stemImage.transform.localScale = new Vector3(0.0f, 0.0f, 0.0f);
    }

    private IEnumerator LoadStemImage(string imageURL)
    {
        Texture2D temp = new Texture2D(0, 0);
        WWW www = new WWW(imageURL);

        // Wait for download to complete
        yield return www;

        stemImage.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        temp = www.texture;
        Sprite sprite = Sprite.Create(temp, new Rect(0, 0, temp.width, temp.height), new Vector2(0.5f, 0.5f));
        Transform thumb = stemImage.transform;
        stemImage.sprite = sprite;
    }

    public void AlternativeClicked(int ctr)
    {

        Alternative alternative = this.currentQuestion.alternatives[ctr];
        MultipleChoiceAnswer answer = new MultipleChoiceAnswer();
        answer.alternativeId = alternative.alternativeId;
        answer.questionId = this.currentQuestion.questionId;
        LOLSDK.Instance.SubmitAnswer(answer);

        int correctId = -1;

        _altButtonColor = alternativeButtons[ctr].colors.normalColor;

        for (int i = 0; i < alternativeButtons.Length; ++i)
        {
            alternativeButtons[i].interactable = false;

            if (this.currentQuestion.correctAlternativeId.Equals(this.currentQuestion.alternatives[i].alternativeId))
                correctId = i;
        }

        

        if (this.currentQuestion.correctAlternativeId.Equals(answer.alternativeId))
        {
            //SetFieldText("MessageText", "Good Job!!");
            SetBlinkRight(alternativeButtons[ctr]);
            PlaySfx(answer_correct_sfx_str);
            ++rightAnswers;
        }
        else 
        {
            //SetFieldText("MessageText", "Sorry, but wrong!");
            SetBlinkRight(alternativeButtons[correctId]);
            SetBlinkWrong(alternativeButtons[ctr]);
            PlaySfx(answer_wrong_sfx_str);
        }

        StartCoroutine(GoToNextQuestion());
    }

    public void SetBlinkRight(Button altButton)
    {
        BlinkingButton blink = altButton.gameObject.AddComponent<BlinkingButton>();
        blink.fromColor = Color.white;
        blink.toColor = Color.green;
        blink.blinkTime = 0.2f;
    }

    public void SetBlinkWrong(Button altButton)
    {
        BlinkingButton blink = altButton.gameObject.AddComponent<BlinkingButton>();
        blink.fromColor = _altButtonColor;
        blink.toColor = Color.red;
        blink.blinkTime = 0.2f;
    }

    private IEnumerator GoToNextQuestion()
    {
        yield return new WaitForSeconds(2.0f);

        for (int i = 0; i < alternativeButtons.Length; ++i)
        {
            alternativeButtons[i].interactable = true;

            BlinkingButton blink = alternativeButtons[i].GetComponent<BlinkingButton>();
            if (blink != null)
                Destroy(blink);

            ColorBlock cb = alternativeButtons[i].colors;
            cb.disabledColor = _altButtonColor;
            alternativeButtons[i].colors = cb;
        }

        this.questionCounter++;
        RefreshQuestion();
    }

    public void PlaySfx(string my_clip_str)
    {
        if (LoLController.Exists() && LoLController.instance.isUsingLoL)
            LOLSDK.Instance.PlaySound(my_clip_str);
    }

    public void PlayButtonSfx()
    {
#if UNITY_WEBGL
        PlaySfx(button_sfx_str);
#endif
    }

    public void LOLSubmitProgressWithCurrentScore(int progress)
    {
        if (LoLController.Exists() && LoLController.instance.isUsingLoL)
            LoLSDK.LOLSDK.Instance.SubmitProgress(LoLController.instance.score, progress, 12);
    }
}
