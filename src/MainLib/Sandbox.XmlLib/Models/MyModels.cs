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

        /// <summary>
        /// 
        /// </summary>
        Third_Value,
    }

    /// <summary>
    /// Interface for IMyModelClassBase.
    /// </summary>
    public interface IMyModelClassBase
    {
        /// <summary>
        /// The Alias value.
        /// </summary>
        string? Alias { get; set; }
        /// <summary>
        /// The MyFlag value.
        /// </summary>
        bool MyFlag { get; set; }

        /// <summary>
        /// The Name value.
        /// </summary>
        string? Name { get; set; }

        /// <summary>
        /// The Test value.
        /// </summary>
        string? Test { get; set; }
    }

    /// <summary>
    /// Abstract MyModelClassBase.
    /// </summary>
    public abstract class MyModelClassBase : IMyModelClassBase
    {
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        [Display(Name = "The Abstract Name (Should only be in Base class, not dervied class.")]
        public abstract string? Name { get; set; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public virtual string? Alias { get; set; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public string? Test { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool MyFlag { get; set; }
    }

    /// <summary>
    /// Interface for MyModelClass.
    /// </summary>
    public interface IMyModelClass: IMyModelClassBase
    {
        /// <summary>
        /// Static Counter.
        /// </summary>
        static abstract int Counter { get; set; }

        /// <summary>
        /// Static Percentage.
        /// </summary>
        static abstract double Percentage { get; set; }
    }

    /// <summary>
    /// My model class test.
    /// </summary>
    [Display(Name = "MyClass Name")]
    public class MyModelClass : MyModelClassBase, IMyModelClass
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

        /// <summary>
        /// 
        /// </summary>
        public static double Percentage { get; set; }
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
