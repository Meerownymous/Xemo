# Xemo is a true object oriented, DTO free data storage library
If you agree to these statements, Xemo may be for you:
- All data should be encapsulated
- Objects are for processing data, not for holding data
- Object oriented programming is about messages between objects
- Clean architecture requires state management seperated from entity behaviour

Xemo gives you an abstracted memory container which you can use in your objects.
You pull state from and push state to this object, while staying totally flexible about the type of storage.
- In unittests, the storage is RAM
- In production, the storage is database or azure tables or files
- When the application needs change within time, you can swap the storage from for example files to database without refactoring

## Getting started
We take the todo-list app as example.
This is how you define a todo with xemo.

First, you design a schema which is the blueprint for every todo. Provide default values and the resulting type is clear. ```Done = false``` results in a boolean named "Done", for example.

```
{
    Done = false,
    Created = DateTime.Now,
    Subject = String.Empty,
    Author = String.Empty
}
```

Turn this into a Xemo like this:

```csharp
var todo =
    new
    {
        Done = false,
        Created = DateTime.Now,
        Subject = subject,
        Author = ""
    }.AsXemo("todo", new Ram()); //Ram is a xemo memory. 
                                 //It can be ram, file, database or others. 
                                 //We'll get to that later.
```

Your schema is now encapsulated. The Xemo takes care of querying or manipulating the data.
Now manipulate the empty schema by patching it with actual data.

```csharp

todo.Mutate(new { Subject = "Complete Examples", Author = "John Doe", Done = false });

```

You can now get a slice of data by passing an empty data structure to the Xemo. It will then fill that for you by merging the encapsulated data into the passed slice.

```
var simple =
   todo.Fill(new { Subject = "", Done = false });

//Note that the above slice is an anonymous object. You can also use concrete objects with properties, if you prefer. Using anonymous types might have advantages in agile development. Here is an example using a property object:

public class Simple
{
	String Suject { get; set; }
    Bool Done { get; set; }
}

var simple = todo.Fill(new Simple());

//Either way, this will result in an object filled with data, making this assumption true:
Assert.Equal(
    simple.Subject,
    "Complete Examples"
);

```




**A "Xemo" which holds data for an individual thing:**

```csharp
/// A piece of data which is based on a schema.
public interface IXemo
{
   /// ID Card which uniquely identifies this Xemo.
   IIDCard Card();

   /// Fill properties of the given slice with data available inside.
   TSlice Fill<TSlice>(TSlice wanted);

   /// Set the schema for this Xemo. Properties of this schema
   /// define the properties that can be obtained via Fill()
   IXemo Schema<TSchema>(TSchema schema);

   /// Mutate contents by property values of the given patch.
   IXemo Mutate<TPatch>(TPatch patch);
}

```

