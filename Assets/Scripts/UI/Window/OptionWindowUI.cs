using UnityEngine.UI;

namespace PJR
{
    public class OptionWindowUI : UINode
    {
        public Slider musicSlider;
        public Slider soundEffectSlider;
        public Button closeBtn;
        public Toggle hintToggle;
        public Image toggleOffMark;
        public Text musicVolumeTxt;
        public Text soundEffectVolumeTxt;
        public override void OnStart()
        {
            base.OnStart();
            closeBtn.onClick.AddListener(() => Close());
            toggleOffMark.gameObject.SetActive(!hintToggle.isOn);
            hintToggle.onValueChanged.AddListener((isOn) => { toggleOffMark.gameObject.SetActive(!isOn); });
            musicVolumeTxt.text = musicSlider.value.ToString();
            soundEffectVolumeTxt.text = soundEffectSlider.value.ToString();
            musicSlider.onValueChanged.AddListener((value) => { musicVolumeTxt.text = value.ToString(); });
            soundEffectSlider.onValueChanged.AddListener((value) => { soundEffectVolumeTxt.text = value.ToString(); });
        }
        
    }
}

