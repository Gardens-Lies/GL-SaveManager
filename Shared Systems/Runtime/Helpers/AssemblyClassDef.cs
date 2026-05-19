/*
 * Copyright (c) 2025 Carter Games
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

using System;
using UnityEngine;

namespace CarterGames.Shared.SaveManager
{
	/// <summary>
	/// A class for storing info about a class, so it can be referenced from its assembly & type names.
	/// </summary>
	[Serializable]
	public sealed class AssemblyClassDef
	{
		/* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
		|   Fields
		───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
		
		[SerializeField] private string assembly;
		[SerializeField] private string type;

		/* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
		|   Properties
		───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
		
		/// <summary>
		/// Gets if the class is valid or not.
		/// </summary>
		public bool IsValid => !string.IsNullOrEmpty(assembly) && !string.IsNullOrEmpty(type);

		
		/// <summary>
		/// The assembly string stored.
		/// </summary>
		public string StoredAssembly => assembly;
		
		
		/// <summary>
		/// The type string stored.
		/// </summary>
		public string StoredType => type;
		
		
		/// <summary>
		/// The assembly qualified string stored.
		/// </summary>
		public string StoredAssemblyQualified => $"{StoredType}, {StoredAssembly}";

		/* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
		|   Fields
		───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
		
		/// <summary>
		/// Creates a new definition when called.
		/// </summary>
		/// <param name="storedAssembly">The assembly to reference.</param>
		/// <param name="type">The type to reference.</param>
		public AssemblyClassDef(string storedAssembly, string type)
		{
			this.assembly = storedAssembly;
			this.type = type;
		}

		/* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
		|   Fields
		───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
		
		/// <summary>
		/// Converts System.Type to a AssemblyClassDef instance.
		/// </summary>
		/// <param name="type">The type to convert.</param>
		/// <returns>The created AssemblyClassDef from the type.</returns>
		public static implicit operator AssemblyClassDef(Type type)
		{
			return new AssemblyClassDef(type.Assembly.FullName, type.FullName);
		}
		
		/* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
		|   Fields
		───────────────────────────────────────────────────────────────────────────────────────────────────────────── */

		/// <summary>
		/// Tries to get the type stored.
		/// </summary>
		/// <param name="typeStored">The type stored</param>
		/// <returns>Bool</returns>
		public bool TryGetType(out Type typeStored)
		{
			typeStored = null;
			
			try
			{
				typeStored = Type.GetType(StoredAssemblyQualified);
				return true;
			}
#pragma warning disable 0168
			catch (Exception e)
#pragma warning restore
			{
				return false;
			}
		}
		
		
		/// <summary>
		/// Gets the type stored in this AssemblyClassDef as an instance of its type.
		/// Use <see cref="TryGetType"/> to just get the type.
		/// </summary>
		/// <typeparam name="T">The type to make.</typeparam>
		/// <returns>The made type or the types default on failure.</returns>
		public T GetTypeInstance<T>()
		{
			if (!IsValid)
			{
				Debug.LogError("[GetDefinedType]: Data not valid to generate the defined type");
				return default;
			}
			
			try
			{
				if (TryGetType(out var typeValue))
				{
					return (T)Activator.CreateInstance(typeValue);
				}
				
				Debug.LogError("[GetDefinedType]: Type resolved is null, have you refactored the typename, namespace or assembly?");
					
				return default;
			}
#pragma warning disable 0168
			catch (Exception e)
			{
				Debug.LogError(
					"[GetDefinedType]: Failed to generate type from stored data. If you have refactored the class selected, please reselect it to update the record.");

				return default;
			}
#pragma warning restore
		}


		/// <summary>
		/// Gets if a type is the same as this assembly class define.
		/// </summary>
		/// <param name="targetType">The type to compare</param>
		/// <returns>bool</returns>
		public bool IsDefineType(Type targetType)
		{
			return StoredAssembly == targetType.Assembly.FullName && StoredType == targetType.FullName;
		}

		
		/// <summary>
		/// Gets if the type entered is a base class of the stored value.
		/// </summary>
		/// <param name="targetType">The type to compare</param>
		/// <returns>Bool</returns>
		public bool InheritsFrom(Type targetType)
		{
			if (!TryGetType(out var thisType))
			{
				Debug.Log(
					"Stored type is not parsing to the desired type. Please reselect if you have changed the types namespace or assembly.");
				
				return false;
			}
			
			return thisType.IsAssignableFrom(targetType);
		}
	}
}