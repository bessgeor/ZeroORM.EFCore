using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using HellBrick;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using ZeroORM.Extensibility.Metadata;
using ZeroORM.Extensibility.Metadata.Exceptions;
using ZeroORM.Extensibility.Common.Expressions;
using System.Collections.Generic;

namespace ZeroORM.EFCore.Metadata
{
	internal class EFCoreTableMetadata<TEntity> : ITableMetadata<TEntity>
	{
		private readonly Dictionary<PropertyInfo, string> _columnNames;

		public string TableName { get; }
		public string SchemaName { get; }

		public EFCoreTableMetadata( IEntityType efTableMetadata )
		{
			TableName = efTableMetadata.GetTableName();
			SchemaName = efTableMetadata.GetSchema();
			_columnNames =
				efTableMetadata
					.GetProperties()
					.Where( prop => !prop.IsShadowProperty() )
					.ToDictionary( prop => prop.PropertyInfo, prop => prop.GetColumnName() )
				;
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
