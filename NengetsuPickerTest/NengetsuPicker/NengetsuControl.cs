using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls.Primitives;

namespace NengetsuPicker
{
    public class NengetsuControl : Control
    {
        static NengetsuControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NengetsuControl), new FrameworkPropertyMetadata(typeof(NengetsuControl)));
        }
        public NengetsuControl()
        {
            //デフォルトを設定
            SetValue(BackgroundProperty, new SolidColorBrush(Colors.White));
            SetValue(BorderBrushProperty, new SolidColorBrush(Color.FromRgb(0xC3, 0xC3, 0xC3)));
            SetValue(BorderThicknessProperty, new Thickness(1));

        }
        #region Year
        private string _Year;
        public string Year
        {
            get { return _Year; }
            set
            {
                _Year = value;
                ChangeYearLabel.Content = _Year;
            }
        }
        public int iYear
        {
            get
            {
                return int.Parse(Year);
            }
            set
            {
                Year = value.ToString();
            }
        }

        #endregion

        #region Labels
        public Label YearLabel;
        private Label ChangeYearLabel;
        private Label Rich_YearLabel;
        #endregion

        #region Grid
        private Grid RichLabelGrid;
        private Grid SimpleLabelGrid;
        #endregion

        #region Buttons
        private List<Button> MonthButtons = new List<Button>();
        private Button CancelButton;
        private Image ForeYearButton;
        private Image NextYearButton;
        private Button SelectButton;
        #endregion

        #region popup
        private Popup Popup;
        #endregion

        #region SelectedItem
        public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register("SelectedItem",
                        typeof(string),
                        typeof(NengetsuControl),
                        new PropertyMetadata(DateTime.Today.ToString("yyyyMM"), new PropertyChangedCallback(NengetsuControl.OnSelectedItemChanged)));

        public string SelectedItem
        {
            get { return (string)GetValue(SelectedItemProperty); }
            set
            {
                SetValue(SelectedItemProperty, value);
            }
        }

        // 3. 依存プロパティが変更されたとき呼ばれるコールバック関数の定義
        private static void OnSelectedItemChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            NengetsuControl ctrl = obj as NengetsuControl;

            try
            {
                if (ctrl.SelectedItem != null)
                {
                    ctrl.YearLabel.Content = ctrl.SelectedItem.Substring(0,4) + "/" + ctrl.SelectedItem.Substring(4,2);
                    ctrl.Rich_YearLabel.Content = ctrl.SelectedItem.Substring(0, 4) + "/" + ctrl.SelectedItem.Substring(4, 2);
                }
            }
            catch
            {
            }

        }

        #endregion

        #region IsEnabledProperty
        public new static readonly DependencyProperty IsEnabledProperty = DependencyProperty.Register("IsEnabled",
                        typeof(bool),
                        typeof(NengetsuControl),
                        new PropertyMetadata(false, new PropertyChangedCallback(NengetsuControl.OnIsEnabledChanged)));

        public new bool IsEnabled
        {
            get { return (bool)GetValue(IsEnabledProperty); }
            set
            {
                SetValue(IsEnabledProperty, value);
            }
        }

        // 3. 依存プロパティが変更されたとき呼ばれるコールバック関数の定義
        private static void OnIsEnabledChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            NengetsuControl ctrl = obj as NengetsuControl;
        }

        #endregion

        #region IsSimpleProperty
        public static readonly DependencyProperty IsSimpleProperty = DependencyProperty.Register("IsSimple",
                        typeof(bool),
                        typeof(NengetsuControl),
                        new PropertyMetadata(false, new PropertyChangedCallback(NengetsuControl.OnIsSimpleChanged)));

        public bool IsSimple
        {
            get { return (bool)GetValue(IsSimpleProperty); }
            set
            {
                SimpleRichChange(value);
                SetValue(IsSimpleProperty, value);
            }
        }

        private void SimpleRichChange(bool value)
        {
            if (value == true)
            {
                SimpleLabelGrid.Visibility = Visibility.Visible;
                RichLabelGrid.Visibility = Visibility.Collapsed;
            }
            else
            {
                SimpleLabelGrid.Visibility = Visibility.Collapsed;
                RichLabelGrid.Visibility = Visibility.Visible;
            }
        }

        // 3. 依存プロパティが変更されたとき呼ばれるコールバック関数の定義
        private static void OnIsSimpleChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            NengetsuControl ctrl = obj as NengetsuControl;
        }

        #endregion

        public override void OnApplyTemplate()
        {
            GetControls();

            MakeEventHandler();

            SetDefaultYear();
        }

        private void SetDefaultYear()
        {
            Year = DateTime.Today.Year.ToString();
            YearLabel.Content = Year +"/" + DateTime.Today.ToString("MM");
            Rich_YearLabel.Content = YearLabel.Content;
        }
        private void GetControls()
        {
            YearLabel = this.GetTemplateChild("YearLabel") as Label;

            foreach (int index in Enumerable.Range(1, 12))
            {
                var button = GetTemplateChild("Month_Button" + index) as Button;

                //イベントハンドラを追加
                button.Click += (object sender, RoutedEventArgs e) => { X_Click(sender, e, index); };

                MonthButtons.Add(button);
            }

            CancelButton = this.GetTemplateChild("CancelButton") as Button;

            Popup = GetTemplateChild("Popup") as Popup;
            ForeYearButton = GetTemplateChild("ForeYearButton") as Image;
            NextYearButton = GetTemplateChild("NextYearButton") as Image;
            ChangeYearLabel = GetTemplateChild("ChangeYearLabel") as Label;

            RichLabelGrid = GetTemplateChild("RichLabelGrid") as Grid;
            SimpleLabelGrid = GetTemplateChild("SimpleLabelGrid") as Grid;
            Rich_YearLabel = GetTemplateChild("Rich_YearLabel") as Label;
            SelectButton = GetTemplateChild("SelectButton") as Button;

            SelectButton.Click += SelectButton_Click;
            SimpleRichChange(IsSimple);
        }

        private void MakeEventHandler()
        {
            ///日付をクリックした時のイベント
            YearLabel.MouseDown += HeaderYearMonth_Label_MouseDown;
            RichLabelGrid.MouseDown += HeaderYearMonth_Label_MouseDown;

            ///キャンセルボタン　イベント
            CancelButton.Click += (sender, e) => { Popup.IsOpen = false; };

            ForeYearButton.MouseDown += (sender, e) => { iYear--;e.Handled = true; };
            NextYearButton.MouseDown += (sender, e) => { iYear++; e.Handled = true; };
        }

        /// <summary>
        /// 月選択イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <param name="SelectMonth"></param>
        private void X_Click(object sender, RoutedEventArgs e,int SelectMonth)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("{0}{1}", Year, SelectMonth.ToString("d2"));
            SelectedItem = sb.ToString();

            Popup.IsOpen = false;
        }

        /// <summary>
        /// 年月クリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HeaderYearMonth_Label_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ShowPopup();
        }

        /// <summary>
        /// Selectボタンクリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectButton_Click(object sender, RoutedEventArgs e)
        {
            ShowPopup();
        }

        /// <summary>
        /// 年月選択ポップアップを開く
        /// </summary>
        private void ShowPopup()
        {
            if (IsEnabled)
            {
                //IsEnabledの時にしかポップアップしない
                Popup.IsOpen = true;
                Popup.StaysOpen = true;

                ///Popupを表示する時にStaysOpenをtrueにすると、すぐPopupが閉じてしまうので、表示後少し待ってからStaysOpenをTrueにするとうまくいく。
                Task.Run(new Action(() =>
                {
                    System.Threading.Thread.Sleep(500);
                    Dispatcher.Invoke(() =>
                    {
                        Popup.StaysOpen = false;
                    });

                }));
            }
        }
    }
}
