//由LSFlagEditorUtil.Editor_ConvertFlagDefineSetToStaticClass根据Assets/__LS/Config/Flag下配置生成
//为了在代码里可以直接引用到对应的Flag
public class FlagHandleReference
{
	public static class CharacterAction
	{
		public static FlagHandle512 Category = new FlagHandle512(1000000);
		public static FlagHandle512 None = new FlagHandle512(1000001);
		public static FlagHandle512 Run = new FlagHandle512(1000002);
		public static FlagHandle512 Jump = new FlagHandle512(1000003);
		public static FlagHandle512 Roll = new FlagHandle512(1000004);
		public static FlagHandle512 Attack = new FlagHandle512(1000005);
		public static FlagHandle512 Aim = new FlagHandle512(1000006);
		public static FlagHandle512 AimCancel = new FlagHandle512(1000007);
		public static FlagHandle512 Shoot = new FlagHandle512(1000008);
		public static FlagHandle512 AttackCharge = new FlagHandle512(1000009);
		public static FlagHandle512 AttackJustRelease = new FlagHandle512(1000010);
		public static FlagHandle512 AttackRelease = new FlagHandle512(1000011);
		public static FlagHandle512 Defend = new FlagHandle512(1000012);
		public static FlagHandle512 UseItem = new FlagHandle512(1000013);
		public static FlagHandle512 Interact = new FlagHandle512(1000014);
		public static FlagHandle512 TestTemp = new FlagHandle512(1000015);
	}
	public static class EnemyAction
	{
		public static FlagHandle512 Category = new FlagHandle512(2000000);
		public static FlagHandle512 None = new FlagHandle512(2000001);
		public static FlagHandle512 Walk = new FlagHandle512(2000002);
		public static FlagHandle512 Run = new FlagHandle512(2000003);
		public static FlagHandle512 Jump = new FlagHandle512(2000004);
		public static FlagHandle512 Roll = new FlagHandle512(2000005);
		public static FlagHandle512 Idle_Pose = new FlagHandle512(2000006);
		public static FlagHandle512 Idle_Sleep = new FlagHandle512(2000007);
		public static FlagHandle512 Idle_Wake = new FlagHandle512(2000008);
		public static FlagHandle512 Idle_Sit = new FlagHandle512(2000009);
		public static FlagHandle512 Idle_SitUp = new FlagHandle512(2000010);
		public static FlagHandle512 Idle_Entrance = new FlagHandle512(2000011);
		public static FlagHandle512 Vigilant_Alert = new FlagHandle512(2000012);
		public static FlagHandle512 Vigilant_LookAround = new FlagHandle512(2000013);
		public static FlagHandle512 Vigilant_ConfirmTarget = new FlagHandle512(2000014);
		public static FlagHandle512 Vigilant_ThreatenTarget = new FlagHandle512(2000015);
		public static FlagHandle512 Attack_Normal1_普通攻击1 = new FlagHandle512(2000016);
		public static FlagHandle512 Attack_Normal2_普通攻击2 = new FlagHandle512(2000017);
		public static FlagHandle512 Attack_Normal3_普通攻击3 = new FlagHandle512(2000018);
		public static FlagHandle512 Attack_Heavy1_重攻击1 = new FlagHandle512(2000019);
		public static FlagHandle512 Attack_Heavy2_重攻击2 = new FlagHandle512(2000020);
		public static FlagHandle512 Attack_Heavy3_重攻击3 = new FlagHandle512(2000021);
		public static FlagHandle512 Attack_Combo2_两连击 = new FlagHandle512(2000022);
		public static FlagHandle512 Attack_Combo3_三连击 = new FlagHandle512(2000023);
		public static FlagHandle512 Attack_Combo4_四连击 = new FlagHandle512(2000024);
		public static FlagHandle512 Attack_Special_特殊招式 = new FlagHandle512(2000025);
		public static FlagHandle512 Attack_Skill_技能招式 = new FlagHandle512(2000026);
		public static FlagHandle512 Attack_Final_终极招式 = new FlagHandle512(2000027);
		public static FlagHandle512 Attack_Dash1_冲刺攻击1 = new FlagHandle512(2000028);
		public static FlagHandle512 Attack_Dash2_冲刺攻击2 = new FlagHandle512(2000029);
		public static FlagHandle512 Interaction1_交互1 = new FlagHandle512(2000030);
		public static FlagHandle512 Interaction2_交互2 = new FlagHandle512(2000031);
		public static FlagHandle512 Interaction3_交互3 = new FlagHandle512(2000032);
		public static FlagHandle512 Interaction4_交互4 = new FlagHandle512(2000033);
		public static FlagHandle512 Angry_Input = new FlagHandle512(2000034);
		public static FlagHandle512 JumpBack_Input = new FlagHandle512(2000035);
		public static FlagHandle512 JumpLeft_Input = new FlagHandle512(2000036);
		public static FlagHandle512 JumpRight_Input = new FlagHandle512(2000037);
		public static FlagHandle512 AirThreaten_Input = new FlagHandle512(2000038);
		public static FlagHandle512 IdlePose2_Input = new FlagHandle512(2000039);
		public static FlagHandle512 IdlePose3_Input = new FlagHandle512(2000040);
		public static FlagHandle512 DefaultPerform1_Input = new FlagHandle512(2000041);
		public static FlagHandle512 DefaultPerform2_Input = new FlagHandle512(2000042);
		public static FlagHandle512 DefaultPerform3_Input = new FlagHandle512(2000043);
		public static FlagHandle512 ChangeStage_Input = new FlagHandle512(2000044);
		public static FlagHandle512 Roar1_Input = new FlagHandle512(2000045);
		public static FlagHandle512 Roar2_Input = new FlagHandle512(2000046);
		public static FlagHandle512 Ranged_Input = new FlagHandle512(2000047);
		public static FlagHandle512 GrabStart_Input = new FlagHandle512(2000048);
		public static FlagHandle512 GrabAttack_Input = new FlagHandle512(2000049);
		public static FlagHandle512 Ogre_windMill_Input = new FlagHandle512(2001001);
		public static FlagHandle512 Ogre_JumpHit_Input = new FlagHandle512(2001002);
		public static FlagHandle512 Ogre_JumpHit2_Input = new FlagHandle512(2001003);
		public static FlagHandle512 Ogre_RoarShock_Input = new FlagHandle512(2001004);
		public static FlagHandle512 Ogre_Trample_Input = new FlagHandle512(2001005);
		public static FlagHandle512 Ogre_ShowWeapon_Input = new FlagHandle512(2001006);
		public static FlagHandle512 Ogre_Hungry_Input = new FlagHandle512(2001007);
		public static FlagHandle512 Ogre_MotorDrive_Input = new FlagHandle512(2001008);
		public static FlagHandle512 Ogre_PoisonFog_Input = new FlagHandle512(2001009);
		public static FlagHandle512 Boss_EarthGenie_TestCombo_Input = new FlagHandle512(2002001);
		public static FlagHandle512 Boss_EarthGenie_CommonRedirect_Input = new FlagHandle512(2002002);
		public static FlagHandle512 KingForestPig_HeadHit_Input = new FlagHandle512(2003001);
		public static FlagHandle512 KingForestPig_HeadHit2_Input = new FlagHandle512(2003002);
		public static FlagHandle512 KingForestPig_JumpHit_Input = new FlagHandle512(2003003);
		public static FlagHandle512 KingForestPig_DragonCar_Input = new FlagHandle512(2003004);
		public static FlagHandle512 KingForestPig_AirBlow_Input = new FlagHandle512(2003005);
		public static FlagHandle512 KingForestPig_TurnLeftHit_Input = new FlagHandle512(2003006);
		public static FlagHandle512 KingForestPig_TurnRightHit_Input = new FlagHandle512(2003007);
	}
	public static class SailAction
	{
		public static FlagHandle512 Category = new FlagHandle512(3000000);
		public static FlagHandle512 None = new FlagHandle512(3000001);
		public static FlagHandle512 Interact = new FlagHandle512(3000002);
		public static FlagHandle512 EngineStartStop = new FlagHandle512(3000003);
		public static FlagHandle512 Anchor = new FlagHandle512(3000004);
		public static FlagHandle512 ExitSail = new FlagHandle512(3000005);
	}
	public static class FishingAction
	{
		public static FlagHandle512 Category = new FlagHandle512(4000000);
		public static FlagHandle512 None = new FlagHandle512(4000001);
		public static FlagHandle512 Cast = new FlagHandle512(4000002);
		public static FlagHandle512 HoldIdle = new FlagHandle512(4000003);
		public static FlagHandle512 Interrupt = new FlagHandle512(4000004);
		public static FlagHandle512 Hook = new FlagHandle512(4000005);
		public static FlagHandle512 HookFail = new FlagHandle512(4000006);
		public static FlagHandle512 HookedIdle = new FlagHandle512(4000007);
		public static FlagHandle512 Control = new FlagHandle512(4000008);
		public static FlagHandle512 ControlLeft = new FlagHandle512(4000009);
		public static FlagHandle512 ControlRight = new FlagHandle512(4000010);
		public static FlagHandle512 OffHook = new FlagHandle512(4000011);
		public static FlagHandle512 Reel = new FlagHandle512(4000012);
		public static FlagHandle512 LiftFish = new FlagHandle512(4000013);
		public static FlagHandle512 LiftFishIdle = new FlagHandle512(4000014);
	}
	public static class LSStateTag_Character
	{
		public static FlagHandle512 Category = new FlagHandle512(5000000);
		public static FlagHandle512 FishingAction_None_State = new FlagHandle512(5000001);
		public static FlagHandle512 FishingAction_Cast_State = new FlagHandle512(5000002);
		public static FlagHandle512 FishingAction_HoldIdle_State = new FlagHandle512(5000003);
		public static FlagHandle512 FishingAction_Interrupt_State = new FlagHandle512(5000004);
		public static FlagHandle512 FishingAction_Hook_State = new FlagHandle512(5000005);
		public static FlagHandle512 FishingAction_HookFail_State = new FlagHandle512(5000006);
		public static FlagHandle512 FishingAction_HookedIdle_State = new FlagHandle512(5000007);
		public static FlagHandle512 FishingAction_Control_State = new FlagHandle512(5000008);
		public static FlagHandle512 FishingAction_ControlLeft_State = new FlagHandle512(5000009);
		public static FlagHandle512 FishingAction_ControlRight_State = new FlagHandle512(5000010);
		public static FlagHandle512 FishingAction_OffHook_State = new FlagHandle512(5000011);
		public static FlagHandle512 FishingAction_Reel_State = new FlagHandle512(5000012);
		public static FlagHandle512 FishingAction_LiftFish_State = new FlagHandle512(5000013);
		public static FlagHandle512 FishingAction_LiftFishIdle_State = new FlagHandle512(5000014);
		public static FlagHandle512 STATE_IDLE = new FlagHandle512(5000015);
		public static FlagHandle512 STATE_WALK = new FlagHandle512(5000016);
		public static FlagHandle512 STATE_RUN = new FlagHandle512(5000017);
		public static FlagHandle512 STATE_JUMP = new FlagHandle512(5000018);
		public static FlagHandle512 STATE_JUMP_AIRBORNE = new FlagHandle512(5000019);
		public static FlagHandle512 STATE_JUMP_LAND = new FlagHandle512(5000020);
		public static FlagHandle512 STATE_PUNCH_ATTACK_1 = new FlagHandle512(5000021);
		public static FlagHandle512 STATE_SWORD_NORMAL_ATTACK_1 = new FlagHandle512(5000022);
		public static FlagHandle512 STATE_SWORD_NORMAL_ATTACK_2 = new FlagHandle512(5000023);
		public static FlagHandle512 STATE_SWORD_NORMAL_ATTACK_3 = new FlagHandle512(5000024);
		public static FlagHandle512 STATE_SWORD_DASH_ATTACK = new FlagHandle512(5000025);
		public static FlagHandle512 STATE_SWORD_CHARGING = new FlagHandle512(5000026);
		public static FlagHandle512 STATE_SWORD_CHARGE_ATTACK = new FlagHandle512(5000027);
		public static FlagHandle512 STATE_SHIELD_BLOCKED_ATTACK = new FlagHandle512(5000028);
		public static FlagHandle512 STATE_SHIELD_SLAM = new FlagHandle512(5000029);
		public static FlagHandle512 STATE_SHIELD_ENTER_DEFENSE = new FlagHandle512(5000030);
		public static FlagHandle512 STATE_SHIELD_DEFENDING = new FlagHandle512(5000031);
		public static FlagHandle512 STATE_SHIELD_BLOCK = new FlagHandle512(5000032);
		public static FlagHandle512 STATE_HURT_NO = new FlagHandle512(5000033);
		public static FlagHandle512 STATE_HURT_WEAK_F = new FlagHandle512(5000034);
		public static FlagHandle512 STATE_HURT_WEAK_B = new FlagHandle512(5000035);
		public static FlagHandle512 STATE_HURT_HEAVY_F = new FlagHandle512(5000036);
		public static FlagHandle512 STATE_HURT_HEAVY_B = new FlagHandle512(5000037);
		public static FlagHandle512 STATE_HURT_KNOCKFLY_F = new FlagHandle512(5000038);
		public static FlagHandle512 STATE_HURT_KNOCKFLY_B = new FlagHandle512(5000039);
		public static FlagHandle512 STATE_HURT_KNOCKFLYING = new FlagHandle512(5000040);
		public static FlagHandle512 STATE_HURT_KNOCKDOWN = new FlagHandle512(5000041);
		public static FlagHandle512 STATE_HURT_STUN = new FlagHandle512(5000042);
		public static FlagHandle512 STATE_HURT_STAND_UP = new FlagHandle512(5000043);
		public static FlagHandle512 STATE_HURT_GROUND_DIE = new FlagHandle512(5000044);
		public static FlagHandle512 STATE_HURT_DIE = new FlagHandle512(5000045);
		public static FlagHandle512 STATE_RD_FA = new FlagHandle512(5000046);
		public static FlagHandle512 STATE_RD_FD = new FlagHandle512(5000047);
	}
	public static class LSStateTag_Enemy
	{
		public static FlagHandle512 Category = new FlagHandle512(6000000);
		public static FlagHandle512 STATE_IDLE = new FlagHandle512(6000001);
		public static FlagHandle512 STATE_TURN_AROUND = new FlagHandle512(6000002);
		public static FlagHandle512 STATE_POSE = new FlagHandle512(6000003);
		public static FlagHandle512 STATE_SLEEP = new FlagHandle512(6000004);
		public static FlagHandle512 STATE_WAKE = new FlagHandle512(6000005);
		public static FlagHandle512 STATE_IDLE_SIT = new FlagHandle512(6000006);
		public static FlagHandle512 STATE_IDLE_SITTING = new FlagHandle512(6000007);
		public static FlagHandle512 STATE_IDLE_SITUP = new FlagHandle512(6000008);
		public static FlagHandle512 STATE_ENTRANCE = new FlagHandle512(6000009);
		public static FlagHandle512 STATE_WALK = new FlagHandle512(6000010);
		public static FlagHandle512 STATE_RUN = new FlagHandle512(6000011);
		public static FlagHandle512 STATE_DASH = new FlagHandle512(6000012);
		public static FlagHandle512 STATE_AERIAL = new FlagHandle512(6000013);
		public static FlagHandle512 STATE_ALERT = new FlagHandle512(6000014);
		public static FlagHandle512 STATE_LOOK_AROUND = new FlagHandle512(6000015);
		public static FlagHandle512 STATE_CONFIRM_TARGET = new FlagHandle512(6000016);
		public static FlagHandle512 STATE_THREATEN_TARGET = new FlagHandle512(6000017);
		public static FlagHandle512 STATE_NORMAL_ATTACK_1 = new FlagHandle512(6000018);
		public static FlagHandle512 STATE_NORMAL_ATTACK_2 = new FlagHandle512(6000019);
		public static FlagHandle512 STATE_NORMAL_ATTACK_3 = new FlagHandle512(6000020);
		public static FlagHandle512 STATE_HEAVY_ATTACK_1 = new FlagHandle512(6000021);
		public static FlagHandle512 STATE_HEAVY_ATTACK_2 = new FlagHandle512(6000022);
		public static FlagHandle512 STATE_HEAVY_ATTACK_3 = new FlagHandle512(6000023);
		public static FlagHandle512 STATE_COMBO_ATTACK_2 = new FlagHandle512(6000024);
		public static FlagHandle512 STATE_COMBO_ATTACK_3 = new FlagHandle512(6000025);
		public static FlagHandle512 STATE_COMBO_ATTACK_4 = new FlagHandle512(6000026);
		public static FlagHandle512 STATE_SPECIAL_ATTACK = new FlagHandle512(6000027);
		public static FlagHandle512 STATE_SKILL_ATTACK = new FlagHandle512(6000028);
		public static FlagHandle512 STATE_FINAL_ATTACK = new FlagHandle512(6000029);
		public static FlagHandle512 STATE_DASH_ATTACK_1 = new FlagHandle512(6000030);
		public static FlagHandle512 STATE_DASH_ATTACK_2 = new FlagHandle512(6000031);
		public static FlagHandle512 STATE_DASH_TIRED = new FlagHandle512(6000032);
		public static FlagHandle512 STATE_HURT_WEAK_F = new FlagHandle512(6000033);
		public static FlagHandle512 STATE_HURT_WEAK_B = new FlagHandle512(6000034);
		public static FlagHandle512 STATE_HURT_HEAVY_F = new FlagHandle512(6000035);
		public static FlagHandle512 STATE_HURT_HEAVY_B = new FlagHandle512(6000036);
		public static FlagHandle512 STATE_HURT_KNOCKFLY_F = new FlagHandle512(6000037);
		public static FlagHandle512 STATE_HURT_KNOCKFLY_B = new FlagHandle512(6000038);
		public static FlagHandle512 STATE_HURT_KNOCKFLYING = new FlagHandle512(6000039);
		public static FlagHandle512 STATE_HURT_KNOCKDOWN = new FlagHandle512(6000040);
		public static FlagHandle512 STATE_HURT_STUN = new FlagHandle512(6000041);
		public static FlagHandle512 STATE_HURT_STAND_UP = new FlagHandle512(6000042);
		public static FlagHandle512 STATE_HURT_GROUND_DIE = new FlagHandle512(6000043);
		public static FlagHandle512 STATE_HURT_DIE = new FlagHandle512(6000044);
		public static FlagHandle512 STATE_INTERACTION1 = new FlagHandle512(6000045);
		public static FlagHandle512 STATE_INTERACTION2 = new FlagHandle512(6000046);
		public static FlagHandle512 STATE_INTERACTION3 = new FlagHandle512(6000047);
		public static FlagHandle512 STATE_INTERACTION4 = new FlagHandle512(6000048);
		public static FlagHandle512 Angry_State = new FlagHandle512(6000049);
		public static FlagHandle512 JumpBack_State = new FlagHandle512(6000050);
		public static FlagHandle512 JumpLeft_State = new FlagHandle512(6000051);
		public static FlagHandle512 JumpRight_State = new FlagHandle512(6000052);
		public static FlagHandle512 AirThreaten_State = new FlagHandle512(6000053);
		public static FlagHandle512 IdlePose2_State = new FlagHandle512(6000054);
		public static FlagHandle512 IdlePose3_State = new FlagHandle512(6000055);
		public static FlagHandle512 DefaultPerform1_State = new FlagHandle512(6000056);
		public static FlagHandle512 DefaultPerform2_State = new FlagHandle512(6000057);
		public static FlagHandle512 DefaultPerform3_State = new FlagHandle512(6000058);
		public static FlagHandle512 ChangeStage_State = new FlagHandle512(6000059);
		public static FlagHandle512 Roar1_State = new FlagHandle512(6000060);
		public static FlagHandle512 Roar2_State = new FlagHandle512(6000061);
		public static FlagHandle512 Ranged_State = new FlagHandle512(6000062);
		public static FlagHandle512 GrabStart_State = new FlagHandle512(6000063);
		public static FlagHandle512 GrabAttack_State = new FlagHandle512(6000064);
		public static FlagHandle512 Ogre_windMill_State = new FlagHandle512(6001001);
		public static FlagHandle512 Ogre_JumpHit_State = new FlagHandle512(6001002);
		public static FlagHandle512 Ogre_JumpHit2_State = new FlagHandle512(6001003);
		public static FlagHandle512 Ogre_RoarShock_State = new FlagHandle512(6001004);
		public static FlagHandle512 Ogre_Trample_State = new FlagHandle512(6001005);
		public static FlagHandle512 Ogre_ShowWeapon_State = new FlagHandle512(6001006);
		public static FlagHandle512 Ogre_Hungry_State = new FlagHandle512(6001007);
		public static FlagHandle512 Ogre_MotorDrive_State = new FlagHandle512(6001008);
		public static FlagHandle512 Ogre_PoisonFog_State = new FlagHandle512(6001009);
		public static FlagHandle512 Boss_EarthGenie_TestCombo_State = new FlagHandle512(6002001);
		public static FlagHandle512 Boss_EarthGenie_CommonRedirect_State = new FlagHandle512(6002002);
		public static FlagHandle512 KingForestPig_HeadHit_State = new FlagHandle512(6003001);
		public static FlagHandle512 KingForestPig_HeadHit2_State = new FlagHandle512(6003002);
		public static FlagHandle512 KingForestPig_JumpHit_State = new FlagHandle512(6003003);
		public static FlagHandle512 KingForestPig_DragonCar_State = new FlagHandle512(6003004);
		public static FlagHandle512 KingForestPig_AirBlow_State = new FlagHandle512(6003005);
		public static FlagHandle512 KingForestPig_TurnLeftHit_State = new FlagHandle512(6003006);
		public static FlagHandle512 KingForestPig_TurnRightHit_State = new FlagHandle512(6003007);
	}
	public static class LSBehaviourTag_Character
	{
		public static FlagHandle512 Category = new FlagHandle512(7000000);
		public static FlagHandle512 BEHAVIOUR_IDLE = new FlagHandle512(7000001);
		public static FlagHandle512 BEHAVIOUR_MOVE = new FlagHandle512(7000002);
		public static FlagHandle512 BEHAVIOUR_ATTACK = new FlagHandle512(7000003);
		public static FlagHandle512 BEHAVIOUR_DEFENSE = new FlagHandle512(7000004);
		public static FlagHandle512 BEHAVIOUR_HURT = new FlagHandle512(7000005);
		public static FlagHandle512 BEHAVIOUR_SWIM = new FlagHandle512(7000006);
	}
	public static class LSBehaviourTag_Enemy
	{
		public static FlagHandle512 Category = new FlagHandle512(8000000);
		public static FlagHandle512 BEHAVIOUR_IDLE = new FlagHandle512(8000001);
		public static FlagHandle512 BEHAVIOUR_MOVE = new FlagHandle512(8000002);
		public static FlagHandle512 BEHAVIOUR_VIGILANT = new FlagHandle512(8000003);
		public static FlagHandle512 BEHAVIOUR_ATTACK = new FlagHandle512(8000004);
		public static FlagHandle512 BEHAVIOUR_HURT = new FlagHandle512(8000005);
		public static FlagHandle512 BEHAVIOUR_INTERACTION = new FlagHandle512(8000006);
	}
}
