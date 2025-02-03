using System.ComponentModel.DataAnnotations;

namespace LinkShortener.Data;

public class LinkAnalytic
{
	[Key]
	public long Id { get; set; }

	public long LinkId { get; set; }

	public DateTime CreatedAt { get; set; }

	public virtual Link Link { get; set; }
}
