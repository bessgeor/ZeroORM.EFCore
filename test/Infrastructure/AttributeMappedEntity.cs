using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZeroORM.EFCore.Test.Infrastructure
{
	[Table(TestDbContext.AttributedTableName)]
	internal class AttributeMappedEntity
	{
		[Key]
		public int Id { get; set; }

		[NotMapped]
		public string IgnoredProperty { get; set; }

		[Column(TestDbContext.AttributedDataColumnName)]
		public string SomeProperty { get; set; }
	}
}
