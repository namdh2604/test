using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

namespace Voltage.Witches.Models.Avatar.Maintenance
{
    public class DefaultAvatarGeneration : MonoBehaviour
    {
        public AvatarGenerator _generator;

        private string AVATAR_GENERATED_PATH;
        private const string AVATAR_PATH = "Assets/Scripts/Models/NewAvatar/Resources/Avatar";

        private void Start()
        {
            AVATAR_GENERATED_PATH = AvatarTexturePathInfo.GetAvatarRoot();
        }


        public void GenerateFunky()
        {
            Outfit outfit = new Outfit() {
                "default_underwear",
                "basic_jeans_darkblue",
                "basic_canvas_shoes_navy",
                "monochrome_blouse_pink",
                "flannel_plaid_shirt_red",
                "wavy_long_hair_darkbrown"
            };
            StartCoroutine(PerformRoutine(AvatarTutorialGenerator.TutorialOutfitPreset.Funky, _generator.SaveAll(outfit)));
        }

        public void GeneratePreppy()
        {
            Outfit outfit = new Outfit() {
                "default_underwear",
                "basic_ruffled_skirt_grey",
                "monochrome_cardigan_blue",
                "monochrome_blouse_pink",
                "monochrome_flats_blue",
                "wavy_long_hair_darkbrown"
            };
            StartCoroutine(PerformRoutine(AvatarTutorialGenerator.TutorialOutfitPreset.Preppy, _generator.SaveAll(outfit)));
        }

        public void GenerateRebel()
        {
            Outfit outfit = new Outfit() {
                "default_underwear",
                "ripped_stockings_black",
                "jean_shorts_blue",
                "monochrome_zipup_hoodie_red",
                "monochrome_blouse_pink",
                "basic_canvas_shoes_navy",
                "wavy_long_hair_darkbrown"
            };
            StartCoroutine(PerformRoutine(AvatarTutorialGenerator.TutorialOutfitPreset.Rebel, _generator.SaveAll(outfit)));
        }

        public void MovePreppy()
        {
            AvatarTutorialGenerator tutorialGenerator = new AvatarTutorialGenerator();
            tutorialGenerator.AssignOutfitResources(AvatarTutorialGenerator.TutorialOutfitPreset.Preppy);
        }

        public void MoveRebel()
        {
            AvatarTutorialGenerator tutorialGenerator = new AvatarTutorialGenerator();
            tutorialGenerator.AssignOutfitResources(AvatarTutorialGenerator.TutorialOutfitPreset.Rebel);
        }

        private IEnumerator PerformRoutine(AvatarTutorialGenerator.TutorialOutfitPreset preset, IEnumerator routine)
        {

            yield return StartCoroutine(routine);

            string dirpath = AVATAR_PATH + "/" + preset.ToString();
            Directory.CreateDirectory(AVATAR_PATH);
            if (Directory.Exists(dirpath))
            {
                Directory.Delete(dirpath, true);
            }
            ChangeExtensions();
            File.Move(AVATAR_GENERATED_PATH, dirpath);

            AssetDatabase.Refresh();

            Debug.Log("Generation for " + preset.ToString() + " complete!");
        }

        private void ChangeExtensions()
        {
            string[] files = Directory.GetFiles(AVATAR_GENERATED_PATH, "*.png");
            foreach (var file in files)
            {
                if (!file.EndsWith(".png"))
                {
                    continue;
                }

                string byteFile = file.Substring(0, file.Length - 3) + "bytes";
                if (File.Exists(byteFile))
                {
                    File.Delete(byteFile);
                }
                File.Move(file, byteFile);
            }
        }
    }
}

