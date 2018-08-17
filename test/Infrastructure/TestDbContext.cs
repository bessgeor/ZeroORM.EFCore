using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ZeroORM.EFCore.Test.Infrastructure
{
	internal class TestDbContext : DbContext
	{
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
				.ToTable( "SomeReallyUnrelatedName" )
				.HasKey( e => e.Id );

			fluentMappedEntityMapper
				.Property( e => e.SomeProperty )
				.HasColumnName( "ReallyUnexpectedColumnName" );

			fluentMappedEntityMapper
				.HasOne( e => e.ConventionMappedEntity )
				.WithMany()
				.HasForeignKey( e => e.ConventionMappedEntityId );
		}
	}
}
