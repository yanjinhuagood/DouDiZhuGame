using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DouDiZhuGame
{
    /// <summary>
    /// 扑克牌 花色、点数和背景
    /// </summary>
    public class PokerCard : Control
    {
        public static readonly DependencyProperty SuitProperty = DependencyProperty.Register(
        "Suit", typeof(Suit), typeof(PokerCard), new PropertyMetadata(Suit.Clubs));

        public static readonly DependencyProperty RankProperty = DependencyProperty.Register(
            "Rank", typeof(Rank), typeof(PokerCard), new PropertyMetadata(Rank.Ace));
        /// <summary>
        /// 花色
        /// </summary>
        public Suit Suit
        {
            get { return (Suit)GetValue(SuitProperty); }
            set { SetValue(SuitProperty, value); }
        }

        public Rank Rank
        {
            get { return (Rank)GetValue(RankProperty); }
            set { SetValue(RankProperty, value); }
        }

        public bool IsBack
        {
            get { return (bool)GetValue(IsBackProperty); }
            set { SetValue(IsBackProperty, value); }
        }

        public static readonly DependencyProperty IsBackProperty =
            DependencyProperty.Register("IsBack", typeof(bool), typeof(PokerCard), new PropertyMetadata(true));

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            var pathImage = "pack://application:,,,/DouDiZhuGame;component/Resources/PokerCard/PokerCardBack.png";
            if(!IsBack)
            {
                if (Rank == Rank.BigWang || Rank == Rank.LittleWang)
                    pathImage = $"pack://application:,,,/DouDiZhuGame;component/Resources/PokerCard/{Rank.ToString()}/{Rank.ToString()}.png";
                else
                    pathImage = $"pack://application:,,,/DouDiZhuGame;component/Resources/PokerCard/{Rank.ToString()}/{Suit.ToString()}.png";
            }
            if (string.IsNullOrWhiteSpace(pathImage)) return;
            var bitmapImage = new BitmapImage(new Uri(pathImage, UriKind.Absolute));
            drawingContext.DrawImage(bitmapImage, new Rect(0, 0, Width, Height));
        }
    }
}
