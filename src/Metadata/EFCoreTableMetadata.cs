﻿using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using HellBrick;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using ZeroORM.Extensibility.Metadata;
using ZeroORM.Extensibility.Metadata.Exceptions;
using ZeroORM.Extensibility.Common.Expressions;

namespace ZeroORM.EFCore.Metadata
{
	internal class EFCoreTableMetadata<TEntity> : ITableMetadata<TEntity>
	{
		private static readonly ConcurrentDictionary<PropertyInfo, string> _columnNames = new ConcurrentDictionary<PropertyInfo, string>();

		public string TableName { get; }
		public string SchemaName { get; }

		public EFCoreTableMetadata( IEntityType efTableMetadata )
		{
			IRelationalEntityTypeAnnotations relationalMetadata = efTableMetadata.Relational();
			TableName = relationalMetadata.TableName;
			SchemaName = relationalMetadata.Schema;
			efTableMetadata
				.GetProperties()
				.Where(prop => !prop.IsShadowProperty)
				.Select( prop => (prop.PropertyInfo, RelationalProps: prop.Relational()) )
				.Select( t => _columnNames.GetOrAdd( t.PropertyInfo, t.RelationalProps.ColumnName ) )
				.ToArray();
		}

		public string GetColumnName<TProperty>( [NoCapture] Expression<Func<TEntity, TProperty>> accessor )
		{
			PropertyInfo property = accessor.IsSimplePropertyAccess();
			return _columnNames.TryGetValue( property, out string columnName )
				? columnName
				: throw new ColumnNotFoundException( property );
		}
	}
}
