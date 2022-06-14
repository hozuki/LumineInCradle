using System;
using System.Runtime.CompilerServices;
using Better;
using HarmonyLib;

namespace LumineInCradle;

public static class FieldHelper
{

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void FastFieldAccess<TObject, TField>(TObject instance, string fieldName, ref AccessTools.FieldRef<object, TField> fieldRef, RefAction<TField> action)
		where TObject : class
	{
		fieldRef ??= AccessTools.FieldRefAccess<TField>(typeof(TObject), fieldName);
		ref var fieldValue = ref fieldRef(instance);
		action(ref fieldValue);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void FastFieldAccess<TObject, TField, TParam1>(TObject instance, string fieldName, ref AccessTools.FieldRef<object, TField> fieldRef, RefAction<TField, TParam1> action, TParam1 arg1)
		where TObject : class
	{
		fieldRef ??= AccessTools.FieldRefAccess<TField>(typeof(TObject), fieldName);
		ref var fieldValue = ref fieldRef(instance);
		action(ref fieldValue, arg1);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void FastFieldValueClear<TObject, TField>(TObject instance, string fieldName, ref AccessTools.FieldRef<object, TField> fieldRef, TField defaultValue = default)
		where TObject : class
		where TField : unmanaged
	{
		// ReSharper disable once RedundantAssignment
		FastFieldAccess(instance, fieldName, ref fieldRef, (ref TField value, TField defVal) => value = defVal, defaultValue);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void FastFieldValueClear<TObject, TArrayElement>(TObject instance, string fieldName, ref AccessTools.FieldRef<object, TArrayElement[]> fieldRef)
		where TObject : class
		where TArrayElement : unmanaged
	{
		FastFieldAccess(instance, fieldName, ref fieldRef, (ref TArrayElement[] value) =>
		{
			Array.Clear(value, 0, value.Length);
		});
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void FastFieldValueClear<TObject, TKey, TValue>(TObject instance, string fieldName, ref AccessTools.FieldRef<object, BDic<TKey, TValue>> fieldRef)
		where TObject : class
	{
		FastFieldAccess(instance, fieldName, ref fieldRef, (ref BDic<TKey, TValue> value) => value.Clear());
	}

}
