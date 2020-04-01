using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZeroORM.EFCore.Test.Infrastructure
{
	[Table("extended_context_additional_entity")]
	internal class ExtendedContextAdditionalEntity
	{
		[Key]
		public int Id { get; set; }

		[NotMapped]
		public string IgnoredProperty { get; set; }

		[Column(TestDbContextExtended.ExtendedEntitySomePropertyColumnName)]
		public string SomeProperty { get; set; }
	}
}
