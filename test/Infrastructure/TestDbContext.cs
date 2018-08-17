using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ZeroORM.EFCore.Test.Infrastructure
{
	internal class TestDbContext : DbContext
	{
		public const string FluentTableName = "SomeReallyUnrelatedName";
		public const string FluentDataColumnName = "ReallyUnexpectedColumnName";
		public const string ConventionTableName = nameof( Conventioned );
		public const string ConventionDataColumnName = nameof( ConventionMappedEntity.SomeString );
		public const string AttributedTableName = "AttributedEntity";
		public const string AttributedDataColumnName = "StringData";

		public DbSet<AttributeMappedEntity> Attributed { get; set; }
		public DbSet<ConventionMappedEntity> Conventioned { get; set; }
		public DbSet<FluentMappedEntity> Fluent { get; set; }

		protected override void OnConfiguring( DbContextOptionsBuilder optionsBuilder )
		{
			if ( !optionsBuilder.IsConfigured )
			{
				optionsBuilder.UseSqlite( "DataSource=:memory:" );
			}
		}

		protected override void OnModelCreating( ModelBuilder builder )
		{
			EntityTypeBuilder<FluentMappedEntity> fluentMappedEntityMapper = builder.Entity<FluentMappedEntity>();

			fluentMappedEntityMapper
				.ToTable( FluentTableName )
				.HasKey( e => e.Id );

			fluentMappedEntityMapper
				.Property( e => e.SomeProperty )
				.HasColumnName( FluentDataColumnName );

			fluentMappedEntityMapper
				.HasOne( e => e.ConventionMappedEntity )
				.WithMany()
				.HasForeignKey( e => e.ConventionMappedEntityId );
		}
	}
}
