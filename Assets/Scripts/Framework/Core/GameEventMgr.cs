using UnityEngine;
using System.Collections.Generic;
using System;
using System.Threading;
using Framework.Library.Singleton;
/*
public enum GameEvent
{
    NONE,
    GAME_BATTLE_START               = 1001,//游戏开始
    GAME_BATTLE_END                 = 1002,//游戏结束
    NERWORK_DISCONNECTED            = 1003,//游戏中网络断线
	GAME_BATTLE_PAUSE				= 1004,//游戏暂停
	GAME_BATTLE_RESUME				= 1005,//游戏恢复
	GAME_ENTER_CG					= 1006,//进入CG播放状态
	GAME_EXIT_CG					= 1007,//CG播放完毕

    #region TickCommand
    BATTLE_ENERGY_CHANGE = 2000,//能量改变
    BATTLE_COMMAND_PLACE_CARD = 2001,//安置卡牌命令
    BATTLE_COMMAND_ADD_ENERGY = 2002,//拾取能量命令
    BATTLE_COMMAND_PLAY_SKILL = 2003,//技能命令
    BATTLE_COMMAND_PLACE_CHARACTER = 2004,//安置角色命令
    BATTLE_COMMAND_REMOVE_CARD = 2005,//铲除卡牌命令
    BATTLE_COMMAND_OPERATE_TYPE = 2006,//统一操作类型命令

    BATTLE_RECV_COMMAND_PLACE       = 2009,//获取放置命令
    #endregion

    BATTLE_INFO_MONSTER_WAVE        = 2100,//怪物波数通知
    BATTLE_INFO_BOSS_HP             = 2101,//BOSS血量通知
    BATTLE_SKIP_WAIT_TIME           = 2102,//跳过等待时长
    BATTLE_INFO_MONSTER_WAVE_SCORE  = 2103,//关卡波数分数通知

    #region level
    LEVEL_LOGIC_ROTATE_Z = 2200,//关卡Z轴旋转
    LEVEL_LOGIC_MONSTER_TRANSFIGURATION = 2201,//关卡怪物变身
	LEVEL_LOGIC_MONSTER_MELT = 2202,//关卡怪物溶解
	LEVEL_LOGIC_MONSTER_TRANSFIGURATION_ENV_ACTION = 2203,//怪物变身时环境效果激发
	LEVEL_LOGIC_MONSTER_MELT_ENV_ACTION=2204,////怪物溶解时环境效果激发.
	LEVEL_LOGIC_MONSTER_TRANSFIGURATION_ENV_WARNNING_ACTION=2205,//怪物变身时环境效果激发报警

    LEVEL_LOGIC_ENTER_DAYTIME = 2206,//时间，进入白天时调用
    LEVEL_LOGIC_ENTER_NIGHT = 2207,//时间，进入到晚上时调用

    LEVEL_COMMAND_BATTLE_DISABLE = 2208, //关闭战斗
    LEVEL_COMMAND_BATTLE_ENABLE = 2209, //开启战斗
    LEVEL_COMMAND_CHANGE_BGM = 2210,   //切换BGM
	LEVEL_LOGIC_PREVIOUSLY_ROTATE_Z = 2211, // 预进行关卡Z旋转


	LEVEL_VIEW_CAMERA_SHAKE_BEGIN = 2300,//显示层关卡相机震动开始
    LEVEL_VIEW_CAMERA_SHAKE_END = 2301,//显示层关卡相机震动结束
    #endregion
}


namespace Framework.Core
{
	public delegate void GameEventDelegate(object sender, EventArgs arg);
	public class GameEventMgr : ToSingleton<GameEventMgr>
    {
        private Dictionary<GameEvent, GameEventDelegate> dictEventCallBack = new Dictionary<GameEvent, GameEventDelegate>();

        private GameEventMgr()
        { }

		protected override void OnSingletonInit()
		{
			
		}

		public void Release()
        {
            dictEventCallBack.Clear();
        }

        private void AddEvent(ref GameEventDelegate _event, GameEventDelegate value)
        {
            GameEventDelegate gameEventDelegate = _event;
            GameEventDelegate gameEventDelegate2;
            do
            {
                gameEventDelegate2 = gameEventDelegate;
                gameEventDelegate = Interlocked.CompareExchange(ref _event, (GameEventDelegate)Delegate.Combine(gameEventDelegate2, value), gameEventDelegate);
            }
            while (gameEventDelegate != gameEventDelegate2);
        }

        private void RemoveEvent(ref GameEventDelegate _event, GameEventDelegate value)
        {
            GameEventDelegate gameEventDelegate = _event;
            GameEventDelegate gameEventDelegate2;
            do
            {
                gameEventDelegate2 = gameEventDelegate;
                gameEventDelegate = Interlocked.CompareExchange(ref _event, (GameEventDelegate)Delegate.Remove(gameEventDelegate2, value), gameEventDelegate);
            }
            while (gameEventDelegate != gameEventDelegate2);
        }

        public void AddListener(GameEvent _uiEvent, GameEventDelegate _funcCallBack)
		{
            if (_funcCallBack == null) return;
            GameEventDelegate _uiEventDelegate = null;
            if (dictEventCallBack.TryGetValue(_uiEvent, out _uiEventDelegate))
            {
                //_uiEventDelegate += _funcCallBack;
                AddEvent(ref _uiEventDelegate, _funcCallBack);
                dictEventCallBack[_uiEvent] = _uiEventDelegate;
            }
            else
            {
                dictEventCallBack.Add(_uiEvent, _funcCallBack);
            }
        }

		public void RemoveListener(GameEvent _uiEvent, GameEventDelegate _funcCallBack)
		{
            if (_funcCallBack == null) return;
            GameEventDelegate _uiEventDelegate = null;
            if (dictEventCallBack.TryGetValue(_uiEvent, out _uiEventDelegate))
            {
                //_uiEventDelegate -= _funcCallBack;
                //var del1 = _uiEventDelegate.GetInvocationList();
                RemoveEvent(ref _uiEventDelegate, _funcCallBack);
                dictEventCallBack[_uiEvent] = _uiEventDelegate;
               
                if (dictEventCallBack[_uiEvent] == null)
                {
                    dictEventCallBack.Remove(_uiEvent);
                }
            }
            else
            {
                Debug.LogWarning("RemoveListener:the dictionary don't contain CallBack _uiEvent:" + _uiEvent);
            }
        }

		public void SendEvent(GameEvent _uiEvent, object sender, EventArgs _eventArgs)
		{
            GameEventDelegate _uiEventDelegate = null;
            if (dictEventCallBack.TryGetValue(_uiEvent, out _uiEventDelegate))
            {
                //_uiEventDelegate(sender, _eventArgs);
                var allDelegate = _uiEventDelegate.GetInvocationList();
                for(int i =0; i < allDelegate.Length; ++i)
                {
                    var target = allDelegate[i] as GameEventDelegate;
                    if (target != null)
                    {
                        target(sender, _eventArgs);
                    }
                }
            }
            else
            {
                Debug.LogWarning("SendEvent:the dictionary don't contain CallBack _uiEvent:" + _uiEvent);
            }
        }

		
	}
}*/