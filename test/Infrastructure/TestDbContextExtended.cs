using Microsoft.EntityFrameworkCore;

namespace ZeroORM.EFCore.Test.Infrastructure
{
	internal class TestDbContextExtended : TestDbContext
	{
		public const string ExtendedEntitySomePropertyColumnName = "some_property_column";
		public DbSet<ExtendedContextAdditionalEntity> Additional { get; set; }
	}
}
