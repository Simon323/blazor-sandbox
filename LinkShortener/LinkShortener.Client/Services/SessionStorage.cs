using Microsoft.JSInterop;

namespace LinkShortener.Client.Services;

public class SessionStorage
{
	public const string LongUrlKey = "long-url";

	private readonly IJSRuntime _jSRuntime;

	public SessionStorage(IJSRuntime jSRuntime)
	{
		_jSRuntime = jSRuntime;
	}

	public async Task SaveAsync(string key, string value)
	{
		await _jSRuntime.InvokeVoidAsync("window.sessionStorage.setItem", key, value);
	}

	public async Task<string> GetAsync(string key)
	{
		return await _jSRuntime.InvokeAsync<string>("window.sessionStorage.getItem", key);
	}

	public async Task RemoveAsync(string key)
	{
		await _jSRuntime.InvokeVoidAsync("window.sessionStorage.removeItem", key);
	}
}
