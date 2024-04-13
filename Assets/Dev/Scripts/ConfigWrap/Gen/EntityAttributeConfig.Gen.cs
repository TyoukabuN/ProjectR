namespace PJR
{
	public class EntityAttributeConfig
	{
		public FloatConstConfig  asset;
		
		/// <summary>
		/// 硬直时间
		/// <summary>
		public float HitStunTime => GetValue(0);
		/// <summary>
		/// 击退时间
		/// <summary>
		public float RepelTime => GetValue(1);
		/// <summary>
		/// 被击飞时间
		/// <summary>
		public float BlowUpTime => GetValue(2);
		/// <summary>
		/// 起身无敌时间
		/// <summary>
		public float GetUpImbaTime => GetValue(3);
		/// <summary>
		/// 无敌时间
		/// <summary>
		public float ImbaTime => GetValue(4);
		/// <summary>
		/// 角色攻击伤害
		/// <summary>
		public float PlayerAtk => GetValue(5);
		/// <summary>
		/// 角色攻击之间的间隔
		/// <summary>
		public float PlayerAtkInterval => GetValue(6);
		float GetValue(int index)
		{
			if(asset == null || asset.items == null || asset.items.Count <= index)
				 return 0f;
			return asset.items[index].value;
		}
	}
}
