// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

#nullable enable

using System;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;
using osu.Game.Graphics;
using osu.Game.Graphics.Containers;
using osu.Game.Graphics.Sprites;
using osu.Game.Online.Chat;
using osu.Game.Users.Drawables;
using osuTK;

namespace osu.Game.Overlays.Chat.ChannelList
{
    public class ChannelListItem : OsuClickableContainer
    {
        public event Action<Channel>? OnRequestSelect;
        public event Action<Channel>? OnRequestLeave;

        public readonly Channel Channel;

        public readonly BindableInt Mentions = new BindableInt();

        public readonly BindableBool Unread = new BindableBool();

        public readonly BindableBool SelectorActive = new BindableBool();

        private Box hoverBox = null!;
        private Box selectBox = null!;
        private OsuSpriteText text = null!;
        private ChannelListItemCloseButton close = null!;

        [Resolved]
        private Bindable<Channel> selectedChannel { get; set; } = null!;

        [Resolved]
        private OverlayColourProvider colourProvider { get; set; } = null!;

        public ChannelListItem(Channel channel)
        {
            Channel = channel;
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            Height = 30;
            RelativeSizeAxes = Axes.X;

            Children = new Drawable[]
            {
                hoverBox = new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = colourProvider.Background3,
                    Alpha = 0f,
                },
                selectBox = new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = colourProvider.Background4,
                    Alpha = 0f,
                },
                new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Padding = new MarginPadding { Left = 18, Right = 10 },
                    Child = new GridContainer
                    {
                        RelativeSizeAxes = Axes.Both,
                        ColumnDimensions = new[]
                        {
                            new Dimension(GridSizeMode.AutoSize),
                            new Dimension(),
                            new Dimension(GridSizeMode.AutoSize),
                            new Dimension(GridSizeMode.AutoSize),
                        },
                        Content = new[]
                        {
                            new[]
                            {
                                createIcon(),
                                text = new OsuSpriteText
                                {
                                    Anchor = Anchor.CentreLeft,
                                    Origin = Anchor.CentreLeft,
                                    Text = Channel.Name,
                                    Font = OsuFont.Torus.With(size: 17, weight: FontWeight.SemiBold),
                                    Colour = colourProvider.Light3,
                                    Margin = new MarginPadding { Bottom = 2 },
                                    RelativeSizeAxes = Axes.X,
                                    Truncate = true,
                                },
                                new ChannelListItemMentionPill
                                {
                                    Anchor = Anchor.CentreLeft,
                                    Origin = Anchor.CentreLeft,
                                    Margin = new MarginPadding { Right = 3 },
                                    Mentions = { BindTarget = Mentions },
                                },
                                close = new ChannelListItemCloseButton
                                {
                                    Anchor = Anchor.CentreLeft,
                                    Origin = Anchor.CentreLeft,
                                    Margin = new MarginPadding { Right = 3 },
                                    Action = () => OnRequestLeave?.Invoke(Channel),
                                }
                            }
                        },
                    },
                },
            };

            Action = () => OnRequestSelect?.Invoke(Channel);
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            selectedChannel.BindValueChanged(_ => updateState(), true);
            SelectorActive.BindValueChanged(_ => updateState(), true);
            Unread.BindValueChanged(_ => updateState(), true);
        }

        protected override bool OnHover(HoverEvent e)
        {
            hoverBox.FadeIn(300, Easing.OutQuint);
            close.FadeIn(300, Easing.OutQuint);
            return base.OnHover(e);
        }

        protected override void OnHoverLost(HoverLostEvent e)
        {
            hoverBox.FadeOut(200, Easing.OutQuint);
            close.FadeOut(200, Easing.OutQuint);
            base.OnHoverLost(e);
        }

        private Drawable createIcon()
        {
            if (Channel.Type != ChannelType.PM)
                return Drawable.Empty();

            return new UpdateableAvatar(Channel.Users.First(), isInteractive: false)
            {
                Size = new Vector2(20),
                Margin = new MarginPadding { Right = 5 },
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.CentreLeft,
                CornerRadius = 10,
                Masking = true,
            };
        }

        private void updateState()
        {
            bool selected = selectedChannel.Value == Channel && !SelectorActive.Value;

            if (selected)
                selectBox.FadeIn(300, Easing.OutQuint);
            else
                selectBox.FadeOut(200, Easing.OutQuint);

            if (Unread.Value || selected)
                text.FadeColour(colourProvider.Content1, 300, Easing.OutQuint);
            else
                text.FadeColour(colourProvider.Light3, 200, Easing.OutQuint);
        }
    }
}
