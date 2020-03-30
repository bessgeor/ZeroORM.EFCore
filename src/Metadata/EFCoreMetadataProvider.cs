using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.EntityFrameworkCore.Metadata;
using ZeroORM.Extensibility.Metadata;
using ZeroORM.Extensibility.Metadata.Exceptions;

namespace ZeroORM.EFCore.Metadata
{
	internal class EFCoreMetadataProvider : IMetadataProvider
	{
		private static Dictionary<Type, object> _entities;

		public EFCoreMetadataProvider( IModel model )
		{
			if ( Volatile.Read( ref _entities ) is null )
			{
				Dictionary<Type, object> newData = model
					.GetEntityTypes()
					.ToDictionary( et => et.ClrType, et => Activator.CreateInstance( typeof( EFCoreTableMetadata<> ).MakeGenericType( et.ClrType ), et ) )
				;
				Interlocked.CompareExchange( ref _entities, newData, null );
			}
		}

		public ITableMetadata<TEntity> GetTable<TEntity>()
			=> _entities.TryGetValue( typeof( TEntity ), out object metadata )
			? (EFCoreTableMetadata<TEntity>)metadata
			: throw new TableNotFoundException( typeof( TEntity ) );
	}
}
