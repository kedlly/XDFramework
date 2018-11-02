using Framework.Core;
using Protocol;
using Protocol.Respond;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Projects.ThirdPerson
{
    public interface IDataExchange
    {

    }

    public static class ProtocolProcessor
    {
        public static Dictionary<Type, Delegate> methodList = new Dictionary<Type, Delegate>();
        public static Type ProcessorClassType { get; private set; }
        public static void Register<T>() where T : class
        {
            ProcessorClassType = typeof(T);
        }

        public static void MessagePump(ProtoBuf.IExtensible message)
        {
            var messageType = message.GetType();
            if (!methodList.ContainsKey(messageType)) {
                var namePath = messageType.Name.Split('.');
                var methodName = "RPC_" + namePath[namePath.Length - 1];
                var method = ProcessorClassType
                                        .GetMethod(methodName
                                                    , System.Reflection.BindingFlags.Static |
                                                    System.Reflection.BindingFlags.Public |
                                                    System.Reflection.BindingFlags.NonPublic
                                                );
                Delegate mehthodDelegate = Delegate.CreateDelegate(typeof(ProtocolWrapper.MessageHandle<>).MakeGenericType(messageType), method);
                methodList[messageType] = mehthodDelegate;
            }
            var methodDelegate = methodList[messageType];
            methodDelegate.DynamicInvoke(message);
        }
    }

    public class NetworkCommunication
    {

        static void RPC_Respond_WorkSheet(Respond_WorkSheet rla)
        {

        }


        static void RPC_Respond_LoginAuth(Respond_LoginAuth rla)
        {
            GamePlayerInfo player = new GamePlayerInfo(rla.player.pid
                                                , rla.token
                                                , rla.player.name
                                                , rla.player.playerType.ToGamePlayerType()
                                            );

            player.Position = rla.player.movement.position.ToUV();
            player.Velocity = rla.player.movement.velocity.ToUV();

            if (PlayerManager.Instance.Current == null) {
                PlayerManager.Instance.SetCurrentPlayer(player);
                player.Login();
            }
            foreach (var p in rla.neighborhood) {
                GamePlayerInfo pc = new GamePlayerInfo(p.pid
                                            , name: p.name
                                            , type: p.playerType.ToGamePlayerType()
                                        );
                pc.Position = p.movement.position.ToUV();
                pc.Velocity = p.movement.velocity.ToUV();
                PlayerManager.Instance.AddPlayer(pc);
                pc.Appear();
            }
        }

        static void RPC_Respond_Moving(Respond_Moving rm)
        {
            foreach (var m in rm.movementList) {
                var player = PlayerManager.Instance.GetPlayer(m.pid);
                if (player != null) {
                    if (player != PlayerManager.Instance.Current) {
                        player.Position = m.movement.position.ToUV();
                        player.Velocity = m.movement.velocity.ToUV();
                        player.RefreshMovment();
                    } else {
                        //TODO:
                    }
                }
            }
        }

        static void RPC_Respond_Logout(Respond_Logout rl)
        {
            {
                var player = PlayerManager.Instance.GetPlayer(rl.pid);
                if (player != null) {
                    if (player == PlayerManager.Instance.Current) {
                        Application.Quit();
                    } else {
                        PlayerManager.Instance.Drop(player);
                    }
                }
            }
        }

        static void RPC_Respond_PlayerAppeared(Respond_PlayerAppeared rpa)
        {
            foreach (var m in rpa.neighborhood) {
                var player = PlayerManager.Instance.GetPlayer(m.pid);
                if (player == null) {
                    player = new GamePlayerInfo(m.pid
                                            , name: m.name
                                            , type: m.playerType.ToGamePlayerType()
                                        );
                    player.Position = m.movement.position.ToUV();
                    player.Velocity = m.movement.velocity.ToUV();
                    PlayerManager.Instance.AddPlayer(player);
                    player.Appear();
                }
                player.Position = m.movement.position.ToUV();
                player.Velocity = m.movement.velocity.ToUV();
                player.RefreshMovment();
            }
        }
    }
}
