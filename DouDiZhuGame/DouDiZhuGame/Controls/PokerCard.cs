using System.Windows;
using System.Windows.Controls;

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
    }
}
