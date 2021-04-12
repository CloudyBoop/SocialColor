using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
using VRC.Core;
using System.Collections;
namespace SocialColor
{
    public class Main : MelonLoader.MelonMod
    {
        private static HarmonyMethod GetPatch(string name) => new HarmonyMethod(typeof(Main).GetMethod(name, BindingFlags.Static | BindingFlags.NonPublic));
        public override void OnApplicationStart()
        {
            harmonyInstance.Patch(
                typeof(UiUserList).GetMethods().Where(x =>
                    x.GetParameters().Length == 2 &&
                    x.GetParameters().First().ParameterType == typeof(VRCUiContentButton)).FirstOrDefault(),
                GetPatch("A"));
        }
        private static bool A(VRCUiContentButton __0, Il2CppSystem.Object __1)
        {
            try
            {
                APIUser oof = __1.Cast<APIUser>();
                if (oof.tags.Contains("admin_moderator"))
                {
                    __0.field_Public_Text_0.color = Color.red;
                }
                else if (oof.tags.Contains("system_legend"))
                {
                    __0.field_Public_Text_0.color = new Color(1f, 1f, 1f);
                }
                else if (oof.tags.Contains("system_trust_legend"))
                {
                    __0.field_Public_Text_0.color = new Color(1f, 1f, 0f);
                }
                else if (oof.tags.Contains("system_trust_veteran"))
                {
                    __0.field_Public_Text_0.color = new Color(0.5f, 0.25f, 0.9f);
                }
                else if (oof.tags.Contains("system_trust_trusted"))
                {
                    __0.field_Public_Text_0.color = new Color(1, 0.48f, 0);
                }
                else if (oof.tags.Contains("system_trust_known"))
                {
                    __0.field_Public_Text_0.color = new Color(0.17f, 0.81f, 0.36f);
                }
                else if (oof.tags.Contains("system_trust_basic"))
                {
                    __0.field_Public_Text_0.color = new Color(0.09f, 0.47f, 1f);
                }
                else
                {
                    __0.field_Public_Text_0.color = Color.grey;
                }
            }
            catch
            {
                //oof
            }
            return true;
        }

    }
}
