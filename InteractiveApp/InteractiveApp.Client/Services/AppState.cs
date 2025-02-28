namespace InteractiveApp.Client.Services;

public class AppState
{
	// Informacja o tym, że coś się zmieniło – event do powiadamiania komponentów.
	public event Action? OnChange;

	private int? _selectedCompanyId;
	public int? SelectedCompanyId
	{
		get => _selectedCompanyId;
		set
		{
			if (_selectedCompanyId != value)
			{
				_selectedCompanyId = value;
				NotifyStateChanged();
			}
		}
	}

	private int? _selectedInstanceId;
	public int? SelectedInstanceId
	{
		get => _selectedInstanceId;
		set
		{
			if (_selectedInstanceId != value)
			{
				_selectedInstanceId = value;
				NotifyStateChanged();
			}
		}
	}

	private void NotifyStateChanged() => OnChange?.Invoke();
}
