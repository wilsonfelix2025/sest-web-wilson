﻿using System;
using System.Runtime.InteropServices;

namespace SestWeb.Domain.Entities.Correlações.Parsers.MuParserWrapper
{
    #region Callbacks internas

    /// <summary>
    /// Internal callback used for variable creation factory functions.
    /// </summary>
    /// <param name="name">The name of the created variable</param>
    /// <param name="userData">The user data object address</param>
    /// <returns>The variable pointer to muParser</returns>
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate IntPtr IntFactoryFunction(string name, IntPtr userData);

    #endregion

    /*
     * Definição dos ponteiros para função que serão passado para o muParser.
     */

    /// <summary>
    /// Callback used for variable creation factory functions.
    /// </summary>
    /// <param name="name">The name of the created variable</param>
    /// <param name="userData">The user data object</param>
    /// <returns>The parser variable</returns>
    [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl)]
    public delegate ParserVariable FactoryFunction(string name, object userData);

    /// <summary>
    /// Callback type used for handle parser errors.
    /// </summary>
    [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl)]
    public delegate void ErrorFuncType();

    /// <summary>
    /// Callback type used for functions without arguments.
    /// </summary>
    /// <returns>The function result</returns>
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate double FunType0();

    /// <summary>
    /// Callback type used for functions with a single arguments.
    /// </summary>
    /// <param name="arg0">The first function argument</param>
    /// <returns>The function result</returns>
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate double FunType1(
        double arg0);

    /// <summary>
    /// Callback type used for functions with two arguments.
    /// </summary>
    /// <param name="arg0">The first function argument</param>
    /// <param name="arg1">The second function argument</param>
    /// <returns>The function result</returns>
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate double FunType2(
        double arg0,
        double arg1);

    /// <summary>
    /// Callback type used for functions with three arguments.
    /// </summary>
    /// <param name="arg0">The first function argument</param>
    /// <param name="arg1">The second function argument</param>
    /// <param name="arg2">The third function argument</param>
    /// <returns>The function result</returns>
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate double FunType3(
        double arg0,
        double arg1,
        double arg2);

    /// <summary>
    /// Callback type used for functions with four arguments.
    /// </summary>
    /// <param name="arg0">The first function argument</param>
    /// <param name="arg1">The second function argument</param>
    /// <param name="arg2">The third function argument</param>
    /// <param name="arg3">The fourth function argument</param>
    /// <returns>The function result</returns>
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate double FunType4(
        double arg0,
        double arg1,
        double arg2,
        double arg3);

    /// <summary>
    /// Callback type used for functions with five arguments.
    /// </summary>
    /// <param name="arg0">The first function argument</param>
    /// <param name="arg1">The second function argument</param>
    /// <param name="arg2">The third function argument</param>
    /// <param name="arg3">The fourth function argument</param>
    /// <param name="arg4">The fifth function argument</param>
    /// <returns>The function result</returns>
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate double FunType5(
        double arg0,
        double arg1,
        double arg2,
        double arg3,
        double arg4);

    /// <summary>
    /// Callback type used for functions with six arguments.
    /// </summary>
    /// <param name="arg0">The first function argument</param>
    /// <param name="arg1">The second function argument</param>
    /// <param name="arg2">The third function argument</param>
    /// <param name="arg3">The fourth function argument</param>
    /// <param name="arg4">The fifth function argument</param>
    /// <param name="arg5">The sixth function argument</param>
    /// <returns>The function result</returns>
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate double FunType6(
        double arg0,
        double arg1,
        double arg2,
        double arg3,
        double arg4,
        double arg5);

    /// <summary>
    /// Callback type used for functions with seven arguments.
    /// </summary>
    /// <param name="arg0">The first function argument</param>
    /// <param name="arg1">The second function argument</param>
    /// <param name="arg2">The third function argument</param>
    /// <param name="arg3">The fourth function argument</param>
    /// <param name="arg4">The fifth function argument</param>
    /// <param name="arg5">The sixth function argument</param>
    /// <param name="arg6">The seventh function argument</param>
    /// <returns>The function result</returns>
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate double FunType7(
        double arg0,
        double arg1,
        double arg2,
        double arg3,
        double arg4,
        double arg5,
        double arg6);

    /// <summary>
    /// Callback type used for functions with eight arguments.
    /// </summary>
    /// <param name="arg0">The first function argument</param>
    /// <param name="arg1">The second function argument</param>
    /// <param name="arg2">The third function argument</param>
    /// <param name="arg3">The fourth function argument</param>
    /// <param name="arg4">The fifth function argument</param>
    /// <param name="arg5">The sixth function argument</param>
    /// <param name="arg6">The seventh function argument</param>
    /// <param name="arg7">The eighth function argument</param>
    /// <returns>The function result</returns>
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate double FunType8(
        double arg0,
        double arg1,
        double arg2,
        double arg3,
        double arg4,
        double arg5,
        double arg6,
        double arg7);

    /// <summary>
    /// Callback type used for functions with nine arguments.
    /// </summary>
    /// <param name="arg0">The first function argument</param>
    /// <param name="arg1">The second function argument</param>
    /// <param name="arg2">The third function argument</param>
    /// <param name="arg3">The fourth function argument</param>
    /// <param name="arg4">The fifth function argument</param>
    /// <param name="arg5">The sixth function argument</param>
    /// <param name="arg6">The seventh function argument</param>
    /// <param name="arg7">The eighth function argument</param>
    /// <param name="arg8">The ninth function argument</param>
    /// <returns>The function result</returns>
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate double FunType9(
        double arg0,
        double arg1,
        double arg2,
        double arg3,
        double arg4,
        double arg5,
        double arg6,
        double arg7,
        double arg8);

    /// <summary>
    /// Callback type used for functions with ten arguments.
    /// </summary>
    /// <param name="arg0">The first function argument</param>
    /// <param name="arg1">The second function argument</param>
    /// <param name="arg2">The third function argument</param>
    /// <param name="arg3">The fourth function argument</param>
    /// <param name="arg4">The fifth function argument</param>
    /// <param name="arg5">The sixth function argument</param>
    /// <param name="arg6">The seventh function argument</param>
    /// <param name="arg7">The eighth function argument</param>
    /// <param name="arg8">The ninth function argument</param>
    /// <param name="arg9">The tenth function argument</param>
    /// <returns>The function result</returns>
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate double FunType10(
        double arg0,
        double arg1,
        double arg2,
        double arg3,
        double arg4,
        double arg5,
        double arg6,
        double arg7,
        double arg8,
        double arg9);

    /// <summary>
    /// Callback type used for functions without arguments.
    /// </summary>
    /// <param name="bulkIndex">The current bulk index</param>
    /// <param name="threadIndex">The thread index that are running the callback</param>
    /// <returns>The function result</returns>
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate double BulkFunType0(int bulkIndex, int threadIndex);

    /// <summary>
    /// Callback type used for functions with a single argument.
    /// </summary>
    /// <param name="bulkIndex">The current bulk index</param>
    /// <param name="threadIndex">The thread index that are running the callback</param>
    /// <param name="arg0">The first function argument</param>
    /// <returns>The function result</returns>
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate double BulkFunType1(int bulkIndex, int threadIndex,
        double arg0);

    /// <summary>
    /// Callback type used for functions with two arguments.
    /// </summary>
    /// <param name="bulkIndex">The current bulk index</param>
    /// <param name="threadIndex">The thread index that are running the callback</param>
    /// <param name="arg0">The first function argument</param>
    /// <param name="arg1">The second function argument</param>
    /// <returns>The function result</returns>
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate double BulkFunType2(int bulkIndex, int threadIndex,
        double arg0,
        double arg1);

    /// <summary>
    /// Callback type used for functions with three arguments.
    /// </summary>
    /// <param name="bulkIndex">The current bulk index</param>
    /// <param name="threadIndex">The thread index that are running the callback</param>
    /// <param name="arg0">The first function argument</param>
    /// <param name="arg1">The second function argument</param>
    /// <param name="arg2">The third function argument</param>
    /// <returns>The function result</returns>
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate double BulkFunType3(int bulkIndex, int threadIndex,
        double arg0,
        double arg1,
        double arg2);

    /// <summary>
    /// Callback type used for functions with four arguments.
    /// </summary>
    /// <param name="bulkIndex">The current bulk index</param>
    /// <param name="threadIndex">The thread index that are running the callback</param>
    /// <param name="arg0">The first function argument</param>
    /// <param name="arg1">The second function argument</param>
    /// <param name="arg2">The third function argument</param>
    /// <param name="arg3">The fourth function argument</param>
    /// <returns>The function result</returns>
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate double BulkFunType4(int bulkIndex, int threadIndex,
        double arg0,
        double arg1,
        double arg2,
        double arg3);

    /// <summary>
    /// Callback type used for functions with five arguments.
    /// </summary>
    /// <param name="bulkIndex">The current bulk index</param>
    /// <param name="threadIndex">The thread index that are running the callback</param>
    /// <param name="arg0">The first function argument</param>
    /// <param name="arg1">The second function argument</param>
    /// <param name="arg2">The third function argument</param>
    /// <param name="arg3">The fourth function argument</param>
    /// <param name="arg4">The fifth function argument</param>
    /// <returns>The function result</returns>
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate double BulkFunType5(int bulkIndex, int threadIndex,
        double arg0,
        double arg1,
        double arg2,
        double arg3,
        double arg4);

    /// <summary>
    /// Callback type used for functions with six arguments.
    /// </summary>
    /// <param name="bulkIndex">The current bulk index</param>
    /// <param name="threadIndex">The thread index that are running the callback</param>
    /// <param name="arg0">The first function argument</param>
    /// <param name="arg1">The second function argument</param>
    /// <param name="arg2">The third function argument</param>
    /// <param name="arg3">The fourth function argument</param>
    /// <param name="arg4">The fifth function argument</param>
    /// <param name="arg5">The sixth function argument</param>
    /// <returns>The function result</returns>
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate double BulkFunType6(int bulkIndex, int threadIndex,
        double arg0,
        double arg1,
        double arg2,
        double arg3,
        double arg4,
        double arg5);

    /// <summary>
    /// Callback type used for functions with seven arguments.
    /// </summary>
    /// <param name="bulkIndex">The current bulk index</param>
    /// <param name="threadIndex">The thread index that are running the callback</param>
    /// <param name="arg0">The first function argument</param>
    /// <param name="arg1">The second function argument</param>
    /// <param name="arg2">The third function argument</param>
    /// <param name="arg3">The fourth function argument</param>
    /// <param name="arg4">The fifth function argument</param>
    /// <param name="arg5">The sixth function argument</param>
    /// <param name="arg6">The seventh function argument</param>
    /// <returns>The function result</returns>
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate double BulkFunType7(int bulkIndex, int threadIndex,
        double arg0,
        double arg1,
        double arg2,
        double arg3,
        double arg4,
        double arg5,
        double arg6);

    /// <summary>
    /// Callback type used for functions with eight arguments.
    /// </summary>
    /// <param name="bulkIndex">The current bulk index</param>
    /// <param name="threadIndex">The thread index that are running the callback</param>
    /// <param name="arg0">The first function argument</param>
    /// <param name="arg1">The second function argument</param>
    /// <param name="arg2">The third function argument</param>
    /// <param name="arg3">The fourth function argument</param>
    /// <param name="arg4">The fifth function argument</param>
    /// <param name="arg5">The sixth function argument</param>
    /// <param name="arg6">The seventh function argument</param>
    /// <param name="arg7">The eighth function argument</param>
    /// <returns>The function result</returns>
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate double BulkFunType8(int bulkIndex, int threadIndex,
        double arg0,
        double arg1,
        double arg2,
        double arg3,
        double arg4,
        double arg5,
        double arg6,
        double arg7);

    /// <summary>
    /// Callback type used for functions with nine arguments.
    /// </summary>
    /// <param name="bulkIndex">The current bulk index</param>
    /// <param name="threadIndex">The thread index that are running the callback</param>
    /// <param name="arg0">The first function argument</param>
    /// <param name="arg1">The second function argument</param>
    /// <param name="arg2">The third function argument</param>
    /// <param name="arg3">The fourth function argument</param>
    /// <param name="arg4">The fifth function argument</param>
    /// <param name="arg5">The sixth function argument</param>
    /// <param name="arg6">The seventh function argument</param>
    /// <param name="arg7">The eighth function argument</param>
    /// <param name="arg8">The ninth function argument</param>
    /// <returns>The function result</returns>
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate double BulkFunType9(int bulkIndex, int threadIndex,
        double arg0,
        double arg1,
        double arg2,
        double arg3,
        double arg4,
        double arg5,
        double arg6,
        double arg7,
        double arg8);

    /// <summary>
    /// Callback type used for functions with ten arguments.
    /// </summary>
    /// <param name="bulkIndex">The current bulk index</param>
    /// <param name="threadIndex">The thread index that are running the callback</param>
    /// <param name="arg0">The first function argument</param>
    /// <param name="arg1">The second function argument</param>
    /// <param name="arg2">The third function argument</param>
    /// <param name="arg3">The fourth function argument</param>
    /// <param name="arg4">The fifth function argument</param>
    /// <param name="arg5">The sixth function argument</param>
    /// <param name="arg6">The seventh function argument</param>
    /// <param name="arg7">The eighth function argument</param>
    /// <param name="arg8">The ninth function argument</param>
    /// <param name="arg9">The tenth function argument</param>
    /// <returns>The function result</returns>
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate double BulkFunType10(int bulkIndex, int threadIndex,
        double arg0,
        double arg1,
        double arg2,
        double arg3,
        double arg4,
        double arg5,
        double arg6,
        double arg7,
        double arg8,
        double arg9);

    /// <summary>
    /// Callback type used for functions with a single argument.
    /// </summary>
    /// <param name="bulkIndex">The current bulk index</param>
    /// <param name="threadIndex">The thread index that are running the callback</param>
    /// <param name="arg0">The argument name</param>
    /// <param name="str">The argument name</param>
    /// <returns>The function result</returns>
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate double BulkFunType11(int bulkIndex, int threadIndex, [MarshalAs(UnmanagedType.LPStr)] string str);

    /// <summary>
    /// Callback type used for functions with a variable argument list.
    /// </summary>
    /// <param name="args">The arguments list</param>
    /// <param name="size">The size of arguments list</param>
    /// <returns>The function result</returns>
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate double MultFunType([MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)]
        double[] args, int size);

    /// <summary>
    /// Callback type used for functions taking a string as an argument.
    /// </summary>
    /// <param name="str">The string function argument</param>
    /// <returns>The function result</returns>
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate double StrFunType1([MarshalAs(UnmanagedType.LPStr)] string str);

    /// <summary>
    /// Callback type used for functions taking a string and a value as arguments.
    /// </summary>
    /// <param name="str">The string function argument</param>
    /// <param name="arg0">The first function argument</param>
    /// <returns>The function result</returns>
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate double StrFunType2([MarshalAs(UnmanagedType.LPStr)] string str, double arg0);

    /// <summary>
    /// Callback type used for functions taking a string and two values as arguments.
    /// </summary>
    /// <param name="str">The string function argument</param>
    /// <param name="arg0">The first function argument</param>
    /// <param name="arg1">The second function argument</param>
    /// <returns>The function result</returns>
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate double StrFunType3([MarshalAs(UnmanagedType.LPStr)] string str, double arg0, double arg1);

    /// <summary>
    /// Callback used for functions that identify values in a string.
    /// </summary>
    /// <param name="remainingExpr">The string function argument</param>
    /// <param name="pos">The position relative to the first position of the
    /// expression. This must be incremented with the number of characters used
    /// to parser de value.</param>
    /// <param name="value">The variable to receive the parsed value</param>
    /// <returns>Must return <code>true</code> if the value was parsed by the callback.
    /// Otherwise, it must return <code>false</code>.</returns>
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate bool IdentFunction([MarshalAs(UnmanagedType.LPStr)] string remainingExpr, ref int pos,
        ref double value);
}
