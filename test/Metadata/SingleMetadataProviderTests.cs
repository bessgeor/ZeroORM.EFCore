﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;
using ZeroORM.EFCore.Test.Infrastructure;
using ZeroORM.Extensibility.Metadata.Exceptions;

namespace ZeroORM.EFCore.Test.Metadata
{
	public class SingleMetadataProviderTests
	{
		private class UnmappedEntity
		{
			public int Id;
		}

		private static readonly TestDbContext _context = new TestDbContext();

		[Fact]
		public void MetadataProviderIsSuccessfullyCreatedFromNotNullContext()
			=> ( (Action) ( () => _context.ZeroORM() ) )
			.Should()
			.NotThrow();

		[Fact]
		public void MetadataProviderIsSuccessfullyCreatedFromNotNullModel()
			=> ( (Action) ( () => _context.Model.ZeroORM() ) )
			.Should()
			.NotThrow();

		[Fact]
		public void MetadataProviderThrowsANEOnNullContext()
			=> ( (Action) ( () => ( null as TestDbContext ).ZeroORM() ) )
			.Should()
			.Throw<ArgumentNullException>()
			.Which
			.ParamName
			.Should()
			.Be( "context" );

		[Fact]
		public void MetadataProviderThrowsANEOnNullModel()
			=> ( (Action) ( () => ( null as Microsoft.EntityFrameworkCore.Metadata.IModel ).ZeroORM() ) )
			.Should()
			.Throw<ArgumentNullException>()
			.Which
			.ParamName
			.Should()
			.Be( "model" );

		[Fact]
		public void MetadataProviderThrowsTableNotFoundOnUnmappedEntity()
			=> ( (Action) ( () => _context.ZeroORM().GetTable<UnmappedEntity>() ) )
			.Should()
			.Throw<TableNotFoundException>()
			.WithMessage( $"Can't find entity type { typeof( UnmappedEntity ).FullName }" );

		[Fact]
		public void MetadataProviderGetsFluentMappedEntityTableName()
			=> _context
			.ZeroORM()
			.GetTable<FluentMappedEntity>()
			.TableName
			.Should()
			.Be( TestDbContext.FluentTableName );

		[Fact]
		public void MetadataProviderGetsConventionMappedEntityTableName()
			=> _context
			.ZeroORM()
			.GetTable<ConventionMappedEntity>()
			.TableName
			.Should()
			.Be( TestDbContext.ConventionTableName );

		[Fact]
		public void MetadataProviderGetsAttributeMappedEntityTableName()
			=> _context
			.ZeroORM()
			.GetTable<AttributeMappedEntity>()
			.TableName
			.Should()
			.Be( TestDbContext.AttributedTableName );

		[Fact]
		public void MetadataProviderThrowsOnNavigationPropertyAsColumnUse()
			=> (
				(Action)
				(
					() => _context
					.ZeroORM()
					.GetTable<FluentMappedEntity>()
					.GetColumnName( e => e.ConventionMappedEntity )
				)
			)
			.Should()
			.Throw<ColumnNotFoundException>()
			.WithMessage( $@"Can't find column for the property ""ConventionMappedEntity"" of type ""{ typeof( FluentMappedEntity ).FullName }""" );

		[Fact]
		public void MetadataProviderThrowsOnIgnoredPropertyAsColumnUse()
			=> (
				(Action)
				(
					() => _context
					.ZeroORM()
					.GetTable<AttributeMappedEntity>()
					.GetColumnName( e => e.IgnoredProperty )
				)
			)
			.Should()
			.Throw<ColumnNotFoundException>()
			.WithMessage( $@"Can't find column for the property ""IgnoredProperty"" of type ""{ typeof( AttributeMappedEntity ).FullName }""" );

		[Fact]
		public void MetadataProviderDontCrashIfShadowPropertyIsPresent()
			=> (
				(Action)(() =>
					_context
					.ZeroORM()
					.GetTable<EntityWithShadowProperty>()
					.GetColumnName( e => e.Id )
				)
			)
			.Should()
			.NotThrow();

		[Fact]
		public void MetadataProviderGetsFluentMappedEntityColumnName()
			=> _context
			.ZeroORM()
			.GetTable<FluentMappedEntity>()
			.GetColumnName( e => e.SomeProperty )
			.Should()
			.Be( TestDbContext.FluentDataColumnName );

		[Fact]
		public void MetadataProviderGetsConventionMappedEntityColumnName()
			=> _context
			.ZeroORM()
			.GetTable<ConventionMappedEntity>()
			.GetColumnName( e => e.SomeString )
			.Should()
			.Be( TestDbContext.ConventionDataColumnName );

		[Fact]
		public void MetadataProviderGetsAttributeMappedEntityColumnName()
			=> _context
			.ZeroORM()
			.GetTable<AttributeMappedEntity>()
			.GetColumnName( e => e.SomeProperty )
			.Should()
			.Be( TestDbContext.AttributedDataColumnName );

		[Fact]
		public async Task OneMetadataMayBeUsedConcurrently()
		{
			IEnumerable<int> range = Enumerable.Range(0, 100);
			TaskCompletionSource<int>[] tcs = range.Select( v => new TaskCompletionSource<int>() ).ToArray();
			// threads usage makes concurrency multi-threaded explicitly which is the worst case for sync APIs
			(int index, Thread thread)[] threads = range
				.Select
				(
					index =>
					(
						index,
						thread: new Thread
						(
							param =>
							{
								int v = (int) param;
								try
								{
									_context
										.ZeroORM()
										.GetTable<AttributeMappedEntity>()
										.GetColumnName( e => e.SomeProperty )
										.Should()
										.Be( TestDbContext.AttributedDataColumnName );
									tcs[ v ].SetResult( v );
								}
								catch (Exception e)
								{
									tcs[ v ].SetException( e );
								}
							}
						)
					)
				)
				.ToArray()
			;

			foreach ( (int index, Thread thread) in threads )
				thread.Start( index );

			await Task.WhenAll( tcs.Select( v => v.Task ) ).ConfigureAwait( false );
		}
	}
}
