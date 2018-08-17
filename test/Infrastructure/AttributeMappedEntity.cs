using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZeroORM.EFCore.Test.Infrastructure
{
	[Table("AttributedEntity")]
	internal class AttributeMappedEntity
	{
		[Key]
		public int Id { get; set; }

		[NotMapped]
		public string IgnoredProperty { get; set; }

		[Column("StringData")]
		public string SomeProperty { get; set; }
	}
}
