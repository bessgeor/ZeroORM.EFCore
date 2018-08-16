using System;
using System.Collections.Concurrent;
using System.Linq;
using Microsoft.EntityFrameworkCore.Metadata;
using ZeroORM.Extensibility.Metadata;
using ZeroORM.Extensibility.Metadata.Exceptions;

namespace ZeroORM.EFCore.Metadata
{
	internal class EFCoreMetadataProvider : IMetadataProvider
	{
		private static readonly ConcurrentDictionary<Type, IEntityType> _entities = new ConcurrentDictionary<Type, IEntityType>();

		public EFCoreMetadataProvider( IModel model )
			=> model
				.GetEntityTypes()
				.Select( et => _entities.GetOrAdd( et.ClrType, et ) )
				.ToArray();

		public ITableMetadata<TEntity> GetTable<TEntity>()
			=> _entities.TryGetValue( typeof( TEntity ), out IEntityType entity )
			? new EFCoreTableMetadata<TEntity>( entity )
			: throw new TableNotFoundException( typeof( TEntity ) );
	}
}
