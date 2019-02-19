namespace ZeroORM.EFCore.Test.Infrastructure
{
	internal class EntityWithShadowProperty
	{
		public int Id { get; set; }

		public virtual FluentMappedEntity FluentMappedEntity { get; set; }
	}
}
