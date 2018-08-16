using ZeroORM.Extensibility.Metadata;

namespace ZeroORM.EFCore.Metadata
{
	internal class EFCoreMetadataProvider : IMetadataProvider
	{
		public ITableMetadata<TEntity> GetTable<TEntity>()
			=> new EFCoreTableMetadata<TEntity>();
	}
}
