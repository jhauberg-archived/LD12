using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace LD12.Achievements
{
    class AchievementMetaData
    {
        public Texture2D Icon;

        public string Title;
        public string Description;
        public float Progress
        {
            get
            {
                return progress;
            }
            set
            {
                if (progress != value) {
                    progress = value;

                    if (progress != 0) {
                        IsActive = true;
                        from = DateTime.Now;
                    } else {
                        IsActive = false;
                    }
                }
            }
        }

        float progress;

        public bool IsActive;

        public const float TimeOut = 5;

        DateTime from;

        public void Update()
        {
            if (IsActive) {
                TimeSpan since = DateTime.Now - from;

                if (since.TotalSeconds > TimeOut) {
                    IsActive = false;

                    from = DateTime.Now;
                }
            }
        }
    }
}
