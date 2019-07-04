using System;

namespace Monads.Result {
  public interface IResult<T, TError> {
    T ok { get; }
    TError error { get; }
    bool IsOk();
    bool IsError();
  }

  public enum Reason {
    Canceled,
    Exception
  }

  public class Ok<T, TError> : IResult<T, TError> {
    public T ok { get; }
    public TError error { get; }
    public Ok(T ok) {
      this.ok = ok;
    }

    public bool IsOk() {
      return true;
    }

    public bool IsError() {
      return false;
    }
  }

  public class Error<T, TError> : IResult<T, TError> {
    public T ok { get; }
    public TError error { get; }
    public Error(TError error) {
      this.error = error;
    }

    public bool IsOk() {
      return false;
    }

    public bool IsError() {
      return true;
    }
  }

  public static class Result {
    public static IResult<T, TError> Return<T, TError>(T @value) {
      return new Ok<T, TError>(@value);
    }

    public static IResult<TResult, TError> Map<T, TError, TResult>(
      this IResult<T, TError> result,
      Func<T, TResult> handler
    ) {
      if (result.IsOk()) {
        return Return<TResult, TError>(handler(result.ok));
      }
      return result as IResult<TResult, TError>;
    }

    public static void Do<T, TError>(
      this IResult<T, TError> result,
      Action<T> handler
    ) {
      if (result.IsOk()) {
        handler(result.ok);
      }
    }

    public static IResult<TResult, TError> Bind<T, TError, TResult>(
      this IResult<T, TError> result,
      Func<T, IResult<TResult, TError>> handler
    ) {
      if (result.IsOk()) {
        return handler(result.ok);
      }
      return result as IResult<TResult, TError>;
    }

    public static T OrElse<T, TError>(
      this IResult<T, TError> result,
      T @default
    ) {
      if (result.IsOk()) {
        return result.ok;
      }
      return @default;
    }

    public static T OrElseWith<T, TError>(
      this IResult<T, TError> result,
      Func<TError, T> generator
    ) {
      if (result.IsOk()) {
        return result.ok;
      }
      return generator(result.error);
    }
  }
}
