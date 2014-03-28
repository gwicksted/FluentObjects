# FluentObjects

Use your data structures **before you write them!**

*Fluently access dynamic structures without defining them beforehand or worrying about exceptions.*

In other words: **do everything they told you not to do!**

Simple usage example:

```C#
dynamic o = new FluentObject();
  
o.Name = "Joe";
o.AddressArray[0] = new Address();
o.Friends["12345-6789"].Name = "John";
o.Friends["12345-6789"].AddressArray[0] = new Address();

// Now what?
// Serialize to JSON or XML
// Access it with LINQ queries (MVC!)
// Cast to a native list (Dictionary/IDictionary*)
// * See Caveats for IDictionary example
```
## But why?

There are only a handful of good reasons to use this (or any dynamic).  It's up to you to decide when and where.

- The best example is to reduce the complexity involved in setting up complex structures such as JSON, XML, or MVC model hierarchies.  This is helpful during the early phase of a top-down development pattern (**GUI or webservice first**).  Use the data structures **BEFORE** you create them!

### Why not use ExpandoObject?

[ExpandoObject](http://msdn.microsoft.com/en-us/library/system.dynamic.expandoobject(v=vs.110).aspx) is very limited out of the box and FluentObject **takes care of all the ugly hair-removing code** required to get things just the way they should be.

Can your Expando do this?

```C#
dynamic o = new FluentObject();

o.test[0].Name.Is.So[12].Cool["YAY"].Oh.Yeah = "MyTest";
o.test[0].Name.Is.So[12].Yes["HEY"] = 4;
o.test[0].Name.Is.So[12].Yes["YOU"] = 7;

var dict = o.test[0].Name.Is.So[12].Yes;
IDictionary<string, int> implements = dict;

Debug.Print(string.Join(", ", implements));
```
## Caveats

Casting to IDictionary cannot be done via direct cast:
```C#
// Won't work
IDictionary<string, object> d = (IDictionary<string, object>)o.Friends;
// Won't work
IDictionary<string, object> d = o.Friends;
```
It must be done indirectly as such:
```C#
// This works!
var extracted = o.Friends;
IDictionary<string, object> d = extracted;
```
Why? That's a harder question to answer... and the information is not very useful.  If you can make it work seamlessly (ie. without using a method), send in a push request!!

A Converter pattern or an extension method are being considered to replace this ugly hack.

## Method to the Madness

Keep all the ugly bottled up in FluentObject and test like mad to make sure it *always* works!
