using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using System;

namespace Avalonia.JobApplication.Controls.DockControls;

public class DockPanelSplitter : Control
{
    private const double _defaultSize = 3;

    private bool _isDragging;
    private Point _lastPosition;
    private Control? _targetControl;
    private Dock _dockSide;


    //##################################################################################################################
    #region BackgroundProperty

    public static readonly StyledProperty<IBrush?> BackgroundProperty =
        Border.BackgroundProperty.AddOwner<DockPanelSplitter>();

    public IBrush? Background
    {
        get => GetValue(BackgroundProperty);
        set => SetValue(BackgroundProperty, value);
    }

    #endregion // BackgroundProperty


    /// <summary>
    /// CTOR
    /// </summary>
    public DockPanelSplitter()
    {
        Background = Brushes.Gray;
        Cursor = new Cursor(StandardCursorType.SizeWestEast);
    }


    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);

        if (Parent is DockPanel dockPanel)
        {
            int index = dockPanel.Children.IndexOf(this);
            if (index > 0)
            {
                _targetControl = dockPanel.Children[index - 1];
                _dockSide = DockPanel.GetDock(_targetControl);

                UpdateSize();
                UpdateCursor();
            }
        }
    }


    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
        _isDragging = true;
        _lastPosition = e.GetPosition(this);
        e.Pointer.Capture(this);
    }


    protected override void OnPointerMoved(PointerEventArgs e)
    {
        base.OnPointerMoved(e);

        if (!_isDragging
            || _targetControl == null)
        {
            return;
        }

        Point currentPosition = e.GetPosition(this);
        Point delta = currentPosition - _lastPosition;

        switch (_dockSide)
        {
            case Dock.Left:
                _targetControl.Width = Math.Max(_targetControl.MinWidth, _targetControl.Width + delta.X);
                break;
            case Dock.Right:
                _targetControl.Width = Math.Max(_targetControl.MinWidth, _targetControl.Width - delta.X);
                break;
            case Dock.Top:
                _targetControl.Height = Math.Max(_targetControl.MinHeight, _targetControl.Height + delta.Y);
                break;
            case Dock.Bottom:
                _targetControl.Height = Math.Max(_targetControl.MinHeight, _targetControl.Height - delta.Y);
                break;
        }

        _lastPosition = currentPosition;
    }


    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);
        _isDragging = false;
        e.Pointer.Capture(null);
    }


    public override void Render(DrawingContext context)
    {
        base.Render(context);
        if (Background != null)
        {
            context.FillRectangle(Background, new Rect(0, 0, Bounds.Width, Bounds.Height));
        }
    }

    private void UpdateSize()
    {
        switch (_dockSide)
        {
            case Dock.Left:
            case Dock.Right:
                Width = _defaultSize;
                Height = double.NaN; // Auto-size in Avalonia
                SetValue(HorizontalAlignmentProperty, Avalonia.Layout.HorizontalAlignment.Stretch);
                SetValue(VerticalAlignmentProperty, Avalonia.Layout.VerticalAlignment.Stretch);
                break;

            case Dock.Top:
            case Dock.Bottom:
                Height = _defaultSize;
                Width = double.NaN; // Auto-size in Avalonia
                SetValue(HorizontalAlignmentProperty, Avalonia.Layout.HorizontalAlignment.Stretch);
                SetValue(VerticalAlignmentProperty, Avalonia.Layout.VerticalAlignment.Stretch);
                break;
        }
    }

    private void UpdateCursor()
    {
        Cursor = _dockSide is Dock.Left or Dock.Right
            ? new Cursor(StandardCursorType.SizeWestEast)
            : new Cursor(StandardCursorType.SizeNorthSouth);
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        return _dockSide switch
        {
            Dock.Left or Dock.Right => new Size(5, availableSize.Height),
            Dock.Top or Dock.Bottom => new Size(availableSize.Width, 5),
            _ => base.MeasureOverride(availableSize)
        };
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        return _dockSide switch
        {
            Dock.Left or Dock.Right => new Size(5, finalSize.Height),
            Dock.Top or Dock.Bottom => new Size(finalSize.Width, 5),
            _ => base.ArrangeOverride(finalSize)
        };
    }
}
