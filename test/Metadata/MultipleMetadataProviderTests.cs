using System;
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
	public class MetadataProviderTests
	{
		[Fact]
		public void MetadataProviderGetsAttributeMappedEntityColumnNameFromBaseContext()
			=> new TestDbContext()
			.ZeroORM()
			.GetTable<AttributeMappedEntity>()
			.GetColumnName( e => e.SomeProperty )
			.Should()
			.Be( TestDbContext.AttributedDataColumnName );

		[Fact]
		public void MetadataProviderGetsAttributeMappedEntityColumnNameFromExtendedContext()
			=> new TestDbContextExtended()
			.ZeroORM()
			.GetTable<AttributeMappedEntity>()
			.GetColumnName( e => e.SomeProperty )
			.Should()
			.Be( TestDbContext.AttributedDataColumnName );

		[Fact]
		public void MetadataProviderGetsAdditionalAttributeMappedEntityColumnNameFromExtendedContext()
			=> new TestDbContextExtended()
			.ZeroORM()
			.GetTable<ExtendedContextAdditionalEntity>()
			.GetColumnName( e => e.SomeProperty )
			.Should()
			.Be( TestDbContextExtended.ExtendedEntitySomePropertyColumnName );

		// recreation is to allow to reference non-static fields. May be easily fixed by initialization from constructor if becomes tests performance bottleneck for some reason
		private Action[] AssertionsOnSingleContext => new Action[]
		{
			MetadataProviderGetsAttributeMappedEntityColumnNameFromBaseContext,
			MetadataProviderGetsAttributeMappedEntityColumnNameFromExtendedContext,
			MetadataProviderGetsAdditionalAttributeMappedEntityColumnNameFromExtendedContext
		};

		private IEnumerable<Action> GetAssertionsInOrder( int[] order ) =>
			AssertionsOnSingleContext
			.Zip( order, ( act, order ) => (act, order) )
			.OrderBy( t => t.order )
			.Select( t => t.act )
			;

		public static IEnumerable<object[]> AssertionOrders =
			new[]
			{
				new[] { 1, 2, 3 },
				new[] { 1, 3, 2 },
				new[] { 2, 1, 3 },
				new[] { 2, 3, 1 },
				new[] { 3, 1, 2 },
				new[] { 3, 2, 1 }
			}
			.Select(v => new object[] { v } )
			;

		[Theory]
		[MemberData(nameof(AssertionOrders))]
		public void AssertionsWorkInAnyOrder(int[] order)
		{
			foreach ( Action assertion in GetAssertionsInOrder( order ) )
				assertion();
		}

		[Fact]
		public async Task ManyMetadataMayBeUsedConcurrently()
		{
			IEnumerable<int> range = Enumerable.Range(0, 100);
			TaskCompletionSource<int>[] tcs = range.Select( v => new TaskCompletionSource<int>() ).ToArray();
			Action[] assertions = AssertionsOnSingleContext;

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
									assertions[ v % assertions.Length ]();
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
