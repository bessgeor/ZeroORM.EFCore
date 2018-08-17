namespace ZeroORM.EFCore.Test.Infrastructure
{
	internal class FluentMappedEntity
	{
		public int Id { get; set; }

		public string SomeProperty { get; set; }

		public int ConventionMappedEntityId { get; set; }

		public virtual ConventionMappedEntity ConventionMappedEntity { get; set; }
	}
}
