namespace MedCorVis.Modules.Users.Tests.Helpers;

using System.Collections;
using System.Linq.Expressions;

internal sealed class TestAsyncQueryable<T> : IQueryable<T>, IAsyncEnumerable<T>
{
    private readonly IQueryable<T> _inner;

    public TestAsyncQueryable(IEnumerable<T> data)
    {
        _inner = data.AsQueryable();
    }

    public Type ElementType => _inner.ElementType;
    public Expression Expression => _inner.Expression;
    public IQueryProvider Provider => new TestAsyncQueryProvider<T>(_inner.Provider);

    public IEnumerator<T> GetEnumerator() => _inner.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken ct = default)
        => new TestAsyncEnumerator<T>(_inner.GetEnumerator());
}

internal sealed class TestAsyncEnumerator<T> : IAsyncEnumerator<T>
{
    private readonly IEnumerator<T> _inner;

    public TestAsyncEnumerator(IEnumerator<T> inner)
    {
        _inner = inner;
    }

    public T Current => _inner.Current;

    public ValueTask<bool> MoveNextAsync() => ValueTask.FromResult(_inner.MoveNext());

    public ValueTask DisposeAsync()
    {
        _inner.Dispose();
        return ValueTask.CompletedTask;
    }
}

internal sealed class TestAsyncQueryProvider<T> : IQueryProvider
{
    private readonly IQueryProvider _inner;

    public TestAsyncQueryProvider(IQueryProvider inner)
    {
        _inner = inner;
    }

    public IQueryable CreateQuery(Expression expression)
        => new TestAsyncQueryable<T>((IEnumerable<T>)_inner.CreateQuery(expression));

    public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        => new TestAsyncQueryable<TElement>(
            _inner.CreateQuery<TElement>(expression));

    public object? Execute(Expression expression) => _inner.Execute(expression);

    public TResult Execute<TResult>(Expression expression) => _inner.Execute<TResult>(expression);
}