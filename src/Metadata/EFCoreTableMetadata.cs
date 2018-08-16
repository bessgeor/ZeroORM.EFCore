using System;
using HellBrick;
using ZeroORM.Extensibility.Metadata;

namespace ZeroORM.EFCore.Metadata
{
	internal class EFCoreTableMetadata<TEntity> : ITableMetadata<TEntity>
	{
		public string TableName => throw new NotImplementedException();

		public string GetColumnName<TProperty>( [NoCapture] System.Linq.Expressions.Expression<Func<TEntity, TProperty>> accessor )
		{
			throw new NotImplementedException();
		}
	}
}
