using System.ComponentModel.DataAnnotations;

namespace Sandbox.XmlLib.Models
{
    /// <summary>
    /// MyModelEnum does this.
    /// </summary>
    public enum MyModelEnum
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
    /// Abstract MyModelClassBase.
    /// </summary>
    public abstract class MyModelClassBase
    {
        /// <summary>
        /// Abstract Name.
        /// </summary>
        [Display(Name = "The Abstract Name (Should only be in Base class, not dervied class.")]
        public abstract string? Name { get; set; }

        /// <summary>
        /// Virtual Alias.
        /// </summary>
        public virtual string? Alias { get; set; }

        /// <summary>
        /// Test value.
        /// </summary>
        public string? Test { get; set; }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public bool MyFlag { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }

    /// <summary>
    /// My model class test.
    /// </summary>
    [Display(Name = "MyClass Name")]
    public class MyModelClass : MyModelClassBase
    {
        /// <summary>
        /// Override abstract Name.
        /// </summary>
        public override string? Name { get; set; }

        /// <summary>
        /// Override virtual Alias.
        /// </summary>
        public override string? Alias { get; set; }

        /// <summary>
        /// Static Counter.
        /// </summary>
        public static int Counter { get; set; }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static double Percentage { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }

    /// <summary>
    /// Typed version of MyModelClass.
    /// </summary>
    [Display(Name = "MyModelClass{T} Name")]
    public class MyModelClass<T> where T : class
    {
        /// <summary>
        /// The constructor.
        /// </summary>
        public MyModelClass()
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
