using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using ZeroORM.EFCore.Metadata;
using ZeroORM.Extensibility.Metadata;

namespace System
{
	public static class EfCoreMetadataProviderFactoryExtension
	{
		public static IMetadataProvider ZeroORM( this DbContext context )
			=> context
			?.Model
			?.ZeroORM()
			?? throw new ArgumentNullException( nameof( context ) );

		public static IMetadataProvider ZeroORM( this IModel model )
			=> new EFCoreMetadataProvider( model ?? throw new ArgumentNullException( nameof( model ) ) );
	}
}
