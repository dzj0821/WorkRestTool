namespace WorkRestTool
{
    using Microsoft.Toolkit.Uwp.Notifications;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Navigation;
    using System.Windows.Shapes;
    using System.Windows.Threading;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DispatcherTimer timer;
        private bool working = false;
        private int state = -1;

        public MainWindow()
        {
            InitializeComponent();


            timer = new DispatcherTimer(new TimeSpan(0, 0, 1), DispatcherPriority.Normal, Update, Application.Current.Dispatcher);

            timer.Start();
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            OutHour.Text = Convert.ToString(Save.Instance.OutHour);
            OutMinute.Text = Convert.ToString(Save.Instance.OutMinute);
            WorkHour.Text = Convert.ToString(Save.Instance.WorkHour);
            WorkMinute.Text = Convert.ToString(Save.Instance.WorkMinute);
            WorkSecond.Text = Convert.ToString(Save.Instance.WorkSecond);
            MinWorkMinute.Text = Convert.ToString(Save.Instance.MinWorkMinute);
            MinRestMinute.Text = Convert.ToString(Save.Instance.MinRestMinute);
            EatUseMinute.Text = Convert.ToString(Save.Instance.EatUseMinute);
            EatHour.Text = Convert.ToString(Save.Instance.EatHour);
            EatMinute.Text = Convert.ToString(Save.Instance.EatMinute);

            Update(default, default);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            Save.Instance.OutHour = Convert.ToInt32(OutHour.Text);
            Save.Instance.OutMinute = Convert.ToInt32(OutMinute.Text);
            Save.Instance.WorkHour = Convert.ToInt32(WorkHour.Text);
            Save.Instance.WorkMinute = Convert.ToInt32(WorkMinute.Text);
            Save.Instance.WorkSecond = Convert.ToInt32(WorkSecond.Text);
            Save.Instance.MinWorkMinute = Convert.ToInt32(MinWorkMinute.Text);
            Save.Instance.MinRestMinute = Convert.ToInt32(MinRestMinute.Text);
            Save.Instance.EatUseMinute = Convert.ToInt32(EatUseMinute.Text);
            Save.Instance.EatHour = Convert.ToInt32(EatHour.Text);
            Save.Instance.EatMinute = Convert.ToInt32(EatMinute.Text);
        }

        private void Update(object? sender, EventArgs? e)
        {
            if (string.IsNullOrEmpty(OutHour.Text) || string.IsNullOrEmpty(OutMinute.Text) || string.IsNullOrEmpty(WorkHour.Text) || string.IsNullOrEmpty(WorkMinute.Text)
                || string.IsNullOrEmpty(WorkSecond.Text))
            {
                return;
            }
            int outHour = Convert.ToInt32(OutHour.Text), outMinute = Convert.ToInt32(OutMinute.Text), 
                workHour = Convert.ToInt32(WorkHour.Text), workMinute = Convert.ToInt32(WorkMinute.Text), workSecond = Convert.ToInt32(WorkSecond.Text),
                minWorkMinute = Convert.ToInt32(MinWorkMinute.Text), minRestMinute = Convert.ToInt32(MinRestMinute.Text),
                eatUseMinute = Convert.ToInt32(EatUseMinute.Text), eatHour = Convert.ToInt32(EatHour.Text), eatMinute = Convert.ToInt32(EatMinute.Text);
            TimeSpan remainingWorkTime = new TimeSpan(workHour, workMinute, workSecond);
            if (remainingWorkTime.TotalSeconds > 0 && working)
            {
                //更新现在的时间
                remainingWorkTime -= new TimeSpan(0, 0, 1);
                WorkHour.Text = Convert.ToString(remainingWorkTime.Hours);
                WorkMinute.Text = Convert.ToString(remainingWorkTime.Minutes);
                WorkSecond.Text = Convert.ToString(remainingWorkTime.Seconds);
            }

            DateTime currentTime = DateTime.Now;
            DateTime targetTime = new DateTime(currentTime.Year, currentTime.Month, currentTime.Day, outHour, outMinute, 0);
            TimeSpan remainingTime = targetTime - currentTime;
            if (currentTime < new DateTime(currentTime.Year, currentTime.Month, currentTime.Day, eatHour, eatMinute, 0))
            {
                remainingTime -= new TimeSpan(0, eatUseMinute, 0);
            }
            if (remainingTime.TotalSeconds <= 0)
            {
                //下班时间
                if (remainingWorkTime.TotalSeconds > 0)
                {
                    state = 0;
                    Tip.Text = "摸太多了";
                    Tip.Foreground = Brushes.Red;
                    TipDescription.Text = "还有工作时间 要不要加班呢";
                    return;
                }
                if (state != 1)
                {
                    new ToastContentBuilder()
                        .AddText("该下班啦")
                        .AddText("虽然什么都没做 但是辛苦我自己了")
                        .Show();
                }
                state = 1;
                Tip.Text = "该下班啦！";
                Tip.Foreground = Brushes.Green;
                TipDescription.Text = "虽然什么都没做 但是辛苦我自己了";
                return;
            }
            if (remainingWorkTime.TotalSeconds <= 0)
            {
                if (state != 8)
                {
                    new ToastContentBuilder()
                        .AddText("摸摸鱼鱼")
                        .AddText("工作结束 摸鱼永恒")
                        .Show();
                }
                state = 8;
                Tip.Text = "摸摸鱼鱼";
                Tip.Foreground = Brushes.Green;
                TipDescription.Text = "工作结束 摸鱼永恒";
                return;
            }
            if (remainingWorkTime >= remainingTime)
            {
                //工作时间比剩余时间多
                if (working)
                {
                    state = 2;
                    Tip.Text = "加油赶工";
                    Tip.Foreground = Brushes.Red;
                    TipDescription.Text = "已经做不完了 能做一点是一点吧";
                    return;
                }
                if (state != 3)
                {
                    new ToastContentBuilder()
                        .AddText("速速工作")
                        .AddText("不能再摸了没时间了")
                        .Show();
                }
                state = 3;
                Tip.Text = "速速工作";
                Tip.Foreground = Brushes.Red;
                TipDescription.Text = "不能再摸了没时间了";
                return;
            }
            //工作时间比剩余时间少
            double workCount = remainingWorkTime.TotalMinutes / minWorkMinute;
            int minRoundMinute = minWorkMinute + minRestMinute;
            double remainingWork = remainingWorkTime.TotalMinutes % minWorkMinute;
            double mustTime = workCount * minRoundMinute;
            double lastTime = remainingTime.TotalMinutes;
            if (working)
            {
                //还在工作
                if (lastTime >= mustTime + minRestMinute)
                {
                    int time = (int) (lastTime - mustTime);
                    //时间比轮次多的情况
                    if (state != 4)
                    {
                        new ToastContentBuilder()
                        .AddText("可以摸鱼啦")
                        .AddText($"可以先摸{time}分钟再上班")
                        .Show();
                    }
                    state = 4;
                    Tip.Text = "可以摸鱼啦";
                    Tip.Foreground = Brushes.Green;
                    TipDescription.Text = $"可以先摸{time}分钟再上班";
                    return;
                }
                //时间比轮次少的情况
                state = 5;
                Tip.Text = "加把劲";
                Tip.Foreground = Brushes.Orange;
                //每工作1分钟，可以赚1.0 / minWorkMinute * minRoundMinute - 1分钟
                double needTime = (mustTime + minRestMinute - lastTime) / (1.0 / minWorkMinute * minRoundMinute - 1);
                TipDescription.Text = $"再工作{(int) (needTime)}分钟就可以摸鱼了";
                return;
            }
            //不在工作
            if (lastTime >= mustTime)
            {
                //时间比轮次多的情况
                state = 6;
                Tip.Text = "摸鱼时光";
                Tip.Foreground = Brushes.Green;
                TipDescription.Text = $"还能摸{(int) (lastTime - mustTime)}分钟呢";
                return;
            }
            //时间比轮次少的情况
            if (state != 7)
            {
                new ToastContentBuilder()
                .AddText("上班上班上班")
                .AddText($"再摸下去工作越来越多")
                .Show();
            }
            state = 7;
            Tip.Text = "上班上班上班";
            Tip.Foreground = Brushes.Red;
            TipDescription.Text = $"再摸下去工作越来越多";
            return;
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            working = !working;
            Button.Content = working ? "我摸鱼了" : "我工作了";
            Update(default, default);
        }
    }
}
