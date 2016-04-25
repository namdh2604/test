using System;
using UnityEngine;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Text.RegularExpressions;

using Voltage.Story.Import.CharacterImport.Model;

namespace Voltage.Story.Import.CharacterImport
{
    public class PoseParser
    {
        private const string POSITIONING_RECT_NAME = "positionReference";
        private const string SPEAKER_PATTERN = @"speaker_image_icon_MA_\d+";
        private const string EXPRESSION_LAYER_NAME = "expressions";
        private const string OUTFIT_LAYER_NAME = "outfits";

        private const string DEFAULT_OUTFIT = "default";
        private const string DEFAULT_EXPRESSION = "default";

        private List<string> _warnings;
        public List<string> Warnings { get { return _warnings; } }

        public PoseParser()
        {
            _warnings = new List<string>();
        }

        internal Dictionary<string, PoseInfo> Poses { get; set; }
        public Rect SpeakerFrame { get; protected set; }

        public void Parse(JObject jsonDocument, float scale)
        {
            Dictionary<string, PoseInfo> poses = new Dictionary<string, PoseInfo>();
            JToken root = jsonDocument["images"];

            Vector2 positioningCenter = GetPositioningCenter(root, scale);
            SpeakerFrame = GetSpeakerFrame(root, scale, positioningCenter);

            foreach (var child in root.Children())
            {
                string name = child.Value<string>("name");
                if (isPoseLayer(name))
                {
                    PoseInfo pose = new PoseInfo();
                    pose.name = name;
                    // calculate coordinates
                    Vector2 center = GetCenter(child, Vector2.zero, scale);
                    Vector2 offsetCoords = new Vector2(center.x - positioningCenter.x, positioningCenter.y - center.y);
                    pose.offset = offsetCoords;

                    Vector2 localCenter = GetLocalCenter(child, scale);

                    AddImageGroups(pose, child, localCenter, scale);
                    poses[name] = pose;
                }
            }

            Poses = poses;
//
//            return poses;
        }

        private Vector2 GetPositioningCenter(JToken root, float scale)
        {
            // locate the positioning rectangle
            JToken positioningRect = root.Children().First(x => x.Value<string>("name") == POSITIONING_RECT_NAME);
            // compute the center of that rect
            return GetCenter(positioningRect, Vector2.zero, scale);
        }

        private Rect GetSpeakerFrame(JToken root, float scale, Vector2 center)
        {
            JToken speakerRect = root.Children().First(x => IsSpeakerPositionLayer(x.Value<string>("name")));
//            JToken positionInfo = speakerRect["absolute"];
//
//            Rect coords = new Rect(positionInfo.Value<int>("x") * scale, positionInfo.Value<int>("y") * scale, positionInfo.Value<int>("width") * scale, positionInfo.Value<int>("height") * scale);
//
//            return coords;
            LayerInfo layerInfo = ParseLayer(speakerRect, center, Vector2.zero, scale);
            return layerInfo.absolute;
        }

        private bool IsSpeakerPositionLayer(string name)
        {
            string pattern = @"speaker_image_icon_MA_(\d+)";

            return Regex.IsMatch(name, pattern);
        }

        private bool isPoseLayer(string layerName)
        {
            return !((layerName == POSITIONING_RECT_NAME) || (Regex.IsMatch(layerName, SPEAKER_PATTERN)));
        }

        private void AddImageGroups(PoseInfo pose, JToken root, Vector2 parentCenter, float scale)
        {
            bool expressionsProcessed = false;
            int outfitLayersFound = 0;
            int expressionLayersFound = 0;

            // Photoshop writes out the layers from top to bottom,
            // but it makes more sense for us to iterate from bottom up
            // Since expressions and optional, but they are used to divide the outfits up into top and bottom layers,
            // iterating bottom-up means that outfits are, in the absence of expressions, "bottom" layers
            foreach (var child in root["images"].Children().Reverse())
            {
                string name = child.Value<string>("name");
                if (name == EXPRESSION_LAYER_NAME)
                {
                    AddExpressions(pose.expressions, child, parentCenter, scale);
                    expressionLayersFound++;
                    expressionsProcessed = true;
                }
                else if (name == OUTFIT_LAYER_NAME)
                {
                    AddOutfits(pose.outfits, child, parentCenter, expressionsProcessed, scale);
                    outfitLayersFound++;
                }
                else
                {
                    // treat it like an image layer
                    pose.baseImages.Add(ParseLayer(child, parentCenter, Vector2.zero, scale));
                }
            }

            if (expressionLayersFound == 0)
            {
                pose.expressions[DEFAULT_EXPRESSION] = new List<LayerInfo>();
                _warnings.Add("No Expressions found for Pose: " + pose.name);
            }

            if (outfitLayersFound == 0)
            {
                // if no outfits are explicitly provided, it means the background serves as the default outfit
                pose.outfits[DEFAULT_OUTFIT] = new OutfitPair();
            }
        }

