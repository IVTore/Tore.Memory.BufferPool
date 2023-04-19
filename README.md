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