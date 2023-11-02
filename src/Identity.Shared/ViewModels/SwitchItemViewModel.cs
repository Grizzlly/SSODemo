namespace Company.Services.Identity.Shared.ViewModels;

public class SwitchItemViewModel
{
    public string DisplayName { get; private set; }

    public string ItemValue { get; private set; }

    public bool IsSelected { get; set; }

    public SwitchItemViewModel(string itemValue, bool isSelected)
    {
        ItemValue = itemValue;
        IsSelected = isSelected;
        DisplayName = itemValue.Contains(':') ? itemValue[(itemValue.IndexOf(":") + 1)..] : itemValue;
    }

    public SwitchItemViewModel(string displayName, string itemValue, bool isSelected)
    {
        ItemValue = itemValue;
        IsSelected = isSelected;
        DisplayName = displayName;
    }
}
