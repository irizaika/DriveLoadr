namespace DriveLoadr.Controls;

public class SolidButtonBase : Button
{
    public static readonly BindableProperty AccentColorProperty =
    BindableProperty.Create(
        nameof(AccentColor),
        typeof(Color),
        typeof(SmallActionButtonBase),
        Colors.Black,
        propertyChanged: OnVisualChanged);

    public Color AccentColor
    {
        get => (Color)GetValue(AccentColorProperty);
        set => SetValue(AccentColorProperty, value);
    }

    private static void OnVisualChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is SolidButtonBase btn)
            btn.UpdateVisualStates();
    }

    public SolidButtonBase()
    {
        CornerRadius = 5;
        Padding = new Thickness(5, 1);
        UpdateVisualStates();
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
                        new Setter { Property = BackgroundColorProperty, Value = LighterColor(AccentColor, 0.5f) },
                        new Setter { Property = TextColorProperty, Value = Colors.Black }
                    }},
                    new VisualState { Name = "PointerOver", Setters = {
                        new Setter { Property = BackgroundColorProperty, Value = AccentColor },
                        new Setter { Property = TextColorProperty, Value = Colors.White }
                    }},
                    new VisualState { Name = "Pressed", Setters = {
                        new Setter { Property = BackgroundColorProperty, Value = LighterColor(AccentColor, -0.5f) },
                        new Setter { Property = TextColorProperty, Value = Colors.White }
                    }}
                }
            }
        });
    }

    private static Color LighterColor(Color color, float factor)
    {
        // factor = 0.0 (no change), 1.0 (full white)
        return new Color(
            color.Red + (1 - color.Red) * factor,
            color.Green + (1 - color.Green) * factor,
            color.Blue + (1 - color.Blue) * factor,
            color.Alpha);
    }
}
