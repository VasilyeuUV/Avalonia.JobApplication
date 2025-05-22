using System;
using System.Globalization;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml.Templates;
using Calendar = Avalonia.Controls.Calendar;

namespace Avalonia.JobApplication.Controls.SimpleControls;

[TemplatePart(PART_Button, typeof(Button))]
[TemplatePart(PART_Popup, typeof(Popup))]
[TemplatePart(PART_TextBox, typeof(TextBox))]
[TemplatePart(PART_Calendar, typeof(Calendar))]
[TemplatePart(PART_TimePickerPresenter, typeof(TimePickerPresenter))]
public class CalendarDateTimePicker : CalendarDatePicker
{

    private const string PART_Button = "PART_Button";
    private const string PART_Popup = "PART_Popup";
    private const string PART_TextBox = "PART_TextBox";
    private const string PART_Calendar = "PART_Calendar";
    private const string PART_TimePickerPresenter = "PART_TimePickerPresenter";

    private Button? _button;
    private Popup? _popup;
    private TextBox? _textBox;
    private Calendar? _calendar;
    private TimePickerPresenter? _timePickerPresenter;

    private Button? _clearButton;
    private Button? _acceptButton;
    private Button? _dismissButton;

    private DateTimeOffset? _previousValue;
    private DateTimeOffset? _tempSelectedDate;

    /// <summary>
    /// CTOR
    /// </summary>
    static CalendarDateTimePicker()
    {
        SelectedDateProperty.Changed.AddClassHandler<CalendarDateTimePicker, DateTime?>((picker, args)
            => picker.UpdateTextBox());
    }








    //private void OnSelectionChanged(AvaloniaPropertyChangedEventArgs<DateTime?> args)
    //{
    //    SyncSelectedDateToText(args.NewValue.Value);
    //}


    //private void SyncSelectedDateToText(DateTime? dtg)
    //{
    //    if (dtg is null)
    //    {
    //        _textBox?.SetValue(TextBox.TextProperty, null);
    //    }
    //    else
    //    {
    //        _textBox?.SetValue(TextBox.TextProperty, "Hello");
    //        //_textBox?.SetValue(TextBox.TextProperty, dtg.Value.ToString(CultureInfo.InvariantCulture.DateTimeFormat.FullDateTimePattern));
    //        //_calendar?.MarkDates(date.Value.Date, date.Value.Date);
    //        //_timePickerPresenter?.SyncTime(date.Value.TimeOfDay);
    //    }
    //}


    //public void ClearSelection(bool start = true, bool end = true)
    //{
    //    SelectedDate = null;
    //}












    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        _calendar = e.NameScope.Find<Calendar>("PART_Calendar");
        _textBox = e.NameScope.Find<TextBox>("PART_TextBox");
        _popup = e.NameScope.Find<Popup>("PART_Popup");
        _clearButton = e.NameScope.Find<Button>("PART_ClearButton");
        _button = e.NameScope.Find<Button>("PART_Button");
        _timePickerPresenter = e.NameScope.Find<TimePickerPresenter>("PART_TimePickerPresenter");

        if (_calendar != null)
        {
            _calendar.SelectedDatesChanged += OnCalendarSelectedDatesChanged;
        }

        if (_clearButton is not null)
        {
            _clearButton.IsVisible = SelectedDate is not null;

            _clearButton.Click += (_, _) => ClearSelection();
            _clearButton.IsVisible = false;
        }

        if (_button is not null)
        {            
            _button.PointerEntered += (_, _) => _clearButton!.IsVisible = true;
            _button.PointerExited += (_, _) => _clearButton!.IsVisible = false;
        }

        if (_popup is not null)
        {
            _popup.Opened += (_, _) =>
            {
                _previousValue = SelectedDate;
                if (_previousValue.HasValue
                    && _timePickerPresenter is not null)
                {
                    _timePickerPresenter.Time = _previousValue.Value.TimeOfDay;
                }
            };
        }

        if (_timePickerPresenter is not null)
        {
            _acceptButton = _timePickerPresenter.FindControl<Button>("PART_AcceptButton");
            _dismissButton = _timePickerPresenter.FindControl<Button>("PART_DismissButton");

            if (_acceptButton != null)
            {
                _acceptButton.Click += OnAccept;
            }

            if (_dismissButton != null)
            {
                _dismissButton.Click += OnDismiss;
            }
        }


