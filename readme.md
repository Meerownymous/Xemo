# Xemo is a true object oriented, DTO free data storage library
If you agree to these statements, Xemo may be for you:
- All data should be encapsulated
- Objects are for processing data, not for holding data
- Object oriented programming is about messages between objects
- Clean architecture requires state management seperated from entity behaviour

# Motivation
This library is motivated by the [Clean Architecture Principle](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html). It suggests to postpone any infrastructure decisions as long as possible, which has multiple advantages. We are able to develop the problem domain usecases of our software independently from everything else. 
If we are building a calculator, we would have usecases like sum, factor, squareroot. Maybe we have graphs, history, recall.
But we don't need to decide how we draw the UI. We can then build an iOS app, or a console interface, or a webapp - and reuse the same usecases everywhere. 

We can easily apply this to other infrastructure details: Why decide early that our online protocol is http? Why decide that the best way to deliver information is email? Shouldn't it better be a push notification?
When we build our app in a way that all this is not relevant to the core problem domain of our code, then we can switch from http to socket without refactoring anything, just by adding a few new implementations of transport objects.
When we design our usecase for example like Information.Push(IAudience audience), then we can have a PushAudience and an Emailaudience easily.

There is another infrastructure detail: The software's state and the access to it - its memory.
If you build your code in a way that the memory can be switched without any costs, it has multiple advantages:
- Inject a Ram memory in Unittests, which can be prefilled with anything
- Use simple binary file storage for small projects
- Add a cache if the application grows
- Switch to a NoSQL database like MongoDB to scale up as your amount of data grows
- Switch to an SQL database like SQL Lite when your software changes in a way that you rely more on relations between entries
- Move from local to cloud

Ideally, following the Clean Architecture, this should be as simple as replacing 'var memory = new Ram()' with 'var memory = new SQL()'. 
And of course, we do not want to lose any advantages of the storage types. If we switch, we want advantage of what we switch to.

Xemo aims to allow this.

It gives you an abstracted memory container which you can inject into your objects, avoiding Data Transfer Objects by working with context-close anonymous data objects.
You pull state from and push state to this container, while staying totally flexible about the type of storage.
- In unittests, the storage is RAM
- In production, the storage is database or azure tables or files
- When the application needs change within time, you can swap the storage from for example files to database without refactoring

## Getting started
We take the todo-list app as example.
This is how you define a todo with xemo.

First, you design a schema which is the blueprint for every todo. Provide default values and the resulting type is clear. ```Done = false``` results in a boolean named "Done", for example.

```csharp
new
{
    Done = false,
    Created = DateTime.Now,
    Subject = String.Empty,
    Author = String.Empty
}
```

Turn this into a Cocoon like this:

```csharp
var todo =
    new
    {
        Done = false,
        Created = DateTime.Now,
        Subject = subject,
        Author = ""
    }.AsCocoon("todo", new Ram()); //Ram is a xemo memory. 
                                 //It can be ram, file, database or others. 
```

Your schema is now encapsulated. The Cocoon takes care of querying or manipulating the data.
Now manipulate the empty schema by patching it with actual data.

```csharp

todo.Mutate(new { Subject = "Complete Examples", Author = "John Doe", Done = false });

```

You can now get a slice of data by passing an empty data structure to the Cocoon. It will then fill that for you by merging the encapsulated data into the passed slice.

```csharp

var simple =
   todo.Fill(new { Subject = "", Done = false });

//Note that the above slice is an anonymous object. 
//You can also use concrete objects with properties, if you prefer. 
//Using anonymous types might have advantages in agile development. 
//Here is an example using a property object:

public class Simple
{
    String Subject { get; set; }
    Bool Done { get; set; }
}

var simple = todo.Fill(new Simple());

//Either way, this will result in an object filled with data, making this assumption true:
Assert.Equal(
    simple.Subject,
    "Complete Examples"
);

```




**A "Cocoon" which holds data for an individual thing:**

```csharp
/// A piece of data which is based on a schema.
public interface ICocoon
{
   /// Grip is a unique identifier, carrying what subject the cocoon is about as well as a unique id.
   IGrip Grip();

   /// Fill properties of the given slice with data available inside.
   TSlice Fill<TSlice>(TSlice wanted);

   /// Set the schema for this Xemo. Properties of this schema
   /// define the properties that can be obtained via Fill()
   IXemo Schema<TSchema>(TSchema schema);

   /// Mutate contents by property values of the given patch.
   IXemo Mutate<TPatch>(TPatch patch);
}
```
------

## Not yet documented but available
    - Cluster (multiple Cocoons, filterable)
    - Relations
    - Data Sampling
