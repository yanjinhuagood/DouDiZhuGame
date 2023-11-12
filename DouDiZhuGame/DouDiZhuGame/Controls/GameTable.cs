using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace DouDiZhuGame
{
    /// <summary>
    /// 一副牌斗地主的牌有1-13各4张，+1个大鬼1个小鬼共计54张牌,一个玩家,两个机器人，每人17牌
    /// </summary>
    [TemplatePart(Name = CanvasTemplateName, Type = typeof(Canvas))]
    [TemplatePart(Name = WrapPanelTemplateName, Type = typeof(WrapPanel))]
    public class GameTable : Control
    {
        private const string CanvasTemplateName = "PART_Canvas";
        private const string WrapPanelTemplateName = "PART_WrapPanel";
        private Canvas _canvas;
        private WrapPanel _wrapPanel;
        //private MediaElement _backgroudMediaElement;
        private MediaElement _shuffleMediaElement;
        private IEnumerable<PokerCard> selfPokerCards;
        private double x = 0, y = 0;
        private SoundPlayer backgroudWav;




        public bool IsStart
        {
            get { return (bool)GetValue(IsStartProperty); }
            set { SetValue(IsStartProperty, value); }
        }

        public static readonly DependencyProperty IsStartProperty =
            DependencyProperty.Register("IsStart", typeof(bool), typeof(GameTable), new PropertyMetadata(false));



        static GameTable()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GameTable), new FrameworkPropertyMetadata(typeof(GameTable)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _canvas = GetTemplateChild(CanvasTemplateName) as Canvas;
            if (_canvas != null)
            {
                var imageBrush = new ImageBrush();
                imageBrush.ImageSource = new BitmapImage(new Uri("pack://application:,,,/DouDiZhuGame;component/Resources/Images/background-1.png", UriKind.Absolute));
                _canvas.Background = imageBrush;
            }
            _wrapPanel = GetTemplateChild(WrapPanelTemplateName) as WrapPanel;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            if (_canvas == null) return;
            GetPokerCard();
        }

        void GetPokerCard()
        {
            _canvas.Children.Clear();
            x = ActualWidth / 2 - 100 / 2 - 54;
            y = ActualHeight / 2 - 140 / 2;
            var cards = new List<PokerCard>();
            for (int i = 0; i <= 3; i++)
            {
                for (int j = 0; j <= 12; j++)
                {
                    var card = new PokerCard();
                    card.Suit = (Suit)i;
                    card.Rank = (Rank)j;
                    cards.Add(card);

                }
            }
            var cardLittleWang = new PokerCard();
            cardLittleWang.Rank = Rank.LittleWang;
            cards.Add(cardLittleWang);

            var cardBigWang = new PokerCard();
            cardBigWang.Rank = Rank.BigWang;
            cards.Add(cardBigWang);
            Debug.WriteLine(cards.Count);

            Shuffle(cards);
            //pokerCards = cards;
            foreach (var item in cards)
            {
                GetPokerCard(item);
            }
        }

        void GetPokerCard(PokerCard card)
        {
            _canvas.Children.Add(card);
            Canvas.SetLeft(card, x);
            x += 2;
            Canvas.SetTop(card, y);
        }

        /// <summary>
        /// 打乱顺序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        void Shuffle<T>(IList<T> list)
        {
            Random random = new Random();
            for (int i = 0; i < list.Count; i++)
            {
                int j = random.Next(i + 1);
                T temp = list[i];
                list[i] = list[j];
                list[j] = temp;
            }
        }

        void AnimateCardsInReverseOrder(int index)
        {
            if (index < 37)
            {
                _shuffleMediaElement.Stop();
                return;
            }

            var card = _canvas.Children.Cast<PokerCard>().ElementAtOrDefault(index);
            if (card == null) return;
            var moveNum = Canvas.GetTop(card);
            var movePoint = moveNum * 1.8;

            var animationDuration = TimeSpan.FromMilliseconds(300);
            var moveYAnimation = new DoubleAnimation();
            moveYAnimation.Duration = animationDuration;

            var property = Canvas.TopProperty;
            moveYAnimation.Completed += (sender, args) =>
            {
                _canvas.Children.Remove(card);
                card.Margin = new Thickness(-card.Width / 1.5, 0, 0, 0);
                card.IsBack = false;
                if(_wrapPanel.Children.Count == 0)
                    _wrapPanel.Children.Add(card);
                else
                {
                    bool inserted = false;
                    for (int i = 0; i < _wrapPanel.Children.Count; i++)
                    {
                        var child = _wrapPanel.Children[i] as PokerCard;
                        if (IsRankGreaterThan(card.Rank, child.Rank))
                        {
                            _wrapPanel.Children.Insert(i, card);
                            inserted = true;
                            break;
                        }
                    }
                    if (!inserted)
                    {
                        _wrapPanel.Children.Add(card);
                    }
                }
                AnimateCardsInReverseOrder(index - 1);
            };
            moveYAnimation.From = moveNum;
            moveYAnimation.To = movePoint;

            card.BeginAnimation(property, moveYAnimation);
        }

        bool IsRankGreaterThan(Rank rank1, Rank rank2)
        {
            return rank1 > rank2;
        }

        public void StartDispatched()
        {
            var audioFilePath = AppDomain.CurrentDomain.BaseDirectory + "\\Resources\\Audios\\Start.wav";
            backgroudWav = new SoundPlayer(audioFilePath);
            backgroudWav.PlayLooping();


            //_backgroudMediaElement = new MediaElement(); 
            //_backgroudMediaElement.MediaEnded += MediaElement_MediaEnded;
            //_backgroudMediaElement.LoadedBehavior = MediaState.Manual;
            //_backgroudMediaElement.UnloadedBehavior = MediaState.Manual;
            //var audioFilePath = AppDomain.CurrentDomain.BaseDirectory + "Resources\\Audios\\Start.wav";
            //_backgroudMediaElement.Source = new Uri(audioFilePath, UriKind.Relative);
            //_backgroudMediaElement.Play();


            _shuffleMediaElement = new MediaElement();
            _shuffleMediaElement.LoadedBehavior = MediaState.Manual;
            _shuffleMediaElement.UnloadedBehavior = MediaState.Manual;
            audioFilePath = AppDomain.CurrentDomain.BaseDirectory + "Resources\\Audios\\Shuffle.wav";
            _shuffleMediaElement.Source = new Uri(audioFilePath, UriKind.Relative);
            _shuffleMediaElement.Play();


            IsStart = true;
            var lastIndex = _canvas.Children.Count - 1;
            AnimateCardsInReverseOrder(lastIndex);
        }

        //private void MediaElement_MediaEnded(object sender, RoutedEventArgs e)
        //{
        //    _backgroudMediaElement.Stop();
        //    _backgroudMediaElement.Play(); 
        //}
    }
}
