using System.ComponentModel.DataAnnotations;

namespace Sandbox.XmlLib
{
    /// <summary>
    /// MyEnum does this.
    /// </summary>
    public enum MyEnum
    {
        /// <summary>
        /// First Value does this.
        /// </summary>
        [Display(Name = "First Value")]
        FirstValue,
        
        /// <summary>
        /// Second Value does this.
        /// </summary>
        [Display(Name = "Second Value")]
        Second_Value,

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        Third_Value,
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }

    /// <summary>
    /// My test class.
    /// </summary>
    /// <summary>second summary</summary>
    [Display(Name = "MyClass Name")]
    public class MyClass
    {
        /// <summary>
        /// MyClass constructor.
        /// </summary>
        public MyClass()
        {
            
        }
    }

    /// <summary>
    /// Typed version of MyClass.
    /// </summary>
    [Display(Name = "MyClass{T} Name")]
    public class MyClass<T> where T : class
    {
        /// <summary>
        /// The constructor.
        /// </summary>
        public MyClass()
        {

        }

        /// <summary>
        /// The typed value.
        /// </summary>
        public T? Value { get; set;  }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="P">The typeparam P is this...</typeparam>
        /// <param name="value">The value for P.</param>
        public void SetValue<P>(P value) where P:struct
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a new instance of <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="Y">The typeparam Y is this...<see cref="MyClass{T}"/></typeparam>
        /// <returns></returns>
        public Y Create<Y>() where Y : new()
        {
            return new Y();
        }

        /// <summary>
        /// The name.
        /// </summary>
        /// <remarks>The name remarks.</remarks>
        [Display(Name = "The Name")]
        public string? Name { get; set; }

        /// <summary>
        /// The Flag. If <see langword="true"/> then this... else that. <see href="www.myorg.com"/>
        /// </summary>
        public bool? Flag { get; set; }

        /// <summary>
        /// Do some stuff.
        /// </summary>
        /// <remarks>Do stuff remarks 1.</remarks>
        /// <remarks>Do stuff remarks 2.</remarks>
        /// <returns>Do stuff returns 1.</returns>
        /// <returns>Do stuff returns 2.</returns>
        public void DoStuff()
        {

        }
    }

    /// <summary>
    /// Class with nested class.
    /// </summary>
    [Display(Name = "My Class with Nested")]
    public class MyClassWithNested
    {
        /// <summary>
        /// A nested class.
        /// </summary>
        [Display(Name = "My Nested Class")]
        public MyClass? MyNestedClass { get; set; }
    }
}
