using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using ZeroORM.Extensibility.Metadata;
using ZeroORM.Extensibility.Metadata.Exceptions;

namespace ZeroORM.EFCore.Metadata
{
	internal class EFCoreMetadataProvider : IMetadataProvider
	{
		private static ConcurrentDictionary<Type, object> _entities;
		private readonly IModel _model;

		public EFCoreMetadataProvider( IModel model )
		{
			if ( Volatile.Read( ref _entities ) is null )
			{
				Dictionary<Type, object> newData = model
					.GetEntityTypes()
					.ToDictionary( et => et.ClrType, et => Activator.CreateInstance( typeof( EFCoreTableMetadata<> ).MakeGenericType( et.ClrType ), et ) )
				;
				Interlocked.CompareExchange( ref _entities, new ConcurrentDictionary<Type, object>( newData ), null );
			}
			_model = model;
		}

		public ITableMetadata<TEntity> GetTable<TEntity>()
		{
			if ( _entities.TryGetValue( typeof( TEntity ), out object metadata ) )
				return (EFCoreTableMetadata<TEntity>) metadata;

			IEntityType newEntityType = _model.FindRuntimeEntityType(typeof(TEntity));
			if (newEntityType is null)
				throw new TableNotFoundException( typeof( TEntity ) );

			metadata = _entities.AddOrUpdate( typeof( TEntity ), new EFCoreTableMetadata<TEntity>( newEntityType ), ( k, old ) => old );
			return (EFCoreTableMetadata<TEntity>) metadata;
		}
	}
}
