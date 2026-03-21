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
    [Display(Name = "MyClass Name")]
    public class MyClass
    {

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
        /// The name.
        /// </summary>
        /// <remarks>The name remarks.</remarks>
        [Display(Name = "The Name")]
        public string? Name { get; set; }

        /// <summary>
        /// Do some stuff.
        /// </summary>
        /// <remarks>Do stuff remarks.</remarks>
        /// <returns>Do stuff returns.</returns>
        public void DoStuff()
        {

        }
    }
}
