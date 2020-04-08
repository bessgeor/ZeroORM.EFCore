using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

using ZeroORM.Extensibility.Metadata;
using ZeroORM.Extensibility.Metadata.Exceptions;

namespace ZeroORM.EFCore.Metadata
{
	internal class EFCoreMetadataProvider : IMetadataProvider
	{
		private readonly IModel _model;

		public EFCoreMetadataProvider( IModel model ) => _model = model;

		public ITableMetadata<TEntity> GetTable<TEntity>()
		{
			IEntityType found = _model.FindRuntimeEntityType( typeof( TEntity ) );

			if ( found is null )
				throw new TableNotFoundException( typeof( TEntity ) );

			return new EFCoreTableMetadata<TEntity>( found );
		}
	}
}
