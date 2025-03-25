using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PJR
{
    public class InGameUI : UINode
    {
        public Slider goalSlider;
        public Image dangerImg;

        public GameObject model;
        public Image modelTxtImg;
        public Image playerClasses;
        public Text timeCounterTxt;
    }
}

