namespace DriveLoadr.Controls;
public class SmallActionButtonBase : Button
{
    public static readonly BindableProperty AccentColorProperty =
        BindableProperty.Create(
            nameof(AccentColor),
            typeof(Color),
            typeof(SmallActionButtonBase),
            Colors.Black,
            propertyChanged: OnVisualChanged);

    public static readonly BindableProperty HoverColorProperty =
        BindableProperty.Create(
            nameof(HoverColor),
            typeof(Color),
            typeof(SmallActionButtonBase),
            Colors.Gray,
            propertyChanged: OnVisualChanged);

    public static readonly BindableProperty PressedColorProperty =
        BindableProperty.Create(
            nameof(PressedColor),
            typeof(Color),
            typeof(SmallActionButtonBase),
            Colors.Gray,
            propertyChanged: OnVisualChanged);

    public Color AccentColor
    {
        get => (Color)GetValue(AccentColorProperty);
        set => SetValue(AccentColorProperty, value);
    }

    public Color HoverColor
    {
        get => (Color)GetValue(HoverColorProperty);
        set => SetValue(HoverColorProperty, value);
    }

    public Color PressedColor
    {
        get => (Color)GetValue(PressedColorProperty);
        set => SetValue(PressedColorProperty, value);
    }

    public SmallActionButtonBase()
    {
        Background = Brush.Transparent;
        Padding = new Thickness(2);
        CornerRadius = 5;
        UpdateVisualStates();
    }

    private static void OnVisualChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is SmallActionButtonBase btn)
            btn.UpdateVisualStates();
    }

    private void UpdateVisualStates()
    {
        VisualStateManager.SetVisualStateGroups(this, new VisualStateGroupList
        {
            new VisualStateGroup
            {
                Name = "CommonStates",
                States =
                {
                    new VisualState { Name = "Normal", Setters = {
                        new Setter { Property = BackgroundProperty, Value = Brush.Transparent },
                        new Setter { Property = TextColorProperty, Value = AccentColor }
                    }},
                    new VisualState { Name = "PointerOver", Setters = {
                        new Setter { Property = BackgroundProperty, Value = Brush.Transparent },
                        new Setter { Property = TextColorProperty, Value = HoverColor }
                    }},
                    new VisualState { Name = "Pressed", Setters = {
                        new Setter { Property = BackgroundProperty, Value = Brush.Transparent },
                        new Setter { Property = TextColorProperty, Value = PressedColor }
                    }}
                }
            }
        });
    }
}
