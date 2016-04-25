
using System;
using System.Collections.Generic;

namespace Voltage.Witches.EditorTools
{
	using System.IO;

	using UnityEngine;
	using UnityEditor;

    public class WitchesEditorClearLocalPlayer : Editor
    {
		[MenuItem("Curses/Clear Local Player")]
		private static void ClearLocalPlayer()
		{
			string filepath = Application.persistentDataPath + "/voltage_kisses_and_curses_player.json";
            string avatarFolder = Application.persistentDataPath + "/AvatarImages";

			if (EditorUtility.DisplayDialog("Clear Local Player?", string.Format("Clear your local player:\n\n{0}", filepath), "Ok", "Cancel"))
			{
				File.Delete(filepath);
                if (Directory.Exists(avatarFolder))
                {
                    Directory.Delete(avatarFolder, true);
                }
			}
		}

    }
    
}



