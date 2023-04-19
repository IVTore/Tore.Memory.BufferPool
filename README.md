# Tore.Memory.BufferPool
A thread safe ByteArray buffer object pool class library for C# By İ. Volkan Töre.

Language: C#.

Nuget package: [Tore.Core](https://www.nuget.org/packages/Tore.Memory.BufferPool/)

[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](https://opensource.org/licenses/MIT)

Dependencies: <br/>
net7.0<br/>


## BufferPool.cs :
Contains a no nonsense simple thread safe ByteArray buffer object pool class.
Buffer pool object is a stack of ByteArray buffers that can be quickly acquired and released. 

Behaviours:
    -   When a buffer is requested from the pool, if there are buffers in the pool, one is returned. 
        Otherwise a new buffer is created and returned.
    -   When a buffer is returned to the pool, primarily it is checked for null and size,
        if it fits and the pool is not full, the buffer is cleared and added to the pool.
        Otherwise the buffer is discarded.
    -   The pool is thread safe.


Constructor arguments are:
    - poolCapacity  : The maximum number of buffers allowed in the pool.
    - sizePerBuffer : The size of each ByteArray buffer in the pool.
    - preAllocate   : If true, the pool is populated with buffers. Default is false.

Constraints: 
    - MIN_BUFFER_COUNT : poolCapacity must be greater than 16.
    - MIN_BUFFER_SIZE  : sizePerBuffer must be greater than 64.
    - MAX_RAM_GB       : poolCapacity * sizePerBuffer must be less than 2GB.

```C#
public class ExamplePoolUserClass {

    private BufferPool _pool;
    
    public ExamplePoolUserClass() {
        _pool = new BufferPool(100, 1024, true);    // build a pool of 100 1KB buffers pre allocated.
        Console.WriteLine(_pool.count);             // 100
        Console.WriteLine(_pool.capacity);          // 100
        Console.WriteLine(_pool.bufferSize);        // 1024
    }

    public void DoSomething() {
        byte[] buffer = _pool.GetBuffer();          // Fetch a buffer from the pool.
                                                    // Do something with the buffer.

        _pool.ReturnBuffer(buffer);                 // Return the buffer to the pool.
    }
}
```














Contains static utility methods for simple encrypted configuration file support. 
Configurations are loaded to and saved from <b> public static fields </b> of a class.

## Extensions.cs :
Contains static utility extension methods for string, char, ICollection, List of T.

## Hex.cs :
Contains static utility methods for Hex string conversions.

## Json.cs :
Contains static utility methods for Json conversions.

## StrLst.cs :
Defines the class StrLst which is a string associated object list (key - value) class with tricks.

StrLst provides:
1) Numerically indexed access to keys and objects.
2) Ordering.
3) Translation forward and backward to various formats.
4) Duplicate key support.

* Keys can not be null empty or whitespace.            
* Lists are public in this class intentionally.        
* StrLst also acts as a bridge for,

   - Json, 
   - Objects (public properties), 
   - Static classes (public static fields),
   - IDictionary <string, object> and
   - List KeyValuePair <string, string>.     
 
Has Enumerator and Nested conversion support.           

## Sys.cs :
Defines the static class Sys containing a library of utility methods treated as global functions which is used for managing:
  - Exceptions by Exc().
  - Parameter checking by Chk().
  - Several application information routines.
  - Attributes, reflection and type juggling.  

The best way of using them is by adding: 
```C#
using static Tore.Core.Sys;
```
to the source for filewide or by adding:

```C#
global using static Tore.Core.Sys;
```
to the usings.cs file for projectwide access.

## Utc.cs :
Contains static utility methods for DateTime conversions. 
String, Seconds, Milliseconds return <b>UtcNow</b> values.

CultureInfo is <b>CultureInfo.InvariantCulture.</b>       

## Utf8File.cs :
Contains static utility methods to load and save UTF8 files.

## Xor.cs :
Contains static utility methods for simple xor encryption and decryption.

