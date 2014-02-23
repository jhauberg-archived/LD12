using System;
using System.Collections.Generic;
using System.Linq;

using Mantra.Framework;

namespace Mantra.XNA.Graphics
{
    /// <summary>
    /// Represents a system for maintaining cameras. (i.e. storing multiple, but only allowing one to be active at any given time)
    /// </summary>
    public sealed class Cameras : Module<Camera>
    {
        static Camera current = null;
        static Camera change = null;

        public static Camera Current
        {
            get
            {
                return current;
            }
            set
            {
                if (current != value) {
                    change = value;
                }
            }
        }

        public Cameras()
            : base() { }

        public Cameras(Delegater delegater)
            : base(delegater) { }

        protected override void OnItemAdded(Camera item)
        {
            if (current == null) {
                current = item;
            }
        }

        protected override void OnItemRemoved(Camera item)
        {
            if (current == item) {
                if (items.Count > 0) {
                    current = items[0];
                } else {
                    current = null;
                }
            }
        }

        public override void Update()
        {
            /////////////
            // uncertain about whether or not to toggle enabled; cameras might want to keep updating even though they aren't the active one

            if (change != null) {
                if (items.Contains(change)) {
                    current = change;

                    //current.Enabled = true;
                    current.Visible = true;
                }

                change = null;
            }

            for (int i = 0; i < items.Count; i++) {
                if (items[i] != current) {
                    //items[i].Enabled = false;
                    items[i].Visible = false;
                }
            }
        }
    }
}