        private void AddExpressions(Dictionary<string, List<LayerInfo>> expressions, JToken root, Vector2 parentCenter, float scale)
        {
            Vector2 offsets = GetOffset(root, scale);

            foreach (var child in root["images"].Children())
            {
                string name = child.Value<string>("name");
                LayerInfo expressionLayer = ParseLayer(child, parentCenter, offsets, scale);
                if (!expressions.ContainsKey(name))
                {
                    expressions[name] = new List<LayerInfo>();
                }

                expressions[name].Add(expressionLayer);
            }
        }

        private void AddOutfits(Dictionary<string, OutfitPair> outfits, JToken root, Vector2 parentCenter, bool isTop, float scale)
        {
            Vector2 offsets = GetOffset(root, scale);

            foreach (var child in root["images"].Children())
            {
                Vector2 childOffsets = GetOffset(child, scale);
                childOffsets = new Vector2(offsets.x + childOffsets.x, offsets.y + childOffsets.y);

                string name = child.Value<string>("name");
                if (!outfits.ContainsKey(name))
                {
                    outfits[name] = new OutfitPair();
                }

                List<LayerInfo> layers;

                if (child.Value<bool>("isGroup"))
                {
                    layers = ParseLayers(child, parentCenter, childOffsets, scale);
                }
                else
                {
                    layers = new List<LayerInfo>();
                    layers.Add(ParseLayer(child, parentCenter, childOffsets, scale));
                }

                if (isTop)
                {
                    outfits[name].top.AddRange(layers);
                }
                else 
                {
                    outfits[name].bottom.AddRange(layers);
                }
            }
        }

        private List<LayerInfo> ParseLayers(JToken root, Vector2 parentCenter, Vector2 parentOffset, float scale)
        {
            List<LayerInfo> layers = new List<LayerInfo>();

            foreach (var child in root["images"].Children().Reverse())
            {
                layers.Add(ParseLayer(child, parentCenter, parentOffset, scale));
            }

            return layers;
        }

        private LayerInfo ParseLayer(JToken layer, Vector2 parentCenter, Vector2 parentOffset, float scale)
        {
            LayerInfo info = JsonConvert.DeserializeObject<LayerInfo>(layer.ToString());

            if (layer.Value<bool>("isGroup"))
            {
                info.isImage = false;
//                info.children = new List<LayerInfo>();
//                foreach (var child in layer["images"].Children().Reverse())
//                {
//                    info.children.Add(ParseLayer(child, scale));
//                }
                Vector2 localCenter = GetLocalCenter(layer, scale);
                info.children = ParseLayers(layer, localCenter, Vector2.zero, scale);
            }
            else
            {
                info.isImage = true;
            }

            NormalizeCoords(info, parentCenter, parentOffset, scale);

            return info;
        }

        private void NormalizeCoords(LayerInfo info, Vector2 parentCenter, Vector2 parentOffset, float scale)
        {
            Vector2 center = GetCenter(info.absolute, parentOffset, scale);
            Vector2 adjustedCoords = new Vector2(center.x - parentCenter.x, parentCenter.y - center.y);
            info.absolute = new Rect(adjustedCoords.x, adjustedCoords.y, info.absolute.width * scale, info.absolute.height * scale);
        }

        private Vector2 GetCenter(JToken layer, Vector2 parentOffset, float scale)
        {
            float x = layer["absolute"].Value<float>("x");
            float y = layer["absolute"].Value<float>("y");
            float width = layer["absolute"].Value<float>("width");
            float height = layer["absolute"].Value<float>("height");

            Rect coords = new Rect(x, y, width, height);

            return GetCenter(coords, parentOffset, scale);
        }

        private Vector2 GetCenter(Rect layer, Vector2 parentOffset, float scale)
        {
            return new Vector2(parentOffset.x + (layer.x + layer.width / 2.0f) * scale, parentOffset.y + (layer.y + layer.height / 2.0f) * scale);
        }

        private Vector2 GetLocalCenter(JToken layer, float scale)
        {
            float width = layer["absolute"].Value<float>("width");
            float height = layer["absolute"].Value<float>("height");

            return new Vector2(width / 2.0f * scale, height / 2.0f * scale);
        }

        private Vector2 GetOffset(JToken layer, float scale)
        {
            float x = layer["absolute"].Value<float>("x") * scale;
            float y = layer["absolute"].Value<float>("y") * scale;

            return new Vector2(x, y);
        }
    }
}