        UpdateTextBox();
    }

    private void OnCalendarSelectedDatesChanged(object? sender, SelectionChangedEventArgs e)
    {
        // Важно: не задавай SelectedDate напрямую
        _tempSelectedDate = _calendar?.SelectedDate;

        // Откат
        if (_popup != null)
            _popup.IsOpen = true; // не даём закрыться
    }






    private void OnAccept(object? sender, RoutedEventArgs e)
    {
        if (_timePickerPresenter is not null && _popup is not null)
        {
            if (SelectedDate.HasValue)
            {
                DateTime date = SelectedDate.Value.Date;
                TimeSpan time = _timePickerPresenter.Time;
                SelectedDate = (new DateTimeOffset(date + time)).DateTime;
            }
            _popup.IsOpen = false;
        }
    }

    private void OnDismiss(object? sender, RoutedEventArgs e)
    {
        if (_popup is not null)
        {
            SelectedDate = _previousValue?.DateTime;
            _popup.IsOpen = false;
        }
    }

    private void ClearSelection()
    {
        SelectedDate = null;
    }

    private void UpdateTextBox()
    {
        if (_textBox is not null)
        {
            _textBox.Text = SelectedDate?.ToLocalTime().ToString("g")
                ?? string.Empty;
        }
    }





































    /*

    private Button? _acceptButton;
    private Button? _dismissButton;
    private Button? _clearButton;
    private Button? _calendarButton;
    private TimePicker? _timePicker;
    private TimePickerPresenter? _timePickerPresenter;
    private Popup? _popup;
    private TextBox? _textBox;
    private Calendar? _calendar;

    private DateTime? _originalDateTime;
    private bool _isUpdating;

    private readonly FuncControlTemplate<TimePicker> _timePickerTemplate =
        new FuncControlTemplate<TimePicker>((timePicker, scope) =>
        {
            var presenter = new TimePickerPresenter
            {
                Name = "PART_TimePickerPresenter",
                MinuteIncrement = 1
            };

            // Двустороннее связывание TimeProperty
            presenter.Bind(
                TimePickerPresenter.TimeProperty,
                timePicker.GetObservable(TimePicker.SelectedTimeProperty));

            timePicker.Bind(
                TimePicker.SelectedTimeProperty,
                presenter.GetObservable(TimePickerPresenter.TimeProperty));

            return presenter;
        });

    /// <summary>
    /// CTOR
    /// </summary>
    static CalendarDateTimePicker()
    {
        SelectedDateProperty.Changed.AddClassHandler<CalendarDateTimePicker>((x, e)
            => x.OnSelectedDateChanged(e));
        TimeProperty.Changed.AddClassHandler<CalendarDateTimePicker>((x, e)
            => x.OnTimeChanged(e));
    }

    //############################################################################################################
    #region DateTime StyledProperty

    /// <summary>
    /// DateTime StyledProperty definition
    /// </summary>
    public static readonly StyledProperty<DateTime?> DateTimeProperty =
        AvaloniaProperty.Register<CalendarDateTimePicker, DateTime?>(
            nameof(DateTime),
            defaultBindingMode: Data.BindingMode.TwoWay);

    /// <summary>
    /// Gets or sets the DateTime property. This StyledProperty
    /// indicates ....
    /// </summary>
    public DateTime? DateTime
    {
        get => this.GetValue(DateTimeProperty);
        set => SetValue(DateTimeProperty, value);
    }

    #endregion // DateTime StyledProperty



    //############################################################################################################
    #region Time StyledProperty


    /// <summary>
    /// Time StyledProperty definition
    /// </summary>
    public static readonly StyledProperty<TimeSpan?> TimeProperty =
        AvaloniaProperty.Register<CalendarDateTimePicker, TimeSpan?>(nameof(Time));

    /// <summary>
    /// Gets or sets the Time property. This StyledProperty
    /// indicates ....
    /// </summary>
    public TimeSpan? Time
    {
        get => this.GetValue(TimeProperty);
        set => SetValue(TimeProperty, value);
    }

    #endregion // DateTime StyledProperty



    //############################################################################################################
    #region DateTimeFormatProperty StyledProperty


    /// <summary>
    /// DateTimeFormat StyledProperty definition
    /// </summary>
    public static readonly StyledProperty<string> DateTimeFormatProperty =
        AvaloniaProperty.Register<CalendarDateTimePicker, string>(nameof(DateTimeFormat), defaultValue: "g");

    /// <summary>
    /// Gets or sets the DateTimeFormat property. This StyledProperty 
    /// indicates ....
    /// </summary>
    public string DateTimeFormat
    {
        get => this.GetValue(DateTimeFormatProperty);
        set => SetValue(DateTimeFormatProperty, value);
    }

    #endregion // DateTimeFormatProperty StyledProperty



    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        _acceptButton = e.NameScope.Find<Button>("PART_AcceptButton");
        _dismissButton = e.NameScope.Find<Button>("PART_DismissButton");
        _clearButton = e.NameScope.Find<Button>("PART_ClearButton");
        _calendarButton = e.NameScope.Find<Button>("PART_Button");
        _timePicker = e.NameScope.Find<TimePicker>("PART_TimePicker");
        //_timePickerPresenter = e.NameScope.Find<TimePickerPresenter>("PART_TimePickerPresenter");
        _popup = e.NameScope.Find<Popup>("PART_Popup");
        _textBox = e.NameScope.Find<TextBox>("PART_TextBox");
        _calendar = e.NameScope.Find<Calendar>("PART_Calendar");

        if (_acceptButton != null)
        {
            _acceptButton.Click += OnAcceptButtonClick;
        }

        if (_dismissButton != null)
        {
            _dismissButton.Click += OnDismissButtonClick;
        }

        if (_clearButton != null)
        {
            _clearButton.Click += OnClearButtonClick;
        }

        if (_calendarButton != null)
        {
            _calendarButton.PointerEntered += OnCalendarButtonPointerEntered;
            _calendarButton.PointerExited += OnCalendarButtonPointerExited;
        }
        if (_popup != null)
        {
            _popup.Opened += OnPopupOpened;
        }

        if (_timePicker != null)
        {
            _timePicker.Template = _timePickerTemplate;
            _timePicker.ApplyTemplate();
        }

        //if (_timePickerPresenter != null)
        //{

        //}

        UpdateText();

    }


    private void OnSelectedDateChanged(AvaloniaPropertyChangedEventArgs e)
    {
        if (_isUpdating)
        {
            return;
        }

        UpdateDateTime();
    }


    private void OnTimeChanged(AvaloniaPropertyChangedEventArgs e)
    {
        if (_isUpdating)
        {
            return;
        }

        UpdateDateTime();
    }


    private void OnPopupOpened(object? sender, EventArgs e)
    {
        _originalDateTime = DateTime;
        _isUpdating = true;

        SelectedDate = DateTime?.Date;
        Time = DateTime?.TimeOfDay;

        _isUpdating = false;
    }


    private void UpdateDateTime()
    {
        if (SelectedDate.HasValue
            && Time.HasValue)
        {
            DateTime = SelectedDate.Value.Date + Time.Value;
        }
        else
        {
            DateTime = null;
        }

        UpdateText();
    }


    private void OnAcceptButtonClick(object? sender, RoutedEventArgs e)
    {
        if (_popup != null)
        {
            _popup.IsOpen = false;
        }
    }


    private void OnDismissButtonClick(object? sender, RoutedEventArgs e)
    {
        if (_popup != null)
        {
            _popup.IsOpen = false;
        }

        _isUpdating = true;

        DateTime = _originalDateTime;
        SelectedDate = _originalDateTime?.Date;
        Time = _originalDateTime?.TimeOfDay;

        _isUpdating = false;
        UpdateText();
    }


    private void OnClearButtonClick(object? sender, RoutedEventArgs e)
    {
        DateTime = null;
        SelectedDate = null;
        Time = null;
        UpdateText();
        ShowCalendarButton();
    }


    private void OnCalendarButtonPointerEntered(object? sender, PointerEventArgs e)
    {
        if (DateTime.HasValue
            && _clearButton != null
            && _calendarButton != null)
        {
            _calendarButton.IsVisible = false;
            _clearButton.IsVisible = true;
        }
    }


    private void OnCalendarButtonPointerExited(object? sender, PointerEventArgs e)
    {
        ShowCalendarButton();
    }


    private void ShowCalendarButton()
    {
        if (_calendarButton != null
            && _clearButton != null)
        {
            _calendarButton.IsVisible = true;
            _clearButton.IsVisible = false;
        }
    }


    private void UpdateText()
    {
        if (_textBox != null)
        {
            _textBox.Text = DateTime?.ToString(DateTimeFormat)
                ?? string.Empty;
        }
    }


    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
        e.Handled = true;
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        if (_acceptButton != null)
        {
            _acceptButton.Click -= OnAcceptButtonClick;
        }

        if (_dismissButton != null)
        {
            _dismissButton.Click -= OnDismissButtonClick;
        }

        if (_clearButton != null)
        {
            _clearButton.Click -= OnClearButtonClick;
        }

        if (_calendarButton != null)
        {
            _calendarButton.PointerEntered -= OnCalendarButtonPointerEntered;
            _calendarButton.PointerExited -= OnCalendarButtonPointerExited;
        }

        if (_popup != null)
        {
            _popup.Opened -= OnPopupOpened;
        }

        base.OnDetachedFromVisualTree(e);
    }
    */
}