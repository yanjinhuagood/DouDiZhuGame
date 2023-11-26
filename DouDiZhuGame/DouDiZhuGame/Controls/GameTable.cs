using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace DouDiZhuGame
{
    /// <summary>
    /// 一副牌斗地主的牌有1-13各4张，+1个大鬼1个小鬼共计54张牌,一个玩家,两个机器人，每人17牌 地主多三张
    /// 单牌按分值比大小，依次是 大王 > 小王 >2>A>K>Q>J>10>9>8>7>6>5>4>3 ，不分花色。
    /// </summary>
    [TemplatePart(Name = CanvasTemplateName, Type = typeof(Canvas))]
    [TemplatePart(Name = WrapPanelTopTemplateName, Type = typeof(WrapPanel))]
    [TemplatePart(Name = WrapPanelBottomTemplateName, Type = typeof(WrapPanel))]
    [TemplatePart(Name = StackPanelLeftTemplateName, Type = typeof(StackPanel))]
    [TemplatePart(Name = StackPanelRightTemplateName, Type = typeof(StackPanel))]
    [TemplatePart(Name = ButtonGrabLordTemplateName, Type = typeof(Button))]
    public class GameTable : Control
    {
        private const string CanvasTemplateName = "PART_Canvas";
        private const string WrapPanelTopTemplateName = "PART_WrapPanelTop";
        private const string WrapPanelBottomTemplateName = "PART_WrapPanelBottom";
        private const string StackPanelLeftTemplateName = "PART_StackPanelLeft";
        private const string StackPanelRightTemplateName = "PART_StackPanelRight";
        private const string ButtonGrabLordTemplateName = "PART_ButtonGrabLord";
        private Canvas _canvas;
        private WrapPanel _wrapPanelBottom, _wrapPanelTop;
        private StackPanel _stackPanelLeft, _stackPanelRight;
        private MediaElement _shuffleMediaElement;
        private Button _grabLordButton;
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

        public bool IsSendComplete
        {
            get { return (bool)GetValue(IsSendCompleteProperty); }
            set { SetValue(IsSendCompleteProperty, value); }
        }
        public static readonly DependencyProperty IsSendCompleteProperty =
            DependencyProperty.Register("IsSendComplete", typeof(bool), typeof(GameTable), new PropertyMetadata(false));



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
            _wrapPanelTop = GetTemplateChild(WrapPanelTopTemplateName) as WrapPanel;
            _wrapPanelBottom = GetTemplateChild(WrapPanelBottomTemplateName) as WrapPanel;
            _stackPanelLeft = GetTemplateChild(StackPanelLeftTemplateName) as StackPanel;
            _stackPanelRight = GetTemplateChild(StackPanelRightTemplateName) as StackPanel;
            _grabLordButton = GetTemplateChild(ButtonGrabLordTemplateName) as Button;
            if (_grabLordButton != null)
                _grabLordButton.Click += GrabLordButton_Click;
        }

        private void GrabLordButton_Click(object sender, RoutedEventArgs e)
        {
            PlayWav("Grab");
            if (_wrapPanelTop != null && _wrapPanelBottom != null)
            {
                for (int i = _wrapPanelTop.Children.Count - 1; i >= 0; i--)
                {
                    var card = _wrapPanelTop.Children.Cast<PokerCard>().ElementAtOrDefault(i);
                    if (card == null) continue;
                    card.Margin = new Thickness(-card.Width / 1.5, 20, 0, 0);
                    _wrapPanelTop.Children.Remove(card);
                    AddPanel(_wrapPanelBottom, card);
                }
            }
            _grabLordButton.Visibility = Visibility.Collapsed;
            IsStart = true;
            if (_wrapPanelBottom == null) return;
            foreach (PokerCard item in _wrapPanelBottom.Children)
            {
                item.MouseEnter -= Item_MouseEnter;
                item.MouseEnter += Item_MouseEnter;
                item.MouseDown -= Item_MouseDown;
                item.MouseDown += Item_MouseDown;
            }
        }

        private void Item_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var model = sender as PokerCard;
            UpdateMargin(model);
        }
        void UpdateMargin(PokerCard pokerCard)
        {
            if (pokerCard == null) return;
            if (pokerCard.Margin.Bottom == 20)
                pokerCard.Margin = new Thickness(pokerCard.Margin.Left, 20, 0, 0);
            else
                pokerCard.Margin = new Thickness(pokerCard.Margin.Left, 0, 0, 20);
        }
        private void Item_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed) return;
            var model = sender as PokerCard;
            UpdateMargin(model);
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            if (_canvas == null || IsStart) return;
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
            if (index.Equals(-1))
            {
                _shuffleMediaElement.Stop();
                IsSendComplete = true;
                return;
            }

            var dock = Dock.Top;
            if (index > 2) //剩下最后三张牌给地主
            {
                if (index % 3 == 2)//bottom
                {
                    dock = Dock.Bottom;
                }
                else if (index % 3 == 1)//left
                {
                    dock = Dock.Left;
                }
                else //right
                {
                    dock = Dock.Right;
                }
            }

            var card = _canvas.Children.Cast<PokerCard>().ElementAtOrDefault(index);
            if (card == null) return;
            var moveNum = Canvas.GetTop(card);
            var movePoint = moveNum * 1.8;

            var animationDuration = TimeSpan.FromMilliseconds(200);
            var moveYAnimation = new DoubleAnimation();
            moveYAnimation.Duration = animationDuration;

            var property = Canvas.TopProperty;
            moveYAnimation.Completed += (sender, args) =>
            {
                _canvas.Children.Remove(card);
                PokerCardOrientation(dock, card);
                AnimateCardsInReverseOrder(index - 1);
            };
            switch (dock)
            {
                case Dock.Left:
                    moveNum = Canvas.GetLeft(card);
                    movePoint = 20 + index;
                    property = Canvas.LeftProperty;
                    break;
                case Dock.Top:
                    moveNum = Canvas.GetTop(card);
                    movePoint = -(moveNum * 2);
                    property = Canvas.TopProperty;
                    break;
                case Dock.Right:
                    moveNum = Canvas.GetLeft(card);
                    movePoint = moveNum * 1.8;
                    property = Canvas.LeftProperty;
                    break;
                case Dock.Bottom:
                    moveNum = Canvas.GetTop(card);
                    movePoint = moveNum * 2;
                    property = Canvas.TopProperty;
                    break;
            }

            moveYAnimation.From = moveNum;
            moveYAnimation.To = movePoint;

            card.BeginAnimation(property, moveYAnimation);
        }

        bool IsRankGreaterThan(Rank rank1, Rank rank2)
        {
            return rank1 > rank2;
        }

        void PokerCardOrientation(Dock dock, PokerCard card)
        {
            switch (dock)
            {
                case Dock.Left:
                    card.Margin = new Thickness(0, -card.Height * 0.9, 0, 0);
                    AddPanel(_stackPanelLeft, card, true);
                    break;
                case Dock.Top:
                    card.Margin = new Thickness(card.Width * 0.5, 0, 0, 0);
                    card.IsBack = false;
                    AddPanel(_wrapPanelTop, card);
                    break;
                case Dock.Right:
                    card.Margin = new Thickness(0, -card.Height * 0.9, 0, 0);
                    AddPanel(_stackPanelRight, card, true);
                    break;
                case Dock.Bottom:
                    card.Margin = new Thickness(-card.Width / 1.5, 20, 0, 0);
                    card.IsBack = false;
                    AddPanel(_wrapPanelBottom, card);
                    break;
            }
        }

        void AddPanel(Panel panel, PokerCard card, bool insert = false)
        {
            if (panel.Children.Count == 0)
                panel.Children.Add(card);
            else
            {
                bool inserted = false;
                if (!insert)
                {
                    for (int i = 0; i < panel.Children.Count; i++)
                    {
                        var child = panel.Children[i] as PokerCard;
                        if(child == null) continue;
                        if (IsRankGreaterThan(card.Rank, child.Rank))
                        {
                            panel.Children.Insert(i, card);
                            inserted = true;
                            break;
                        }
                    }
                }
                if (!inserted)
                {
                    panel.Children.Add(card);
                }
            }
        }

        public void StartDispatched()
        {
            var audioFilePath = AppDomain.CurrentDomain.BaseDirectory + "\\Resources\\Audios\\Start.wav";
            backgroudWav = new SoundPlayer(audioFilePath);
            backgroudWav.PlayLooping();

            PlayWav("Shuffle");


            var lastIndex = _canvas.Children.Count - 1;
            AnimateCardsInReverseOrder(lastIndex);
        }

        void PlayWav(string wavPath)
        {
            _shuffleMediaElement = new MediaElement();
            _shuffleMediaElement.LoadedBehavior = MediaState.Manual;
            _shuffleMediaElement.UnloadedBehavior = MediaState.Manual;
            _shuffleMediaElement.Volume = 1;
            var audioFilePath = AppDomain.CurrentDomain.BaseDirectory + $"Resources\\Audios\\{wavPath}.wav";
            _shuffleMediaElement.Source = new Uri(audioFilePath, UriKind.Relative);
            _shuffleMediaElement.Play();
        }
    }
}