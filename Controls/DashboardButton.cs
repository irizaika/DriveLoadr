using Color = Microsoft.Maui.Graphics.Color;

namespace DriveLoadr.Controls;

public class DashboardButton : Button
{
    public static readonly BindableProperty AccentColorProperty =
    BindableProperty.Create(
        nameof(AccentColor),
        typeof(Color),
        typeof(DashboardButton),
        Colors.Black,
        propertyChanged: OnAccentColorChanged);

    public Color AccentColor
    {
        get => (Color)GetValue(AccentColorProperty);
        set => SetValue(AccentColorProperty, value);
    }

    public DashboardButton()
    {
        FontSize = 20;
        HeightRequest = 50;
        MinimumHeightRequest = 50;
        Margin = new Thickness(0, 20, 20, 0);
        BorderWidth = 0;
        CornerRadius = 8;
    }

    private static void OnAccentColorChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is DashboardButton button)
        {
            button.UpdateVisualStates();
        }
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
                        new Setter { Property = BackgroundColorProperty, Value = Colors.White }
                    }},
                    new VisualState { Name = "PointerOver", Setters = {
                        new Setter { Property = BackgroundColorProperty, Value =  LighterColor(AccentColor, 0.7f) }
                    }},
                    new VisualState { Name = "Pressed", Setters = {
                        new Setter { Property = BackgroundColorProperty, Value = LighterColor(AccentColor, 0.5f) }
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
