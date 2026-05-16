using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;

namespace ImmersingHomework.Views;

public partial class FloatingButtonWindow : Window
{
    private bool _isDragging;
    private bool _hasMoved;
    private Point _dragStartPoint;
    private PixelPoint _dragStartWindowPosition;
    private const double DragThreshold = 5; // 拖动阈值，超过此距离才认为是拖动
    private const double SnapThreshold = 30; // 边缘吸附阈值
    
    public event EventHandler? FloatingButtonClicked;
    
    public FloatingButtonWindow()
    {
        InitializeComponent();
        
        Position = new PixelPoint(100, 100);
        
        Opacity = 0;
        
        this.PointerPressed += OnPointerPressed;
        this.PointerMoved += OnPointerMoved;
        this.PointerReleased += OnPointerReleased;
    }

    public void ShowWithAnimation()
    {
        Show();
        AnimateOpacity(1, TimeSpan.FromMilliseconds(200));
    }

    public void HideWithAnimation()
    {
        AnimateOpacity(0, TimeSpan.FromMilliseconds(200), () => Hide());
    }

    private void AnimateOpacity(double targetOpacity, TimeSpan duration, Action? onComplete = null)
    {
        var startOpacity = Opacity;
        var startTime = DateTime.Now;
        
        Dispatcher.UIThread.Post(async () =>
        {
            while (DateTime.Now - startTime < duration)
            {
                var progress = (DateTime.Now - startTime).TotalMilliseconds / duration.TotalMilliseconds;
                Opacity = startOpacity + (targetOpacity - startOpacity) * progress;
                await Task.Delay(16);
            }
            Opacity = targetOpacity;
            onComplete?.Invoke();
        });
    }

    private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
        {
            _isDragging = true;
            _hasMoved = false;
            _dragStartPoint = e.GetPosition(this);
            _dragStartWindowPosition = Position;
            e.Pointer.Capture(this);
        }
    }

    private void OnPointerMoved(object? sender, PointerEventArgs e)
    {
        if (_isDragging && e.Pointer.Captured == this)
        {
            var currentPoint = e.GetPosition(this);
            var delta = currentPoint - _dragStartPoint;
            
            // 检查是否超过拖动阈值
            if (Math.Abs(delta.X) > DragThreshold || Math.Abs(delta.Y) > DragThreshold)
            {
                _hasMoved = true;
            }
            
            var newX = _dragStartWindowPosition.X + (int)delta.X;
            var newY = _dragStartWindowPosition.Y + (int)delta.Y;
            
            // 获取屏幕尺寸并应用边界限制和边缘吸附
            if (Screens.Primary != null)
            {
                var screenBounds = Screens.Primary.Bounds;
                var windowWidth = (int)Bounds.Width;
                var windowHeight = (int)Bounds.Height;
                
                // 边界限制
                newX = Math.Max(0, Math.Min(newX, screenBounds.Width - windowWidth));
                newY = Math.Max(0, Math.Min(newY, screenBounds.Height - windowHeight));
                
                // 边缘吸附
                if (newX < SnapThreshold)
                    newX = 0;
                else if (newX > screenBounds.Width - windowWidth - SnapThreshold)
                    newX = screenBounds.Width - windowWidth;
                
                if (newY < SnapThreshold)
                    newY = 0;
                else if (newY > screenBounds.Height - windowHeight - SnapThreshold)
                    newY = screenBounds.Height - windowHeight;
            }
            
            Position = new PixelPoint(newX, newY);
        }
    }

    private void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (_isDragging && e.Pointer.Captured == this)
        {
            _isDragging = false;
            e.Pointer.Capture(null);
            
            // 如果没有移动，则触发点击事件
            if (!_hasMoved)
            {
                FloatingButtonClicked?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    private void FloatingButton_OnClick(object? sender, RoutedEventArgs e)
    {
        // 这个事件处理程序保留但不使用，因为我们现在在 OnPointerReleased 中处理
        // 这样可以避免与拖动操作冲突
    }
}
