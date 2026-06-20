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
    /// My field class.
    /// </summary>
    public class MyFieldClass
    {
        /// <summary>
        /// My field value.
        /// </summary>
        public bool MyField;
    }

    /// <summary>
    /// My full model Inteface.
    /// </summary>
    public interface IMyFullModel
    {
        /// <summary>
        /// The Alias value with get/set.
        /// </summary>
        string? Alias { get; set; }

        /// <summary>
        /// The Name value get only. To override use the <see cref="Alias"/> property. Then some other stuff.
        /// </summary>
        string? Name { get; }

        /// <summary>
        /// My Test with no parameters.
        /// </summary>
        /// <returns></returns>
        string? MyTest();

        /// <summary>
        /// My Test with IEnumerable{bool}.
        /// </summary>
        /// <param name="flags">The 'flags' parameter.</param>
        /// <returns></returns>
        [Display(Name = "My Flags")]
        string? MyTest(IEnumerable<bool> flags);

        /// <summary>
        /// My Test with MyModelClassItem.
        /// </summary>
        /// <param name="item">The 'item' parameter.</param>
        /// <returns></returns>
        [Display(Name = "My Flags")]
        string? MyTest(MyModelClassItem item);

        /// <summary>
        /// My Test with list.
        /// </summary>
        /// <param name="flags">The 'flags' parameter.</param>
        /// <returns></returns>
        [Display(Name = "My Flags")]
        string? MyTest(List<bool> flags);

        /// <summary>
        /// My Test{T}.
        /// </summary>
        /// <param name="flags"></param>
        /// <returns></returns>
        [Display(Name = "My Flags{T}")]
        string? MyTest<T>(T flags);

        /// <summary>
        /// My Test{T,P}.
        /// </summary>
        /// <param name="flags">The 'flags' parameter.</param>
        /// <param name="other">The 'other' parameter.</param>
        /// <returns></returns>
        [Display(Name = "My Flags{T}")]
        string? MyTest<T, P>(T flags, P other);

        /// <summary>
        /// My Test{T,P} with bool.
        /// </summary>
        /// <param name="flags">The 'flags' parameter.</param>
        /// <param name="flag">The 'flag' parameter.</param>
        /// <param name="other">The 'other' parameter.</param>
        /// <returns></returns>
        [Display(Name = "My Flags{T}")]
        string? MyTest<T, P>(T flags, bool flag, P other);

        /// <summary>
        /// My Event handler.
        /// </summary>
        event EventHandler? MyEvent;

        /// <summary>
        /// My Event handler with bool.
        /// </summary>
        event EventHandler<bool>? MyEventB;
    }

    /// <summary>
    /// My Full model that does a lot.
    /// </summary>
    public class MyFullModel : IMyFullModel
    {
        /// <summary>
        /// My Full Model empty Constructor,
        /// </summary>
        public MyFullModel()
        {
            
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public MyFullModel(string? test)
        {
            //This constructor should not be included because it has not xml comment.
        }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <summary>
        /// My Full Model Constructor with parameters,
        /// </summary>
        /// <param name="test"></param>
        public MyFullModel(bool test)
        {
            Test = test;
        }

        /// <summary>
        /// Non-Inteface property Test.
        /// </summary>
        public bool Test { get; }

        /// <summary>
        /// <inheritdoc cref="IMyFullModel.Alias"/>
        /// </summary>
        public string? Alias { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        /// <summary>
        /// <inheritdoc cref="IMyFullModel.Name"/>
        /// </summary>
        public string? Name => throw new NotImplementedException();

        /// <summary>
        /// <inheritdoc cref="IMyFullModel.MyEvent"/>
        /// </summary>
        public event EventHandler? MyEvent;

        /// <summary>
        /// <inheritdoc cref="IMyFullModel.MyEventB"/>
        /// </summary>
        public event EventHandler<bool>? MyEventB;

        /// <summary>
        /// <inheritdoc cref="IMyFullModel.MyTest(IEnumerable{bool})"/>
        /// </summary>
        public string? MyTest(IEnumerable<bool> flags)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// <inheritdoc cref="IMyFullModel.MyTest(MyModelClassItem)"/>
        /// </summary>
        public string? MyTest(MyModelClassItem item)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// <inheritdoc cref="IMyFullModel.MyTest(List{bool})"/>
        /// </summary>
        public string? MyTest(List<bool> flags)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// <inheritdoc cref="IMyFullModel.MyTest{T}(T)"/>
        /// </summary>
        public string? MyTest<T>(T flags)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// <inheritdoc cref="IMyFullModel.MyTest{T, P}(T, P)"/>
        /// </summary>
        public string? MyTest<T, P>(T flags, P other)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// <inheritdoc cref="IMyFullModel.MyTest{T, P}(T, bool, P)"/>
        /// </summary>
        public string? MyTest<T, P>(T flags, bool flag, P other)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// <inheritdoc cref="IMyFullModel.MyTest()"/>
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public string? MyTest()
        {
            throw new NotImplementedException();
        }
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

        /// <summary>
        /// Gets my string.
        /// </summary>
        /// <returns></returns>
        string? GetMyString() => "";

        /// <summary>
        /// Gets my string.
        /// </summary>
        /// <returns></returns>
        string? GetMyString(bool flag) => "";

        /// <summary>
        /// Gets my single value.
        /// </summary>
        /// <param name="flags"></param>
        /// <returns></returns>
        string? GetMySingleMethodValue(IEnumerable<bool> flags) => "";
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
    public interface IMyModelClass : IMyModelClassBase
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

        /// <summary>
        /// MyItem that does stuff. <inheritdoc cref="MyModelClassItem"/>
        /// </summary>
        public MyModelClassItem? MyItem { get; set; }

        /// <summary>
        /// MyItem that does other stuff. <see cref="MyModelClassItem"/>
        /// </summary>
        public MyModelClassItem? MyItem2 { get; set; }

        /// <summary>
        /// <see cref="MyModelClassItem"/>
        /// </summary>
        public MyModelClassItem? MyItem3 { get; set; }

        /// <summary>
        /// <see cref="MyModelClassItem"/>. MyItem4 does stuff.
        /// </summary>
        public MyModelClassItem? MyItem4 { get; set; }
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

    /// <summary>
    /// This is MyModelClassItem.
    /// </summary>
    public class MyModelClassItem
    {
        /// <summary>
        /// The enum value.
        /// </summary>
        public MyModelEnum EnumValue { get; set; }
    }

    /// <summary>
    /// This is my static class.
    /// </summary>
    public static class MyStaticClass
    {
        /// <summary>
        /// Gets my string.
        /// </summary>
        /// <returns></returns>
        public static string? GetMyString() => "my string.";

        /// <summary>
        /// Gets my other string.
        /// </summary>
        /// <param name="x">Longitude</param>
        /// <param name="y">Latitude</param>
        /// <returns></returns>
        public static string? GetMyOtherString(double x, double y) => $"[{x},{y}]";

        /// <summary>
        /// Gets my other string with z.
        /// </summary>
        /// <param name="x">Longitude</param>
        /// <param name="y">Latitude</param>
        /// <param name="z">Elevation</param>
        /// <returns></returns>
        public static string? GetMyOtherString(double x, double y, double z) => $"[{x},{y},{z}]";

        /// <summary>
        /// <see cref="GetMyOtherString(double, double)"/>
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static string? GetMySingleMethodWithParams(double x, double y) => GetMyOtherString(x, y);
    }
}
