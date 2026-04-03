using System;
using System.Reflection;
using UnityEngine;

using BepInEx;
using BepInEx.Logging;

using EFT.Communications;

#if SPT_4_0
using NotificationManager = NotificationManagerClass;
#endif

namespace tarkin.Director.Bep
{
    internal class EFTLogger : ILogSource, IDisposable
    {
        public string SourceName { get; }

        public event EventHandler<LogEventArgs> LogEvent;

        readonly Func<bool> shouldDisplayInGame;

        public EFTLogger(BaseUnityPlugin plugin, Func<bool> shouldDisplayInGame)
        {
            this.shouldDisplayInGame = shouldDisplayInGame;

            SourceName = plugin?.Info?.Metadata?.Name;
        }

        private void Log(LogLevel level, object data, object sender = null)
        {
            this.LogEvent?.Invoke(sender ?? this, new LogEventArgs(data, level, this));

            if (shouldDisplayInGame != null && shouldDisplayInGame())
            {
                string text = $"<alpha=#44>{sender ?? SourceName}:<alpha=#FF> {data}";

                var icon = ENotificationIconType.Default;
                var color = Color.white;
                switch (level)
                {
                    case LogLevel.Info:
                        icon = ENotificationIconType.WishlistQuest;
                        break;
                    case LogLevel.Warning:
                        icon = ENotificationIconType.WishlistQuest;
                        color = Color.yellow;
                        break;
                    case LogLevel.Error:
                        icon = ENotificationIconType.Alert;
                        color = Color.red;
                        break;
                }

                NotificationManager.DisplayMessageNotification(
                    text, 
                    duration: default, 
                    iconType: icon,
                    textColor: color);
            }
        }

        public void LogInfo(object data, object sender = null)
        {
            Log(LogLevel.Info, data, sender);
        }

        public void LogWarning(object data, object sender = null)
        {
            Log(LogLevel.Warning, data, sender);
        }

        public void LogError(object data, object sender = null)
        {
            Log(LogLevel.Error, data, sender);
        }

        public void Dispose()
        {
        }
    }
}
