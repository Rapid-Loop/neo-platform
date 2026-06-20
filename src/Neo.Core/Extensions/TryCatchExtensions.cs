// BSD 2-Clause License
//
// Copyright (c) 2026, Rapid Loop
//
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are met:
//
// 1. Redistributions of source code must retain the above copyright notice, this
//    list of conditions and the following disclaimer.
//
// 2. Redistributions in binary form must reproduce the above copyright notice,
//    this list of conditions and the following disclaimer in the documentation
//    and/or other materials provided with the distribution.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES

using System;

namespace Neo.Core.Extensions
{
    public static class TryCatchExtensions
    {
        public static TResult? TryCatch<TSource, TResult>(this TSource obj, Func<TSource?, TResult?> func)
        {
            try
            {
                return func(obj);
            }
            catch
            {
                return default;
            }
        }

        public static TSource TryCatch<TSource, TException>(this TSource obj, Action<TSource> action, Action<TSource?, TException>? onError = default)
            where TException : Exception
        {
            try
            {
                action(obj);
            }
            catch (TException ex)
            {
                onError?.Invoke(obj, ex);
            }

            return obj;
        }

        public static TResult TryCatch<TSource, TException, TResult>(this TSource obj, Func<TSource, TResult> func, Func<TSource, TException, TResult>? onError = default)
            where TException : Exception
        {
            try
            {
                return func(obj);
            }
            catch (TException ex)
            {
                if (onError == null) throw;
                return onError(obj, ex);
            }
        }

        public static TSource TryCatchThrow<TSource, TException>(this TSource obj, Action<TSource?> action)
            where TException : Exception
        {
            try
            {
                action(obj);

                return obj;
            }
            catch (TException)
            {
                throw;
            }
        }

        public static TResult TryCatchThrow<TSource, TException, TResult>(this TSource obj, Func<TSource, TResult> func)
            where TException : Exception
        {
            try
            {
                return func(obj);
            }
            catch (TException)
            {
                throw;
            }
        }

        public static TSource TryCatchThrow<TSource, TException>(this TSource obj, Action<TSource?> action, string? errorMessage = default)
            where TException : Exception, new()
        {
            try
            {
                action(obj);

                return obj;
            }
            catch (TException innerException)
            {
                if (string.IsNullOrEmpty(errorMessage))
                    throw;
                else
                {
                    if (Activator.CreateInstance(typeof(TException), errorMessage, innerException) is not TException ex)
                        throw;
                    else
                        throw ex;
                }

            }
        }

        public static TResult? TryCatchThrow<TSource, TException, TResult>(this TSource obj, Func<TSource?, TResult?> func, string? errorMessage = default)
            where TException : Exception
        {
            try
            {
                return func(obj);
            }
            catch (TException innerException)
            {
                if (string.IsNullOrEmpty(errorMessage))
                    throw;
                else
                {
                    if (Activator.CreateInstance(typeof(TException), errorMessage, innerException) is not TException ex)
                        throw;
                    else
                        throw ex;
                }

            }
        }
    }
}
