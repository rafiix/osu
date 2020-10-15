// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Game.Graphics.Sprites;
using osu.Game.Graphics.UserInterface;

namespace osu.Game.Skinning
{
    public class LegacyScoreCounter : ScoreCounter
    {
        private readonly ISkin skin;

        protected override double RollingDuration => 1000;
        protected override Easing RollingEasing => Easing.Out;

        public new Bindable<double> Current { get; } = new Bindable<double>();

        public LegacyScoreCounter(ISkin skin)
            : base(6)
        {
            Anchor = Anchor.TopRight;
            Origin = Anchor.TopRight;

            this.skin = skin;

            // base class uses int for display, but externally we bind to ScoreProcesssor as a double for now.
            Current.BindValueChanged(v => base.Current.Value = (int)v.NewValue);

            Margin = new MarginPadding { Bottom = 10, Left = 10 };
            Scale = new Vector2(1.2f);
        }

        protected sealed override OsuSpriteText CreateSpriteText() =>
            new LegacySpriteText(skin, "score" /*, true*/)
            {
                Anchor = Anchor.TopRight,
                Origin = Anchor.TopRight,
            };
    }
}
