namespace ProductsService.Data.UnitOfWork;

public sealed class AsyncFactory<TAbstraction, TImplementation> : IAsyncFactory<TAbstraction>
    where TImplementation : class, TAbstraction, IInitializeAsync
    
{
    public AsyncFactory(Func<TImplementation> resolveObject) => ResolveObject = resolveObject;

    private Func<TImplementation> ResolveObject { get; }

    public ValueTask<TAbstraction> CreateAsync(CancellationToken cancellationToken = default)
    {
        var @object = ResolveObject();
        return @object.IsInitialized ? new (@object) : InitializeObjectAsync(@object, cancellationToken);
    }

    private static async ValueTask<TAbstraction> InitializeObjectAsync(TImplementation @object,
                                                                       CancellationToken cancellationToken)
    {
        await @object.InitializeAsync(cancellationToken);
        return @object;
    }
}
