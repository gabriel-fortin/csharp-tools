namespace org.g14.AlgebraicSum;

/// <summary>
/// An algebraic sum of the given generic parameters.
/// In other words, it carries a single value which would be of one of the provided generic types
/// </summary>
public class Sum<T1, T2>
{
    private T1? val1;
    private T2? val2;

    // Tells which of the values we actually carry
    byte which = 0;

    public Sum(T1 val)
    {
        which = 1;
        val1 = val;
    }

    public Sum(T2 val)
    {
        which = 2;
        val2 = val;
    }

    /// <summary>
    /// Use the value. Executes one of the actions depending on the current content
    /// of this <see cref="Sum{T1, T2, T3}" />.
    /// </summary>
    public void Use(Action<T1> action1, Action<T2> action2)
    {
        switch (which)
        {
            case 1:
                action1(val1!);
                return;
            case 2:
                action2(val2!);
                return;
            default:
                throw new Exception(
                   $"Invalid internal state. The value of 'which' is {which} which was unexpected");
        };
    }

    /// <summary>
    /// Collapses this algebraic sum into a single value. Executes one of the given mappers depending
    /// on the content of this <see cref="Sum{T1, T2}" /> and returns the result from that mapper.
    /// </summary>
    /// <remark>All functions must have the same return type</remark>
    public TResult Reduce<TResult>(Func<T1, TResult> mapper1, Func<T2, TResult> mapper2)
    {
        return which switch
        {
            1 => mapper1(val1!),
            2 => mapper2(val2!),
            _ => throw new Exception(
                   $"Invalid internal state. The value of 'which' is {which} which was unexpected")
        };
    }

    /// <summary>
    /// Maps this algebraic sum into a new sum, allowing to change the type of all elements.
    /// Executes one of the given mappers depending on the content of this
    /// <see cref="Sum{T1, T2}" /> and returns a new <see cref="Sum{T1, T2}" />.
    /// </summary>
    public Sum<U1, U2> Map<U1, U2>(Func<T1, U1> mapper1, Func<T2, U2> mapper2)
    {
        return which switch
        {
            1 => new Sum<U1, U2>(mapper1(val1!)),
            2 => new Sum<U1, U2>(mapper2(val2!)),
            _ => throw new Exception(
                   $"Invalid internal state. The value of 'which' is {which} which was unexpected")
        };
    }

    /// <summary>
    /// Maps this algebraic sum into a new sum, allowing to change the type of <typeparamref name="T1" />.
    /// Executes the given mapper if the content of this <see cref="Sum{T1, T2}" /> is
    /// of type <typeparamref name="T1" /> and returns a new <see cref="Sum{T1, T2}" />.
    /// </summary>
    public Sum<T1New, T2> MapT1<T1New>(Func<T1, T1New> mapper)
    {
        return Map(mapper, x => x);
    }

    /// <summary>
    /// Maps this algebraic sum into a new sum, allowing to change the type of <typeparamref name="T2" />.
    /// Executes the given mapper if the content of this <see cref="Sum{T1, T2}" /> is
    /// of type <typeparamref name="T2" /> and returns a new <see cref="Sum{T1, T2}" />.
    /// </summary>
    public Sum<T1, T2New> MapT2<T2New>(Func<T2, T2New> mapper)
    {
        return Map(x => x, mapper);
    }

    /// <summary>
    /// Casts this <see cref="Sum{T1, T2}" /> to the <typeparamref name="T1" /> type
    /// or throws an <see cref="InvalidCastException" />
    /// </summary>
    public T1 AsT1()
    {
        return (which == 1) ? val1!
            : throw new InvalidCastException($"This {nameof(Sum<T1, T2>)} contains a T1");
    }

    /// <summary>
    /// Casts this <see cref="Sum{T1, T2, T3}" /> to the <typeparamref name="T2" /> type
    /// or throws an <see cref="InvalidCastException" />
    /// </summary>
    public T2 AsT2()
    {
        return (which == 2) ? val2!
            : throw new InvalidCastException($"This {nameof(Sum<T1, T2>)} contains a T2");
    }

    public bool TryGetT1(out T1? val)
    {
        val = (which == 1) ? val1 : default;
        return which == 1;
    }

    public bool TryGetT2(out T2? val)
    {
        val = (which == 2) ? val2 : default;
        return which == 2;
    }


    public static implicit operator Sum<T1, T2> (T1 val) => new Sum<T1, T2>(val);
    public static implicit operator Sum<T1, T2> (T2 val) => new Sum<T1, T2>(val);
}
