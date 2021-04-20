using Harmony;
using MelonLoader;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using VRC;
using VRC.Core;

namespace SocialTrustColor
{
    public class Main : MelonMod
    {
        private static MelonPreferences_Category socialTrustColor;
        private static MelonPreferences_Entry<bool> Enabled;
        public override void OnApplicationStart()
        {
           Harmony.Patch(typeof(UiUserList).GetMethod(nameof(UiUserList.Method_Protected_Virtual_Void_VRCUiContentButton_Object_1)),
                new HarmonyMethod(typeof(Main).GetMethod(nameof(SetPickerContentFromApiModelPatch), BindingFlags.Static | BindingFlags.NonPublic)), null, null);

            Harmony.Patch(typeof(NetworkManager).GetMethod(nameof(NetworkManager.Method_Public_Void_Player_1)),
                new HarmonyMethod(typeof(Main).GetMethod(nameof(OnPlayerJoinedPatch), BindingFlags.Static | BindingFlags.NonPublic)), null, null);

            socialTrustColor = MelonPreferences.CreateCategory("SocialTrustColor");
            Enabled = (MelonPreferences_Entry<bool>)socialTrustColor.CreateEntry("Enabled", true, "Decides if mod is enabled by default");
        }


        private static void SetPickerContentFromApiModelPatch(ref VRCUiContentButton __0, ref Il2CppSystem.Object __1)
        {
            APIUser user = __1.Cast<APIUser>();
            var apiUser = PlayerWrappers.CachedApiUsers.Find(x => x.id == user.id) ?? user;
            if (Enabled.Value)
                __0.field_Public_Text_0.color = GetTrustColor(apiUser);
            else
                __0.field_Public_Text_0.color = new Color(0.4139273f, 0.8854161f, 0.9705882f);
        }

        private static void OnPlayerJoinedPatch(ref Player __0)
        {
            FetchAPIUserOnJoined(ref __0);
        }

        private static void FetchAPIUserOnJoined(ref Player __0)
        {
            var apiUser = __0.field_Private_APIUser_0;

            if (PlayerWrappers.CachedApiUsers.Exists(x => x.id == apiUser.id))
                return;

            APIUser.FetchUser(apiUser.id, new Action<APIUser>(user =>
            {
                PlayerWrappers.CachedApiUsers.Add(user);
            }), new Action<string>(error =>
            {
                MelonLogger.Error($"Could not fetch APIUser object of {apiUser.displayName}-{apiUser.id}");
            }));
        }

        private static Color GetTrustColor(APIUser apiUser)
        {
            switch (PlayerWrappers.GetUserTrustRank(apiUser))
            {
                case PlayerWrappers.TrustRanks.Admin:
                    return RGBColor(255, 0, 0);

                case PlayerWrappers.TrustRanks.Legendary:
                    return RGBColor(255, 105, 180);

                case PlayerWrappers.TrustRanks.Veteran:
                    return RGBColor(180, 105, 255);

                case PlayerWrappers.TrustRanks.Trusted:
                    return RGBColor(180, 105, 255);

                case PlayerWrappers.TrustRanks.Known:
                    return RGBColor(255, 123, 66);

                case PlayerWrappers.TrustRanks.User:
                    return RGBColor(43, 207, 92);

                case PlayerWrappers.TrustRanks.New:
                    return RGBColor(0, 237, 255);

                case PlayerWrappers.TrustRanks.Visitor:
                    return RGBColor(255, 255, 255);
            }
            return new Color(0.4139273f, 0.8854161f, 0.9705882f);
        }

        private static Color RGBColor(float r, float g, float b)
        {
            if (r > 255)
                r = 255f;

            if (g > 255)
                g = 255f;

            if (b > 255)
                b = 255f;

            r /= 255f;
            g /= 255f;
            b /= 255f;

            return new Color(r, g, b);
        }
    }

    static class PlayerWrappers
    {
        public static List<APIUser> CachedApiUsers = new List<APIUser>();
        public static TrustRanks GetUserTrustRank(this APIUser user)
        {
            if (user != null)
            {
                if (user.developerType == APIUser.DeveloperType.Internal) // admin user
                {
                    return TrustRanks.Admin;
                }
                if (user.HasTag("system_legend")) // legend user
                {
                    return TrustRanks.Legendary;
                }
                if (user.hasLegendTrustLevel) // veteran user
                {
                    return TrustRanks.Veteran;
                }
                if (user.hasVeteranTrustLevel) // trusted user
                {
                    return TrustRanks.Trusted;
                }
                if (user.hasTrustedTrustLevel) // known user 
                {
                    return TrustRanks.Known;
                }
                if (user.hasKnownTrustLevel) // user user
                {
                    return TrustRanks.User;
                }
                if (user.hasBasicTrustLevel) // new user
                {
                    return TrustRanks.New;
                }
                if (user.HasTag(string.Empty) && !user.canPublishAvatars) // visitor user
                {
                    return TrustRanks.Visitor;
                }
            }
            return TrustRanks.Visitor;
        }

        public enum TrustRanks
        {
            Admin,
            Legendary,
            Veteran,
            Trusted,
            Known,
            User,
            New,
            Visitor
        }
    }

}
