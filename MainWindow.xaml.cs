using System;
using System.Drawing;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace Smudge_Timer;


/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private DispatcherTimer timer;
    private TimeSpan elapsedTime;
    private bool isTimerRunning;
    private bool isPrimed = false;

    private DispatcherTimer flashTimer = new DispatcherTimer();
    private SolidColorBrush flashColor1 = new SolidColorBrush(Colors.MediumPurple);
    private SolidColorBrush flashColor2 = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 81, 43, 212));
    private bool isColor1 = true; // Flag to track the current color


    private KeyboardHook keyboardHook;

    public MainWindow()
    {
        InitializeComponent();

        // Initialize the MouseHook
        MouseHook.OnRightMouseClick += MouseHook_OnRightMouseClick;

        // Initialize the KeyboardHook with a custom VKCode (e.g., VK_F1)
        keyboardHook = new KeyboardHook(0x70); // VK_F1
        keyboardHook.OnKeyPress += KeyboardHook_OnKeyPress;

        // Initialize the flash timer
        flashTimer.Interval = TimeSpan.FromMilliseconds(300); // Set the interval to 500 milliseconds
        flashTimer.Tick += FlashTimer_Tick;

        // Initialize the Timer used for Smudge Timer
        timer = new DispatcherTimer();
        timer.Interval = TimeSpan.FromSeconds(1);
        timer.Tick += Timer_Tick;

        startButton.Click += StartButton_Click;
    }

    private void Timer_Tick(object sender, EventArgs e)
    {
        elapsedTime += TimeSpan.FromSeconds(1);
        timerLabel.Content = elapsedTime.ToString(@"mm\:ss");
    }

    private void MainWindow_OnClosed(object sender, EventArgs e)
    {
        // Stop the hooks when the MainWindow is closed to prevent memory leaks
        MouseHook.Stop();
        keyboardHook.Dispose();
    }

    private void StartButton_Click(object sender, RoutedEventArgs e)
    {
        if (isTimerRunning)
        {
            timer.Stop();
            startButton.Content = "Start";
        }
        else
        {
            elapsedTime = TimeSpan.Zero;
            timerLabel.Content = "00:00";
            timer.Start();
            startButton.Content = "Stop";
            if(isPrimed)
            {
                MouseHook.Stop();
                flashTimer.Stop();
                timerLabel.Foreground = flashColor2;
                isPrimed = false;
            }
        }

        isTimerRunning = !isTimerRunning;
    }

    private void MouseHook_OnRightMouseClick()
    {
        // Handle right mouse click event
        elapsedTime = TimeSpan.Zero;
        timerLabel.Content = "00:00";
        timerLabel.Foreground = flashColor2;
        timer.Start();
        startButton.Content = "Stop";
        isTimerRunning = true;
        isPrimed = false;
        flashTimer.Stop();
        MouseHook.Stop();
    }

    private void KeyboardHook_OnKeyPress()
    {
        if (isTimerRunning)
        {
            timer.Stop();
            startButton.Content = "Start";
            isTimerRunning = false;
        }
        else
        {
            if (!isPrimed)
            {
                MouseHook.Start();
                flashTimer.Start();
                elapsedTime = TimeSpan.Zero;
                timerLabel.Content = "00:00";
                isPrimed = true;
            }
            else
            {
                MouseHook.Stop();
                flashTimer.Stop();
                timerLabel.Foreground = flashColor2;
                isPrimed = false;
            }

        }
    }
    private void FlashTimer_Tick(object sender, EventArgs e)
    {
        // Toggle between the two colors
        if (isColor1)
        {
            timerLabel.Foreground = flashColor2;
        }
        else
        {
            timerLabel.Foreground = flashColor1;
        }

        isColor1 = !isColor1; // Toggle the flag
    }
}