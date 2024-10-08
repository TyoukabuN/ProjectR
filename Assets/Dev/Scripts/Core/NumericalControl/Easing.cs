﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace PJR
{
	public static class Easing
	{
		public enum Ease
		{
			Unset = 0,
			Linear = 1,
			InSine = 2,
			OutSine = 3,
			InOutSine = 4,
			InQuad = 5,
			OutQuad = 6,
			InOutQuad = 7,
			InCubic = 8,
			OutCubic = 9,
			InOutCubic = 10,
			InQuart = 11,
			OutQuart = 12,
			InOutQuart = 13,
			InQuint = 14,
			OutQuint = 15,
			InOutQuint = 16,
			InExpo = 17,
			OutExpo = 18,
			InOutExpo = 19,
			InCirc = 20,
			OutCirc = 21,
			InOutCirc = 22,
			InElastic = 23,
			OutElastic = 24,
			InOutElastic = 25,
			InBack = 26,
			OutBack = 27,
			InOutBack = 28,
			InBounce = 29,
			OutBounce = 30,
			InOutBounce = 31,
			Flash = 32,
			InFlash = 33,
			OutFlash = 34,
			InOutFlash = 35,
			INTERNAL_Zero = 36,
			INTERNAL_Custom = 37
		}
		//NOTICE//
		//请务必将变量x控制在[0,1]的区间中
		//
		public static float EaseLinear(float x)
		{
			return x;
		}
		//Sine
		public static float EaseInSine(float x)
		{
			return 1 - Mathf.Cos((x * Mathf.PI) / 2);
		}
		public static float EaseOutSine(float x)
		{
			return Mathf.Sin((x * Mathf.PI) / 2);
		}
		public static float EaseInOutSine(float x)
		{
			return -(Mathf.Cos(Mathf.PI * x) - 1) / 2;
		}
		//Quad
		public static float EaseInQuad(float x)
		{
			return x * x;
		}
		public static float EaseOutQuad(float x)
		{
			return 1 - (1 - x) * (1 - x);
		}
		public static float EaseInOutQuad(float x)
		{
			return x < 0.5 ? 2 * x * x : 1 - Mathf.Pow(-2 * x + 2, 2) / 2;
		}
		//Cubic
		public static float EaseInCubic(float x)
		{
			return x * x * x;
		}
		public static float EaseOutCubic(float x)
		{
			return 1 - Mathf.Pow(1 - x, 3);
		}
		public static float EaseInOutCubic(float x)
		{
			return x < 0.5 ? 4 * x * x * x : 1 - Mathf.Pow(-2 * x + 2, 3) / 2;
		}
		//Quart
		public static float EaseInQuart(float x)
		{
			return x * x * x * x;
		}
		public static float EaseOutQuart(float x)
		{
			return 1 - Mathf.Pow(1 - x, 4);
		}
		public static float EaseInOutQuart(float x)
		{
			return x < 0.5 ? 8 * x * x * x * x : 1 - Mathf.Pow(-2 * x + 2, 4) / 2;
		}
		//Quint
		public static float EaseInQuint(float x)
		{
			return x * x * x * x * x;
		}
		public static float EaseOutQuint(float x)
		{
			return 1 - Mathf.Pow(1 - x, 5);
		}
		public static float EaseInOutQuint(float x)
		{
			return x < 0.5 ? 16 * x * x * x * x * x : 1 - Mathf.Pow(-2 * x + 2, 5) / 2; ;
		}
		//Expo
		public static float EaseInExpo(float x)
		{
			return x == 0 ? 0 : Mathf.Pow(2, 10 * x - 10);
		}
		public static float EaseOutExpo(float x)
		{
			return x == 1 ? 1 : 1 - Mathf.Pow(2, -10 * x);
		}
		public static float EaseInOutExpo(float x)
		{
			return x == 0
			? 0
			: x == 1
			? 1
			: x < 0.5
			? Mathf.Pow(2, 20 * x - 10) / 2
			: (2 - Mathf.Pow(2, -20 * x + 10)) / 2;
		}
		//Circ
		public static float EaseInCirc(float x)
		{
			return 1 - Mathf.Sqrt(1 - Mathf.Pow(x, 2));
		}
		public static float EaseOutCirc(float x)
		{
			return Mathf.Sqrt(1 - Mathf.Pow(x - 1, 2));
		}
		public static float EaseInOutCirc(float x)
		{
			return x < 0.5
		  ? (1 - Mathf.Sqrt(1 - Mathf.Pow(2 * x, 2))) / 2
		  : (Mathf.Sqrt(1 - Mathf.Pow(-2 * x + 2, 2)) + 1) / 2;
		}
		//Elastic
		public static float EaseInElastic(float x)
		{
			float c4 = (2 * Mathf.PI) / 3;

			return x == 0
			  ? 0
			  : x == 1
			  ? 1
			  : -Mathf.Pow(2, 10 * x - 10) * Mathf.Sin((x * 10 - 10.75f) * c4);
		}
		public static float EaseOutElastic(float x)
		{
			float c4 = (2 * Mathf.PI) / 3;

			return x == 0
			  ? 0
			  : x == 1
			  ? 1
			  : Mathf.Pow(2, -10 * x) * Mathf.Sin((x * 10 - 0.75f) * c4) + 1;
		}
		public static float EaseInOutElastic(float x)
		{
			float c5 = (2 * Mathf.PI) / 4.5f;

			return x == 0
			  ? 0
			  : x == 1
			  ? 1
			  : x < 0.5
			  ? -(Mathf.Pow(2, 20 * x - 10) * Mathf.Sin((20 * x - 11.125f) * c5)) / 2
			  : (Mathf.Pow(2, -20 * x + 10) * Mathf.Sin((20 * x - 11.125f) * c5)) / 2 + 1;
		}
		//Back
		public static float EaseInBack(float x)
		{
			float c1 = 1.70158f;
			float c3 = c1 + 1;

			return c3 * x * x * x - c1 * x * x;
		}
		public static float EaseOutBack(float x)
		{
			float c1 = 1.70158f;
			float c3 = c1 + 1;

			return 1 + c3 * Mathf.Pow(x - 1, 3) + c1 * Mathf.Pow(x - 1, 2);
		}
		public static float EaseInOutBack(float x)
		{
			float c1 = 1.70158f;
			float c2 = c1 * 1.525f;

			return x < 0.5
			  ? (Mathf.Pow(2 * x, 2) * ((c2 + 1) * 2 * x - c2)) / 2
			  : (Mathf.Pow(2 * x - 2, 2) * ((c2 + 1) * (x * 2 - 2) + c2) + 2) / 2;
		}
		//Bounce
		public static float EaseOutBounce(float x)
		{
			float n1 = 7.5625f;
			float d1 = 2.75f;

			if (x < 1 / d1)
			{
				return n1 * x * x;
			}
			else if (x < 2 / d1)
			{
				return n1 * (x -= 1.5f / d1) * x + 0.75f;
			}
			else if (x < 2.5 / d1)
			{
				return n1 * (x -= 2.25f / d1) * x + 0.9375f;
			}
			else
			{
				return n1 * (x -= 2.625f / d1) * x + 0.984375f;
			}
		}
		public static float EaseInBounce(float x)
		{
			return 1 - EaseOutBounce(1 - x);
		}
		public static float EaseInOutBounce(float x)
		{
			return x < 0.5
			  ? (1 - EaseOutBounce(1 - 2 * x)) / 2
			  : (1 + EaseOutBounce(2 * x - 1)) / 2;
		}

		public static float DoEasing(Ease easeType,float x)
		{
			switch (easeType)
			{
				case Ease.Unset:
					return EaseLinear(x);				
				case Ease.Linear:
					return EaseLinear(x);				
				case Ease.InSine:
					return EaseInSine(x);				
				case Ease.OutSine:
					return EaseOutSine(x);
				case Ease.InOutSine:
					return EaseInOutSine(x);
				case Ease.InQuad:
					return EaseInQuad(x);
				case Ease.OutQuad:
					return EaseOutQuad(x);
				case Ease.InOutQuad:
					return EaseInOutQuad(x);
				case Ease.InCubic:
					return EaseInCubic(x);
				case Ease.OutCubic:
					return EaseOutCubic(x);
				case Ease.InOutCubic:
					return EaseInOutCubic(x);
				case Ease.InQuart:
					return EaseInQuart(x);
				case Ease.OutQuart:
					return EaseOutQuart(x);
				case Ease.InOutQuart:
					return EaseInOutQuart(x);
				case Ease.InQuint:
					return EaseInQuint(x);
				case Ease.OutQuint:
					return EaseOutQuint(x);
				case Ease.InOutQuint:
					return EaseInOutQuint(x);
				case Ease.InExpo:
					return EaseInExpo(x);
				case Ease.OutExpo:
					return EaseOutExpo(x);
				case Ease.InOutExpo:
					return EaseInOutExpo(x);
				case Ease.InCirc:
					return EaseInCirc(x);
				case Ease.OutCirc:
					return EaseOutCirc(x);
				case Ease.InOutCirc:
					return EaseInOutCirc(x);
				case Ease.InElastic:
					return EaseInElastic(x);
				case Ease.OutElastic:
					return EaseOutElastic(x);
				case Ease.InOutElastic:
					return EaseInOutElastic(x);
				case Ease.InBack:
					return EaseInBack(x);
				case Ease.OutBack:
					return EaseOutBack(x);
				case Ease.InOutBack:
					return EaseInOutBack(x);
				case Ease.InBounce:
					return EaseInBounce(x);
				case Ease.OutBounce:
					return EaseOutBounce(x);
				case Ease.InOutBounce:
					return EaseInOutBounce(x);
				default:
					return EaseLinear(x);
			}
		}
	}
}