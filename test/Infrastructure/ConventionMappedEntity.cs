namespace ZeroORM.EFCore.Test.Infrastructure
{
	internal class ConventionMappedEntity
	{
		public int Id { get; set; }

		public string SomeString { get; set; }

		public int AttributeMappedEntityId { get; set; }

		public virtual AttributeMappedEntity AttributeMappedEntity { get; set; }
	}
}
