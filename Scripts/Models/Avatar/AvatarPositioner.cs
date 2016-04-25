using System.Collections.Generic;
using UnityEngine;

namespace Voltage.Witches.Models.Avatar
{

    /*
     * Controls positioning for the avatar's container.
     */
    public interface IAvatarPositioner
    {
        void Reposition(RectTransform container, AvatarType shotType);
    }

    public class AvatarPositioner : IAvatarPositioner
    {
        public void Reposition(RectTransform container, AvatarType shotType)
        {
            ShotInformation shotInfo = _shots[shotType];

            container.localScale = new Vector3(shotInfo.Ratio, shotInfo.Ratio, 1.0f);

            if (shotType == AvatarType.Story)
            {
                SetAnchorPoint(container, BOTTOM_CENTER);
            }
            else
            {
                SetAnchorPoint(container, CENTER);
            }

            container.anchoredPosition = new Vector2(0.0f, shotInfo.OffsetY);
        }

        private void SetAnchorPoint(RectTransform container, Vector2 anchorPoint)
        {
            container.anchorMin = anchorPoint;
            container.anchorMax = anchorPoint;
        }

        private struct ShotInformation
        {
            public float Ratio;
            public float OffsetY;

            public ShotInformation(float ratio, float offsetY)
            {
                Ratio = ratio;
                OffsetY = offsetY;
            }
        }

        // coordinates are based on a 2560x1600 canvas, and are specific to Witches
        private readonly static Dictionary<AvatarType, ShotInformation> _shots = new Dictionary<AvatarType, ShotInformation>()
        {
            { AvatarType.Fullbody, new ShotInformation(0.45f, 500.0f) },
            { AvatarType.Story, new ShotInformation(1.0f, 1012.5f) },
            { AvatarType.Headshot, new ShotInformation(1.66f, -150.0f) }
        };

        private static readonly Vector2 BOTTOM_CENTER = new Vector2(0.5f, 0.0f);
        private static readonly Vector2 CENTER = new Vector2(0.5f, 0.5f);
    }
}

