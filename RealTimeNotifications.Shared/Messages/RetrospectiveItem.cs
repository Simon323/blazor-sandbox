namespace RealTimeNotifications.Shared.Messages;

public class RetrospectiveItem
{
	public RetrospectiveItem()
	{
		Id = Guid.NewGuid();
		CreatedAt = DateTime.Now;
	}

	public Guid Id { get; set; }
	public string Description { get; set; } = string.Empty;
	public ItemType Type { get; set; }
	public DateTime CreatedAt { get; set; }
}

public enum ItemType
{
	WhatWentWell,
	WhatCouldBeImproved,
	ActionItem
}
